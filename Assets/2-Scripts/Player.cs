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
        private Vector2 jumpDirection;


        [Header("Collision")]
        [SerializeField] private LayerMask collisionMask;
        private bool canJump = true; // if player can jump
        private bool isJumping;

        private void Update() {
            // jump input
            if (canJump && Input.GetMouseButtonDown(0)) {
                // jump
                jumpDirection = Aim.instance.GetAimDirection(transform.position);
                Jump(jumpDirection);
            }
            else if (isJumping) {
                // land on collision
                JumpCollisionResult check = JumpCollisionCheck(jumpDirection, 0.64f);
                if (!check.Success) {
                    Land(check.CollisionPoint);
                }
            }
        }

        // jumps in a given direction
        public void Jump(Vector2 jumpDirection) {
            // only if jump is viable
            if (JumpCollisionCheck(jumpDirection, 2.56f).Success) {
                // rotate sprite
                float rotation = Mathf.Atan2(-jumpDirection.y, -jumpDirection.x) * Mathf.Rad2Deg;
                sprite.rotation = Quaternion.Euler(0f, 0f, rotation + 90f);

                // jump
                rb.velocity = jumpDirection * jumpSpeed;
                canJump = false;
                isJumping = true;

                // jump anim
                anim.SetBool("canJump", canJump);
            }
        }

        // checks if jump is possible
        private JumpCollisionResult JumpCollisionCheck(Vector2 jumpDirection, float checkDistance) {
            bool success = false;
            Vector2 collisionPoint = Vector2.zero;

            // cast multiple rays
            for (int i = -1; i <= 1; i++) {
                RaycastHit2D hit = Physics2D.Raycast(transform.position + Vector3.right * 0.32f * i, jumpDirection, checkDistance, collisionMask);

                if (hit.collider != null) {
                    collisionPoint = hit.collider.ClosestPoint(transform.position);
                    break;
                }
            }

            if (collisionPoint == Vector2.zero) {
                success = true;
            }

            return new JumpCollisionResult(success, collisionPoint);
        }

        // lands on the correct rotation
        private void Land(Vector2 landPoint) {
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
            isJumping = false;

            // landing anim
            anim.SetBool("canJump", canJump);
        }



        // land on collision
        //private void OnTriggerEnter2D(Collider2D collision) {
        //    Land(collision.ClosestPoint(transform.position));
        //}
    }

    // special struct to use on jump checks
    public struct JumpCollisionResult {
        private bool success;
        private Vector2 collisionPoint;

        public bool Success { get => success; }
        public Vector2 CollisionPoint { get => collisionPoint; }

        public JumpCollisionResult(bool _success, Vector2 _collisionPoint) {
            success = _success;
            collisionPoint = _collisionPoint;
        }
    }
}