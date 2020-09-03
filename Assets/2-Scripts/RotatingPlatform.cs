using System.Collections.Generic;
using UnityEngine;

namespace devlog98.Obstacle {
    public class RotatingPlatform : MonoBehaviour {
        [Header("Rotation")]
        [SerializeField] private List<Transform> angles; // rotation angle points
        [SerializeField] private bool cyclicRotation; // when activated, makes platform go from the last angle to the first, instead of going back and forth
        [SerializeField] private float rotateSpeed; // platform speed

        private int angleIndex; // index from current rotation angle
        private Quaternion targetRotation; // rotation from current angle point

        // initialize rotation variables
        private void Start() {
            targetRotation = angles[angleIndex].rotation;
        }

        // check arrival at rotation angles
        private void Update() {
            if (transform.rotation == angles[angleIndex].rotation) {
                NextPoint();
            }
        }

        // rotate platform
        private void FixedUpdate() {
            float step = rotateSpeed * Time.deltaTime;
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, step);
        }

        // swap to next point on rotation
        private void NextPoint() {
            angleIndex++;

            // end of rotation angles
            if (angleIndex == angles.Count) {
                angleIndex = 0;

                // activate back and forth movement if platform rotation is not cyclic
                if (!cyclicRotation) {
                    angles.Reverse();
                    angleIndex++;
                }
            }

            // set new target rotation
            targetRotation = angles[angleIndex].rotation;
        }

        // debug
        private void OnDrawGizmos() {
            if (angles.Count > 0) {
                for (int i = 0; i < angles.Count - 1; i++) {
                    Gizmos.DrawRay(transform.position, angles[i].up * 2.64f);
                }
            }
        }
    }
}