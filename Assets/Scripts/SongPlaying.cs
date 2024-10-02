using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
namespace Game
{
    public class SongPlaying : MonoBehaviour
    {
        // Start is called before the first frame update
        public GameObject Note_1;
        public GameObject TargetNote_1;
        private GameObject Note;
        public float speed = 1f; // Speed at which the note moves

        void Start()
        {
            CreateNote();
        }

        // Update is called once per frame
        void Update()
        {
            // Move the note towards the target every frame
            if (Note != null && TargetNote_1 != null)
            {
                Note.transform.position = Vector3.MoveTowards(Note.transform.position, TargetNote_1.transform.position, speed * Time.deltaTime);
                if (Note.transform.position.Equals(TargetNote_1.transform.position))
                {
                    Destroy(Note);
                    CreateNote();
                }
            }
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                SceneManager.LoadScene("Main");
            }
        }

        void CreateNote()
        {
            GameObject Canvas = GameObject.FindGameObjectWithTag("Canvas");
            Note = Instantiate(Note_1);
            Note.SetActive(true);
            Note.transform.SetParent(Canvas.transform, false);
        }
    }

}
