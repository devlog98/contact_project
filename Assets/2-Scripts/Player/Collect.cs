using devlog98.Collectables;
using System.Collections.Generic;
using UnityEngine;

namespace devlog98.Actor {
    public class Collect : MonoBehaviour {
        private readonly List<string> collectableTags = new List<string>() { "Collectable" }; // all possible tags for the collectables

        // collect element and increases count
        private void GetCollectable(GameObject gameObject) {
            Collectable collectable = gameObject.GetComponent<Collectable>();

            // increase count

            collectable.IsCollected = true; // collects element
        }

        // collision detection
        private void OnTriggerEnter2D(Collider2D collision) {
            if (collectableTags.Contains(collision.tag)) {
                GetCollectable(collision.gameObject);
            }
        }
    }
}