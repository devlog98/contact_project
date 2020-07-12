using UnityEngine;
using devlog98.Mouse;

namespace devlog98.Player {
    public class Player : MonoBehaviour {
        [Header("General")]
        [SerializeField] private Animator anim;
        [SerializeField] private Transform sprite;

        [Header("Jump")]
        [SerializeField] private Rigidbody2D rb;
        [SerializeField] private float jumpSpeed; // jump speed

        [Header("Collision")]
        [SerializeField] private LayerMask collisionMask;

        private bool canJump = true; // if player can jump

        private void Update() {
            // jump input
            if (canJump && Input.GetMouseButtonDown(0)) {
                // jump
                Vector2 jumpDirection = Aim.instance.GetAimDirection(transform.position);
                Jump(jumpDirection);
                
            }
        }

        // jumps in a given direction
        public void Jump(Vector2 jumpDirection) {
            // rotate sprite
            float rotation = Mathf.Atan2(-jumpDirection.y, -jumpDirection.x) * Mathf.Rad2Deg;
            sprite.rotation = Quaternion.Euler(0f, 0f, rotation + 90f);

            // jump
            rb.velocity = jumpDirection * jumpSpeed;
            canJump = false;

            // jump anim
            anim.SetBool("canJump", canJump);
        }

        // lands on the correct rotation
        public void Land(Vector2 landPoint) {
            // calculate distance on x and y axis
            float yDistance = Mathf.Abs(landPoint.y - transform.position.y);
            float xDistance = Mathf.Abs(landPoint.x - transform.position.x);

            if (yDistance < xDistance) {
                // landed on vertical surface
                if (landPoint.x > transform.position.x) {
                    sprite.rotation = Quaternion.Euler(Vector3.forward * 90);
                }
                else {
                    sprite.rotation = Quaternion.Euler(Vector3.forward * 270);
                }
            }
            else {
                // landed on horizontal surface
                if (landPoint.y > transform.position.y) {
                    sprite.rotation = Quaternion.Euler(Vector3.forward * 180);
                }
                else {
                    sprite.rotation = Quaternion.Euler(Vector3.forward * 0);
                }
            }

            // landing
            rb.velocity = Vector2.zero;
            canJump = true;

            // landing anim
            anim.SetBool("canJump", canJump);
        }

        // land on collision
        private void OnTriggerEnter2D(Collider2D collision) {
            Land(collision.ClosestPoint(transform.position));
        }
    }
}