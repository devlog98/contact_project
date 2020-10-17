using UnityEngine;

/*
 * Checks if gameObject is in specific boundaries
 * Must be used with another component that derives from IBounded
 */ 

namespace devlog98.Boundaries {
    public class CheckBoundaries : MonoBehaviour {
        private IBounded bounded; // object that will be checked

        [Header("Collision")]
        [SerializeField] private Collider2D collider; // gameObject collider
        [SerializeField] private Boundaries boundaries; // scene boundaries

        [Header("Performance")]
        [Range(1, 99)] [SerializeField] private int frameInterval = 1; // how many times check must happen per frames

        // initialize bounded
        private void Start() {
            bounded = GetComponent<IBounded>();
        }

        // boundaries check
        private void LateUpdate() {
            if (Time.frameCount % frameInterval == 0) {
                // out of bounds
                if (!boundaries.Bounds.Intersects(collider.bounds)) {
                    bounded.OutOfBounds();
                }
            }
        }
    }
}