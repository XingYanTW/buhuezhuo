using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System;
using System.Linq;

namespace Game
{
    public class SongPlaying : MonoBehaviour
    {



        // Define a structure to store note data
        public class Note
        {
            public int lane;
            public float beat;

            public Note(int lane, float beat)
            {
                this.lane = lane;
                this.beat = beat;
            }
        }



        public GameObject Note_1, Note_2, Note_3, Note_4;
        public GameObject TargetNote_1, TargetNote_2, TargetNote_3, TargetNote_4;
        public GameObject Judge;
        //public Sprite Judge_Perfect, Judge_Perfect_Plus, Judge_Perfect_Minus;
        //public Sprite Judge_Great, Judge_Great_Plus, Judge_Great_Minus, Judge_Miss;
        public Sprite Judge_Perfect, Judge_Great, Judge_Good, Judge_Miss;
        public GameObject JudgeTime, Playing;
        public float speed = 1f; // Speed at which the note moves

        // Time windows in seconds (convert milliseconds to seconds)
        public float perfectWindow = 33;  // 33ms
        public float greatWindow = 66;    // 66ms
        public float missWindow = 200;

        private List<GameObject> _Note_1_List = new List<GameObject>();
        private List<GameObject> _Note_2_List = new List<GameObject>();
        private List<GameObject> _Note_3_List = new List<GameObject>();
        private List<GameObject> _Note_4_List = new List<GameObject>();

        // Track the time each note is created
        private List<float> _Note_1_Times = new List<float>();
        private List<float> _Note_2_Times = new List<float>();
        private List<float> _Note_3_Times = new List<float>();
        private List<float> _Note_4_Times = new List<float>();

        public GameObject BGM;

        public GameObject timeToSpawnOBJ, songTimeOBJ, BPMOBJ;

        public GameObject pause;

        Coroutine judgeResetCoroutine;

        private Boolean playing;
        private Boolean isPause=true;


        private float bpm;
        private float secPerBeat;
        private List<Note> notes = new List<Note>();
        private float songTime = 0f;
        private Dictionary<Note, bool> noteSpawned = new Dictionary<Note, bool>();

        void Start()
        {
            playing = false;
            //StartCoroutine(TestNote());
            Playing.GetComponent<TextMeshProUGUI>().text = gameObject.AddComponent<PlayButton>().GetPlaySong();
            AudioClip _BGM = Resources.Load<AudioClip>("Songs/" + gameObject.AddComponent<PlayButton>().GetPlaySong() + "/track");
            
            BGM.GetComponent<AudioSource>().clip = _BGM;
            var ChartData = Resources.Load<TextAsset>("Songs/" + gameObject.AddComponent<PlayButton>().GetPlaySong() + "/chart");
            //Debug.Log(ChartData);
            ParseChart(ChartData.ToString());
            secPerBeat = 60f / bpm;
            StartCoroutine(StartSongPlaying());
            
        }

        void Update()
        {
            if (!isPause)
            {
                songTime = BGM.GetComponent<AudioSource>().time;
                songTimeOBJ.GetComponent<TextMeshProUGUI>().text = songTime.ToString();
                int index = 0;
                foreach (var note in notes)
                {
                    index++;
                    float timeToSpawn = note.beat * secPerBeat * index;
                    timeToSpawnOBJ.GetComponent<TextMeshProUGUI>().text = timeToSpawn.ToString();
                    if (songTime >= timeToSpawn - 2f && noteSpawned[note] == false)
                    {
                        //Debug.Log(timeToSpawn);
                        CreateNote(note.lane);
                        noteSpawned[note] = true;
                    }
                }
            }


            if (!BGM.GetComponent<AudioSource>().isPlaying && (playing = true))
            {
                //SceneManager.LoadScene("SongSelect");
            }

            // Move all notes in the lists towards their targets
            if (!isPause)
            {
                MoveNotes(_Note_1_List, TargetNote_1);
                MoveNotes(_Note_2_List, TargetNote_2);
                MoveNotes(_Note_3_List, TargetNote_3);
                MoveNotes(_Note_4_List, TargetNote_4);
            }


            // Handle input
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                //SceneManager.LoadScene("SongSelect");
                pause.SetActive(!pause.activeSelf);
                isPause = !isPause;
                if (isPause)
                {
                    BGM.GetComponent<AudioSource>().Pause();
                }else{
                    BGM.GetComponent<AudioSource>().UnPause();
                }

            }

