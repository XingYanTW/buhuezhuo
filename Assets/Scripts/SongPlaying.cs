using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
namespace Game
{
    public class SongPlaying : MonoBehaviour
    {
        // Start is called before the first frame update
        public GameObject Note_1, Note_2, Note_3, Note_4;
        public GameObject TargetNote_1, TargetNote_2, TargetNote_3, TargetNote_4;
        private GameObject _Note_1, _Note_2, _Note_3, _Note_4;
        public float speed = 1f; // Speed at which the note moves

        void Start()
        {
            //CreateNote(1);
            //CreateNote(4);
        }

        // Update is called once per frame
        void Update()
        {
            // Move the note towards the target every frame
            if (_Note_1 != null && TargetNote_1 != null)
            {
                _Note_1.transform.position = Vector3.MoveTowards(_Note_1.transform.position, TargetNote_1.transform.position, speed * Time.deltaTime);
                if (_Note_1.transform.position.y == TargetNote_1.transform.position.y)
                {
                    Destroy(_Note_1);
                    //CreateNote(1);
                }
            }
            if (_Note_2 != null && TargetNote_2 != null)
            {
                _Note_2.transform.position = Vector3.MoveTowards(_Note_2.transform.position, TargetNote_2.transform.position, speed * Time.deltaTime);
                if (_Note_2.transform.position.y == TargetNote_2.transform.position.y)
                {
                    Destroy(_Note_2);
                    //CreateNote(2);
                }
            }
            if (_Note_3 != null && TargetNote_3 != null)
            {
                _Note_3.transform.position = Vector3.MoveTowards(_Note_3.transform.position, TargetNote_3.transform.position, speed * Time.deltaTime);
                if (_Note_3.transform.position.y == TargetNote_3.transform.position.y)
                {
                    Destroy(_Note_3);
                    //CreateNote(3);
                }
            }
            if (_Note_4 != null && TargetNote_4 != null)
            {
                _Note_4.transform.position = Vector3.MoveTowards(_Note_4.transform.position, TargetNote_4.transform.position, speed * Time.deltaTime);
                if (_Note_4.transform.position.y == TargetNote_4.transform.position.y)
                {
                    Destroy(_Note_4);
                    //CreateNote(4);
                }
            }
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                SceneManager.LoadScene("Main");
            }
            if (Input.GetKey(KeyCode.D))
            {
                TargetNote_1.SetActive(true);
            }
            else
            {
                TargetNote_1.SetActive(false);
            }
            if (Input.GetKey(KeyCode.F))
            {
                TargetNote_2.SetActive(true);
            }
            else
            {
                TargetNote_2.SetActive(false);
            }
            if (Input.GetKey(KeyCode.J))
            {
                TargetNote_3.SetActive(true);
            }
            else
            {
                TargetNote_3.SetActive(false);
            }
            if (Input.GetKey(KeyCode.K))
            {
                TargetNote_4.SetActive(true);
            }
            else
            {
                TargetNote_4.SetActive(false);
            }


            KeyCode[] keys = { KeyCode.D, KeyCode.F, KeyCode.J, KeyCode.K };

            for (int i = 0; i < keys.Length; i++)
            {
                if (Input.GetKeyDown(keys[i]))
                {
                    CreateNote(i+1);
                }

            }
        }

        void CreateNote(int Note)
        {
            GameObject Canvas = GameObject.FindGameObjectWithTag("Canvas");
            switch (Note)
            {
                case 1:
                    _Note_1 = Instantiate(Note_1);
                    _Note_1.SetActive(true);
                    _Note_1.transform.SetParent(Canvas.transform, false);
                    break;
                case 2:
                    _Note_2 = Instantiate(Note_2);
                    _Note_2.SetActive(true);
                    _Note_2.transform.SetParent(Canvas.transform, false);
                    break;
                case 3:
                    _Note_3 = Instantiate(Note_3);
                    _Note_3.SetActive(true);
                    _Note_3.transform.SetParent(Canvas.transform, false);
                    break;
                case 4:
                    _Note_4 = Instantiate(Note_4);
                    _Note_4.SetActive(true);
                    _Note_4.transform.SetParent(Canvas.transform, false);
                    break;
                default:
                    break;
            }

        }
    }

}
