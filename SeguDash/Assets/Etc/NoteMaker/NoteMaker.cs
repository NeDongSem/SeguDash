using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;
using UnityEngine.Networking;
using System.Text;
using System.Text.RegularExpressions;

public class NoteMaker : MonoBehaviour
{
    [SerializeField] Button musicButton;
    [SerializeField] TMP_Text musicButtonText;

    [SerializeField] TMP_InputField bPMInputField;
    [SerializeField] Button speedDownButton;
    [SerializeField] TMP_Text speedText;
    [SerializeField] Button speedUpButton;
    [SerializeField] TMP_InputField gapInputField;
    [SerializeField] TMP_Text noteText;

    [SerializeField] Button plusTimeButton;
    [SerializeField] Button playAndPauseButton;
    [SerializeField] Button minusTimeButton;
    [SerializeField] TMP_InputField syncInputField; 

    [SerializeField] Slider timeSlider;

    [SerializeField] TMP_InputField bGMVolumeInputField;
    [SerializeField] TMP_InputField noteVolumeInputField;
    [SerializeField] TMP_InputField timeInpuField;

    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip audioClip;
    [SerializeField] AudioSource noteAudioSource;

    [Space]
    [SerializeField] GameObject folder;
    [SerializeField] GameObject linePrefab;
    [SerializeField] GameObject notePrefab;
    GameObject[] linePrefabList;
    GameObject[] fShortNotePrefabList;
    GameObject[] jShortNotePrefabList;
    int[] fShortNoteInt;
    int[] jShortNoteInt;
    [SerializeField] Sprite[] noteSprite;

    [Space]
    [SerializeField] int frame;

    [Space]
    [SerializeField] int bPM;
    [SerializeField] float timeSignature;

    [Space]
    [SerializeField] float gap;
    [SerializeField] float speed;

    [Space]
    [SerializeField] bool isPlaying;
    [SerializeField] float sec;
    [SerializeField] float nowBeatFloat;
    [SerializeField] int nowBeat;
    [SerializeField] float maxBeat;
    int additionalBeat;
    float sync;

    int amountOfBeat;
    int amount;
    float minus;
    float gS;
    float bT;
    int noteInfo=999;

    // Start is called before the first frame update
    void Start()
    {
        //바탕화면 경로 가져옴
        string path=System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop);
        //직접 지정한 폴더 
        string directoryName = "/Please_Input_wav";
        if(Directory.Exists(path+directoryName))
        {
            Debug.Log("directory is already here");
        }
        else
        {
            Debug.Log("what about make directory");
            Directory.CreateDirectory(path+ directoryName);
        }
        ////////////////////////////여기까지 음악 경로 확인 후 체크

        //프레임 조절
        Application.targetFrameRate = frame;
        //해상도 설정, 전체화면은 아님
        Screen.SetResolution(1920, 1080, false);
        //오디오 소스 가져오기
        audioSource = GetComponent<AudioSource>();
        //오디오 클립 넣기
        audioSource.clip = audioClip;
        //노트 이미지 담을 공간
        noteSprite = new Sprite[12];
        //이미지 이름은 숫자로 되어있고 리소시즈 폴더에서 가져옴
        for (int i = 1; i < 12; i++)
        {
            string s = (i).ToString();
            noteSprite[i] = Resources.Load<Sprite>(s);
        }
        //오디오 클립이 없을 경우
        if (!audioClip)
        {
            //musicButton이 약간 제지하는 버튼인 것 같음. 예외 설정
            musicButton.gameObject.SetActive(true);
            musicButtonText.text = "바탕화면의 \'Please_Input_wav\'폴더에 음악을 넣어주세요.\n음악 파일 이름은 \'0.wav\'입니다.";
        }

        //Setting();
        //NoteSetting();
        //GapUpdate();

