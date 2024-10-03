using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game
{
    public class SongPlaying : MonoBehaviour
    {
        public GameObject Note_1, Note_2, Note_3, Note_4;
        public GameObject TargetNote_1, TargetNote_2, TargetNote_3, TargetNote_4;
        public float speed = 1f; // Speed at which the note moves

        private List<GameObject> _Note_1_List = new List<GameObject>();
        private List<GameObject> _Note_2_List = new List<GameObject>();
        private List<GameObject> _Note_3_List = new List<GameObject>();
        private List<GameObject> _Note_4_List = new List<GameObject>();

        void Update()
        {
            // Move all notes in the lists towards their targets
            MoveNotes(_Note_1_List, TargetNote_1);
            MoveNotes(_Note_2_List, TargetNote_2);
            MoveNotes(_Note_3_List, TargetNote_3);
            MoveNotes(_Note_4_List, TargetNote_4);

            // Handle input
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                SceneManager.LoadScene("Main");
            }

            HandleTargetVisibility(KeyCode.D, TargetNote_1);
            HandleTargetVisibility(KeyCode.F, TargetNote_2);
            HandleTargetVisibility(KeyCode.J, TargetNote_3);
            HandleTargetVisibility(KeyCode.K, TargetNote_4);

            // Instantiate notes on key press
            KeyCode[] keys = { KeyCode.D, KeyCode.F, KeyCode.J, KeyCode.K };

            for (int i = 0; i < keys.Length; i++)
            {
                if (Input.GetKeyDown(keys[i]))
                {
                    CreateNote(i + 1);
                }
            }
        }

        void MoveNotes(List<GameObject> noteList, GameObject target)
        {
            for (int i = noteList.Count - 1; i >= 0; i--)
            {
                if (noteList[i] != null && target != null)
                {
                    noteList[i].transform.position = Vector3.MoveTowards(noteList[i].transform.position, target.transform.position, speed * Time.deltaTime);

                    // Destroy note if it reaches the target and remove it from the list
                    if (noteList[i].transform.position.y == target.transform.position.y)
                    {
                        Destroy(noteList[i]);
                        noteList.RemoveAt(i);
                    }
                }
            }
        }

        void HandleTargetVisibility(KeyCode key, GameObject target)
        {
            if (Input.GetKey(key))
            {
                target.SetActive(true);
            }
            else
            {
                target.SetActive(false);
            }
        }

        void CreateNote(int note)
        {
            GameObject Canvas = GameObject.FindGameObjectWithTag("Canvas");
            GameObject newNote = null;

            switch (note)
            {
                case 1:
                    newNote = Instantiate(Note_1);
                    _Note_1_List.Add(newNote);
                    break;
                case 2:
                    newNote = Instantiate(Note_2);
                    _Note_2_List.Add(newNote);
                    break;
                case 3:
                    newNote = Instantiate(Note_3);
                    _Note_3_List.Add(newNote);
                    break;
                case 4:
                    newNote = Instantiate(Note_4);
                    _Note_4_List.Add(newNote);
                    break;
            }

            if (newNote != null)
            {
                newNote.SetActive(true);
                newNote.transform.SetParent(Canvas.transform, false);
            }
        }
    }
}
