using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace Option
{
	public class AudioManager : MonoBehaviour
	{

		public Slider audioSlider;
		public Button audioButton;
		public Sprite AudioOn;
		public Sprite AudioOff;
		private float audioVolume;

		public System.Options option = new System.Options();


		// Invoked when the value of the slider changes.
		public void ValueChangeCheck()
		{
			Debug.Log(audioSlider.value);
			GameObject.FindGameObjectWithTag("BGM").GetComponent<Main.BGMScript>().ChangeVolume(audioSlider.value);
			if (audioSlider.value == 0)
			{
				audioButton.GetComponent<Image>().sprite = AudioOff;
			}
			else
			{
				audioButton.GetComponent<Image>().sprite = AudioOn;
			}
		}

		public void click()
		{
			if (audioButton.GetComponent<Image>().sprite.Equals(AudioOn))
			{
				audioVolume = audioSlider.value;
				audioSlider.value = 0;
				option.audioVolume = 0;
				audioButton.GetComponent<Image>().sprite = AudioOff;
			}
			else
			{
				audioButton.GetComponent<Image>().sprite = AudioOn;
				audioSlider.value = audioVolume;
				option.audioVolume = audioVolume;
			}
			//string _json = JsonUtility.ToJson(option);
			//File.WriteAllText(Application.persistentDataPath+"/config.json", _json);
		}

		//void Start(){
		//	audioSlider.value = option.audioVolume;
		//}

	}
}

