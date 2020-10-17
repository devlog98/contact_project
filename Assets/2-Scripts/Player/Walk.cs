using devlog98.Mouse;
using PathCreation;
using System.Collections.Generic;
using UnityEngine;

/*
 * Walk on surfaces using PathCreator bezier curve
 */ 

namespace devlog98.Player {
    public class Walk : MonoBehaviour {
        private Dictionary<int, VertexPath> pathDictionary = new Dictionary<int, VertexPath>();

        [Header("Movement")]
        [SerializeField] private float walkSpeed;
        [SerializeField] private Transform walkPoint;
        private VertexPath path;

        private bool isWalking;
        public bool IsWalking { get { return isWalking; } }

        // initialize walk 
        public void ExecuteStart() {
            Vector2 aimPosition = Aim.instance.GetAimPosition();

            pathDictionary.TryGetValue(transform.parent.GetInstanceID(), out path);

            if (path == null) {
                PathCreator pathCreator = transform.parent.GetComponentInChildren<PathCreator>();
                path = pathCreator.path;
                pathDictionary.Add(transform.parent.GetInstanceID(), path);
            }

            walkPoint.position = path.GetClosestPointOnPath(aimPosition);
            walkPoint.parent = transform.parent;

            Vector2 landPoint = transform.parent.GetComponent<Collider2D>().ClosestPoint(aimPosition);
            Vector2 landDirection = ((Vector2)aimPosition - landPoint).normalized;

            // rotate sprite
            float rotation = Mathf.Atan2(-landDirection.y, -landDirection.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, rotation + 90f);

            isWalking = true;
        }

        public void ExecuteUpdate() {
            if (WalkCheck()) {
                //executes walking
                transform.position = Vector3.MoveTowards(transform.position, walkPoint.position, walkSpeed * Time.deltaTime);
            }
        }

        public void ExecuteEnd() {
            isWalking = false;
        }

        private bool WalkCheck() {
            if (isWalking) {
                RaycastHit2D hit = Physics2D.Linecast(transform.position, walkPoint.position, Player.instance.CollisionMask);

                // if no object will collide with player jump
                if (hit.collider != null) {
                    isWalking = false;
                }
                else {
                    isWalking = transform.position != walkPoint.position ? true : false;
                }
            }

            return isWalking;
        }

        // debug
        private void OnDrawGizmos() {
            if (walkPoint != null) {
                Gizmos.color = Color.yellow;
                Gizmos.DrawCube(walkPoint.position, Vector3.one * 0.32f);
            }
        }
    }
}