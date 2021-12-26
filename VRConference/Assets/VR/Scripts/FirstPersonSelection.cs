using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class FirstPersonSelection : MonoBehaviour
{
    public UIWorldDetect m_inputModule;

    private void Start()
    {
        m_inputModule = GameObject.Find("EventSystem").GetComponent<UIWorldDetect>();
    }
    void Update()
    {
        PointerEventData data = m_inputModule.GetData();
        float targetLength = data.pointerCurrentRaycast.distance;
        RaycastHit hit = CreateRaycast(targetLength);
    }

    private RaycastHit CreateRaycast(float length)
    {
        RaycastHit hit;
        Ray ray = new Ray(transform.position, transform.forward);
        Physics.Raycast(ray, out hit, length);

        return hit;
    }
}

