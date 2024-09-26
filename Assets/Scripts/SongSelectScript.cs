using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class SongSelectScript : MonoBehaviour
{
    public TextAsset jsonFile;
    public Sprite SongSelectSprite;
    // Start is called before the first frame update
    void Start()
    {
        string jsonText = jsonFile.text;
        Song song = JsonUtility.FromJson<Song>(jsonText);
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

        GameObject Canvas = GameObject.FindGameObjectWithTag("Canvas");
        GameObject songobj = new GameObject("SongSelectButton");

        songobj.transform.SetParent(Canvas.transform);
        RectTransform rt = songobj.AddComponent<RectTransform>();
        rt.sizeDelta = new Vector2(450f, 75f);
        rt.localPosition = new Vector3(220f, 180f, 0f);
        rt.localScale = new Vector3(1f, 1f, 1f);
        songobj.AddComponent<Image>();
        songobj.GetComponent<Image>().sprite = SongSelectSprite;
        songobj.AddComponent<Button>();
        songobj.SetActive(true);


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
