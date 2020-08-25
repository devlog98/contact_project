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

        [Header("Shoot")]
        [SerializeField] private Bullet bullet;

        [Header("Audio")]
        [SerializeField] private AudioClip jumpClip;
        [SerializeField] private AudioClip landClip;
        [SerializeField] private AudioClip deathClip;

        private Vector2 jumpDirection;
        private Vector2 shootDirection;

        private int landCounter;

        [Header("Collision")]
        [SerializeField] private Collider2D collider;
        [SerializeField] private LayerMask collisionMask;
        private bool canJump = true; // if player can jump

        private float collisionTime;
        private float collisionTolerance = .1f;

        private void Update() {
            // jump input
            if (canJump && Input.GetMouseButtonDown(0)) {
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
        public void Jump(Vector2 jumpDirection) {
            // only if jump is viable
            if (JumpCollisionCheck(jumpDirection)) {
                // unparent from land object
                transform.SetParent(null);

                // rotate sprite
                float rotation = Mathf.Atan2(-jumpDirection.y, -jumpDirection.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Euler(0f, 0f, rotation + 90f);

                // jump
                rb.velocity = jumpDirection * jumpSpeed;
                canJump = false;

                // jump anim
                anim.SetBool("canJump", canJump);
                AudioManager.instance.PlayClip(jumpClip);

                // collision tolerance
                StopCoroutine(DisableCollider());
                StartCoroutine(DisableCollider());
            }
        }

        // checks if jump is possible
        private bool JumpCollisionCheck(Vector2 jumpDirection) {
            bool success = false;

            RaycastHit2D hit = Physics2D.Raycast(transform.position, jumpDirection, 1.28f, collisionMask);
            if (hit.collider == null) {
                success = true;
            }

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
            canJump = true;

            // landing anim
            anim.SetBool("canJump", canJump);
            AudioManager.instance.PlayClip(landClip);

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
            Jump(shootDirection * -1);
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
                canJump = true;

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
            if (!canJump) {
                Land(collision.gameObject.transform, collision.ClosestPoint(transform.position));
            }
        }

        private void OnDrawGizmos() {
            Gizmos.color = Color.green;
            Gizmos.DrawRay(transform.position, jumpDirection * 1.28f);
        }
    }
}