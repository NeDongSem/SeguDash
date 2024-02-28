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


    //구글 시트 아이디
    string m_strSheetID = "1hC_eG8S76hn0dCfM_JPw0qsg7d1Laj5K2Tcx39rAxrk";
    //데이터를 가져오는데 시간이 걸리기 때문에, 데이터를 가져온 후 처리할 함수를 델리게이트로 받아서 실행해준다.
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

        //예외 처리
        if (unityWebRequest.result == UnityWebRequest.Result.ConnectionError ||
           unityWebRequest.result == UnityWebRequest.Result.ProtocolError)
        {
            Debug.Log("GoogleSheetMng.GetDataProcessing -> 네트워크 환경이 안좋");
        }
        else
        {
            //데이터를 나눠서 넘겨줌
            string strGetData = unityWebRequest.downloadHandler.text;

            //받은 데이터를 줄 별로 나눔 (행으로 나눔)
            string[] strDataLineArray = strGetData.Split('\n');

            //줄로 나눈 데이터들을 다시 열 별로 나눔 (@#@로)
            for (int i = 0; i < strDataLineArray.Length; ++i)
            {
                strDataLineArray[i] = strDataLineArray[i].Replace("\",", "@#@");
                strDataLineArray[i] = strDataLineArray[i].Replace("\"", ""); //셀 시작이 \고 ,가 다음 셀 간다는 뜻
            }

            //데이터를 행, 열(@#@)로 다 나눈 다음 함수 실행
            _delDataProcessingFunc(strDataLineArray);
        }
    }

}
