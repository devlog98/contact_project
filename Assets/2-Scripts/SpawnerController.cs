using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace devlog98.Obstacle {
    public class SpawnerController : MonoBehaviour {
        [Header("Spawn")]
        [SerializeField] private List<Spawner> spawnerList;
        [SerializeField] private float spawnInterval;
        [SerializeField] private float spawnReduction;

        private int spawnerIndex;
        private float spawnTime;

        // initialize controller
        private void Start() {
            StartCoroutine("SpawnerCoroutine");
        }

        private IEnumerator SpawnerCoroutine() {
            spawnTime = spawnInterval;

            while (true) {
                foreach(Spawner spawner in spawnerList) {
                    spawner.Spawn();
                    spawnTime -= spawnReduction;
                    yield return new WaitForSeconds(spawnTime);
                }
            }
            
        }
    }
}