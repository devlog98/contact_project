using UnityEngine;
using devlog98.Mouse;
using System.Collections;
using devlog98.UI;
using UnityEngine.SceneManagement;
using devlog98.Audio;
using devlog98.Ammunition;

namespace devlog98.Player {
    public class Player : MonoBehaviour {
        [Header("General")]
        [SerializeField] private Animator anim;
        [SerializeField] private Transform sprite;

        [Header("Jump")]
        [SerializeField] private Rigidbody2D rb;
        [SerializeField] private float jumpSpeed; // jump speed
        [SerializeField] private float jumpSpeedBonus;
        public float currentJumpSpeed;

        [SerializeField] private int maxJumps;
        private int jumps;

        [Header("Shoot")]
        [SerializeField] private Bullet bullet;

        [Header("Audio")]
        [SerializeField] private AudioClip jumpClip;
        [SerializeField] private AudioClip landClip;
        [SerializeField] private AudioClip deathClip;

        private Vector2 currentDirection;
        private Vector2 jumpDirection;
        private Vector2 shootDirection;

        private int landCounter;

        [Header("Collision")]
        [SerializeField] private Collider2D collider;
        [SerializeField] private LayerMask collisionMask;
        [SerializeField] private float collisionThreshold;

        private bool isLanded = true; // if player is landed on the ground

        private float collisionTime;
        private float collisionTolerance = .1f;

        // initial setup
        private void Start() {
            currentDirection = Vector2.zero;
            currentJumpSpeed = jumpSpeed;
        }

        private void Update() {
            // jump input
            if (jumps < maxJumps && Input.GetMouseButtonDown(0)) {
                // jump
                jumpDirection = Aim.instance.GetAimDirection(transform.position);
                Jump(jumpDirection);
            }

            // shoot input
            if (Input.GetMouseButtonDown(1)) {
                // shoot
                shootDirection = Aim.instance.GetAimDirection(transform.position);
                Shoot(shootDirection);
            }

            CheckDeath();
        }

        // jumps in a given direction
        private void Jump(Vector2 jumpDirection) {
            // only if jump is viable
            if (!isLanded || JumpCollisionCheck(jumpDirection)) {
                // jump
                Launch(jumpDirection);
                jumps++;

                if (isLanded) {
                    // unparent from land object
                    transform.SetParent(null);

                    // collision tolerance
                    StopCoroutine(DisableCollider());
                    StartCoroutine(DisableCollider());

                    isLanded = false;
                }

                // jump anim
                anim.SetBool("canJump", isLanded);
                AudioManager.instance.PlayClip(jumpClip);
            }
        }

        // checks if jump is possible
        private bool JumpCollisionCheck(Vector2 jumpDirection) {
            bool success = false;

            Debug.Log(Vector2.Angle(jumpDirection, transform.up));
            float jumpAngle = Vector2.Angle(jumpDirection, transform.up);
            if (jumpAngle < 90) {
                RaycastHit2D hit = Physics2D.Raycast(transform.position, jumpDirection, collisionThreshold, collisionMask);
                if (hit.collider == null) {
                    success = true;
                }
            }
            //else if (jumpAngle < 125) {
            //    RaycastHit2D hit = Physics2D.Raycast(transform.position, jumpDirection, 2.56f, collisionMask);
            //    if (hit.collider == null) {
            //        success = true;
            //    }
            //}

            return success;
        }

        // lands on the correct rotation
        private void Land(Transform landTransform, Vector2 landPoint) {
            // parent to land object
            transform.SetParent(landTransform);

            // rotate sprite
            Vector2 landDirection = (landPoint - (Vector2)transform.position).normalized;
            float rotation = Mathf.Atan2(-landDirection.y, -landDirection.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, rotation - 90f);

            // landing
            rb.velocity = Vector2.zero;
            isLanded = true;
            jumps = 0;

            // landing anim
            anim.SetBool("canJump", isLanded);
            AudioManager.instance.PlayClip(landClip);

            // reset speed
            currentJumpSpeed = jumpSpeed;
            currentDirection = Vector2.zero;

            landCounter++;
            UIController.instance.UpdateCounter(landCounter);
        }

        // disable collider on beginning of jump
        private IEnumerator DisableCollider() {
            collider.gameObject.SetActive(false);
            yield return new WaitForSeconds(collisionTolerance);
            collider.gameObject.SetActive(true);
        }

        private void Shoot(Vector2 shootDirection) {
            // instantiate bullet
            Bullet newBullet = Instantiate(bullet, transform.position, transform.rotation);
            newBullet.Shoot(shootDirection);

            // launch player if in mid air
            if (!isLanded) {
                Launch(shootDirection * -1);
            }
        }

        // launches player in direction
        private void Launch(Vector2 direction) {
            // rotate sprite
            float rotation = Mathf.Atan2(-direction.y, -direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, rotation + 90f);

            // modify speed depending on movement angle
            if (Vector2.Angle(direction, currentDirection) < 30) {
                currentJumpSpeed += jumpSpeedBonus;
            }
            else {
                currentJumpSpeed = jumpSpeed;
            }

            // launch
            rb.velocity = direction * currentJumpSpeed;

            // set new direction
            currentDirection = direction;
        }

        // player death
        private void CheckDeath() {
            // out of bounds
            if (transform.position.x < -10.24 || transform.position.x > 10.24 || transform.position.y < -5.12 || transform.position.y > 6.4) {
                // reset parent
                transform.SetParent(null);

                // reset position
                transform.position = new Vector3(0f, -3.65f, 0f);
                transform.rotation = Quaternion.Euler(Vector3.zero);

                // rest jump
                isLanded = true;

                // reset velocity
                rb.velocity = Vector3.zero;

                // reset counter
                landCounter = 0;
                UIController.instance.UpdateCounter(landCounter);

                AudioManager.instance.PlayClip(deathClip);
            }
        }

        // land on collision
        private void OnTriggerEnter2D(Collider2D collision) {
            if (!isLanded) {
                Land(collision.gameObject.transform, collision.ClosestPoint(transform.position));
            }
        }

        private void OnDrawGizmos() {
            Gizmos.color = Color.green;
            Gizmos.DrawRay(transform.position, jumpDirection * collisionThreshold);

            Gizmos.color = Color.red;
            Gizmos.DrawRay(transform.position, transform.up * 1.28f);
            Gizmos.DrawRay(transform.position, transform.right * -1.28f);
            Gizmos.DrawRay(transform.position, transform.right * 1.28f);
        }
    }
}