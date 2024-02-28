using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GoogleSheetMng : MonoBehaviour
{
    static private GoogleSheetMng instance = null;
    static public GoogleSheetMng Instance { get { return instance; } }

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


    //���� ��Ʈ ���̵�
    string m_strSheetID = "1hC_eG8S76hn0dCfM_JPw0qsg7d1Laj5K2Tcx39rAxrk";
    //�����͸� �������µ� �ð��� �ɸ��� ������, �����͸� ������ �� ó���� �Լ��� ��������Ʈ�� �޾Ƽ� �������ش�.
    public delegate void delDataProcessingFunc(string[] _strDataLineArray);
    
    private void Init()
    {

    }

    public void Get_GoogleSheetData(string _strSheetName, string _strRange, delDataProcessingFunc _delDataProcessingFunc)
    {
        string strURL = string.Format("https://docs.google.com/spreadsheets/d/{0}/gviz/tq?tqx=out:csv&sheet={1}&range={2}", m_strSheetID, _strSheetName, _strRange);
        StartCoroutine(GetDataProcessing(strURL, _delDataProcessingFunc));
    }

    IEnumerator GetDataProcessing(string _strURL, delDataProcessingFunc _delDataProcessingFunc)
    {
        UnityWebRequest unityWebRequest = UnityWebRequest.Get(_strURL); 
        yield return unityWebRequest.SendWebRequest();

        //���� ó��
        if (unityWebRequest.result == UnityWebRequest.Result.ConnectionError ||
           unityWebRequest.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.Log("GoogleSheetMng.GetDataProcessing -> ��Ʈ��ũ ȯ���� ����");
        }
        else
        {
            //�����͸� ������ �Ѱ���
            string strGetData = unityWebRequest.downloadHandler.text;

            //���� �����͸� �� ���� ���� (������ ����)
            string[] strDataLineArray = strGetData.Split('\n');

            //�ٷ� ���� �����͵��� �ٽ� �� ���� ���� (@#@��)
            for (int i = 0; i < strDataLineArray.Length; ++i)
            {
                strDataLineArray[i] = strDataLineArray[i].Replace("\",", "@#@");
                strDataLineArray[i] = strDataLineArray[i].Replace("\"", ""); //�� ������ \�� ,�� ���� �� ���ٴ� ��
            }

            //�����͸� ��, ��(@#@)�� �� ���� ���� �Լ� ����
            _delDataProcessingFunc(strDataLineArray);
        }
    }

}
