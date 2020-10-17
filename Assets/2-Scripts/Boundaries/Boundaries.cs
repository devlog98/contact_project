using UnityEngine;

/*
 * Creates Bounds on Scene
 */
 
namespace devlog98.Boundaries {
    public class Boundaries : MonoBehaviour {
        [Header("Bounds")]
        [SerializeField] private Vector2 boundsSize; // how large boundaries will be
        [SerializeField] private Color boundsColor = new Vector4(0, 0, 0, 1); // debug color

        private Bounds bounds; // used to check if objects are inside boundaries
        public Bounds Bounds { get { return bounds; } }

        // initialize bounds
        private void Awake() {
            bounds = new Bounds(transform.position, new Vector2(boundsSize.x, boundsSize.y));
        }

        // debug
        private void OnDrawGizmos() {
            Gizmos.color = boundsColor;
            Gizmos.DrawWireCube(transform.position, boundsSize);
        }
    }
}