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
    private GameObject      m_CharacterPrefab; //ĳ���� ������
    [SerializeField]
    private GameObject      m_CharacterParent; //ĳ���� �θ�
    [SerializeField]
    private Image           m_BackGroundImage; //�� ���
    [SerializeField]
    private TextMeshProUGUI m_TextName; //ĳ���� �̸� �ؽ�Ʈ
    [SerializeField]
    private TextMeshProUGUI m_TextSpeech; //ĳ���� ��� �ؽ�Ʈ

    //ĳ���� ������ ��ųʸ�
    private Dictionary<string, GameObject> m_CharacterDictionary;

    //���� ���丮 �ѹ�
    private int         m_StoryNumber = 0;
    //���� ���丮 ������ (�� ���� ������ ������)
    private string[]    m_StoryData;
    //���� ���丮 �������
    private int         m_StoryCount;
    //��縦 ġ�� ���̰ų� �ϸ� ����Ѵ�
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
        //���� �����, �����尡 �ƴ� �ڷ�ƾ�� ����� �� �̱� ������ ���ÿ� �����ϴ� �� x
        if (m_iWait > 0)
        {
            return;
        }
        //�̹� ���丮 ��
        if(m_StoryCount == m_StoryData.Length)
        {
            StoryEnd();
            return;
        }
        
        string strData = m_StoryData[m_StoryCount++];
        string strName, strCommand;

        //Name ã��
        int iNameStartIndex = strData.IndexOf("[");
        int iNameEndIndex = strData.IndexOf("]");
        if (iNameStartIndex == -1 || iNameEndIndex == -1) //]�� �� ã�Ҵٸ�
        {
            Debug.Log("Story.StoryDataProcess() -> Data ���� �߸� �Էµ�");
        }

        //[]�� �����ϱ� ���ؼ� iNameEndIndex���� ���� �ε����� ���� -> ����� ���� [] ����.
        strName = strData.Substring(iNameStartIndex + 1, iNameEndIndex - (iNameStartIndex + 1)).Trim(); //���� ����

        //Command ã��
        int iCommandStartIndex = strData.IndexOf("[", iNameEndIndex + 1);
        int iCommandEndIndex = strData.IndexOf("]", iCommandStartIndex + 1);
        if (iCommandStartIndex == -1 || iCommandEndIndex == -1) //]�� �� ã�Ҵٸ�
        {
            Debug.Log("Story.StoryDataProcess() -> Data ���� �߸� �Էµ�");
        }

        //���� ������ ��.
        strCommand = strData.Substring(iCommandStartIndex + 1, iCommandEndIndex - (iCommandStartIndex + 1)).Trim(); //���� ����

        //��ó�� ����� ������ ���ϰ� ù��° ], �ι�° ]�� �����ָ� ������ ���� �پ�������
        //������ ���� �ʿ� �� �κе� �ƴϰ�, ��ȹ�� �е��� �Ǽ��� �� �� �ִ� �κ��̱� ������
        //(������ �밡�ٷ� ��� �� ȿ���� �� �־�� �ϱ� ������)
        //�޸� ���� ������ ���� ������ ���
        CommandSwitch(strName, strCommand, strData.Substring(iCommandEndIndex + 1));
    }

    //�̹� ���丮 �����͸� ���� ó���ߴ� (�̹� ���丮�� ������)
    private void StoryEnd()
    {

    }

    private void CommandSwitch(string _strName, string _strCommand, string _strLeftData)
    {
        //_strCommand Ȯ��
        StoryCommand Command = (StoryCommand)Enum.Parse(typeof(StoryCommand), _strCommand);
        if(ReferenceEquals(Command,null))
        {
            Debug.Log("Story.CommandSwitch() -> Command ���� �߸� �Էµ�");
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

    //Command ����, []�� � ���ҳĿ� ���� []���� ���빰�� ������.
    private string[] DataDetail(string _strLeftData, int _iLeftAmount)
    {
        if(_iLeftAmount == 1)
        {
            string[] DataDetail = new string[1];

            int iIndex1 = _strLeftData.IndexOf("[");
            //[] �ȿ� ]�� �� ��� ���� ���� �����Ƿ�, ���������� ã�´�. (ex ���)
            int iIndex2 = _strLeftData.LastIndexOf("]");

            DataDetail[0] = _strLeftData.Substring(iIndex1 + 1, iIndex2 - (iIndex1 + 1));

            return DataDetail;
        }
        else if(_iLeftAmount == 2)
        {
            string[] DataDetail = new string[2];

            //ù��°
            int iIndex1 = _strLeftData.IndexOf("[");
            int iIndex2 = _strLeftData.IndexOf("]");

            DataDetail[0] = _strLeftData.Substring(iIndex1 + 1, iIndex2 - (iIndex1 + 1));

            //�ι�° (iIndex ���� ��Ȱ��)
            iIndex1 = _strLeftData.IndexOf("[" , iIndex2 + 1);
            iIndex2 = _strLeftData.LastIndexOf("]");

            DataDetail[1] = _strLeftData.Substring(iIndex1 + 1, iIndex2 - (iIndex1 + 1));

            return DataDetail;
        }
        else
        {
            Debug.Log("Story.DataDetail() -> _iLeftAmount ���� �߸� �Էµ�");
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

        //ĳ���Ͱ� �����ٸ� �����Ų��
        if(!m_CharacterDictionary.ContainsKey(_strName))
        {
            //�����ؼ� ��ųʸ��� �ִ´�. //�θ� �������ش�.
            m_CharacterDictionary.Add(_strName, Instantiate(m_CharacterPrefab, m_CharacterParent.transform));
            //��������Ʈ(�̹���)�� �־��ش�.
            m_CharacterDictionary[_strName].GetComponent<Image>().sprite = Resources.Load<Sprite>("Image/" + _strName);
            //�����ϵ� �ʱ�ȭ �ѹ� ���ش�.
            m_CharacterDictionary[_strName].transform.localScale = new Vector3(1f, 1f, 1f);
        }

        //���� ��ġ
        Vector2 v2SpawnPos = new Vector2();
        string[] strXYArray = strXY.Split(",");
        //������ x,y��ǥ ��� , �� �߾��� 0,0 �̰� �� ���� -100, 100, �� ���� 200 ���� 200��.
        v2SpawnPos.x = float.Parse(strXYArray[0]) / 200f * /*Screen.width*/ m_BackGroundImage.GetComponent<RectTransform>().rect.width;
        v2SpawnPos.y = float.Parse(strXYArray[1]) / 200f */* Screen.height*/m_BackGroundImage.GetComponent<RectTransform>().rect.height;
        //��ġ ����
        m_CharacterDictionary[_strName].GetComponent<RectTransform>().anchoredPosition = v2SpawnPos;

        //���� �ٷ� ������ ��Ű�� �Ŷ�� �ٷ� ������.
        if (int.Parse(strSecond) == 0)
        {
            return;
        }

        StartCoroutine(SpriteAlphaRaise(m_CharacterDictionary[_strName].GetComponent<Image>(), strSecond));
    }

    private void Command_fo(string _strName, string _strSecond)
    {
        //ĳ���Ͱ� ���ٸ� �ٷ� ������.
        if (!m_CharacterDictionary.ContainsKey(_strName))
        {
            return;
        }

        //���� �ٷ� ���ֱ⸸ �ϴ� �Ŷ�� �ٷ� ������.
        if (int.Parse(_strSecond) == 0)
        {
            return;
        }

        //���� ��������
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
        //�ٷ� �����ִ� ����̶�� �������´�.
        if(_strSecond == "0")
        {
            return;
        }
        StartCoroutine(SpriteAlphaRaise(m_BackGroundImage, _strSecond));
    }

    private void Command_bfo(string _strName, string _strSecond)
    {
        //bfo�� �������� ��, bfi�� ����Ǹ� ���ǳ� �� ����. ���� �� �κ��� ������ġ�� �ɾ� ���� �� ����.
        //�� ����� �������� �ȵǱ� ������, ������ ��. ü�� ���� ���� �� �Ⱦ�. ����? �� ����� ���ñ�?

        //���� �ٷ� ������ �Ŷ��
        if(int.Parse(_strSecond) == 0)
        {
            //�̰� �ұ� ���� ���. ��¥�� 0�̶�� ���� ������� �ٷ� �ٲٷ��� �� �ٵ�..
            //�׳� �� ������ �� �ְ�, ���� �����ӿ� �ٲ��ٵ�.. �Ź�.
            m_BackGroundImage.color = new Color(1f, 1f, 1f, 0.5f);
            return;
        }

        //�� ����� ���� ����. ����� �����Ǹ� �ȵ��ڴ�
        StartCoroutine(SpriteAlphaLower(m_BackGroundImage, _strSecond));
    }

    //���
    private IEnumerator CharacterSpeech(string _strName, string _strStoryLine)
    {
        //��� ������ ��ư�� ���� �ѱ�� ���� ���
        //�ڷ�ƾ�̶� ���� ���� x
        //��� 1 ���� -> ��� ������ ��ư�� ���� �Ѿ�� ���� ����
        ++m_iWait;
        //��� 1 ���� -> ��� ġ�� ���� �˸�
        ++m_iWait;

        //�̸�
        m_TextName.text = _strName;
        //���
        string strSpeech = "";
        int iSpeechCharAmount = 0;
        //�� �ʸ��� ���ڰ� ������ �� ������
        float fNextTextTime = 0.1f;
        float fTime = 0f;

        //��� ���� �ϳ��� ��������
        while (iSpeechCharAmount < _strStoryLine.Length)
        {
            m_TextSpeech.text = strSpeech;
            yield return null;

            //��� ġ�� �߿� �ƹ� Ű�� �����ٸ�
            if(Input.anyKeyDown || Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
            {
                //�ٷ� ��� �ϼ��ϰ� �������´�.
                m_TextSpeech.text = _strStoryLine;
                break;
            }

            fTime += Time.deltaTime;

            //���� ���� Ÿ�̹��̶��
            if(fTime >= fNextTextTime)
            {
                strSpeech += _strStoryLine[iSpeechCharAmount++];
                fTime = fTime - fNextTextTime;
            }
        }

        //��簡 �����ٴ� ��.
        --m_iWait;

        while(true)
        {
            yield return null;

            //����ڰ� ��縦 ���� Ȯ���ϰ� �ƹ� ��ư�̳� ������ �Ѿ��
            if (Input.anyKeyDown || Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
            {
                break;
            }
        }

        //�Ѿ��� ��
        --m_iWait;
    }


    //���� �� ����
    private IEnumerator SpriteAlphaRaise(Image _Image, string _strSecond)
    {
        float fStartAlpha, fTime, fSpeed;
        fTime = 0f;
        //���� ���� ��
        fStartAlpha = 0.2f;
        //�ɸ��� �ð����� ���ǵ� ���� ���
        fSpeed = (1f - fStartAlpha) / int.Parse(_strSecond);
        Color SpriteColor = new Color(1f, 1f, 1f, fStartAlpha);
        //���İ��� 1�� �Ѿ�� ���� ������ ���߰� 1�� ����
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

    //���� �� ���� (���� ����)
    private IEnumerator SpriteAlphaLower(Image _Image, string _strSecond)
    {
        float fLastAlpha, fTime, fSpeed;
        fTime = 0f;
        //������ ���� ��
        fLastAlpha = 0.2f;
        //�ɸ��� �ð����� ���ǵ� ���� ���
        fSpeed = (1f - fLastAlpha) / int.Parse(_strSecond);
        Color SpriteColor = new Color(1f, 1f, 1f, 1f);
        //���İ��� fLastAlpha ���� �۾����� ���� ������ ���߰� fLastAlpha�� ����
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

    //���� �� ���� (���� ����)
    private IEnumerator SpriteAlphaLower(Image _Image, string _strName, string _strSecond)
    {
        float fLastAlpha, fTime, fSpeed;
        fTime = 0f;
        //������ ���� ��
        fLastAlpha = 0.2f;
        //�ɸ��� �ð����� ���ǵ� ���� ���
        fSpeed = (1f - fLastAlpha) / int.Parse(_strSecond);
        Color SpriteColor = new Color(1f, 1f, 1f, 1f);
        //���İ��� fLastAlpha ���� �۾����� ���� ������ ���߰� fLastAlpha�� ����
        while (SpriteColor.a > fLastAlpha)
        {
            _Image.color = SpriteColor;
            yield return null;

            fTime += Time.deltaTime * fSpeed;
            SpriteColor.a = Mathf.Lerp(1f, fLastAlpha, fTime);
        }

        //���İ��� �ٿ��ٸ� �����Ѵ�.
        GameObject DeleteCharacter = m_CharacterDictionary[_strName];
        m_CharacterDictionary.Remove(_strName);
        GameObject.Destroy(DeleteCharacter);
    }

    //������
    private IEnumerator SpriteMove(string _strName, string _strXY, string _strSpeed)
    {
        //���� ���ٸ� �׳� ������
        if(!m_CharacterDictionary.ContainsKey(_strName))
        {
            yield break;
        }

        RectTransform SpriteRectTransform = m_CharacterDictionary[_strName].GetComponent<RectTransform>();

        //0�� x ����, 1�� y ����
        string[] strXYArray = _strXY.Split(",");
        float fSpeed, fX, fY;
        //���ǵ�
        fSpeed = float.Parse(_strSpeed);
        //ȭ�� Screen�� width, height �� �ƴ� �� ��� �Ϸ���Ʈ�� ũ��� �Ѵ� (���߿� �ٲ� �� ����)
        Rect BackGroundRect = m_BackGroundImage.GetComponent<RectTransform>().rect;
        //������ x,y��ǥ ��� , �� �߾��� 0,0 �̰� �� ���� -100, 100, �� ���� 200 ���� 200��.
        fX = float.Parse(strXYArray[0]) / 200f * BackGroundRect.width  /*Screen.width*/;
        fY = float.Parse(strXYArray[1]) / 200f * BackGroundRect.height/*Screen.height*/;
        //������ ����
        Vector2 v2Goal = new Vector3(fX, fY);
        //���� ���� ���ϱ�
        Vector2 v2Dir = new Vector2(fX - SpriteRectTransform.anchoredPosition.x, fY - SpriteRectTransform.anchoredPosition.y);
        v2Dir = v2Dir.normalized;
        Vector3 v2Move = new Vector3(0f,0f);

        //�ʹ� �ָ� �������� ������ ��
        float fMaxW, fMaxH;
        fMaxW = /*Screen.width*/ BackGroundRect.width * 2f;
        fMaxH = /*Screen.height*/ BackGroundRect.height * 2f;
        //�̵�
        while (Vector2.Distance(v2Goal, v2Move) > 10f)
        {
            //���� ��ü�� ������ٸ�
            if(SpriteRectTransform == null)
            {
                //�̵� �Լ��� �ٷ� ������
                yield break;
            }

            //�̵�
            v2Move.x = SpriteRectTransform.anchoredPosition.x + v2Dir.x * Time.deltaTime * fSpeed;
            v2Move.y = SpriteRectTransform.anchoredPosition.y + v2Dir.y * Time.deltaTime * fSpeed;
            SpriteRectTransform.anchoredPosition = v2Move;
            yield return null;

            //���� �ʹ� �������� �� ������
            if(v2Move.x > fMaxW || v2Move.x < -fMaxW || v2Move.y > fMaxH || v2Move.y < -fMaxH)
            {
                break;
            }
        }

        SpriteRectTransform.anchoredPosition = v2Goal;
    }
}
