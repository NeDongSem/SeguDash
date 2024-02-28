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

    ///////////////////////싱글톤/////////////////////////////

    //현재 씬
    private SceneType m_CurrentSceneType = SceneType.Logo;
    public  SceneType CurrentSceneType { get { return m_CurrentSceneType; } }
    //이전 씬
    private SceneType m_PreSceneType = SceneType.SceneType_End;
    public  SceneType PreSceneType { get { return m_PreSceneType; } }

    //allowSceneActivation을 이용하여 다음 Scene을 바로 보이기 위함
    private AsyncOperation  m_AsyncOperation;
    //로딩 얼마나 되었나, 0.9가 되면 사실상 100%
    private float           m_LoadingProgress = 0f;
    //씬 넘기는 Event 함수 호출시 예외처리에 필요한 bool값
    private bool            m_EventCheck = false;
    //씬 넘기는 Event 함수 호출시 예외처리에 필요한 bool값
    private bool            m_UsingEvent = false;

    [SerializeField]
    private float m_FakeLoadingTime = 2f;

    private void Init()
    {
    }

    //_Event : 씬을 넘어갈 때 이벤트를 받아서 넘어갈 것인지 //_LoadingScene : 로딩 씬을 통해 넘어 갈 것인지 //fakeloading 시간을 줄건지
    public void Set_SceneChange(SceneType _ChangeSceneType, bool _LoadingScene = false, bool _Event = false, bool _FakeLoading = false)
    {
        //Event 사용 여부
        m_UsingEvent = _Event;
        //_Event를 반전시키는 이유 : 사람이 생각하기엔 _Event에 true를 넣어주면 이벤트를 줘야 넘어간다고
        //생각하겠으나, _Event가 사용 되는 allowSceneActivation는 false일 때, 0.9에서 멈춰서 대기함.
        //그리고 allowSceneActivation가 다시 true가 되어야 다음 씬 로딩을 완료하고 바꿈.
        //그래서 여기서 반전시켜서 넘겨주고 사용.
        //_LoadingScene = true : 로딩씬을 경유한다 (로딩씬 다음 씬이 _ChangeSceneType)
        if (_LoadingScene && !_Event)
        {
            //로딩 씬을 쓰고, 이벤트를 안 쓸 때만 페이크 로딩을 한다!
            StartCoroutine(IELoadSceneAsync(_ChangeSceneType, _LoadingScene, !_Event, _FakeLoading));
        }
        else
        {
            //로딩씬을 통하지 않는데도 _FakeLoading을 설정 할 수도 있으니, 여기서 예외처리 한번 한다.
            //이벤트를 사용할 때는, 페이크 로딩을 안한다!
            StartCoroutine(IELoadSceneAsync(_ChangeSceneType, _LoadingScene, !_Event));
        }
    }

    private IEnumerator IELoadSceneAsync(SceneType _ChangeSceneType, bool _LoadingScene, bool _Event, bool _FakeLoading = false)
    {
        //로딩 씬을 불러오는데 10초 이상이 걸린다면 터트려버린다!
        float TimeOut   = 10f;
        float TimeDelay = 0f;

        //LoadingScene
        if (_LoadingScene)
        { 
            m_AsyncOperation = SceneManager.LoadSceneAsync(SceneType.Loading.ToString());

            //우선 바로 로딩으로 가야 하니까 로딩으로는 바로 감
            m_AsyncOperation.allowSceneActivation = true;

            while (TimeDelay < TimeOut) //SceneLoading (LoadingScene)
            {
                //로딩 씬 이동 완료
                if (m_AsyncOperation.isDone)
                {
                    break;
                }
                yield return null;
                TimeDelay += Time.deltaTime; //코루틴이라 프레임당 호출 횟수 같으니까 델타 타임 사용
            }

            //만약 10초 이상 걸렸을 경우
            if(TimeDelay >= TimeOut)
            {
                Debug.Log("SceneMng.IELoadSceneAsync() -> 로딩씬 로딩이 10초 이상 걸려서 터트렸습니다~");
                yield break;
            }
        }

        //ChangeScene 로딩
        m_EventCheck        = _Event; //Event 안 쓴다면 m_EventCheck를 true로 만들어 줘야 함 - 체크할 때 false로 있으면 안될테니, 반대면 false
        m_LoadingProgress   = 0f;

        TimeOut     = 30f; //로딩하는데 로딩이 다 된 상태가 아니고, 멈춰있는 상태에서 30초 이상 걸리면 터트린다
        TimeDelay   = 0f;

        //FakeLoading을 한다면 사용할 축적 공간
        float FakeTimeDelay = 0f;

        float PreLoadingProgress = -999.0f; //m_LoadingProgress 처음이 0f이기 때문에 음수 아무거나 넣어주었음

        m_AsyncOperation = SceneManager.LoadSceneAsync(_ChangeSceneType.ToString());

        //다음 씬 넘어갈 준비를 마친 후 이벤트를 기다릴지
        m_AsyncOperation.allowSceneActivation = _Event;

        //FakeLoading을 한다면, 일정 시간이 되기 전까진 무조건 allowSceneActivation를 false로 만든다.
        if (_FakeLoading)
        {
            m_AsyncOperation.allowSceneActivation = false;
        }

        while (TimeDelay < TimeOut) //SceneLoading (_ChangeSceneType)
        {
            //씬 로딩(전환)이 끝났다면
            if (m_AsyncOperation.isDone)
            {
                m_PreSceneType      = m_CurrentSceneType;
                m_CurrentSceneType  = _ChangeSceneType;
                m_AsyncOperation    = null;
                m_EventCheck        = false;
                m_LoadingProgress   = 0f;
                break;
            }
            else //씬 로딩 중
            {
                //얼마나 진행되었는지 (바로 안넘어갈 시 0.9에서 대기)
                m_LoadingProgress = m_AsyncOperation.progress;

                //페이크 로딩이고, 페이크 로딩 시간이 아직 지나지 않았다면
                if(_FakeLoading && FakeTimeDelay < m_FakeLoadingTime)
                {
                    //시간 누적
                    FakeTimeDelay += Time.deltaTime;
                }
        
                //로딩이 끝난 상태, m_AsyncOperation.allowSceneActivation이 false일 때
                if (m_LoadingProgress >= 0.9f)
                {
                    //FakeLoading 이고, 시간 충분히 지났을 때!
                    if (_FakeLoading && FakeTimeDelay >= m_FakeLoadingTime)
                    {
                        //이제 다음 씬으로 넘긴다!
                        m_AsyncOperation.allowSceneActivation = true;
                    }

                    m_EventCheck = true;
                }
                else if(PreLoadingProgress == m_LoadingProgress) //로딩이 안되고 있는 상태
                {
                    TimeDelay += Time.deltaTime;
                }
                else //평범하게 로딩하는 상태
                {
                    PreLoadingProgress  = m_LoadingProgress;
                    TimeDelay           = 0f;
                }
            }
            yield return null;
        }

        if (TimeDelay >= TimeOut)
        {
            //로딩이 다 안된 상태인데 같은 상태에서 30초 이상 걸려 터졌을 때
            Debug.Log("SceneMng.IELoadSceneAsync() -> 로딩이 멈춘지 30초가 지나서 터트려 버려쭙니다.");
            yield break;
        }

        yield break;
    }


    //씬 Event 주는 함수 (호출 시)
    public bool Set_SceneChangeEvent(bool _Ignore = false)
    {
        //예외 처리 //_Ignore -> 로딩 다 완료되지 않아도 버튼 눌렀다면 알아서 바로 장면 전환
        if (m_EventCheck || _Ignore)
        {
            m_AsyncOperation.allowSceneActivation = true;
            return true;
        }

        //m_EventCheck 안 열림 -> 로딩이 다 안되었을 때 타는 곳
        Debug.Log("SceneMng.Set_SceneChangeEvent() -> 아직 로딩이 전부 안되었는데 Set_SceneChangeEvent 호출");
        return false;
    }

    //씬 넘어갈 때 대기하는지 확인하는 함수
    public bool Get_UsingEvent()
    {
        //예외 처리
        if(!ReferenceEquals(m_AsyncOperation,null))
        {
            //이벤트를 사용하는지 여부
            return m_UsingEvent;
        }

        Debug.Log("SceneMng.Get_allowSceneActivation() -> m_AsyncOperation 가 Null입니다.");
        return false;
    }

    //현재 로딩 퍼센트 확인
    public float Get_LoadingPercent()
    {
        //예외 처리
        if (!ReferenceEquals(m_AsyncOperation, null))
        {
            if(m_LoadingProgress >= 0.9f) //대기중이면 100퍼센트
            {
                return 100f;
            }
            return m_LoadingProgress * 111f;
        }

        Debug.Log("SceneMng.Get_LoadingPercent() -> m_AsyncOperation 가 Null입니다.");
        return -1f;
    }
}
