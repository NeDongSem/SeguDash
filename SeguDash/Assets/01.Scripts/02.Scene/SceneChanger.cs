using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneChanger : MonoBehaviour
{
    [Header("Scene Change")]
    [Tooltip("���� ��")]
    [SerializeField]
    private SceneType m_ChangeScene = SceneType.SceneType_End;
    [Tooltip("�ε� ���� ���� ���� ����")]
    [SerializeField]
    private bool m_LoadingScene = false;
    [Tooltip("�� ��ü�� �� [�ٷ� or �̺�Ʈ] ����")]
    [SerializeField]
    private bool m_Event = false;
    [Tooltip("�ε� ���� ���� ��, ����ũ �ε� �ð� ���� ����")]
    [SerializeField]
    private bool m_FakeLoading = false;

    //�� ��ü
    public void SceneChange()
    {
        SceneMng.Instance.Set_SceneChange(m_ChangeScene, m_LoadingScene, m_Event, m_FakeLoading);
    }
}
