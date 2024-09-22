using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{

    public Slider audioSlider;
	//public Toggle audioToggle;
	public Button audioButton;
	//public Image Checkmark;
	public Sprite AudioOn;
	public Sprite AudioOff;
	private float audioVolume;


    // Invoked when the value of the slider changes.
	public void ValueChangeCheck()
	{
		Debug.Log (audioSlider.value);
        GameObject.FindGameObjectWithTag("BGM").GetComponent<BGMScript>().ChangeVolume(audioSlider.value);
		if(audioSlider.value == 0){
			audioButton.GetComponent<Image>().sprite = AudioOff;
		}else{
			audioButton.GetComponent<Image>().sprite = AudioOn;
		}
	}

	public void click(){
		if (audioButton.GetComponent<Image>().sprite.Equals(AudioOn)){
			audioVolume = audioSlider.value;
			audioSlider.value = 0;
			audioButton.GetComponent<Image>().sprite = AudioOff;
		}else{
			audioButton.GetComponent<Image>().sprite = AudioOn;
			audioSlider.value = audioVolume;
		}
	}
}
