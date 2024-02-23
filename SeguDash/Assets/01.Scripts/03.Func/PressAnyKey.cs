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
        //아무키 or 마우스 오른쪽, 왼쪽 클릭
        if(Input.anyKeyDown || Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
        {
            m_PressAnyKeyEvent.Invoke();
        }
    }
}
