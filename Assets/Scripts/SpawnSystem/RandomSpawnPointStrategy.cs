using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace Platformer
{
    public class RandomSpawnPointStrategy : ISpawnPointStrategy
    {
        List<Transform> unusedSpawnPoints;
        Transform[] spawnPoints;

        public RandomSpawnPointStrategy(Transform[] _spawnPoints)
        {
            this.spawnPoints = _spawnPoints;
            this.unusedSpawnPoints = new List<Transform>(spawnPoints);
        }

        public Transform NextSpawnPoint()
        {
            if(!unusedSpawnPoints.Any()) //when we have no spawn points left, repopulate the list
            {
                unusedSpawnPoints = new List<Transform>(spawnPoints);
            }

            int randomIndex = Random.Range(0, unusedSpawnPoints.Count);
            Transform result = unusedSpawnPoints[randomIndex];
            unusedSpawnPoints.RemoveAt(randomIndex);
            return result;
        }


    }
}

