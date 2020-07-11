using UnityEngine;
using devlog98.Mouse;

namespace devlog98.Player {
    public class Player : MonoBehaviour {
        [Header("Jump")]
        [SerializeField] private Rigidbody2D rb;
        [SerializeField] private float jumpSpeed; // jump speed
        
        private bool canJump = true; // if player can jump

        private void Update() {
            // jump input
            if (canJump && Input.GetMouseButtonDown(0)) {
                Vector2 jumpDirection = Aim.instance.GetAimDirection(transform.position);
                Jump(jumpDirection);
                canJump = false;
            }
        }

        // jumps in a given direction
        public void Jump(Vector2 jumpDirection) {
            rb.velocity = jumpDirection * jumpSpeed;
        }

        private void OnTriggerEnter2D(Collider2D collision) {
            // stops jump on collisions
            rb.velocity = Vector2.zero;
            canJump = true;
        }
    }
}