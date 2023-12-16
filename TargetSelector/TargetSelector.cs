using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace BasDidon.TargetSelector
{
    public class TargetSelector<T>
    {
        MonoBehaviour Invoker { get; }
        public bool AllowToCancle { get; set; }

        IEnumerable<T> Targets { get; }
        Func<T, bool> Predicate { get; }
        Func<T> Updater { get; }
        IEnumerable<T> Selectable { get; }

        T focusTarget;
        T FocusTarget
        {
            get => focusTarget;
            set
            {
                if (Equals(focusTarget, value))
                    return;

                focusTarget = value;
                OnFocusChange?.Invoke(focusTarget);
            }
        }

        // state control
        public enum SelectorPhase { created, started, performed, cancled }
        public SelectorPhase Phase { get; private set; }

        // Routine
        public Coroutine SelectTargetRoutine { get; set; }

        // Events
        public event Action OnSelectionStart;   // when start select
        public event Action<T> OnFocusChange;   // when player focus on any target
        public event Action<T> OnSelect;        // when player decided
        public event Action OnCancle;           // when player cancle to select
        public event Action OnSelectionEnd;     // when selection end

        public TargetSelector(MonoBehaviour invoker, IEnumerable<T> targets, Func<T, bool> predicate,Func<T> updater, bool allowToCancle)
        {
            Invoker = invoker;
            Targets = targets;
            Predicate = predicate;
            Updater = updater;

            Phase = SelectorPhase.created;

            AllowToCancle = allowToCancle;

            Selectable = Targets.Where(target => Predicate(target));
        }

        public void Start()
        {
            Phase = SelectorPhase.started;
            OnSelectionStart?.Invoke();

            if (Selectable.Any())
            {
                SelectTargetRoutine = Invoker.StartCoroutine(SelectionAction());
            }
            else
            {
                // can't select anything.
                EndSelection();
            }
        }

        // let user cancle selection
        public void Cancle()
        {
            if (!AllowToCancle)
                return;

            Phase = SelectorPhase.cancled;
            OnCancle?.Invoke();

            Invoker.StopCoroutine(SelectTargetRoutine);

            EndSelection();
        }

        // let user select
        public void Select()
        {
            if (Selectable.Contains(FocusTarget))
            {
                Phase = SelectorPhase.performed;
                OnSelect?.Invoke(FocusTarget);

                Invoker.StopCoroutine(SelectTargetRoutine);

                EndSelection();
            }
            else
            {
                Debug.Log("not in selectable");
            }
        }

        void EndSelection()
        {
            OnSelectionEnd?.Invoke();
        }

        IEnumerator SelectionAction()
        {
            yield return new WaitUntil(()=> {
                FocusTarget = Updater();
                return false;
            });
        }
    }
}