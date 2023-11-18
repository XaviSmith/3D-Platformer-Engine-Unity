using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Platformer
{
    /// <summary>
    /// For instantiating entities based on their data. EntitySpawners use this + a spawn strategy
    /// Needs a spawn point
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class EntityFactory<T> : IEntityFactory<T> where T : Entity
    {
        EntityData[] data; //EntityData is a scriptable Object containing an entity's data

        public EntityFactory(EntityData[] _data)
        {
            this.data = _data;
        }

        public T Create(Transform spawnPoint)
        {
            EntityData entityData = data[Random.Range(0, data.Length)]; //Not sure why this is random right now
            GameObject instance = GameObject.Instantiate(entityData.prefab, spawnPoint.position, spawnPoint.rotation);
            return instance.GetComponent<T>();
        }
    }
}

