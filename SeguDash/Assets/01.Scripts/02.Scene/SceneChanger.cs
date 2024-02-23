using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneChanger : MonoBehaviour
{
    [Header("Scene Change")]
    [Tooltip("다음 씬")]
    [SerializeField]
    private SceneType m_ChangeScene = SceneType.SceneType_End;
    [Tooltip("로딩 씬을 통해 갈지 설정")]
    [SerializeField]
    private bool m_LoadingScene = false;
    [Tooltip("씬 교체할 때 [바로 or 이벤트] 설정")]
    [SerializeField]
    private bool m_Event = false;
    [Tooltip("로딩 씬을 통할 때, 페이크 로딩 시간 줄지 설정")]
    [SerializeField]
    private bool m_FakeLoading = false;

    //씬 교체
    public void SceneChange()
    {
        SceneMng.Instance.Set_SceneChange(m_ChangeScene, m_LoadingScene, m_Event, m_FakeLoading);
    }
}
