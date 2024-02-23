using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingbarFilled : MonoBehaviour
{
    [SerializeField]
    private Image m_Loadingbar;
    private float m_LoadingbarFillAmount;

    [SerializeField]
    private float m_Speed;

    //위로 차오르고 있는지?
    private bool m_Up = false;

    [SerializeField]
    [Range(0f,1f)]
    private float m_MinimumFill = 0.3f;

    private void Awake()
    {
        m_LoadingbarFillAmount = m_Loadingbar.fillAmount;
    }

    private void Update()
    {
        if(m_Up)
        {
            //fill 증가
            m_LoadingbarFillAmount += Time.deltaTime * m_Speed;
            if(m_LoadingbarFillAmount > 1f)
            {
                m_LoadingbarFillAmount = 2f - m_LoadingbarFillAmount;
                m_Up = false;
            }
            m_Loadingbar.fillAmount = m_LoadingbarFillAmount;
        }
        else
        {
            //fill 감소
            m_LoadingbarFillAmount -= Time.deltaTime * m_Speed;
            if (m_LoadingbarFillAmount < m_MinimumFill)
            {
                m_LoadingbarFillAmount = m_MinimumFill * 2f - m_LoadingbarFillAmount;
                m_Up = true;
            }
            m_Loadingbar.fillAmount = m_LoadingbarFillAmount;
        }
    }
}
