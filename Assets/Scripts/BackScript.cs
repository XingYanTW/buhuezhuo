using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Option
{
    public class BackScript : MonoBehaviour
    {
        public void click()
        {
            SceneManager.LoadScene("Main");
        }
    }
}
