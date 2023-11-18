using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Platformer
{
    public class LinearSpawnPointStrategy : ISpawnPointStrategy
    {
        int index = 0;
        Transform[] spawnPoints;

        public LinearSpawnPointStrategy(Transform[] _spawnPoints)
        {
            this.spawnPoints = _spawnPoints;
        }

        public Transform NextSpawnPoint()
        {
            Transform result = spawnPoints[index];
            index = (index + 1) % spawnPoints.Length;
            return result;
        }
    }
}
