using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class menuDropdown : MonoBehaviour, IPointerEnterHandler
{
    public RectTransform dropdownMenu;
    public int dropdownDistance = 322;

    public void OnPointerEnter(PointerEventData eventData)
    {
        Debug.Log("The cursor has entered the dropdown area.");
        dropdownMenu.transform.localPosition = new Vector2(0, dropdownDistance);
    }
}