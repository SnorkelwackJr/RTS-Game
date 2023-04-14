using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private BuildingPlacer _buildingPlacer;
    private Dictionary<string, Button> _buildingButtons;
    public Transform buildingMenu;
    public GameObject buildingButtonPrefab;
    public Transform resourcesUIParent;
    public GameObject gameResourceDisplayPrefab;
    public GameObject gameResourceCostPrefab;
    public GameObject infoPanel;
    private TMPro.TextMeshProUGUI _infoPanelTitleText;
    private TMPro.TextMeshProUGUI _infoPanelDescriptionText;
    private Transform _infoPanelResourcesCostParent;

    private Dictionary<string, TMPro.TextMeshProUGUI> _resourceTexts;
    public Transform selectedUnitsListParent;
    public GameObject selectedUnitDisplayPrefab;
    public Transform selectionGroupsParent;

    private void Awake()
    {
        // create texts for each in-game resource (gold, wood, stone...)
        _resourceTexts = new Dictionary<string, TMPro.TextMeshProUGUI>();
        foreach (KeyValuePair<string, GameResource> pair in Globals.GAME_RESOURCES)
        {
            GameObject display = Instantiate(gameResourceDisplayPrefab, resourcesUIParent);
            display.name = pair.Key;
            _resourceTexts[pair.Key] = display.transform.Find("Text").GetComponent<TMPro.TextMeshProUGUI>();
            _SetResourceText(pair.Key, pair.Value.Amount);
        }


        // create buttons for each building type
        _buildingPlacer = GetComponent<BuildingPlacer>();
        _buildingButtons = new Dictionary<string, Button>();
        if (Globals.BUILDING_DATA.Length == 0) Debug.Log("NO BUILDINGS!");
        for (int i = 0; i < Globals.BUILDING_DATA.Length; i++)
        {
            BuildingData data = Globals.BUILDING_DATA[i];
            GameObject button = Instantiate(buildingButtonPrefab, buildingMenu);
            button.name = data.unitName;
            button.transform.Find("Text (TMP)").GetComponent<TMPro.TextMeshProUGUI>().text = data.unitName;
            Button b = button.GetComponent<Button>();
            _buildingButtons[data.code] = b;

            _AddBuildingButtonListener(b, i);
            if (!Globals.BUILDING_DATA[i].CanBuy())
            {
                b.interactable = false;
            }

            button.GetComponent<BuildingButton>().Initialize(Globals.BUILDING_DATA[i]);
        }

        Transform infoPanelTransform = infoPanel.transform;
        _infoPanelTitleText = infoPanelTransform.Find("Content/Title").GetComponent<TMPro.TextMeshProUGUI>();
        _infoPanelDescriptionText = infoPanelTransform.Find("Content/Description").GetComponent<TMPro.TextMeshProUGUI>();
        _infoPanelResourcesCostParent = infoPanelTransform.Find("Content/GameResourceCost");
        ShowInfoPanel(false);

        // hide all selection group buttons
        for (int i = 1; i <= 9; i++)
            ToggleSelectionGroupButton(i, false);
    }

    private void OnEnable()
    {
        EventManager.AddListener("UpdateResourceTexts", _OnUpdateResourceTexts);
        EventManager.AddListener("CheckBuildingButtons", _OnCheckBuildingButtons);
        EventManager.AddTypedListener("HoverBuildingButton", _OnHoverBuildingButton);
        EventManager.AddListener("UnhoverBuildingButton", _OnUnhoverBuildingButton);
        EventManager.AddTypedListener("SelectUnit", _OnSelectUnit);
        EventManager.AddTypedListener("DeselectUnit", _OnDeselectUnit);
    }

    private void OnDisable()
    {
        EventManager.RemoveListener("UpdateResourceTexts", _OnUpdateResourceTexts);
        EventManager.RemoveListener("CheckBuildingButtons", _OnCheckBuildingButtons);
        EventManager.RemoveTypedListener("HoverBuildingButton", _OnHoverBuildingButton);
        EventManager.RemoveListener("UnhoverBuildingButton", _OnUnhoverBuildingButton);
        EventManager.RemoveTypedListener("SelectUnit", _OnSelectUnit);
        EventManager.RemoveTypedListener("DeselectUnit", _OnDeselectUnit);
    }

    private void _AddBuildingButtonListener(Button b, int i)
    {
        b.onClick.AddListener(() => _buildingPlacer.SelectPlacedBuilding(i));
    }

    private void _SetResourceText(string resource, int value)
    {
        _resourceTexts[resource].text = value.ToString();
    }

    public void UpdateResourceTexts()
    {
        foreach (KeyValuePair<string, GameResource> pair in Globals.GAME_RESOURCES)
        {
            _SetResourceText(pair.Key, pair.Value.Amount);
        }
    }

    public void CheckBuildingButtons()
    {
        foreach (BuildingData data in Globals.BUILDING_DATA)
        {
            _buildingButtons[data.code].interactable = data.CanBuy();
        }
    }

    public void _OnUpdateResourceTexts()
    {
        foreach(KeyValuePair<string, GameResource> pair in Globals.GAME_RESOURCES)
        {
            _SetResourceText(pair.Key, pair.Value.Amount);
        }
    }

    public void _OnCheckBuildingButtons()
    {
        foreach(BuildingData data in Globals.BUILDING_DATA)
        {
            _buildingButtons[data.code].interactable = data.CanBuy();
        }
    }

    private void _OnHoverBuildingButton(CustomEventData data)
    {
        SetInfoPanel(data.unitData);
        ShowInfoPanel(true);
    }

    private void _OnUnhoverBuildingButton()
    {
        ShowInfoPanel(false);
    }

    public void SetInfoPanel(UnitData data)
    {
        // update texts
        if (data.unitName != "")
            _infoPanelTitleText.text = data.unitName;
        if (data.description != "")
            _infoPanelDescriptionText.text = data.description;
        
        // clear resource costs and reinstantiate new ones
        foreach (Transform child in _infoPanelResourcesCostParent)
        {
            Destroy(child.gameObject);
        }
            
        if (data.Cost.Count > 0)
        {
            GameObject g; Transform t;
            foreach (ResourceValue resource in data.Cost)
            {
                g = GameObject.Instantiate(gameResourceCostPrefab, _infoPanelResourcesCostParent);
                t = g.transform;
                t.Find("Text").GetComponent<TMPro.TextMeshProUGUI>().text = resource.amount.ToString();
                t.Find("Icon").GetComponent<Image>().sprite = Resources.Load<Sprite>(
                    $"Textures/GameResources/{resource.code}");

                // check to see if resource requirement is not
                // currently met - in that case, turn the text into the "invalid"
                // color
                if (Globals.GAME_RESOURCES[resource.code].Amount < resource.amount)
                    t.Find("Text").GetComponent<TMPro.TextMeshProUGUI>().color = new Color32(200, 0, 0, 255);
            }
        }
    }

    public void ShowInfoPanel(bool show)
    {
        infoPanel.SetActive(show);
    }

    private void _OnSelectUnit(CustomEventData data)
    {
        _AddSelectedUnitToUIList(data.unit);
    }

    private void _OnDeselectUnit(CustomEventData data)
    {
        _RemoveSelectedUnitFromUIList(data.unit.Code);
    }

    public void _AddSelectedUnitToUIList(Unit unit)
    {
        // if there is another unit of the same type already selected,
        // increase the counter
        Transform alreadyInstantiatedChild = selectedUnitsListParent.Find(unit.Code);
        if (alreadyInstantiatedChild != null)
        {
            TMPro.TextMeshProUGUI t = alreadyInstantiatedChild.Find("Count").GetComponent<TMPro.TextMeshProUGUI>();
            int count = int.Parse(t.text);
            t.text = (count + 1).ToString();
        }
        // else create a brand new counter initialized with a count of 1
        else
        {
            GameObject g = GameObject.Instantiate(
                selectedUnitDisplayPrefab, selectedUnitsListParent);
            g.name = unit.Code;
            Transform t = g.transform;
            t.Find("Count").GetComponent<TMPro.TextMeshProUGUI>().text = "1";
            t.Find("Name").GetComponent<TMPro.TextMeshProUGUI>().text = unit.Data.unitName;
        }
    }

    public void _RemoveSelectedUnitFromUIList(string code)
    {
        Transform listItem = selectedUnitsListParent.Find(code);
        if (listItem == null) return;
        TMPro.TextMeshProUGUI t = listItem.Find("Count").GetComponent<TMPro.TextMeshProUGUI>();
        int count = int.Parse(t.text);
        count -= 1;
        if (count == 0)
            DestroyImmediate(listItem.gameObject);
        else
            t.text = count.ToString();
    }

    public void ToggleSelectionGroupButton(int groupIndex, bool on)
    {
        selectionGroupsParent.Find(groupIndex.ToString()).gameObject.SetActive(on);
    }
}