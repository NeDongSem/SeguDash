using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class StoryMng : MonoBehaviour
{
    static private StoryMng instance = null;
    static public StoryMng Instance { get { return instance; } }

    private void Awake()
    {
        //InstanceCheck
        if (ReferenceEquals(instance, null))
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
            Init();
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    [SerializeField]
    private string m_GoogleSheetName;
    [SerializeField]
    private string m_GoogleSheetRange;

    private List<string[]> m_StoryDataList;

    //현재 스토리가 몇인지? 이거 나중에 저장해놓을 것. 레지스트리 or 파일 따로 제작
    private int m_StoryNumber = 0;
    public int StoryNumber
    {
        get { return m_StoryNumber++; }
    }

    private void Init()
    {
        m_StoryDataList = new List<string[]>();
    }

    private void Start()
    {
        //구글 시트 매니저 생성과 꼬일 수 있으므로 스타트 함수에서 데이터를 가져온다.
        GoogleSheetMng.Instance.Get_GoogleSheetData(m_GoogleSheetName, m_GoogleSheetRange, Set_StoryData);
    }

    //@#@로 셀이 나뉘어져 있으므로, @#@로 나누어 저장한다.
    private void Set_StoryData (string[] _strDataLineArray)
    {
        for(int i = 0; i < _strDataLineArray.Length; ++i)
        {
            //@#@로 나누어 해당 줄의 셀들을 다시 나눈다
            string[] StoryArray = _strDataLineArray[i].Split("@#@");

            //뒤에서 부터 확인하여, 빈 공간의 셀을 확인한다.
            int iBlankCount = 0;
            for(int j = StoryArray.Length - 1; j > 0; --j)
            {
                if(StoryArray[j] == "")
                {
                    ++iBlankCount;
                }
                else
                {
                    break;
                }
            }
            //빈 공간을 없앤 배열을 담을 공간을 만든다.
            string[] FixStoryArray = new string[StoryArray.Length - iBlankCount];
            //카피한다.
            Array.Copy(StoryArray, FixStoryArray, FixStoryArray.Length);

            m_StoryDataList.Add(FixStoryArray);
        }
    }

    //특정 스토리의 데이터를 넘겨준다
    public string[] Get_StoryData(int _iStoryNumber)
    {
        return m_StoryDataList[_iStoryNumber];
    }
}
