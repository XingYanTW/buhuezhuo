using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class SongSelectScript : MonoBehaviour
{
    public TextAsset jsonFile;
    public Sprite SongSelectSprite;
    //public TextMeshProUGUI temp;
    public GameObject SongSelectButton;
    // Start is called before the first frame update
    void Start()
    {
        string jsonText = jsonFile.text;
        Song song = JsonUtility.FromJson<Song>(jsonText);
        GameObject Canvas = GameObject.FindGameObjectWithTag("Canvas");
        if (jsonFile != null)
        {
            Debug.Log("JSON Text: " + jsonText);

            foreach (Songs songs in song.songs)
            {
                Debug.Log(songs.id + " " + songs.name + " " + songs.artist);
                foreach (Level level in songs.levels)
                {
                    Debug.Log("Diff: " + level.diff + ", Level: " + level.level);
                }
            }
        }
        else
        {
            Debug.LogError("JSON file is null!");
        }

        foreach (Songs songs in song.songs)
        {
            foreach (Level level in songs.levels)
            {
                if (level.diff == 2)
                {
                    GameObject songobj = GameObject.Instantiate(SongSelectButton);
                    songobj.transform.SetParent(Canvas.transform, false);
                    songobj.SetActive(true);
                    songobj.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = level.level.ToString();
                    songobj.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = songs.artist;
                    songobj.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = songs.name;
                    Debug.Log(songobj.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text);
                    Debug.Log(songobj.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text);
                    Debug.Log(songobj.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text);
                }

            }

        }

    }

    // Update is called once per frame
    void Update()
    {

    }



}


[System.Serializable]
public class Song
{
    public Songs[] songs;
}

[System.Serializable]
public class Songs
{
    public string id;
    public string name;
    public string artist;
    public Level[] levels;  // Add this line to include levels
}

[System.Serializable]
public class Level
{
    public int diff;
    public int level;
}
