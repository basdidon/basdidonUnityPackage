using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BasDidon
{
    public class Vector
    {
        /// <returns>is b closer than a</returns>
        public static bool CloserThan(Vector3 origin, Vector3 a, Vector3 b) => (origin - a).sqrMagnitude > (origin-b).sqrMagnitude;
    }
}

