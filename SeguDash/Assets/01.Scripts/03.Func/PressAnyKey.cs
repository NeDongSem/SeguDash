using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PressAnyKey : MonoBehaviour
{
    [SerializeField]
    private UnityEvent m_PressAnyKeyEvent;

    private void Update()
    {
        //�ƹ�Ű or ���콺 ������, ���� Ŭ��
        if(Input.anyKeyDown || Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
        {
            m_PressAnyKeyEvent.Invoke();
        }
    }
}
