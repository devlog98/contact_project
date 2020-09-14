﻿using UnityEngine;
using devlog98.Mouse;
using System.Collections;
using devlog98.UI;
using devlog98.Audio;
using devlog98.Ammunition;
using System.Collections.Generic;

namespace devlog98.Player {
    public class Player : MonoBehaviour {
        [Header("Movement")]
        [SerializeField] private Rigidbody2D rb;
        [SerializeField] private Collider2D collider;
        private Vector2 currentDirection = Vector2.zero; // movement direction
        private float currentSpeed; // movement speed

        [Header("Jump")]
        [SerializeField] private int maxJumpCount; // number of times player can jump without touching ground
        [SerializeField] private float jumpSpeed; // default jump speed
        [SerializeField] private float jumpSpeedBonus; // speed increase when chaining multiple jumps
        [SerializeField] private float jumpSpeedAngle; // max angle difference for bonus speed to be applied
        private int jumpCount; // how many times has the player jumped right now

        [Header("Walk")]
        [SerializeField] private float walkSpeed;
        [SerializeField] Transform walkPoint;
        private bool isWalking;

        [Header("Shoot")]
        [SerializeField] private List<Bullet> bullets; // list of player ammunition
        [SerializeField] private float shootSpeed; // default shoot speed
        [SerializeField] private float shootSpeedBonus; // speed increase when chaining multiple shots
        [SerializeField] private float shootSpeedAngle; // max angle difference for bonus speed to be applied

        [Header("Collision")]
        [SerializeField] private LayerMask collisionMask; // mask that can collide with player
        [SerializeField] private float collisionDistance; // how far mask check will go
        private float collisionTolerance = .1f; // period collider will be turned off for smooth launching
        private bool isGrounded = true; // if player is on the ground

        [Header("Animation")]
        [SerializeField] private Animator anim;
        [SerializeField] private Transform sprite;

        [Header("Audio")]
        [SerializeField] private AudioClip jumpClip;
        [SerializeField] private AudioClip landClip;
        [SerializeField] private AudioClip deathClip;

        private int landCounter; // times player landed on the ground

        private void Update() {
            // jump
            if (Input.GetMouseButtonDown(0)) {
                Vector2 jumpDirection = Aim.instance.GetAimDirection(transform.position);
                Jump(jumpDirection);
            }

            // shoot
            if (Input.GetMouseButtonDown(1)) {
                Vector2 shootDirection = Aim.instance.GetAimDirection(transform.position);
                Shoot(shootDirection);
            }

            CheckOutOfBounds();
        }

        private void FixedUpdate() {
            Walk();
        }

        // jumps in a given direction
        private void Jump(Vector2 jumpDirection) {
            // jump if possible
            if (JumpCheck(jumpDirection)) {
                if (isGrounded) {
                    // unparent from land object
                    transform.SetParent(null);

                    // collision tolerance
                    StopCoroutine(DisableCollider());
                    StartCoroutine(DisableCollider());

                    isGrounded = false;
                }

                // jump
                Launch(jumpDirection, jumpSpeed, jumpSpeedBonus, jumpSpeedAngle);
                jumpCount++;

                // jump feedback
                anim.SetBool("canJump", isGrounded);
                AudioManager.instance.PlayClip(jumpClip);
            }
        }

        // checks if jump is possible
        private bool JumpCheck(Vector2 jumpDirection) {
            // jump counter check
            if (jumpCount >= maxJumpCount) {
                return false;
            }
            // jump in mid air check
            else if (!isGrounded) {
                return true;
            }
            // jump in ground check
            else {
                float jumpAngle = Vector2.Angle(jumpDirection, transform.up); // jumpDirection angle based on player transform

                if (jumpAngle < 90) {
                    RaycastHit2D hit = Physics2D.Raycast(transform.position, jumpDirection, collisionDistance, collisionMask);

                    // if no object will collide with player jump
                    if (hit.collider == null) {
                        return true;
                    }
                }

                WalkStart();
                return false;
            }
        }

        // lands on the correct rotation
        private void Land(Transform landTransform, Vector2 landPoint) {
            // parent to land object
            transform.SetParent(landTransform);
            isGrounded = true;

            // landing
            rb.velocity = Vector2.zero;
            landCounter++;

            // landing feedback
            anim.SetBool("canJump", isGrounded);
            AudioManager.instance.PlayClip(landClip);

            // rotate sprite
            Vector2 landDirection = (landPoint - (Vector2)transform.position).normalized;
            float rotation = Mathf.Atan2(-landDirection.y, -landDirection.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, rotation - 90f);

            // reset movement
            jumpCount = 0;
            currentDirection = Vector2.zero;
            currentSpeed = 0;

            // update land counter
            UIController.instance.UpdateCounter(landCounter);
        }

