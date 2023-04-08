using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private BlueprintScript _blueprintScript;

    public Transform contextualMenu;
    public GameObject buildingButtonPrefab;

    private void Awake()
    {
        _blueprintScript = GetComponent<BlueprintScript>();

        for (int i = 0; i < Globals.BUILDING_DATA.Length; i++)
        {
            GameObject button = GameObject.Instantiate(
                buildingButtonPrefab,
                contextualMenu);
            string code = Globals.BUILDING_DATA[i].Code;
            button.name = code;
            button.transform.Find("Text").GetComponent<Text>().text = code;
            Button b = button.GetComponent<Button>();
            //_AddBuildingButtonListener(b, i);
        }
    }

    /*private void _AddBuildingButtonListener(Button b, int i)
    {
        b.onClick.AddListener(() => _blueprintScript.SelectPlacedBuilding(i));   
    }*/
}
