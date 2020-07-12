using TMPro;
using UnityEngine;

namespace devlog98.UI {
    public class UIController : MonoBehaviour {
        public static UIController instance; // singleton

        [SerializeField] private TextMeshProUGUI landCounter;

        // singleton setup
        private void Awake() {
            if (instance != null && instance != this) {
                Destroy(this.gameObject);
            }
            else {
                instance = this;
            }
        }

        public void UpdateCounter(int value) {
            landCounter.text = value.ToString();
        }
    }
}