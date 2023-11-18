using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Platformer
{
    /// <summary>
    /// Scriptable object containing the casic data for any type of entity we want to spawn (enemy, collectible etc)
    /// </summary>
    public class EntityData : ScriptableObject
    {
        public GameObject prefab;
        //other common data

    }
}
