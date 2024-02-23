using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingSceneEventCheck : MonoBehaviour
{
    [SerializeField]
    private GameObject m_Text_PressAnyButton;

    void Awake()
    {
        //������ �����´�.
        m_Text_PressAnyButton.SetActive(false);

        //���� �̺�Ʈ�� �Ⱦ��ٸ� ������ �������
        if (!SceneMng.Instance.Get_UsingEvent())
        {
            gameObject.SetActive(false);
        }
    }

    void Update()
    {
        //�ε��� �����ٸ�
        if(SceneMng.Instance.Get_LoadingPercent() >= 100f)
        {
            //m_PressAnyButton�� Ȱ��ȭ ��Ű�� (�ƹ� ȿ�� ���� �������� �˸��� �뵵�� �ؽ�Ʈ
            if (!m_Text_PressAnyButton.activeSelf)
            {
                m_Text_PressAnyButton.SetActive(true);
            }

            //�ƹ�Ű or ���콺 ���� ��ư�� �����ٸ�
            if (Input.anyKeyDown || Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
            {
                //���س��� ���� ������ �Ѿ��� �̺�Ʈ�� �ش�.
                SceneMng.Instance.Set_SceneChangeEvent();
            }
        }
    }
}
