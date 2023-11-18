using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Platformer
{
    /// <summary>
    /// Basically an Entity spawner we can configure in the editor.
    /// To spawn something we just need the spawn strategy (Linear/random etc), and places it can spawn.
    /// See CollectibleSpawnManager for an example.
    /// </summary>
    public abstract class EntitySpawnManager : MonoBehaviour
    {
        protected enum SpawnPointStrategyType { LINEAR, RANDOM } //So we can configure spawn strategy in editor.

        [SerializeField] protected SpawnPointStrategyType spawnPointStrategyType = SpawnPointStrategyType.LINEAR;
        [SerializeField] protected Transform[] spawnPoints;

        protected ISpawnPointStrategy spawnPointStrategy;

        protected virtual void Awake()
        {
            //set spawnpoint type
            switch(spawnPointStrategyType)
            {
                case SpawnPointStrategyType.LINEAR:
                    spawnPointStrategy = new LinearSpawnPointStrategy(spawnPoints);
                    break;
                case SpawnPointStrategyType.RANDOM:
                    spawnPointStrategy = new RandomSpawnPointStrategy(spawnPoints);
                    break;

            }

            //switch expression ver (simplified version of a switch statement) Requires .NET 7.0 or higher, curr ver .NET 2.10
            /*
            spawnPointStrategy = spawnPointStrategyType switch {
                SpawnPointStrategyType.LINEAR => new LinearSpawnPointStrategy(spawnPoints),           
                SpawnPointStrategyType.RANDOM => new RandomSpawnPointStrategy(spawnPoints),
                _ => spawnPointStrategy
            }; */
    }

        public abstract void Spawn();
    }
}

