using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum SceneType
{
    Logo,
    Main,
    NoteMaking,
    Loading,
    Story,
    Tutorial,
    Game,
    EndingCredit,
    SceneType_End
}

public class SceneMng : MonoBehaviour
{
    static private  SceneMng instance = null;
    static public   SceneMng Instance   { get { return instance; } }
    private         SceneMng()          { }
    private void Awake()
    {
        //InstanceCheck
        if (ReferenceEquals(instance, null))
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
            instance.Init();
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    ///////////////////////�̱���/////////////////////////////

    //���� ��
    private SceneType m_CurrentSceneType = SceneType.Logo;
    public  SceneType CurrentSceneType { get { return m_CurrentSceneType; } }
    //���� ��
    private SceneType m_PreSceneType = SceneType.SceneType_End;
    public  SceneType PreSceneType { get { return m_PreSceneType; } }

    //allowSceneActivation�� �̿��Ͽ� ���� Scene�� �ٷ� ���̱� ����
    private AsyncOperation  m_AsyncOperation;
    //�ε� �󸶳� �Ǿ���, 0.9�� �Ǹ� ��ǻ� 100%
    private float           m_LoadingProgress = 0f;
    //�� �ѱ�� Event �Լ� ȣ��� ����ó���� �ʿ��� bool��
    private bool            m_EventCheck = false;
    //�� �ѱ�� Event �Լ� ȣ��� ����ó���� �ʿ��� bool��
    private bool            m_UsingEvent = false;

    [SerializeField]
    private float m_FakeLoadingTime = 2f;

    private void Init()
    {
    }

    //_Event : ���� �Ѿ �� �̺�Ʈ�� �޾Ƽ� �Ѿ ������ //_LoadingScene : �ε� ���� ���� �Ѿ� �� ������ //fakeloading �ð��� �ٰ���
    public void Set_SceneChange(SceneType _ChangeSceneType, bool _LoadingScene = false, bool _Event = false, bool _FakeLoading = false)
    {
        //Event ��� ����
        m_UsingEvent = _Event;
        //_Event�� ������Ű�� ���� : ����� �����ϱ⿣ _Event�� true�� �־��ָ� �̺�Ʈ�� ��� �Ѿ�ٰ�
        //�����ϰ�����, _Event�� ��� �Ǵ� allowSceneActivation�� false�� ��, 0.9���� ���缭 �����.
        //�׸��� allowSceneActivation�� �ٽ� true�� �Ǿ�� ���� �� �ε��� �Ϸ��ϰ� �ٲ�.
        //�׷��� ���⼭ �������Ѽ� �Ѱ��ְ� ���.
        //_LoadingScene = true : �ε����� �����Ѵ� (�ε��� ���� ���� _ChangeSceneType)
        if (_LoadingScene && !_Event)
        {
            //�ε� ���� ����, �̺�Ʈ�� �� �� ���� ����ũ �ε��� �Ѵ�!
            StartCoroutine(IELoadSceneAsync(_ChangeSceneType, _LoadingScene, !_Event, _FakeLoading));
        }
        else
        {
            //�ε����� ������ �ʴµ��� _FakeLoading�� ���� �� ���� ������, ���⼭ ����ó�� �ѹ� �Ѵ�.
            //�̺�Ʈ�� ����� ����, ����ũ �ε��� ���Ѵ�!
            StartCoroutine(IELoadSceneAsync(_ChangeSceneType, _LoadingScene, !_Event));
        }
    }

    private IEnumerator IELoadSceneAsync(SceneType _ChangeSceneType, bool _LoadingScene, bool _Event, bool _FakeLoading = false)
    {
        //�ε� ���� �ҷ����µ� 10�� �̻��� �ɸ��ٸ� ��Ʈ��������!
        float TimeOut   = 10f;
        float TimeDelay = 0f;

        //LoadingScene
        if (_LoadingScene)
        { 
            m_AsyncOperation = SceneManager.LoadSceneAsync(SceneType.Loading.ToString());

            //�켱 �ٷ� �ε����� ���� �ϴϱ� �ε����δ� �ٷ� ��
            m_AsyncOperation.allowSceneActivation = true;

            while (TimeDelay < TimeOut) //SceneLoading (LoadingScene)
            {
                //�ε� �� �̵� �Ϸ�
                if (m_AsyncOperation.isDone)
                {
                    break;
                }
                yield return null;
                TimeDelay += Time.deltaTime; //�ڷ�ƾ�̶� �����Ӵ� ȣ�� Ƚ�� �����ϱ� ��Ÿ Ÿ�� ���
            }

            //���� 10�� �̻� �ɷ��� ���
            if(TimeDelay >= TimeOut)
            {
                Debug.Log("SceneMng.IELoadSceneAsync() -> �ε��� �ε��� 10�� �̻� �ɷ��� ��Ʈ�Ƚ��ϴ�~");
                yield break;
            }
        }

        //ChangeScene �ε�
        m_EventCheck        = _Event; //Event �� ���ٸ� m_EventCheck�� true�� ����� ��� �� - üũ�� �� false�� ������ �ȵ��״�, �ݴ�� false
        m_LoadingProgress   = 0f;

        TimeOut     = 30f; //�ε��ϴµ� �ε��� �� �� ���°� �ƴϰ�, �����ִ� ���¿��� 30�� �̻� �ɸ��� ��Ʈ����
        TimeDelay   = 0f;

        //FakeLoading�� �Ѵٸ� ����� ���� ����
        float FakeTimeDelay = 0f;

        float PreLoadingProgress = -999.0f; //m_LoadingProgress ó���� 0f�̱� ������ ���� �ƹ��ų� �־��־���

        m_AsyncOperation = SceneManager.LoadSceneAsync(_ChangeSceneType.ToString());

        //���� �� �Ѿ �غ� ��ģ �� �̺�Ʈ�� ��ٸ���
        m_AsyncOperation.allowSceneActivation = _Event;

        //FakeLoading�� �Ѵٸ�, ���� �ð��� �Ǳ� ������ ������ allowSceneActivation�� false�� �����.
        if (_FakeLoading)
        {
            m_AsyncOperation.allowSceneActivation = false;
        }

        while (TimeDelay < TimeOut) //SceneLoading (_ChangeSceneType)
        {
            //�� �ε�(��ȯ)�� �����ٸ�
            if (m_AsyncOperation.isDone)
            {
                m_PreSceneType      = m_CurrentSceneType;
                m_CurrentSceneType  = _ChangeSceneType;
                m_AsyncOperation    = null;
                m_EventCheck        = false;
                m_LoadingProgress   = 0f;
                break;
            }
            else //�� �ε� ��
            {
                //�󸶳� ����Ǿ����� (�ٷ� �ȳѾ �� 0.9���� ���)
                m_LoadingProgress = m_AsyncOperation.progress;

                //����ũ �ε��̰�, ����ũ �ε� �ð��� ���� ������ �ʾҴٸ�
                if(_FakeLoading && FakeTimeDelay < m_FakeLoadingTime)
                {
                    //�ð� ����
                    FakeTimeDelay += Time.deltaTime;
                }
        
                //�ε��� ���� ����, m_AsyncOperation.allowSceneActivation�� false�� ��
                if (m_LoadingProgress >= 0.9f)
                {
                    //FakeLoading �̰�, �ð� ����� ������ ��!
                    if (_FakeLoading && FakeTimeDelay >= m_FakeLoadingTime)
                    {
                        //���� ���� ������ �ѱ��!
                        m_AsyncOperation.allowSceneActivation = true;
                    }

                    m_EventCheck = true;
                }
                else if(PreLoadingProgress == m_LoadingProgress) //�ε��� �ȵǰ� �ִ� ����
                {
                    TimeDelay += Time.deltaTime;
                }
                else //����ϰ� �ε��ϴ� ����
                {
                    PreLoadingProgress  = m_LoadingProgress;
                    TimeDelay           = 0f;
                }
            }
            yield return null;
        }

        if (TimeDelay >= TimeOut)
        {
            //�ε��� �� �ȵ� �����ε� ���� ���¿��� 30�� �̻� �ɷ� ������ ��
            Debug.Log("SceneMng.IELoadSceneAsync() -> �ε��� ������ 30�ʰ� ������ ��Ʈ�� ������ϴ�.");
            yield break;
        }

        yield break;
    }


    //�� Event �ִ� �Լ� (ȣ�� ��)
    public bool Set_SceneChangeEvent(bool _Ignore = false)
    {
        //���� ó�� //_Ignore -> �ε� �� �Ϸ���� �ʾƵ� ��ư �����ٸ� �˾Ƽ� �ٷ� ��� ��ȯ
        if (m_EventCheck || _Ignore)
        {
            m_AsyncOperation.allowSceneActivation = true;
            return true;
        }

        //m_EventCheck �� ���� -> �ε��� �� �ȵǾ��� �� Ÿ�� ��
        Debug.Log("SceneMng.Set_SceneChangeEvent() -> ���� �ε��� ���� �ȵǾ��µ� Set_SceneChangeEvent ȣ��");
        return false;
    }

    //�� �Ѿ �� ����ϴ��� Ȯ���ϴ� �Լ�
    public bool Get_UsingEvent()
    {
        //���� ó��
        if(!ReferenceEquals(m_AsyncOperation,null))
        {
            //�̺�Ʈ�� ����ϴ��� ����
            return m_UsingEvent;
        }

        Debug.Log("SceneMng.Get_allowSceneActivation() -> m_AsyncOperation �� Null�Դϴ�.");
        return false;
    }

    //���� �ε� �ۼ�Ʈ Ȯ��
    public float Get_LoadingPercent()
    {
        //���� ó��
        if (!ReferenceEquals(m_AsyncOperation, null))
        {
            if(m_LoadingProgress >= 0.9f) //������̸� 100�ۼ�Ʈ
            {
                return 100f;
            }
            return m_LoadingProgress * 111f;
        }

        Debug.Log("SceneMng.Get_LoadingPercent() -> m_AsyncOperation �� Null�Դϴ�.");
        return -1f;
    }
}
