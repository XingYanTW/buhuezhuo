using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{

    public Slider mainSlider;


    // Invoked when the value of the slider changes.
	public void ValueChangeCheck()
	{
		Debug.Log (mainSlider.value);
        GameObject.FindGameObjectWithTag("BGM").GetComponent<BGMScript>().ChangeVolume(mainSlider.value);
	}
}