            HandleTargetVisibility(KeyCode.D, TargetNote_1);
            HandleTargetVisibility(KeyCode.F, TargetNote_2);
            HandleTargetVisibility(KeyCode.J, TargetNote_3);
            HandleTargetVisibility(KeyCode.K, TargetNote_4);

            // Instantiate notes on key press and calculate judgment
            KeyCode[] keys = { KeyCode.D, KeyCode.F, KeyCode.J, KeyCode.K };

            for (int i = 0; i < keys.Length; i++)
            {
                if (Input.GetKeyDown(keys[i]))
                {
                    HandleJudgment(i + 1);
                }
            }
        }

        IEnumerator StartSongPlaying(){
            yield return new WaitForSeconds(10f);
            playing = true;
            isPause = false;
            BGM.GetComponent<AudioSource>().Play();
        }

        void ParseChart(string chartData)
        {
            int bpmStart = chartData.IndexOf('(') + 1;
            int bpmEnd = chartData.IndexOf(')');
            bpm = float.Parse(chartData.Substring(bpmStart, bpmEnd - bpmStart));

            string noteData = chartData.Substring(bpmEnd + 1);
            string[] notesArray = noteData.Split(',');

            float currentBeat = 0f;

            foreach (var note in notesArray)
            {
                if (note.StartsWith("{"))
                {
                    // Handle beats
                    int beatStart = note.IndexOf('{') + 1;
                    int beatEnd = note.IndexOf('}');
                    currentBeat = float.Parse(note.Substring(beatStart, beatEnd - beatStart));
                }
                else if (!string.IsNullOrEmpty(note))
                {
                    // Handle lane notes
                    string[] laneNotes = note.Split(',');

                    foreach (var laneNote in laneNotes)
                    {
                        if (string.IsNullOrWhiteSpace(laneNote))
                        {
                            // Handle empty note explicitly
                            notes.Add(new Note(-1, currentBeat)); // -1 indicates an empty lane
                        }
                        else if (int.TryParse(laneNote, out int lane))
                        {
                            notes.Add(new Note(lane, currentBeat));
                        }
                    }

                }
            }
            Debug.Log("notesArray:"+notesArray);

            foreach (var note in notes)
            {
                noteSpawned[note] = false;
            }
        }


        void MoveNotes(List<GameObject> noteList, GameObject target)
        {
            for (int i = noteList.Count - 1; i >= 0; i--)
            {
                if (noteList[i] != null && target != null)
                {
                    noteList[i].transform.position = Vector3.MoveTowards(noteList[i].transform.position, target.transform.position, speed * Time.deltaTime);
                    JudgeTime.GetComponent<TextMeshProUGUI>().text = Mathf.Round(noteList[i].transform.position.y) + "/" + Mathf.Round(target.transform.position.y);
                    // Destroy note if it reaches the target and remove it from the list
                    if (Mathf.Round(noteList[i].transform.position.y) == Mathf.Round(target.transform.position.y))
                    {
                        Destroy(noteList[i]);
                        noteList.RemoveAt(i);
                        DisplayJudgeResult(Judge_Miss);

                    }
                }
            }
        }

        IEnumerator JudgeReset(GameObject judge)
        {
            yield return new WaitForSeconds(0.5f);
            judge.SetActive(false);
        }

