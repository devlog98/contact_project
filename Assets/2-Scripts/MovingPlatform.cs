using System.Collections.Generic;
using UnityEngine;

namespace devlog98.Obstacle {
    public class MovingPlatform : MonoBehaviour {
        [Header("Path")]
        [SerializeField] private List<Transform> points; // movement path points
        [SerializeField] private bool cyclicPath; // when activated, makes platform go from the last point to the first, instead of going back and forth
        [SerializeField] private float moveSpeed; // platform speed

        private int pointIndex; // index from current path point
        private Vector2 targetPosition; // position from current path point

        // initialize movement variables
        private void Start() {
            targetPosition = points[pointIndex].position;
        }

        // check arrival at path points
        private void Update() {
            if (transform.position == points[pointIndex].position) {
                NextPoint();
            }
        }

        // move platform
        private void FixedUpdate() {
            transform.position = Vector2.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
        }

        // swap to next point on path
        private void NextPoint() {
            pointIndex++;

            // end of path points
            if (pointIndex == points.Count) {
                pointIndex = 0;

                // activate back and forth movement if platform path is not cyclic
                if (!cyclicPath) {
                    points.Reverse();
                    pointIndex++;
                }
            }

            // set new target position
            targetPosition = points[pointIndex].position;
        }

        // debug
        private void OnDrawGizmos() {
            if (points.Count > 0) {
                for (int i = 0; i < points.Count - 1; i++) {
                    Gizmos.DrawLine(points[i].position, points[i + 1].position);
                }

                if (cyclicPath) {
                    Gizmos.DrawLine(points[points.Count - 1].position, points[0].position);
                }
            }
        }
    }
}