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
		public Sprite Audio100;
		public Sprite Audio60;
		public Sprite Audio0;
		private float audioVolume;

		public System.Options option = new();


		// Invoked when the value of the slider changes.
		public void ValueChangeCheck()
		{
			Debug.Log(audioSlider.value);
			GameObject.FindGameObjectWithTag("BGM").GetComponent<Main.BGMScript>().ChangeVolume(audioSlider.value);
			if (audioSlider.value == 0)
			{
				audioButton.GetComponent<Image>().sprite = Audio0;
			}
			else if (audioSlider.value<=0.6){
				audioButton.GetComponent<Image>().sprite = Audio60;
			}else
			{
				audioButton.GetComponent<Image>().sprite = Audio100;
			}
			option.audioVolume = audioSlider.value;
			//string _json = JsonUtility.ToJson(option);
            //File.WriteAllText(Application.persistentDataPath + "/config.json", _json);
		}

		public void click()
		{
			if (audioButton.GetComponent<Image>().sprite.Equals(Audio0))
			{
				audioVolume = audioSlider.value;
				audioSlider.value = 0;
				option.audioVolume = 0;
				audioButton.GetComponent<Image>().sprite = Audio0;
			}
			else
			{
				audioButton.GetComponent<Image>().sprite = Audio100;
				audioSlider.value = audioVolume;
				option.audioVolume = audioVolume;
			}
			string _json = JsonUtility.ToJson(option);
            File.WriteAllText(Application.persistentDataPath + "/config.json", _json);
		}

		//void Start(){
		//	audioSlider.value = option.audioVolume;
		//}

	}
}

