using UnityEngine;

namespace devlog98.Ammunition {
    public class Bullet : MonoBehaviour {
        [SerializeField] Rigidbody2D rb;
        [SerializeField] float moveSpeed; // speed of bullet
        [SerializeField] int duration; // duration of bullet on scene

        private Transform parent; // bullet parent

        // shoots bullet based on direction
        public void Shoot(Vector2 startPosition, Vector2 moveDirection) {
            // store parent
            parent = transform.parent;
            transform.SetParent(null);

            // shoot
            this.gameObject.SetActive(true);
            transform.position = startPosition;
            rb.velocity = moveDirection * moveSpeed;

            // set bullet duration
            CancelInvoke("Disable");
            Invoke("Disable", duration);
        }

        // collision checks
        private void OnTriggerEnter2D(Collider2D other) {
            if (other.gameObject.tag != "Player") {
                Disable();
            }
        }

        // disable bullet
        private void Disable() {
            transform.parent = parent;
            this.gameObject.SetActive(false);
        }
    }
}