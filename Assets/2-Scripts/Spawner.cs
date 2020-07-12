using System.Collections.Generic;
using UnityEngine;

namespace devlog98.Obstacle {
    public class Spawner : MonoBehaviour {
        [Header("Spawn")]
        [SerializeField] private List<Platform> spawnObjects;
        [Range(-360f, 360f)] [SerializeField] private int minimumSpawnAngle;
        [Range(-360f, 360f)] [SerializeField] private int maximumSpawnAngle;

        private System.Random randomizer = new System.Random();

        public void Spawn() {
            // get inactive platform
            Platform platform = spawnObjects.Find(x => !x.isActiveAndEnabled);

            if (platform != null) {
                // aim at certain angle
                float rotation = randomizer.Next(minimumSpawnAngle, maximumSpawnAngle);
                transform.rotation = Quaternion.Euler(0f, 0f, rotation);

                // launch direction
                Vector2 launchDirection = transform.up.normalized;
                platform.Launch(transform.position, launchDirection);
            }
        }
    }
}