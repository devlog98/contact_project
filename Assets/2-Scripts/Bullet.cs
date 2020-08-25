using UnityEngine;

namespace devlog98.Ammunition {
    public class Bullet : MonoBehaviour {
        [SerializeField] Rigidbody2D rb;
        [SerializeField] float moveSpeed;

        // shoots bullet based on direction
        public void Shoot(Vector2 moveDirection) {
            rb.velocity = moveDirection * moveSpeed;
        }
    }
}