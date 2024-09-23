using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
namespace Main
{
    public class OptionScript : MonoBehaviour
    {
        public void click()
        {
            SceneManager.LoadScene(1);
        }
    }
}

