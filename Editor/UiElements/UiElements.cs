using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace BasDidon.Editor
{
    public static class UiElements
    {
        public static PropertyField GetDefaultScriptPropertyField(SerializedObject serializedObject)
        {
            var scriptPropertyField = new PropertyField(serializedObject.FindProperty("m_Script"));
            scriptPropertyField.SetEnabled(false);
            return scriptPropertyField;
        }

        public static ListView DrawListview<T>(string title, List<T> sourceList, bool allowSceneObjects = true) where T : UnityEngine.Object
        {
            var listView = new ListView(sourceList)
            {
                virtualizationMethod = CollectionVirtualizationMethod.DynamicHeight,
                showFoldoutHeader = true,
                headerTitle = title,
                showAddRemoveFooter = true,
                showBorder = true,
                reorderable = true,
                reorderMode = ListViewReorderMode.Animated,
                makeItem = () => new ObjectField
                {
                    objectType = typeof(T),
                    allowSceneObjects = allowSceneObjects,

                },
                bindItem = (element, i) =>
                {
                    ((ObjectField)element).value = sourceList[i];
                    ((ObjectField)element).RegisterValueChangedCallback((value) =>
                    {
                        sourceList[i] = (T)value.newValue;
                    });
                },
            };

            return listView;
        }
    }
}
