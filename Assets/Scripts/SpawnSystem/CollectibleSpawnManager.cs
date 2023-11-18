using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

namespace Platformer
{
    /// <summary>
    /// Collectibles implementation of EntitySpawnManager.
    /// </summary>
    public class CollectibleSpawnManager : EntitySpawnManager
    {
        [Tooltip("Scriptable Objects containing the collectible's data")]
        [SerializeField] CollectibleData[] collectibleData;
        [SerializeField] float spawnInterval = 1f;

        EntitySpawner<Collectible> spawner;

        CountdownTimer spawnTimer;
        int counter; //how many things have spawned

        protected override void Awake()
        {
            base.Awake(); //set spawn strategy (linear/random)

            //Initialize a spawner and it's factory along with our spawnPoint strategy
            spawner = new EntitySpawner<Collectible>(new EntityFactory<Collectible>(collectibleData), spawnPointStrategy);

            //Configure spawnTimer + cooldowns and what to do after each spawn.
            spawnTimer = new CountdownTimer(spawnInterval);
            spawnTimer.OnTimerStop += () =>
            {
                //Check if we have things left to spawn. return if not
                if(counter++ >= spawnPoints.Length)
                {
                    spawnTimer.Stop();
                    return;
                }

                //Otherwise spawn something and wait for cooldown
                Spawn();
                spawnTimer.Start();
            };
        }

        public override void Spawn() => spawner.Spawn();

        void Start()
        {
            spawnTimer.Start();
        }

        void Update()
        {
            spawnTimer.Tick(Time.deltaTime);
        }
    }
}

