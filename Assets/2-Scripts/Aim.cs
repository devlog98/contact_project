using UnityEngine;

namespace devlog98.Mouse {
    public class Aim : MonoBehaviour {
        public static Aim instance; // singleton

        private Vector2 mousePosition; // cursor position onscreen

        // singleton setup
        private void Awake() {
            if (instance != null && instance != this) {
                Destroy(this.gameObject);
            }
            else {
                instance = this;
            }
        }

        // unlock and hide mouse cursor
        private void Start() {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = false;
        }

        // update cursor position
        private void Update() {
            mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.position = mousePosition;
        }

        // return direction based on position
        public Vector2 GetAimDirection(Vector2 position) {
            return (mousePosition - position).normalized; 
        }

        // return mouse position
        public Vector2 GetAimPosition() {
            return mousePosition;
        }
    }
}