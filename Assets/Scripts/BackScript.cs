using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using Assets.Scripts;

namespace Option
{
    public class BackScript : MonoBehaviour
    {
        //public Options option = new Options();
        public void click()
        {
            SceneManager.LoadScene("Main");
            //string _json = JsonUtility.ToJson(option);
            //File.WriteAllText(Application.persistentDataPath + "/config.json", _json);
        }
    }
}
