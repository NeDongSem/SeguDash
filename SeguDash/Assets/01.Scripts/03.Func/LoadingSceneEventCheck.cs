using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingSceneEventCheck : MonoBehaviour
{
    [SerializeField]
    private GameObject m_Text_PressAnyButton;

    void Awake()
    {
        //무조건 꺼놓는다.
        m_Text_PressAnyButton.SetActive(false);

        //만약 이벤트를 안쓴다면 스스로 사라진다
        if (!SceneMng.Instance.Get_UsingEvent())
        {
            gameObject.SetActive(false);
        }
    }

    void Update()
    {
        //로딩이 끝났다면
        if(SceneMng.Instance.Get_LoadingPercent() >= 100f)
        {
            //m_PressAnyButton를 활성화 시키고 (아무 효과 없이 유저에게 알리는 용도의 텍스트
            if (!m_Text_PressAnyButton.activeSelf)
            {
                m_Text_PressAnyButton.SetActive(true);
            }

            //아무키 or 마우스 오왼 버튼을 누른다면
            if (Input.anyKeyDown || Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
            {
                //정해놓은 다음 씬으로 넘어가라고 이벤트를 준다.
                SceneMng.Instance.Set_SceneChangeEvent();
            }
        }
    }
}
