using UnityEngine;

namespace KeyboardScripts.Keyboard
{
    public class KeySound : MonoBehaviour
    {
        public KeyboardInfo KeyboardInfo;
    
        [SerializeField]
        private AudioSource audioSource;

        public void PlaySoundOnKeyPress()
        {
            if (KeyboardInfo != null && KeyboardInfo.IsSoundFeedbackEnabled && audioSource != null)
            {
                audioSource.Play();
            }
        }


    }
}