        // disable collider on beginning of jump
        private IEnumerator DisableCollider() {
            collider.gameObject.SetActive(false);
            yield return new WaitForSeconds(collisionTolerance);
            collider.gameObject.SetActive(true);
        }

        private void WalkStart() {
            Vector2 aimPosition = Aim.instance.GetAimPosition();

            Vector2 landPoint = transform.parent.GetComponent<Collider2D>().ClosestPoint(aimPosition);
            Vector2 landDirection = ((Vector2)aimPosition - landPoint).normalized;

            RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.up, -collisionDistance, collisionMask);

            walkPoint.parent = transform.parent;
            walkPoint.position = landPoint + (landDirection * hit.distance);

            isWalking = true;
        }

        private void Walk() {
            if (WalkCheck()) {
                //calculates interpolated distance player will walk
                Vector2 movePosition = new Vector2();
                movePosition.x = Mathf.MoveTowards(transform.position.x, walkPoint.position.x, walkSpeed * Time.deltaTime);
                movePosition.y = Mathf.MoveTowards(transform.position.y, walkPoint.position.y, walkSpeed * Time.deltaTime);

                //executes walking
                rb.MovePosition(movePosition);
            }
        }

        private bool WalkCheck() {
            
            if (isWalking) {
                isWalking = transform.position != walkPoint.position ? true : false;
            }

            return isWalking;
        }

        private void Shoot(Vector2 shootDirection) {
            // get bullet
            Bullet bullet = bullets.Find(x => !x.gameObject.activeSelf);

            // shoot if bullet is available
            if (bullet != null) {
                bullet.Shoot(transform.position, shootDirection);

                // launch player if in mid air
                if (!isGrounded) {
                    shootDirection = shootDirection * -1;
                    Launch(shootDirection, shootSpeed, shootSpeedBonus, shootSpeedAngle);
                }
            }
        }

        // launches player in direction
        private void Launch(Vector2 direction, float speed, float speedBonus, float angle) {
            // rotate sprite
            float rotation = Mathf.Atan2(-direction.y, -direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, rotation + 90f);

            // modify speed depending on movement angle
            float directionAngle = Vector2.Angle(direction, currentDirection);
            if (currentSpeed == 0 || directionAngle > angle) {
                currentSpeed = speed;
            }
            else {
                currentSpeed += speedBonus;
            }

            // launch
            rb.velocity = direction * currentSpeed;

            // set new direction
            currentDirection = direction;
        }

        // player death
        private void CheckOutOfBounds() {
            // out of bounds
            if (transform.position.x < -10.24 || transform.position.x > 10.24 || transform.position.y < -5.12 || transform.position.y > 6.4) {
                Die();
            }
        }

        // land on collision
        private void OnTriggerEnter2D(Collider2D collision) {
            if (!isGrounded && collision.gameObject.tag != "Bullet") {
                Land(collision.gameObject.transform, collision.ClosestPoint(transform.position));
            }
        }

        // player death
        private void Die() {
            // death
            rb.velocity = Vector3.zero; // reset velocity

            transform.position = new Vector3(0f, -3.65f, 0f); // reset position
            transform.rotation = Quaternion.Euler(Vector3.zero);

            transform.SetParent(null); // reset parent

            isGrounded = true; // reset jump

            // death feedback
            AudioManager.instance.PlayClip(deathClip);

            // reset land counter
            landCounter = 0;
            UIController.instance.UpdateCounter(landCounter);
        }

        // debug
        private void OnDrawGizmos() {
            Gizmos.color = Color.green;
            Gizmos.DrawRay(transform.position, currentDirection * collisionDistance);

            Gizmos.color = Color.red;
            Gizmos.DrawRay(transform.position, transform.up * 1.28f);
            Gizmos.DrawRay(transform.position, transform.right * -1.28f);
            Gizmos.DrawRay(transform.position, transform.right * 1.28f);

            Gizmos.color = Color.yellow;
            Gizmos.DrawCube(walkPoint.position, Vector3.one * 0.32f);
        }
    }
}