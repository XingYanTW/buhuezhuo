using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
namespace Main
{
    public class StartScript : MonoBehaviour
    {
        //public Animator anim;
        private Animator anim;
        public Image notify;
        public TextMeshProUGUI head;
        public TextMeshProUGUI body;

        public void Start()
        {
            KeyManager keyManager = new KeyManager();
            keyManager.initkey();
            //QualitySettings.vSyncCount = 1;
            //Application.targetFrameRate = 60;
        }

        public void Click()
        {
            /*head.text = "該帳號沒有登出";
            body.text = "當前使用的帳號沒有正常登出\n請15分鐘後再試";
            anim = notify.GetComponent<Animator>();
            anim.Play("Base Layer.NotifyOn");
            notify.GetComponent<AudioSource>().Play();*/
            SceneManager.LoadScene("SongSelect");
            BGMScript.DestoryBGM();
        }
    }
}

