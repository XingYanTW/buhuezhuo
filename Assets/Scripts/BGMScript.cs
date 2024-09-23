using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Main
{
    public class BGMScript : MonoBehaviour
    {
        private AudioSource _audioSource;
        void Awake()
        {
            GameObject[] objs = GameObject.FindGameObjectsWithTag("BGM");

            if (objs.Length > 1)
            {
                Destroy(this.gameObject);
            }

            DontDestroyOnLoad(this.gameObject);
            _audioSource = GetComponent<AudioSource>();

        }

        public void ChangeVolume(float volume)
        {
            _audioSource.volume = volume;
        }
    }
}