        //현재 볼륨 알려주는 텍스트
        bGMVolumeInputField.text = (audioSource.volume*100).ToString();
        //노트 볼륨 (북? 박수? 같은 소리 였던거 같은데 어제 들었을 때, 어쨌든 노트 소리 (쳤을 때 나는 소리 같음))
        noteVolumeInputField.text = (noteAudioSource.volume*100).ToString();

    }

    // Update is called once per frame
    void Update()
    {
        //LoadCSV가 먼저 불리는 것 같음 

        AddNote();
        MoveNote();
        ChangeNote();
        /*
        if (Input.GetKeyDown(KeyCode.R))
        {
            GapReset();
        }
        */
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (isPlaying == false)
            {
                if (nowBeat > 1)
                {
                    additionalBeat--;
                    sec -= 60 / bT;
                    //sec = (nowBeat + nowBeatFloat) * 60;
                    timeInpuField.text = sec.ToString("F2");
                    timeSlider.value = sec;
                }
            }
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            isPlaying = !isPlaying;


            if (isPlaying == false)
            {
                timeInpuField.text = sec.ToString("F2");
                timeSlider.value = sec;
                audioSource.Stop();
            }
            else if (isPlaying == true)
            {
                audioSource.time = sec;
                audioSource.Play();
            }
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            if (isPlaying == false)
            {
                additionalBeat++;
                sec += 60 / bT;
                //sec = (nowBeat + nowBeatFloat) * 60;
                timeInpuField.text = sec.ToString("F2");
                timeSlider.value = sec;
            }
        }

    }

    void Setting()
    {
        //없을 때
        if (!audioClip)
        {
            musicButton.gameObject.SetActive(true);
            musicButtonText.text = "바탕화면의 \'Please_Input_wav\'폴더에 음악을 넣어주세요.\n음악 파일 이름은 \'0.wav\'입니다.";
        }
        else
        {
            //제지 풀고
            musicButton.gameObject.SetActive(false);
            //화면 줄 갯순가? 이건 잘 모르겠음
            amount = ((int)(960 / (gap * speed)) + 1) * 2 + 3;
            //화면이 지나가면서? 지나간거? 센건가? 화면? 얼마나 지나갔나?
            minus = (1920 - (((int)(960 / (gap * speed)) + 1) * 2 + 3) * (gap * speed)) * 0.5f;

            //지나간 수인가? 시간? 네이밍은 갭스피드 같은데..
            gS = gap * speed;

            //bpm이 분당 비트수라서 비트/분 이니까 초로 하면 비트/60초. // 풀어보면 -> 노래길이 * 비트 / 60초 / 60초?
            //보면 느낌상 노래 시간을 분으로 바꾼다음 bpm을 곱한 것 같음, 그런데 int형이라 잘 모르겠음
            maxBeat = (audioClip.length * bPM) / 60;

            gapInputField.text = gap.ToString();

            //스피드가 배속이였구나
            speedText.text = "X" + speed.ToString();

            audioSource.clip = audioClip;
            //오디오 얼마나 남았는지 알려주는 바
            timeSlider.maxValue = audioClip.length;
        }
        
        

        
    }

    void NoteSetting()
    {
        amountOfBeat = (int)(bPM * 4/*4분*/* timeSignature);/*16분음표*/; //나중에 타임시그니쳐에 따라 변경 가능하게?
        fShortNoteInt = new int[amountOfBeat];
        jShortNoteInt = new int[amountOfBeat];
        bT = bPM * timeSignature;
        bPMInputField.text = bPM.ToString();
    }


    void GapReset()
    {
        
        for (int i = 0; i < amount; i++)
        {
            Destroy(linePrefabList[i]);
            Destroy(fShortNotePrefabList[i]);
            Destroy(jShortNotePrefabList[i]);
        }

        Setting();
        GapUpdate();
    }

    void NoteReset()
    {
        NoteSetting();
    }


    //주요 봐야 할 부분
    void GapUpdate()
    {
        

        linePrefabList = new GameObject[amount];
        fShortNotePrefabList = new GameObject[amount];
        jShortNotePrefabList = new GameObject[amount];

        for (int i= 0; i<amount;i++)
        {
            linePrefabList[i] = Instantiate<GameObject>(linePrefab, folder.transform);
            linePrefabList[i].transform.localPosition = new Vector3((i-(int)(amount/2))*(gS), 0, 0);
        }
        for (int i = 0; i < amount; i++)
        {
            fShortNotePrefabList[i] = Instantiate<GameObject>(notePrefab, folder.transform);
            fShortNotePrefabList[i].transform.localPosition = new Vector3((i - (int)(amount / 2)) * (gS) , 0, 0);

            jShortNotePrefabList[i] = Instantiate<GameObject>(notePrefab, folder.transform);
            jShortNotePrefabList[i].transform.localPosition = new Vector3((i - (int)(amount / 2)) * (gS) , 0, 0);
        }
    }

    void ChangeNote()
    {
        switch (noteInfo)
        {
            case 999:
                noteText.text = "0~9까지를 눌러 노트를 결정해주세요.(넘버패드 제외)";
                break;
            
            case 01:
                noteText.text = "작은 노트(벌레)";
                break;
            case 2:
                noteText.text = "중형 노트(피폭단)";
                break;
            case 3:
                noteText.text = "대형 노트(정령)";
                break;
            case 4:
                noteText.text = "망치 노트(국자)";
                break;
            case 5:
                noteText.text = "톱니 노트(똥)";
                break;
            case 6:
                noteText.text = "롱 노트(별)";
                break;
            case 7:
                noteText.text = "유령 노트(망령)";
                break;
            case 8:
                noteText.text = "음표";
                break;
            case 9:
                noteText.text = "보스 미사일(일렉트릭 볼)";
                break;
            case 10:
                noteText.text = "하트(햄버거)";
                break;
            case 11:
                noteText.text = "양쪽 동시 노트(대나무)";
                break;
        }

        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            noteInfo = 01;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            noteInfo = 02;
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            noteInfo = 3;
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            noteInfo = 4;
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            noteInfo = 5;
        }
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            noteInfo = 6;
        }
        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            noteInfo = 7;
        }
        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            noteInfo = 8;
        }
        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            noteInfo = 9;
        }
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            noteInfo = 10;
        }
        if (Input.GetKeyDown(KeyCode.Minus))
        {
            noteInfo = 11;
        }

    }

    void AddNote()
    {
        //마우스 위치 vec에 담아놓음
        Vector3 vec = Input.mousePosition;
        //(nowBeat + nowBeatFloat) * (gap * speed);
        if (Input.GetMouseButtonDown(0))
        {

                int xPosition = (int)((int)((vec.x - minus + (nowBeat + nowBeatFloat) * (gS)) / (gS)));
            if (vec.y > 200 && vec.y < 540)
            {
                if(noteInfo!=11&&noteInfo!=999)
                {
                    fShortNoteInt[xPosition] = noteInfo;
                }
                else if(noteInfo==11)
                {
                    fShortNoteInt[xPosition] = noteInfo;
                    jShortNoteInt[xPosition] = noteInfo;
                }

            }
            if (vec.y > 540 && vec.y < 880)
            {
                //int xPosition = (int)((int)((vec.x - minus + (nowBeat + nowBeatFloat) * (gS)) / (gS)));
                

                if (noteInfo != 11 && noteInfo != 999)
                {
                    jShortNoteInt[xPosition] = noteInfo;
                }
                else if (noteInfo == 11)
                {
                    fShortNoteInt[xPosition] = noteInfo;
                    jShortNoteInt[xPosition] = noteInfo;
                }
            }

        }
        if (Input.GetMouseButtonDown(1))
        {
                int xPosition = (int)((int)((vec.x - minus + (nowBeat + nowBeatFloat) * (gS)) / (gS)));
            if (vec.y > 200 && vec.y < 540)
            {
                //fShortNoteInt[xPosition] = 0;

                if (jShortNoteInt[xPosition]!=11&& jShortNoteInt[xPosition] != 99)
                {
                    fShortNoteInt[xPosition] = 0;
                }
                else if (noteInfo == 11)
                {
                    fShortNoteInt[xPosition] = 0;
                    jShortNoteInt[xPosition] = 0;
                }

            }
            if (vec.y > 540 && vec.y < 880)
            {
                //int xPosition = (int)((int)((vec.x - minus + (nowBeat + nowBeatFloat) * (gS)) / (gS)));
                //jShortNoteInt[xPosition] = 0;

                if (jShortNoteInt[xPosition] != 11 && jShortNoteInt[xPosition] != 99)
                {
                    jShortNoteInt[xPosition] = 0;
                }
                else if (noteInfo == 11)
                {
                    fShortNoteInt[xPosition] = 0;
                    jShortNoteInt[xPosition] = 0;
                }

            }
        }
    }

    // 중요 봐야 할 부분
    void MoveNote()
    {
        if(isPlaying==true&&sec<audioClip.length)
        {
            sec += Time.deltaTime;
            timeInpuField.text = sec.ToString("F2");
            timeSlider.value = sec;
        }
        else
        {
            isPlaying = false;
            
        }

        
        nowBeatFloat= ((sec * bT+sync) % 60)/60;
        nowBeat = (int)((sec * bT) / 60)+ additionalBeat;
        //nowBeat = (int)(bPM / (sec * 60));
        //nowBeatFloat = (bPM % (sec * 60)) / (sec * 60);

        
        
        for (int i = 0; i < amount; i++)
        {
            int fShortYPosition = (fShortNoteInt[i + nowBeat] == 0 ? -400 : -200);
            int jShortYPosition = (jShortNoteInt[i + nowBeat] == 0 ? 400 : 200);

            linePrefabList[i].transform.localPosition = new Vector3((i - (int)(amount / 2)) * (gS), 0, 0)
                                                      + new Vector3(-((gS) *nowBeatFloat),0,0);

            fShortNotePrefabList[i].transform.localPosition = new Vector3((i - (int)(amount / 2)) * (gS), 0, 0)
                                                      + new Vector3(-((gS) * nowBeatFloat ), fShortYPosition, 0);
            fShortNotePrefabList[i].GetComponent<Image>().sprite = noteSprite[fShortNoteInt[i + nowBeat]];

            jShortNotePrefabList[i].transform.localPosition = new Vector3((i - (int)(amount / 2)) * (gS), 0, 0)
                                                      + new Vector3(-((gS) * nowBeatFloat), jShortYPosition, 0);
            jShortNotePrefabList[i].GetComponent<Image>().sprite = noteSprite[jShortNoteInt[i + nowBeat]];

            
        }
    }

    public void LoadCSV()
    {
        if (isPlaying == false)
        {
            string path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop);
            string directoryName = "/Please_Input_wav";

            //string LINE_SPLIT_RE = @"\r\n|\n\r|\n|\r";
            //string[] lines;

            if (File.Exists(path + directoryName + "/0.csv"))
            {

                //string source;
                StreamReader sR = new StreamReader(path + directoryName + "/0.csv");
                //source = sR.ReadToEnd();
                //sR.Close();

                bool endOfFile = false;
                int indexNum = 0;
                while (!endOfFile)
                {
                    string data_String = sR.ReadLine();
                    if (data_String == null)
                    {
                        endOfFile = true;
                        break;
                    }
                    var data_values = data_String.Split(',');
                    if(indexNum==0)
                    {


                        bPM = int.Parse(data_values[0]);
                        sync = int.Parse(data_values[1]);
                        //bPM = int.Parse(bPMInputField.text);
                        syncInputField.text = sync.ToString();
                        GapReset();
                        NoteReset();
                    }
                    else
                    {
                        fShortNoteInt[indexNum-1] = int.Parse(data_values[0]);
                        jShortNoteInt[indexNum - 1] = int.Parse(data_values[1]);
                    }
                    
                    indexNum++;
                }

            }
        }
    }

    public void SaveCSV()
    {
        if (isPlaying == false)
        {


            string path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop);
            string directoryName = "/Please_Input_wav";
            if (Directory.Exists(path + directoryName))
            {

                if (File.Exists(path + directoryName + "/0.csv"))
                {

                    Debug.Log("csv is alread here");

                    string[] output = new string[fShortNoteInt.Length];
                    for (int i = 0; i < output.Length; i++)
                    {
                        output[i] = fShortNoteInt[i] + "," + jShortNoteInt[i];
                    }
                    int length = output.GetLength(0);
                    StringBuilder sb = new StringBuilder();
                    string firstLine = bPM + "," + sync;
                    sb.AppendLine(firstLine);
                    for (int i = 0; i < length; i++)
                    {
                        sb.AppendLine(output[i]);
                    }
                    string filepath = path + directoryName;
                    if (!Directory.Exists(filepath))
                    {
                        Directory.CreateDirectory(filepath);
                    }
                    StreamWriter outStream = System.IO.File.CreateText(filepath + "/0.csv");
                    outStream.Write(sb);
                    outStream.Close();
                }
                else
                {

                    File.Create(path + directoryName + "/0.csv");
                    Debug.Log("csv is created");
                    string[] output = new string[fShortNoteInt.Length];
                    for (int i = 0; i < output.Length; i++)
                    {
                        output[i] = fShortNoteInt[i] + "," + jShortNoteInt[i];
                    }
                    int length = output.GetLength(0);
                    StringBuilder sb = new StringBuilder();
                    string firstLine = bPM + "," + sync;
                    sb.AppendLine(firstLine);
                    for (int i = 0; i < length; i++)
                    {
                        sb.AppendLine(output[i]);
                    }
                    string filepath = path + directoryName;
                    if (!Directory.Exists(filepath))
                    {
                        Directory.CreateDirectory(filepath);
                    }
                    StreamWriter outStream = System.IO.File.CreateText(filepath + "/0.csv");
                    outStream.Write(sb);
                    outStream.Close();
                }
            }
            else
            {
                Directory.CreateDirectory(path + directoryName);
                Debug.Log("directory is created");
                File.Create(path + directoryName + "/0.csv");
                Debug.Log("csv is created");
                string[] output = new string[fShortNoteInt.Length];
                for (int i = 0; i < output.Length; i++)
                {
                    output[i] = fShortNoteInt[i] + "," + jShortNoteInt[i];
                }
                int length = output.GetLength(0);
                StringBuilder sb = new StringBuilder();
                string firstLine = bPM + "," + sync;
                sb.AppendLine(firstLine);
                for (int i = 0; i < length; i++)
                {
                    sb.AppendLine(output[i]);
                }
                string filepath = path + directoryName;
                if (!Directory.Exists(filepath))
                {
                    Directory.CreateDirectory(filepath);
                }
                StreamWriter outStream = System.IO.File.CreateText(filepath + "/0.csv");
                outStream.Write(sb);
                outStream.Close();

            }

            
        }
    }


    private void OnDisable()
    {
        /*
        string path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop);
        string directoryName = "/Please_Input_wav";
        if (Directory.Exists(path + directoryName))
        {

            if (File.Exists(path + directoryName + "/0.csv"))
            {
                
                Debug.Log("csv is alread here");

                string[] output = new string[fShortNoteInt.Length];
                for (int i = 0; i < output.Length; i++)
                {
                    output[i] = fShortNoteInt[i]+","+ jShortNoteInt[i];
                }
                int length = output.GetLength(0);
                StringBuilder sb = new StringBuilder();
                string firstLine = bPM + "," + sync;
                sb.AppendLine(firstLine);
                for (int i = 0; i < length; i++)
                {
                    sb.AppendLine(output[i]);
                }
                string filepath = path + directoryName ;
                if (!Directory.Exists(filepath))
                {
                    Directory.CreateDirectory(filepath);
                }
                StreamWriter outStream = System.IO.File.CreateText(filepath + "/0.csv");
                outStream.Write(sb);
                outStream.Close();
            }
            else
            {

                File.Create(path + directoryName + "/0.csv");
                Debug.Log("csv is created");
                string[] output = new string[fShortNoteInt.Length];
                for (int i = 0; i < output.Length; i++)
                {
                    output[i] = fShortNoteInt[i] + "," + jShortNoteInt[i];
                }
                int length = output.GetLength(0);
                StringBuilder sb = new StringBuilder();
                string firstLine = bPM + "," + sync;
                sb.AppendLine(firstLine);
                for (int i = 0; i < length; i++)
                {
                    sb.AppendLine(output[i]);
                }
                string filepath = path + directoryName;
                if (!Directory.Exists(filepath))
                {
                    Directory.CreateDirectory(filepath);
                }
                StreamWriter outStream = System.IO.File.CreateText(filepath + "/0.csv");
                outStream.Write(sb);
                outStream.Close();
            }
        }
        else
        {
            Directory.CreateDirectory(path + directoryName);
            Debug.Log("directory is created");
            File.Create(path + directoryName + "/0.csv");
            Debug.Log("csv is created");
            string[] output = new string[fShortNoteInt.Length];
            for (int i = 0; i < output.Length; i++)
            {
                output[i] = fShortNoteInt[i] + "," + jShortNoteInt[i];
            }
            int length = output.GetLength(0);
            StringBuilder sb = new StringBuilder();
            string firstLine = bPM + "," + sync;
            sb.AppendLine(firstLine);
            for (int i = 0; i < length; i++)
            {
                sb.AppendLine(output[i]);
            }
            string filepath = path + directoryName;
            if (!Directory.Exists(filepath))
            {
                Directory.CreateDirectory(filepath);
            }
            StreamWriter outStream = System.IO.File.CreateText(filepath + "/0.csv");
            outStream.Write(sb);
            outStream.Close();

        }
        */
        Debug.Log("end of game");
    }

    private void OnTriggerEnter(Collider other)
    {
        if(isPlaying==true)
        {
            noteAudioSource.Play();

        }

    }

    public void BtsMusic()
    {

        StartCoroutine(LoadAudio());
    }

    IEnumerator LoadAudio()
    {
        string path = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop);
        string directoryName = "/Please_Input_wav";
        if (File.Exists(path + directoryName + "/0.wav"))
        {
            
            string fileName = "file://" + path + directoryName + "/0.wav";
            UnityWebRequest request = UnityWebRequestMultimedia.GetAudioClip(fileName, AudioType.WAV);

            yield return request.SendWebRequest();

            if(request.result == UnityWebRequest.Result.Success)
            {
                audioClip = DownloadHandlerAudioClip.GetContent(request);
            }

        }
        else
        {
            Debug.Log("Pleas input file");
            //Directory.CreateDirectory(path + directoryName);
        }

        yield return null;

        Setting();
        NoteSetting();
        GapUpdate();

    }


    public void InputFieldBPM()
    {
        if (isPlaying == false)
        {
            bPM = int.Parse(bPMInputField.text);
            GapReset();
            NoteReset();
        }
    }
    public void BtsSpeedDown()
    {
        if (isPlaying == false)
        {
            
                speed*=0.5f;

                GapReset();
            
        }
    }
    public void BtsSpeedUp()
    {
        if (isPlaying == false)
        {
            speed*=2;

            GapReset();
        }
    }
    public void InputFieldGap()
    {
        if (isPlaying == false)
        {
            gap = int.Parse(gapInputField.text);
            GapReset();
        }
    }
    
    public void BtsTimeMinus()
    {
        if (isPlaying == false)
        {
            if (nowBeat > 1)
            {
                additionalBeat--;
                sec -= 60 / bT;
                //sec = (nowBeat + nowBeatFloat) * 60;
                timeInpuField.text = sec.ToString("F2");
                timeSlider.value = sec;
            }
        }
    }
    public void BtsPlayAndPause()
    {

        isPlaying = !isPlaying;

        
        if(isPlaying==false)
        {
            timeInpuField.text = sec.ToString("F2");
            timeSlider.value = sec;
            audioSource.Stop();
        }
        else if (isPlaying == true)
        {
            audioSource.time = sec;
            audioSource.Play();
        }
    }

    public void InputFieldSyncInputField()
    {
        //Debug.Log(float.Parse(syncInputField.text));
        sync = float.Parse(syncInputField.text);
        //GapReset();
    }

    public void BtsTimePlus()
    {
        if (isPlaying == false)
        {
            additionalBeat++;
            sec += 60 / bT;
            //sec = (nowBeat + nowBeatFloat) * 60;
            timeInpuField.text = sec.ToString("F2");
            timeSlider.value = sec;
        }
    }

    
    public void SliderTimeChange()
    {
        if (isPlaying == false)
        {
            additionalBeat = 0;
            sec = timeSlider.value;
            timeInpuField.text = sec.ToString("F2");
        }
    }

    public void InputFieldBGMVolumeInputField()
    {
        audioSource.volume = int.Parse(bGMVolumeInputField.text)/100f;
    }

    public void InputFieldNoteVolumeInputField()
    {
        noteAudioSource.volume = int.Parse(noteVolumeInputField.text)/100f;
    }


    public void InputFieldTimeInputField()
    {
        if (isPlaying == false)
        {
            additionalBeat = 0;
            sec = int.Parse(timeInpuField.text);
            timeSlider.value = sec;
        }
    }
}
