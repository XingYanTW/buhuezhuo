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



        enum TokenType
        {
            BPM,        // (120)
            Beats,      // {4}
            Note,       // 4, 3, etc.
            Rest,      // ,
            Slash,      // /
            NewLine,    // \n
            Comment,    // # ...
        }

        class PostInfo
        {
            public int Line { get; }
            public Range Range { get; }

            public PostInfo(int line, Range position)
            {
                Line = line;
                Range = position;
            }

            public override string ToString() => $"{Line}:{Range}";
        }

        class Token : PostInfo
        {
            public Token(TokenType type, string value, int line, Range position)
                : base(line, position)
            {
                Type = type;
                Value = value;
            }

            public Token(TokenType type, string value, int line, int start, int lan = 1)
                : this(type, value, line, new Range(start, start + lan)) { }

            public TokenType Type { get; }
            public string Value { get; }

            public override string ToString() => $"{Type}({Value}) at {base.ToString()}";
        }

        class ErrorPos : Exception
        {
            public PostInfo PositionInfo { get; }


            public ErrorPos(string message, int line, Range position)
                : base($"Error at {line}:{position} - {message}")
            {
                PositionInfo = new PostInfo(line, position);
            }

            public ErrorPos(string message, PostInfo posInfo)
                : base($"Error at {posInfo} - {message}")
            {
                PositionInfo = posInfo;
            }

            public ErrorPos(string message, int line, int start, int lan = 1)
                : this(message, line, new Range(start, start + lan))
            {
            }

            public override string ToString() => $"{Message} (at {PositionInfo})";
        }

        class Note
        {
            public int Lane { get; }
            public float Time { get; }

            public Note(int lane, float time)
            {
                Lane = lane;
                Time = time;
            }
        }



        public GameObject Note_1, Note_2, Note_3, Note_4;
        public GameObject TargetNote_1, TargetNote_2, TargetNote_3, TargetNote_4;
        public GameObject Judge;
        //public Sprite Judge_Perfect, Judge_Perfect_Plus, Judge_Perfect_Minus;
        //public Sprite Judge_Great, Judge_Great_Plus, Judge_Great_Minus, Judge_Miss;
        public Sprite Judge_Perfect, Judge_Great, Judge_Good, Judge_Miss;
        public GameObject JudgeAudio;
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
        private Boolean isPause = true;


        private float bpm;
        private float secPerBeat;
        //private List<Note> notes = new List<Note>();
        private float songTime = 0f;
        private readonly Dictionary<Note, bool> noteSpawned = new();

        int countPerfect = 0, countGreat = 0, countMiss = 0, combo = 0;
        int totalNotes;

        public GameObject countPerfectOBJ, countGreatOBJ, countMissOBJ, countComboOBJ;

        List<Note> notes;

        void Start()
        {
            playing = false;
            //StartCoroutine(TestNote());
            Playing.GetComponent<TextMeshProUGUI>().text = gameObject.AddComponent<PlayButton>().GetPlaySong();
            AudioClip _BGM = Resources.Load<AudioClip>("Songs/" + gameObject.AddComponent<PlayButton>().GetPlaySong() + "/track");

            BGM.GetComponent<AudioSource>().clip = _BGM;
            var ChartData = Resources.Load<TextAsset>("Songs/" + gameObject.AddComponent<PlayButton>().GetPlaySong() + "/chart");
            //Debug.Log(ChartData);
            //ParseChart(ChartData.ToString());
            string chart = ChartData.ToString();
            secPerBeat = 60f / bpm;
            var tokens = LexicalAnalysis(chart, out var tokenWarnings);
            // foreach (var token in tokens)
            //   Debug.Log(token);
            foreach (var warning in tokenWarnings)
                PrintWarning(chart, warning);

            notes = ParseTokens(tokens, out var noteWarnings);
            foreach (var note in notes)
                Debug.Log($"{note.Time}: {note.Lane}");

            foreach (var warning in noteWarnings)
                PrintWarning(chart, warning);
            foreach (var note in notes)
            {
                noteSpawned[note] = false;
            }

            totalNotes = notes.Count; // 計算總音符數量
            StartCoroutine(StartSongPlaying());
        }

        static void PrintWarning(string input, ErrorPos warning)
        {
            var posInfo = warning.PositionInfo;
            var posRange = posInfo.Range;

            int errorStartPos = posRange.Start.Value;
            int errorEndPos = posRange.End.Value;
            string lineStr = input.Split('\n')[posInfo.Line];
            string errorStr = lineStr[errorStartPos..errorEndPos];

            int startPos = Math.Max(0, errorStartPos - 10);
            int endPos = Math.Min(lineStr.Length, posRange.End.Value + 10);
            int errorStartOffset = errorStartPos - startPos;

            string warningMessage = $"{warning.Message}:\n";
            if (startPos != 0)
            {
                warningMessage += "...";
                errorStartOffset += 3;
            }
            warningMessage += lineStr[startPos..errorStartPos];

            warningMessage += $"<color=yellow>{lineStr[errorStartPos..errorEndPos]}</color>";

            warningMessage += lineStr[errorEndPos..endPos];
            if (endPos < lineStr.Length) warningMessage += "...";
            warningMessage += "\n";

            string caret = new string(' ', errorStartOffset) + new string('^', errorStr.Length);
            warningMessage += caret;

            Debug.LogWarning(warningMessage);
        }

        private List<Token> LexicalAnalysis(string input, out List<ErrorPos> warnings)
        {
            warnings = new List<ErrorPos>();
            List<Token> tokens = new List<Token>();
            int line = 0, position = 0;

            for (int i = 0; i < input.Length; i++, position++)
            {
                char c = input[i];

                switch (c)
                {
                    case '(':
                        {
                            int endBpm = input.IndexOf(')', i);
                            if (endBpm == -1)
                            {
                                warnings.Add(new ErrorPos("Unclosed BPM token", line, position, Math.Max(input.Length, 10)));
                                break;
                            }

                            int len = endBpm - i;
                            tokens.Add(new Token(TokenType.BPM, input.Substring(i + 1, len - 1), line, position, len));
                            i = endBpm;
                            position += len;
                        }
                        break;

                    case '{':
                        {
                            int endBeats = input.IndexOf('}', i);
                            if (endBeats == -1)
                            {
                                warnings.Add(new ErrorPos("Unclosed Beats token", line, position, Math.Max(input.Length, 10)));
                                break;
                            }

                            int len = endBeats - i;
                            tokens.Add(new Token(TokenType.Beats, input.Substring(i + 1, len - 1), line, position, len));
                            i = endBeats;
                            position += len;
                        }
                        break;

                    case ',':
                        tokens.Add(new Token(TokenType.Rest, ",", line, position));
                        break;

                    case '/':
                        tokens.Add(new Token(TokenType.Slash, "/", line, position));
                        break;

                    case '\n':
                        tokens.Add(new Token(TokenType.NewLine, "\\n", line, position));
                        line++;
                        position = -1;
                        break;

                    case '#':
                        int endComment = input.IndexOf('\n', i);
                        string comment = endComment == -1
                            ? input[(i + 1)..]
                            : input.Substring(i + 1, endComment - i - 1);
                        tokens.Add(new Token(TokenType.Comment, comment, line, position));
                        i = endComment == -1 ? input.Length : endComment - 1;
                        break;

                    case ' ':
                        break;

                    default:
                        if (char.IsDigit(c))
                            tokens.Add(new Token(TokenType.Note, c.ToString(), line, position));
                        else
                            warnings.Add(new ErrorPos("Invalid character", line, position));
                        break;
                }
            }

            return tokens;
        }

        private List<Note> ParseTokens(List<Token> tokens, out List<ErrorPos> warnings)
        {
            warnings = new List<ErrorPos>();
            List<Note> notes = new List<Note>();
            HashSet<int> currentNotes = new HashSet<int>();

            float bpm = 120;
            int beatsPerMeasure = 4;
            float currentTime = 0;

            Token? lastToken = null;

            foreach (var token in tokens)
            {
                if (token.Type != TokenType.Slash && token.Type != TokenType.Note) currentNotes.Clear();
                if (token.Type != TokenType.Note && lastToken?.Type == TokenType.Slash)
                    warnings.Add(new ErrorPos("Slash without note", token.Line, token.Range));

                switch (token.Type)
                {
                    case TokenType.BPM: // (120)
                        if (float.TryParse(token.Value, out float bpmValue))
                        {
                            if (bpmValue < 0) warnings.Add(new ErrorPos("BPM cannot be negative", token.Line, token.Range));
                            else if (bpmValue == 0) warnings.Add(new ErrorPos("BPM cannot be 0", token.Line, token.Range));
                            else bpm = bpmValue;
                        }
                        else warnings.Add(new ErrorPos("Invalid BPM", token.Line, token.Range));
                        break;

                    case TokenType.Beats: // {<value>}
                        if (float.TryParse(token.Value, out float beatsPerMeasureValue))
                        {
                            if (beatsPerMeasureValue % 1 != 0)
                                warnings.Add(new ErrorPos("Invalid beats per measure, must be an integer", token.Line, token.Range));
                            else if (beatsPerMeasureValue < 1)
                                warnings.Add(new ErrorPos("Invalid beats per measure, must be at least 1", token.Line, token.Range));
                            else if (beatsPerMeasureValue == 0)
                                warnings.Add(new ErrorPos("Invalid beats per measure, cannot be 0", token.Line, token.Range));
                            else beatsPerMeasure = (int)beatsPerMeasureValue;
                        }
                        else
                            warnings.Add(new ErrorPos("Invalid beats per measure", token.Line, token.Range));
                        break;

                    case TokenType.Slash: // /
                        if (lastToken?.Type == TokenType.Slash)
                            warnings.Add(new ErrorPos("Double slash", token.Line, token.Range));
                        break;

                    case TokenType.Rest: // ,
                        currentTime += 60f / bpm * (4f / beatsPerMeasure);
                        break;

                    case TokenType.Note: // number
                        if (int.TryParse(token.Value, out int lane))
                        {
                            if (lane < 1 || lane > 4)
                                warnings.Add(new ErrorPos("Invalid note", token.Line, token.Range));
                            if (!currentNotes.Add(lane))
                                warnings.Add(new ErrorPos("Duplicate note", token.Line, token.Range));
                            else notes.Add(new Note(lane, currentTime));
                        }
                        else
                            warnings.Add(new ErrorPos("Invalid note", token.Line, token.Range));
                        break;

                    default:
                        // Ignore other token types
                        break;
                }

                lastToken = token;
            }

            return notes;
        }

        void Update()
        {
            if (!isPause)
            {
                songTime = BGM.GetComponent<AudioSource>().time;
                //Debug.Log("Current song time: " + songTime);
                songTimeOBJ.GetComponent<TextMeshProUGUI>().text = songTime.ToString();
                int index = 0;
                foreach (var note in notes)
                {
                    //Debug.Log("Note time: " + note.Time + ", Note lane: " + note.Lane);
                    index++;
                    float timeToSpawn = note.Time;
                    //Debug.Log("Note index: " + index + ", Time to spawn: " + timeToSpawn);
                    timeToSpawnOBJ.GetComponent<TextMeshProUGUI>().text = timeToSpawn.ToString();
                    if (songTime >= timeToSpawn - 2f && noteSpawned[note] == false)
                    {
                        Debug.Log("Spawning note at lane: " + note.Lane);
                        CreateNote(note.Lane);
                        noteSpawned[note] = true;
                    }
                }
            }

            // Update the count of perfect, great, miss, and combo
            countPerfectOBJ.GetComponent<TextMeshProUGUI>().text = "Perfect: " + countPerfect;
            countGreatOBJ.GetComponent<TextMeshProUGUI>().text = "Great: " + countGreat;
            countMissOBJ.GetComponent<TextMeshProUGUI>().text = "Miss: " + countMiss;
            countComboOBJ.GetComponent<TextMeshProUGUI>().text = "Combo: " + combo + "/" + totalNotes;

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
                }
                else
                {
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

        IEnumerator StartSongPlaying()
        {
            yield return new WaitForSeconds(1f);
            playing = true;
            isPause = false;
            BGM.GetComponent<AudioSource>().Play();
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
                        countMiss++; // 增加 miss 計數
                        combo = 0; // 重置 combo 計數
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
                    countPerfect++;
                    combo++;
                }
                else if (timeDiff <= perfectWindow)
                {
                    DisplayJudgeResult(Judge_Perfect);
                    countPerfect++;
                    combo++;
                }
                else if (timeDiff <= greatWindow * 2)
                {
                    DisplayJudgeResult(Judge_Great);
                    countGreat++;
                    combo++;
                }
                else if (timeDiff <= greatWindow)
                {
                    DisplayJudgeResult(Judge_Great);
                    countGreat++;
                    combo++;
                }
                else if (timeDiff <= missWindow * 2)
                {
                    DisplayJudgeResult(Judge_Miss);
                    countMiss++;
                    combo = 0; // Reset combo on miss
                }
                else if (timeDiff <= missWindow)
                {
                    DisplayJudgeResult(Judge_Miss);
                    countMiss++;
                    combo = 0; // Reset combo on miss
                }
                else
                {
                    JudgeTime.GetComponent<TextMeshProUGUI>().text = timeDiff.ToString();
                    return;
                }

                JudgeTime.GetComponent<TextMeshProUGUI>().text = timeDiff.ToString();

                // Play judge audio
                JudgeAudio.GetComponent<AudioSource>().Play();

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
            SceneManager.LoadScene("SongPlaying");
        }

        public void ExitButton()
        {
            SceneManager.LoadScene("SongSelect");
        }
    }
}
