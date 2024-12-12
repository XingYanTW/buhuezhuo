using System;
using System.IO;
using UnityEngine;

namespace Assets.Scripts
{
    public class JsonScript : MonoBehaviour
    {
        public static Options Option = new Options();

        // Start is called before the first frame update
        private void Awake()
        {
            Option.AudioVolume = 0.7f;
            string _json = JsonUtility.ToJson(Option);
            Debug.Log(_json);
            Debug.Log(Application.persistentDataPath + "/config.json");
            File.WriteAllText(Application.persistentDataPath + "/config.json", _json);
            Option = JsonUtility.FromJson<Options>(File.ReadAllText(Application.persistentDataPath + "/config.json"));
        }

        // Update is called once per frame
        //void Update()
        //{

        //}

        public static void SaveJsonFile()
        {
            string _json = JsonUtility.ToJson(Option);
            Debug.Log(_json);
            File.WriteAllText(Application.persistentDataPath + "/config.json", _json);
        }
    }

    [Serializable]
    public class Options
    {
        public float AudioVolume { set; get; }
    }
}