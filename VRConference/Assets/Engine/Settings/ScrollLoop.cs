using UnityEngine;
using UnityEngine.UI;

public class ScrollLoop : MonoBehaviour
{
    [SerializeField] private RectTransform listContent;
    [SerializeField] private ScrollRect scroll;
    [SerializeField] private RectTransform viewport;

    [Range(0.0f, 0.5f)]
    [SerializeField] private float idleBound = 0.3f;
    
    private void Update()
    {
        float viewheitgh = viewport.rect.height;
        float viewPos = viewport.position.y;

        for (int i = 0; i < listContent.childCount; i++)
        {
            RectTransform child = (RectTransform) listContent.transform.GetChild(i);
            float percent = ((child.position.y - viewPos)  / (viewheitgh / 2));

            if (percent < 0) { percent *= -1; }
            if (percent < 0.1) { percent = 0.1f; }

            child.sizeDelta = new Vector2(child.sizeDelta.x, 100 * (1 - percent));
        }

        /*
        float percent = scroll.verticalNormalizedPosition;

        if (percent < idleBound)
        {
            RectTransform child = (RectTransform) listContent.transform.GetChild(0);
            child.SetAsLastSibling();
            
            float chagePercent = child.rect.height / 1000;
            scroll.verticalNormalizedPosition += chagePercent;
        }
        else if (percent > 1 - idleBound)
        {
            RectTransform child = (RectTransform) listContent.transform.GetChild(listContent.transform.childCount - 1);
            child.SetAsFirstSibling();
            
            float chagePercent = child.rect.height / 1000;
            scroll.verticalNormalizedPosition -= chagePercent;
        }
        
        Debug.Log(scroll.verticalNormalizedPosition);
        */
    }
}
