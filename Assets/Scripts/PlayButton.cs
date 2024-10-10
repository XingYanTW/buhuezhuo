using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayButton : MonoBehaviour
{

    private static string CurrentSong;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void PlaySong(){
        Debug.Log(CurrentSong);
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