        IEnumerator TestNote()
        {
            for (; ; )
            {
                yield return new WaitForSeconds(.5f);
                CreateNote(1);
                yield return new WaitForSeconds(.5f);
                CreateNote(2);
                yield return new WaitForSeconds(.5f);
                CreateNote(3);
                yield return new WaitForSeconds(.5f);
                CreateNote(4);
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
            //Debug.Log(note);
            GameObject Canvas = GameObject.FindGameObjectWithTag("Canvas");
            GameObject newNote = null;

            switch (note)
            {
                case 1:
                    newNote = Instantiate(Note_1);
                    _Note_1_List.Add(newNote);
                    _Note_1_Times.Add(Time.time); // Store the creation time of the note
                    break;
                case 2:
                    newNote = Instantiate(Note_2);
                    _Note_2_List.Add(newNote);
                    _Note_2_Times.Add(Time.time);
                    break;
                case 3:
                    newNote = Instantiate(Note_3);
                    _Note_3_List.Add(newNote);
                    _Note_3_Times.Add(Time.time);
                    break;
                case 4:
                    newNote = Instantiate(Note_4);
                    _Note_4_List.Add(newNote);
                    _Note_4_Times.Add(Time.time);
                    break;
            }

            if (newNote != null)
            {
                newNote.SetActive(true);
                newNote.transform.SetParent(Canvas.transform, false);
            }
        }

        void HandleJudgment(int note)
        {
            List<GameObject> currentNoteList = null;
            List<float> currentNoteTimes = null;
            GameObject target = null;

            switch (note)
            {
                case 1:
                    currentNoteList = _Note_1_List;
                    currentNoteTimes = _Note_1_Times;
                    target = TargetNote_1;
                    break;
                case 2:
                    currentNoteList = _Note_2_List;
                    currentNoteTimes = _Note_2_Times;
                    target = TargetNote_2;
                    break;
                case 3:
                    currentNoteList = _Note_3_List;
                    currentNoteTimes = _Note_3_Times;
                    target = TargetNote_3;
                    break;
                case 4:
                    currentNoteList = _Note_4_List;
                    currentNoteTimes = _Note_4_Times;
                    target = TargetNote_4;
                    break;
            }

            if (currentNoteList != null && currentNoteList.Count > 0)
            {
                GameObject noteObject = currentNoteList[0]; // Check the first note in the list
                float noteTime = currentNoteTimes[0];
                float timeDiff = Mathf.Abs(noteObject.transform.position.y - target.transform.position.y);


                if (timeDiff <= perfectWindow * 2)
                {
                    DisplayJudgeResult(Judge_Perfect);
                }
                else if (timeDiff <= perfectWindow)
                {
                    DisplayJudgeResult(Judge_Perfect);
                }
                else if (timeDiff <= greatWindow * 2)
                {
                    DisplayJudgeResult(Judge_Great);
                }
                else if (timeDiff <= greatWindow)
                {
                    DisplayJudgeResult(Judge_Great);
                }
                else if (timeDiff <= missWindow * 2)
                {
                    DisplayJudgeResult(Judge_Miss);
                }
                else if (timeDiff <= missWindow)
                {
                    DisplayJudgeResult(Judge_Miss);
                }
                else
                {
                    JudgeTime.GetComponent<TextMeshProUGUI>().text = timeDiff.ToString();
                    return;
                }

                JudgeTime.GetComponent<TextMeshProUGUI>().text = timeDiff.ToString();

                // Remove note from list and destroy it
                Destroy(noteObject);
                currentNoteList.RemoveAt(0);
                currentNoteTimes.RemoveAt(0);
            }
        }

        void DisplayJudgeResult(Sprite judgmentSprite)
        {
            Judge.GetComponent<Image>().sprite = judgmentSprite;
            Judge.SetActive(true);

            // Reset the coroutine if it's already running
            if (judgeResetCoroutine != null)
            {
                StopCoroutine(judgeResetCoroutine);
            }

            judgeResetCoroutine = StartCoroutine(JudgeReset(Judge));
        }

        public void RemuseButton()
        {
            isPause = false;
            pause.SetActive(false);
            BGM.GetComponent<AudioSource>().UnPause();
        }

        public void RestartButton()
        {

        }

        public void ExitButton()
        {
            SceneManager.LoadScene("SongSelect");
        }
    }
}
