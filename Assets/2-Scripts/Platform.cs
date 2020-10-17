using devlog98.Boundaries;
using UnityEngine;

namespace devlog98.Obstacle {
    public class Platform : MonoBehaviour, IBounded {
        [Header("Launch")]
        [SerializeField] private Rigidbody2D rb;
        [SerializeField] private float launchSpeed;

        private System.Random randomizer = new System.Random();

        // launch platform
        public void Launch(Vector2 launchOrigin, Vector2 launchDirection) {
            // unparent from spawner
            transform.parent = null;

            // set platform position and state
            transform.position = launchOrigin;
            gameObject.SetActive(true);

            // rotate platform randomly
            transform.rotation = Quaternion.Euler(0f, 0f, randomizer.Next(0, 360));

            // apply force
            rb.velocity = launchDirection * launchSpeed;
        }

        // IBounded implementation
        // disable platform
        public void OutOfBounds() {
            gameObject.SetActive(false);
        }
    }
}