using System.IO;
using Main;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class AudioManager : MonoBehaviour
    {
        public Slider audioSlider;
        public Button audioButton;
        public Sprite Audio100;
        public Sprite Audio60;
        public Sprite Audio0;
        private float audioVolume;

        // Use the static option variable from JsonScript
        private Options option => JsonScript.Option;

        // Invoked when the value of the slider changes.
        public void ValueChangeCheck()
        {
            Debug.Log(audioSlider.value);
            GameObject.FindGameObjectWithTag("BGM").GetComponent<BGMScript>().ChangeVolume(audioSlider.value);
            if (audioSlider.value == 0)
            {
                audioButton.GetComponent<Image>().sprite = Audio0;
            }
            else if (audioSlider.value <= 0.6)
            {
                audioButton.GetComponent<Image>().sprite = Audio60;
            }
            else
            {
                audioButton.GetComponent<Image>().sprite = Audio100;
            }
            option.AudioVolume = audioSlider.value;
            JsonScript.SaveJsonFile();
        }

        public void Click()
        {
            if (audioButton.GetComponent<Image>().sprite.Equals(Audio0))
            {
                audioVolume = audioSlider.value;
                audioSlider.value = 0;
                option.AudioVolume = 0;
                audioButton.GetComponent<Image>().sprite = Audio0;
            }
            else
            {
                audioButton.GetComponent<Image>().sprite = Audio100;
                audioSlider.value = audioVolume;
                option.AudioVolume = audioVolume;
            }
            JsonScript.SaveJsonFile();
        }

        private void Start()
        {
            audioSlider.value = option.AudioVolume;
        }
    }
}