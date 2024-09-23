using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace Main
{
    public class NotifyScript : MonoBehaviour
    {
        private Animator anim;
        public Image notify;
        public void Colse()
        {
            anim = notify.GetComponent<Animator>();
            anim.Play("Base Layer.NotifyOff");
        }
    }
}

