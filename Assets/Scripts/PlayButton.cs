using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayButton : MonoBehaviour
{

    private static string CurrentSong;

    public GameObject LoadingScreen, SongName, SongJacket;
    private Animator loadingScreenAnimator;

    // Start is called before the first frame update
    void Start()
    {
        loadingScreenAnimator = LoadingScreen.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void PlaySong(){
        Debug.Log(CurrentSong);
        LoadingScreen.SetActive(true);
        loadingScreenAnimator.SetTrigger("SlideUp");

        SongName.GetComponent<TextMeshProUGUI>().text = CurrentSong;
        var _jacket = Resources.Load<Sprite>("Songs/"+CurrentSong+"/Jacket");
        SongJacket.GetComponent<Image>().sprite = _jacket;
        //SceneManager.LoadScene("SongPlaying");

        //switch scene to SongPlaying after 3 seconds
        StartCoroutine(LoadScene());
    }
    
    IEnumerator LoadScene(){
        yield return new WaitForSeconds(3);
        SceneManager.LoadScene("SongPlaying");
    }

    public void SetPlaySong(string SongID){
        CurrentSong = SongID;
        Debug.Log(CurrentSong);
    }

    public string GetPlaySong(){
        return CurrentSong;
    }
}
