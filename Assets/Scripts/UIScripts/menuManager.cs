using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class menuManager : MonoBehaviour, IPointerExitHandler
{
    RectTransform rectTransform;

    public void OnPointerExit(PointerEventData eventData)
    {
        Debug.Log("Pointer left the menu.");
        rectTransform = GetComponent<RectTransform>();
        rectTransform.transform.localPosition = new Vector2(0, 472);
    }
}