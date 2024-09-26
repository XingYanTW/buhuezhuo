using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Option
{
    public class BackScript : MonoBehaviour
    {
        public System.Options option = new System.Options();
        public void click()
        {
            SceneManager.LoadScene("Main");
            //string _json = JsonUtility.ToJson(option);
            //File.WriteAllText(Application.persistentDataPath + "/config.json", _json);
        }
    }
}
