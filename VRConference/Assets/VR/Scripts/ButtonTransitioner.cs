using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonTransitioner : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler,IPointerDownHandler,IPointerUpHandler,IPointerClickHandler
{

    public Color32 m_NormalColor = Color.white;
    public Color32 m_HoverColor = Color.gray;
    public Color32 m_DownColor = Color.red;

    private Image m_image = null;
    private void Awake()
    {
        m_image = GetComponent<Image>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {

        m_image.color = m_HoverColor;
    }
    public void OnPointerExit(PointerEventData eventData)
    {

        m_image.color = m_NormalColor;
    }
    public void OnPointerDown(PointerEventData eventData)
    {

        m_image.color = m_DownColor;
    }
    public void OnPointerUp(PointerEventData eventData)
    {
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        m_image.color = m_HoverColor;
    }
}
