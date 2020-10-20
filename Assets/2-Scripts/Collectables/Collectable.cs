using UnityEngine;

/*
 * Basic Collectable script
 * Useful for items like coins and parts to be collected in huge quantity
 * Must have a Collider2D set as Trigger to work
 */ 

namespace devlog98.Collectables {
    public class Collectable : MonoBehaviour {
        [SerializeField] private int value; // Collectable value
        private bool isCollected; // if collectable is collected or not

        public bool IsCollected {
            set {
                isCollected = value;
                this.gameObject.SetActive(!isCollected);
                Debug.Log("I, " + this.gameObject.name + ", was collected and gave you the value of " + this.value);
            }
        }
    }
}