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
        print("Enter");

        m_image.color = m_HoverColor;
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        print("Exit");

        m_image.color = m_NormalColor;
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        print("Down");

        m_image.color = m_DownColor;
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        print("Up");
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        print("Click");

        m_image.color = m_HoverColor;
    }
}
