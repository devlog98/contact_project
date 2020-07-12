using UnityEngine;

namespace devlog98.Audio {
    public class AudioManager : MonoBehaviour {
        public static AudioManager instance; //static instance can be called any time   

        [SerializeField] private AudioSource sourceFX;

        // initializing singleton
        void Awake() {
            if (instance != null && instance != this) {
                Destroy(this.gameObject);
            }
            else {
                instance = this;
            }
        }

        //UI sounds
        public void PlayClip(AudioClip clip) {
            sourceFX.PlayOneShot(clip);
        }
    }
}