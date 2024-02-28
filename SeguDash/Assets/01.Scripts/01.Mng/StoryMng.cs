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

    //���� ���丮�� ������? �̰� ���߿� �����س��� ��. ������Ʈ�� or ���� ���� ����
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
        //���� ��Ʈ �Ŵ��� ������ ���� �� �����Ƿ� ��ŸƮ �Լ����� �����͸� �����´�.
        GoogleSheetMng.Instance.Get_GoogleSheetData(m_GoogleSheetName, m_GoogleSheetRange, Set_StoryData);
    }

    //@#@�� ���� �������� �����Ƿ�, @#@�� ������ �����Ѵ�.
    private void Set_StoryData (string[] _strDataLineArray)
    {
        for(int i = 0; i < _strDataLineArray.Length; ++i)
        {
            //@#@�� ������ �ش� ���� ������ �ٽ� ������
            string[] StoryArray = _strDataLineArray[i].Split("@#@");

            //�ڿ��� ���� Ȯ���Ͽ�, �� ������ ���� Ȯ���Ѵ�.
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
            //�� ������ ���� �迭�� ���� ������ �����.
            string[] FixStoryArray = new string[StoryArray.Length - iBlankCount];
            //ī���Ѵ�.
            Array.Copy(StoryArray, FixStoryArray, FixStoryArray.Length);

            m_StoryDataList.Add(FixStoryArray);
        }
    }

    //Ư�� ���丮�� �����͸� �Ѱ��ش�
    public string[] Get_StoryData(int _iStoryNumber)
    {
        return m_StoryDataList[_iStoryNumber];
    }
}
