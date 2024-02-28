using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public class Story : MonoBehaviour
{
    enum StoryCommand
    {
        sp,
        fi,
        fo,
        m,
        bfi,
        bfo,
        StoryCommand_End
    }

    [SerializeField]
    private GameObject      m_CharacterPrefab; //캐릭터 프리팹
    [SerializeField]
    private GameObject      m_CharacterParent; //캐릭터 부모
    [SerializeField]
    private Image           m_BackGroundImage; //뒷 배경
    [SerializeField]
    private TextMeshProUGUI m_TextName; //캐릭터 이름 텍스트
    [SerializeField]
    private TextMeshProUGUI m_TextSpeech; //캐릭터 대사 텍스트

    //캐릭터 보관할 딕셔너리
    private Dictionary<string, GameObject> m_CharacterDictionary;

    //현재 스토리 넘버
    private int         m_StoryNumber = 0;
    //현재 스토리 데이터 (셀 별로 나눠진 데이터)
    private string[]    m_StoryData;
    //현재 스토리 진행사항
    private int         m_StoryCount;
    //대사를 치는 중이거나 하면 대기한다
    private int         m_iWait = 0;

    private void Awake()
    {
        m_StoryNumber   = StoryMng.Instance.StoryNumber;
        m_StoryData     = StoryMng.Instance.Get_StoryData(m_StoryNumber);
        m_StoryCount    = 0;
        m_CharacterDictionary = new Dictionary<string, GameObject>();
    }

    private void Update()
    {
        StoryDataProcess();
    }

    private void StoryDataProcess()
    {
        //아직 대기중, 쓰레드가 아닌 코루틴을 사용할 것 이기 때문에 동시에 접근하는 일 x
        if (m_iWait > 0)
        {
            return;
        }
        //이번 스토리 끝
        if(m_StoryCount == m_StoryData.Length)
        {
            StoryEnd();
            return;
        }
        
        string strData = m_StoryData[m_StoryCount++];
        string strName, strCommand;

        //Name 찾기
        int iNameStartIndex = strData.IndexOf("[");
        int iNameEndIndex = strData.IndexOf("]");
        if (iNameStartIndex == -1 || iNameEndIndex == -1) //]를 못 찾았다면
        {
            Debug.Log("Story.StoryDataProcess() -> Data 값이 잘못 입력됨");
        }

        //[]를 제외하기 위해서 iNameEndIndex에서 시작 인덱스를 빼줌 -> 계산해 보면 [] 빠짐.
        strName = strData.Substring(iNameStartIndex + 1, iNameEndIndex - (iNameStartIndex + 1)).Trim(); //공백 제거

        //Command 찾기
        int iCommandStartIndex = strData.IndexOf("[", iNameEndIndex + 1);
        int iCommandEndIndex = strData.IndexOf("]", iCommandStartIndex + 1);
        if (iCommandStartIndex == -1 || iCommandEndIndex == -1) //]를 못 찾았다면
        {
            Debug.Log("Story.StoryDataProcess() -> Data 값이 잘못 입력됨");
        }

        //위와 동일한 식.
        strCommand = strData.Substring(iCommandStartIndex + 1, iCommandEndIndex - (iCommandStartIndex + 1)).Trim(); //공백 제거

        //위처럼 계산을 여러번 안하고 첫번째 ], 두번째 ]로 나눠주면 연산이 조금 줄어들겠으나
        //연산이 많이 필요 한 부분도 아니고, 기획자 분들이 실수를 할 수 있는 부분이기 때문에
        //(손으로 노가다로 대사 및 효과를 쳐 주어야 하기 때문에)
        //휴먼 에러 방지를 위해 나눠서 계산
        CommandSwitch(strName, strCommand, strData.Substring(iCommandEndIndex + 1));
    }

    //이번 스토리 데이터를 전부 처리했다 (이번 스토리가 끝났다)
    private void StoryEnd()
    {

    }

    private void CommandSwitch(string _strName, string _strCommand, string _strLeftData)
    {
        //_strCommand 확인
        StoryCommand Command = (StoryCommand)Enum.Parse(typeof(StoryCommand), _strCommand);
        if(ReferenceEquals(Command,null))
        {
            Debug.Log("Story.CommandSwitch() -> Command 값이 잘못 입력됨");
        }

        switch (Command)
        {
            case StoryCommand.sp:
                Command_sp(_strName, DataDetail(_strLeftData,1)[0]);
                break;
            case StoryCommand.fi:
                Command_fi(_strName, DataDetail(_strLeftData,2));
                break;
            case StoryCommand.fo:
                Command_fo(_strName, DataDetail(_strLeftData,1)[0]);
                break;
            case StoryCommand.m:
                Command_m(_strName, DataDetail(_strLeftData,2));
                break;
            case StoryCommand.bfi:
                Command_bfi(_strName, DataDetail(_strLeftData,1)[0]);
                break;
            case StoryCommand.bfo:
                Command_bfo(_strName, DataDetail(_strLeftData,1)[0]);
                break;
            case StoryCommand.StoryCommand_End:
                break;
        }
    }

    //Command 이후, []가 몇개 남았냐에 따라 []안의 내용물을 나눠줌.
    private string[] DataDetail(string _strLeftData, int _iLeftAmount)
    {
        if(_iLeftAmount == 1)
        {
            string[] DataDetail = new string[1];

            int iIndex1 = _strLeftData.IndexOf("[");
            //[] 안에 ]가 또 들어 있을 수도 있으므로, 마지막부터 찾는다. (ex 대사)
            int iIndex2 = _strLeftData.LastIndexOf("]");

            DataDetail[0] = _strLeftData.Substring(iIndex1 + 1, iIndex2 - (iIndex1 + 1));

            return DataDetail;
        }
        else if(_iLeftAmount == 2)
        {
            string[] DataDetail = new string[2];

            //첫번째
            int iIndex1 = _strLeftData.IndexOf("[");
            int iIndex2 = _strLeftData.IndexOf("]");

            DataDetail[0] = _strLeftData.Substring(iIndex1 + 1, iIndex2 - (iIndex1 + 1));

            //두번째 (iIndex 공간 재활용)
            iIndex1 = _strLeftData.IndexOf("[" , iIndex2 + 1);
            iIndex2 = _strLeftData.LastIndexOf("]");

            DataDetail[1] = _strLeftData.Substring(iIndex1 + 1, iIndex2 - (iIndex1 + 1));

            return DataDetail;
        }
        else
        {
            Debug.Log("Story.DataDetail() -> _iLeftAmount 값이 잘못 입력됨");
            return null;
        }
    }

    private void Command_sp(string _strName, string _strStoryLine)
    {
        StartCoroutine(CharacterSpeech(_strName, _strStoryLine.Replace("@@","\n")));
    }

    private void Command_fi(string _strName, string[] _DataArray)
    {
        string strSecond = _DataArray[0];
        string strXY = _DataArray[1];

        //캐릭터가 없었다면 등장시킨다
        if(!m_CharacterDictionary.ContainsKey(_strName))
        {
            //생성해서 딕셔너리에 넣는다. //부모를 설정해준다.
            m_CharacterDictionary.Add(_strName, Instantiate(m_CharacterPrefab, m_CharacterParent.transform));
            //스프라이트(이미지)를 넣어준다.
            m_CharacterDictionary[_strName].GetComponent<Image>().sprite = Resources.Load<Sprite>("Image/" + _strName);
            //스케일도 초기화 한번 해준다.
            m_CharacterDictionary[_strName].transform.localScale = new Vector3(1f, 1f, 1f);
        }

        //생성 위치
        Vector2 v2SpawnPos = new Vector2();
        string[] strXYArray = strXY.Split(",");
        //목적지 x,y좌표 계산 , 정 중앙이 0,0 이고 양 끝은 -100, 100, 즉 가로 200 세로 200임.
        v2SpawnPos.x = float.Parse(strXYArray[0]) / 200f * /*Screen.width*/ m_BackGroundImage.GetComponent<RectTransform>().rect.width;
        v2SpawnPos.y = float.Parse(strXYArray[1]) / 200f */* Screen.height*/m_BackGroundImage.GetComponent<RectTransform>().rect.height;
        //위치 지정
        m_CharacterDictionary[_strName].GetComponent<RectTransform>().anchoredPosition = v2SpawnPos;

        //만약 바로 생성만 시키는 거라면 바로 끝낸다.
        if (int.Parse(strSecond) == 0)
        {
            return;
        }

        StartCoroutine(SpriteAlphaRaise(m_CharacterDictionary[_strName].GetComponent<Image>(), strSecond));
    }

    private void Command_fo(string _strName, string _strSecond)
    {
        //캐릭터가 없다면 바로 끝낸다.
        if (!m_CharacterDictionary.ContainsKey(_strName))
        {
            return;
        }

        //만약 바로 없애기만 하는 거라면 바로 끝낸다.
        if (int.Parse(_strSecond) == 0)
        {
            return;
        }

        //삭제 버전으로
        StartCoroutine(SpriteAlphaLower(m_CharacterDictionary[_strName].GetComponent<Image>(),_strName ,_strSecond));
    }

    private void Command_m(string _strName, string[] _DataArray)
    {
        string strSpeed = _DataArray[0];
        string strXY = _DataArray[1];

        StartCoroutine(SpriteMove(_strName, strXY, strSpeed));
    }

    private void Command_bfi(string _strName, string _strSecond)
    {
        m_BackGroundImage.sprite = Resources.Load<Sprite>("Image/" + _strName);
        //바로 보여주는 배경이라면 빠져나온다.
        if(_strSecond == "0")
        {
            return;
        }
        StartCoroutine(SpriteAlphaRaise(m_BackGroundImage, _strSecond));
    }

    private void Command_bfo(string _strName, string _strSecond)
    {
        //bfo가 진행중일 때, bfi가 실행되면 개판날 수 있음. 아직 그 부분은 안전장치를 걸어 놓은 건 없음.
        //뒷 배경은 없어지면 안되기 때문에, 유지로 감. 체인 스왑 같은 건 안씀. 굳이? 빈 배경이 나올까?

        //만약 바로 끝나는 거라면
        if(int.Parse(_strSecond) == 0)
        {
            //이걸 할까 말까 고민. 어짜피 0이라면 다음 배경으로 바로 바꾸려는 걸 텐데..
            //그냥 한 프레임 더 주고, 다음 프레임에 바뀔텐데.. 거민.
            m_BackGroundImage.color = new Color(1f, 1f, 1f, 0.5f);
            return;
        }

        //뒷 배경은 유지 버전. 배경이 삭제되면 안되자눔
        StartCoroutine(SpriteAlphaLower(m_BackGroundImage, _strSecond));
    }

    //대사
    private IEnumerator CharacterSpeech(string _strName, string _strStoryLine)
    {
        //대사 끝나도 버튼을 눌러 넘기기 위해 대기
        //코루틴이라 동시 접근 x
        //대기 1 증가 -> 대사 끝나도 버튼을 통해 넘어가기 위해 증가
        ++m_iWait;
        //대기 1 증가 -> 대사 치는 중을 알림
        ++m_iWait;

        //이름
        m_TextName.text = _strName;
        //대사
        string strSpeech = "";
        int iSpeechCharAmount = 0;
        //몇 초마다 글자가 나오게 할 것인지
        float fNextTextTime = 0.1f;
        float fTime = 0f;

        //대사 글자 하나씩 나오도록
        while (iSpeechCharAmount < _strStoryLine.Length)
        {
            m_TextSpeech.text = strSpeech;
            yield return null;

            //대사 치는 중에 아무 키나 눌렀다면
            if(Input.anyKeyDown || Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
            {
                //바로 대사 완성하고 빠져나온다.
                m_TextSpeech.text = _strStoryLine;
                break;
            }

            fTime += Time.deltaTime;

            //글자 나올 타이밍이라면
            if(fTime >= fNextTextTime)
            {
                strSpeech += _strStoryLine[iSpeechCharAmount++];
                fTime = fTime - fNextTextTime;
            }
        }

        //대사가 끝났다는 뜻.
        --m_iWait;

        while(true)
        {
            yield return null;

            //사용자가 대사를 전부 확인하고 아무 버튼이나 누르면 넘어간다
            if (Input.anyKeyDown || Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
            {
                break;
            }
        }

        //넘어가라는 뜻
        --m_iWait;
    }


    //알파 값 증가
    private IEnumerator SpriteAlphaRaise(Image _Image, string _strSecond)
    {
        float fStartAlpha, fTime, fSpeed;
        fTime = 0f;
        //시작 알파 값
        fStartAlpha = 0.2f;
        //걸리는 시간으로 스피드 값을 계산
        fSpeed = (1f - fStartAlpha) / int.Parse(_strSecond);
        Color SpriteColor = new Color(1f, 1f, 1f, fStartAlpha);
        //알파값이 1이 넘어가면 증가 연산을 멈추고 1로 고정
        while (SpriteColor.a < 1f)
        {
            _Image.color = SpriteColor;
            yield return null;

            fTime += Time.deltaTime * fSpeed;
            SpriteColor.a = Mathf.Lerp(fStartAlpha, 1f, fTime);
        }

        SpriteColor.a = 1f;
        _Image.color = SpriteColor;
    }

    //알파 값 감소 (유지 버전)
    private IEnumerator SpriteAlphaLower(Image _Image, string _strSecond)
    {
        float fLastAlpha, fTime, fSpeed;
        fTime = 0f;
        //마지막 알파 값
        fLastAlpha = 0.2f;
        //걸리는 시간으로 스피드 값을 계산
        fSpeed = (1f - fLastAlpha) / int.Parse(_strSecond);
        Color SpriteColor = new Color(1f, 1f, 1f, 1f);
        //알파값이 fLastAlpha 보다 작아지면 감소 연산을 멈추고 fLastAlpha로 고정
        while (SpriteColor.a < fLastAlpha)
        {
            _Image.color = SpriteColor;
            yield return null;

            fTime += Time.deltaTime * fSpeed;
            SpriteColor.a = Mathf.Lerp(1f, fLastAlpha, fTime);
        }

        SpriteColor.a = fLastAlpha;
        _Image.color = SpriteColor;
    }

    //알파 값 감소 (삭제 버전)
    private IEnumerator SpriteAlphaLower(Image _Image, string _strName, string _strSecond)
    {
        float fLastAlpha, fTime, fSpeed;
        fTime = 0f;
        //마지막 알파 값
        fLastAlpha = 0.2f;
        //걸리는 시간으로 스피드 값을 계산
        fSpeed = (1f - fLastAlpha) / int.Parse(_strSecond);
        Color SpriteColor = new Color(1f, 1f, 1f, 1f);
        //알파값이 fLastAlpha 보다 작아지면 감소 연산을 멈추고 fLastAlpha로 고정
        while (SpriteColor.a > fLastAlpha)
        {
            _Image.color = SpriteColor;
            yield return null;

            fTime += Time.deltaTime * fSpeed;
            SpriteColor.a = Mathf.Lerp(1f, fLastAlpha, fTime);
        }

        //알파값을 줄였다면 삭제한다.
        GameObject DeleteCharacter = m_CharacterDictionary[_strName];
        m_CharacterDictionary.Remove(_strName);
        GameObject.Destroy(DeleteCharacter);
    }

    //움직임
    private IEnumerator SpriteMove(string _strName, string _strXY, string _strSpeed)
    {
        //만약 없다면 그냥 나가라
        if(!m_CharacterDictionary.ContainsKey(_strName))
        {
            yield break;
        }

        RectTransform SpriteRectTransform = m_CharacterDictionary[_strName].GetComponent<RectTransform>();

        //0이 x 비율, 1이 y 비율
        string[] strXYArray = _strXY.Split(",");
        float fSpeed, fX, fY;
        //스피드
        fSpeed = float.Parse(_strSpeed);
        //화면 Screen의 width, height 가 아닌 뒷 배경 일러스트의 크기로 한다 (나중에 바뀔 수 있음)
        Rect BackGroundRect = m_BackGroundImage.GetComponent<RectTransform>().rect;
        //목적지 x,y좌표 계산 , 정 중앙이 0,0 이고 양 끝은 -100, 100, 즉 가로 200 세로 200임.
        fX = float.Parse(strXYArray[0]) / 200f * BackGroundRect.width  /*Screen.width*/;
        fY = float.Parse(strXYArray[1]) / 200f * BackGroundRect.height/*Screen.height*/;
        //목적지 벡터
        Vector2 v2Goal = new Vector3(fX, fY);
        //방향 벡터 구하기
        Vector2 v2Dir = new Vector2(fX - SpriteRectTransform.anchoredPosition.x, fY - SpriteRectTransform.anchoredPosition.y);
        v2Dir = v2Dir.normalized;
        Vector3 v2Move = new Vector3(0f,0f);

        //너무 멀리 가버리면 끝내기 용
        float fMaxW, fMaxH;
        fMaxW = /*Screen.width*/ BackGroundRect.width * 2f;
        fMaxH = /*Screen.height*/ BackGroundRect.height * 2f;
        //이동
        while (Vector2.Distance(v2Goal, v2Move) > 10f)
        {
            //만약 객체가 사라졌다면
            if(SpriteRectTransform == null)
            {
                //이동 함수는 바로 끝내라
                yield break;
            }

            //이동
            v2Move.x = SpriteRectTransform.anchoredPosition.x + v2Dir.x * Time.deltaTime * fSpeed;
            v2Move.y = SpriteRectTransform.anchoredPosition.y + v2Dir.y * Time.deltaTime * fSpeed;
            SpriteRectTransform.anchoredPosition = v2Move;
            yield return null;

            //만약 너무 가버리면 걍 끝낸다
            if(v2Move.x > fMaxW || v2Move.x < -fMaxW || v2Move.y > fMaxH || v2Move.y < -fMaxH)
            {
                break;
            }
        }

        SpriteRectTransform.anchoredPosition = v2Goal;
    }
}
