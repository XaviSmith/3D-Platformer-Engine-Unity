using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Platformer
{
    /// <summary>
    /// For spawning entities based on their data. We can also control what behaviours they spawn with etc
    /// </summary>
    public interface IEntityFactory<T> where T : Entity
    {
        T Create(Transform spawnPoint);
    }
}

