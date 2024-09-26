using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
namespace System
{
    public class JsonScript : MonoBehaviour
    {

        public static Options option = new Options();

        // Start is called before the first frame update
        void Awake()
        {
            //option.audioVolume = 0.7f;
            //string _json = JsonUtility.ToJson(option);
            //Debug.Log(_json);
            //Debug.Log(Application.persistentDataPath + "/config.json");
            //File.WriteAllText(Application.persistentDataPath + "/config.json", _json);
            //JsonUtility.FromJson(File.ReadAllText(Application.persistentDataPath + "/config.json"));
        }

        // Update is called once per frame
        //void Update()
        //{

        //}

        /*public static void SaveJsonFile()
        {
            string _json = JsonUtility.ToJson(option);
            Debug.Log(_json);
            File.WriteAllText(Application.persistentDataPath + "/config.json", _json);
        }*/
    }
    [System.Serializable]
    public class Options
    {
        public float audioVolume { set; get; }
    }

}


