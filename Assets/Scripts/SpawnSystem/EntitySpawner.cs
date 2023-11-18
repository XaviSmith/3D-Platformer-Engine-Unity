using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Platformer
{
    /// <summary>
    /// Called by SpawnManagers to actually spawns Entities. An Entity can be a collectible, enemy etc.
    /// Uses an EntityFactory interface and SpawnPointStrategy interface.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class EntitySpawner<T> where T : Entity
    {
        IEntityFactory<T> entityFactory;
        ISpawnPointStrategy spawnPointStrategy;

        public EntitySpawner(IEntityFactory<T> _entityFactory, ISpawnPointStrategy _spawnPointStrategy)
        {
            this.entityFactory = _entityFactory;
            this.spawnPointStrategy = _spawnPointStrategy;
        }

        public T Spawn()
        {
            return entityFactory.Create(spawnPointStrategy.NextSpawnPoint());
        }
    }
}

