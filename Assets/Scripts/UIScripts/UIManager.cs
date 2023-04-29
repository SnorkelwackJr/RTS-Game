using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Transform buildingMenu;
    public GameObject buildingButtonPrefab;
    public Transform resourcesUIParent;
    public GameObject gameResourceDisplayPrefab;
    public GameObject gameResourceCostPrefab;
    public GameObject infoPanel;
    public Transform selectedUnitsListParent;
    public GameObject selectedUnitDisplayPrefab;
    public Transform selectionGroupsParent;
    public GameObject selectedUnitMenu;
    public GameObject unitSkillButtonPrefab;
    public GameObject buildingMenuObject;

    private BuildingPlacer _buildingPlacer;
    private Dictionary<string, Button> _buildingButtons;
    private TMPro.TextMeshProUGUI _infoPanelTitleText;
    private TMPro.TextMeshProUGUI _infoPanelDescriptionText;
    private Transform _infoPanelResourcesCostParent;
    private Dictionary<string, TMPro.TextMeshProUGUI> _resourceTexts;
    private RectTransform _selectedUnitContentRectTransform;
    private RectTransform _selectedUnitButtonsRectTransform;
    private TMPro.TextMeshProUGUI _selectedUnitTitleText;
    private TMPro.TextMeshProUGUI _selectedUnitLevelText;
    private Transform _selectedUnitResourcesProductionParent;
    private Transform _selectedUnitActionButtonsParent;
    private Unit _selectedUnit;
    public GameObject gameSettingsPanel;
    public Transform gameSettingsMenusParent;
    public TMPro.TextMeshProUGUI gameSettingsContentName;
    public Transform gameSettingsContentParent;
    public GameObject gameSettingsMenuButtonPrefab;
    public GameObject gameSettingsParameterPrefab;
    public GameObject sliderPrefab;
    public GameObject togglePrefab;
    private Dictionary<string, GameParameters> _gameParameters;

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
        {
            ToggleSelectionGroupButton(i, false);
        }

        Transform selectedUnitMenuTransform = selectedUnitMenu.transform;
        _selectedUnitContentRectTransform = selectedUnitMenuTransform
            .Find("Content").GetComponent<RectTransform>();
        _selectedUnitButtonsRectTransform = selectedUnitMenuTransform
            .Find("Buttons").GetComponent<RectTransform>();
        _selectedUnitTitleText = selectedUnitMenuTransform
            .Find("Content/Title").GetComponent<TMPro.TextMeshProUGUI>();
        _selectedUnitTitleText = selectedUnitMenuTransform
            .Find("Content/Title").GetComponent<TMPro.TextMeshProUGUI>();
        _selectedUnitLevelText = selectedUnitMenuTransform
            .Find("Content/Level").GetComponent<TMPro.TextMeshProUGUI>();
        _selectedUnitResourcesProductionParent = selectedUnitMenuTransform
            .Find("Content/ResourcesProduction");
        _selectedUnitActionButtonsParent = selectedUnitMenuTransform
            .Find("Buttons/SpecificActions");
        
        _ShowSelectedUnitMenu(false);

        // hide game settings
        gameSettingsPanel.SetActive(false);

        // read the list of game parameters and store them as a dict
        // (then setup the UI panel)
        GameParameters[] gameParametersList = Resources.LoadAll<GameParameters>(
            "ScriptableObjects/Parameters");
        _gameParameters = new Dictionary<string, GameParameters>();
        foreach (GameParameters p in gameParametersList)
            _gameParameters[p.GetParametersName()] = p;
        _SetupGameSettingsPanel();
    }

    private void OnEnable()
    {
        EventManager.AddListener("UpdateResourceTexts", _OnUpdateResourceTexts);
        EventManager.AddListener("CheckBuildingButtons", _OnCheckBuildingButtons);
        EventManager.AddListener("HoverBuildingButton", _OnHoverBuildingButton);
        EventManager.AddListener("UnhoverBuildingButton", _OnUnhoverBuildingButton);
        EventManager.AddListener("SelectUnit", _OnSelectUnit);
        EventManager.AddListener("DeselectUnit", _OnDeselectUnit);
    }

    private void OnDisable()
    {
        EventManager.RemoveListener("UpdateResourceTexts", _OnUpdateResourceTexts);
        EventManager.RemoveListener("CheckBuildingButtons", _OnCheckBuildingButtons);
        EventManager.RemoveListener("HoverBuildingButton", _OnHoverBuildingButton);
        EventManager.RemoveListener("UnhoverBuildingButton", _OnUnhoverBuildingButton);
        EventManager.RemoveListener("SelectUnit", _OnSelectUnit);
        EventManager.RemoveListener("DeselectUnit", _OnDeselectUnit);
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

    private void _OnHoverBuildingButton(object data)
    {
        SetInfoPanel((UnitData) data);
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

    private void _OnSelectUnit(object data)
    {
        Unit unit = (Unit) data;
        _AddSelectedUnitToUIList(unit);
        _SetSelectedUnitMenu(unit);
        _ShowSelectedUnitMenu(true);
    }

    private void _OnDeselectUnit(object data)
    {
        Unit unit = (Unit) data;
        _RemoveSelectedUnitFromUIList(unit.Code);
        if (Globals.SELECTED_UNITS.Count == 0)
            _ShowSelectedUnitMenu(false);
        //else
            //_SetSelectedUnitMenu(Globals.SELECTED_UNITS[Globals.SELECTED_UNITS.Count - 1].Unit);
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

    private void _SetSelectedUnitMenu(Unit unit)
    {
        _selectedUnit = unit;

        // adapt content panel heights to match info to display
        int contentHeight = 60 + unit.Production.Count * 16;
        _selectedUnitContentRectTransform.sizeDelta = new Vector2(64, contentHeight);
        _selectedUnitButtonsRectTransform.anchoredPosition = new Vector2(0, -contentHeight - 20);
        _selectedUnitButtonsRectTransform.sizeDelta = new Vector2(70, Screen.height - contentHeight - 20);
        // update texts
        _selectedUnitTitleText.text = unit.Data.unitName;
        _selectedUnitLevelText.text = $"Level {unit.Level}";
        // clear resource production and reinstantiate new one
        foreach (Transform child in _selectedUnitResourcesProductionParent)
        {
            Destroy(child.gameObject);
        }
        if (unit.Production.Count > 0)
        {
            GameObject g; Transform t;
            foreach (ResourceValue resource in unit.Production)
            {
                g = GameObject.Instantiate(
                    gameResourceCostPrefab, _selectedUnitResourcesProductionParent);
                t = g.transform;
                t.Find("Text").GetComponent<TMPro.TextMeshProUGUI>().text = $"+{resource.amount}";
                t.Find("Icon").GetComponent<Image>().sprite = Resources.Load<Sprite>($"Textures/GameResources/{resource.code}");
            }
        }

        // clear skills and reinstantiate new ones
        foreach (Transform child in _selectedUnitActionButtonsParent)
            Destroy(child.gameObject);
        if (unit.SkillManagers.Count > 0)
        {
            GameObject g; Transform t; Button b;
            for (int i = 0; i < unit.SkillManagers.Count; i++)
            {
                g = GameObject.Instantiate(
                    unitSkillButtonPrefab, _selectedUnitActionButtonsParent);
                t = g.transform;
                b = g.GetComponent<Button>();
                unit.SkillManagers[i].SetButton(b);
                t.Find("Text").GetComponent<TMPro.TextMeshProUGUI>().text =
                    unit.SkillManagers[i].skill.skillName;
                _AddUnitSkillButtonListener(b, i);
            }
        }
    }

    private void _ShowSelectedUnitMenu(bool show)
    {
        selectedUnitMenu.SetActive(show);
        buildingMenuObject.SetActive(!show);
    }

    private void _AddUnitSkillButtonListener(Button b, int i)
    {
        b.onClick.AddListener(() => _selectedUnit.TriggerSkill(i));
    }

    public void ToggleGameSettingsPanel()
    {
        bool showGameSettingsPanel = !gameSettingsPanel.activeSelf;
        gameSettingsPanel.SetActive(showGameSettingsPanel);
        EventManager.TriggerEvent(showGameSettingsPanel ? "PauseGame" : "ResumeGame");
    }

    private void _SetupGameSettingsPanel()
    {
        GameObject g; string n;
        List<string> availableMenus = new List<string>();
        foreach (GameParameters parameters in _gameParameters.Values)
        {
            // ignore game parameters assets that don't have
            // any parameter to show
            if (parameters.FieldsToShowInGame.Count == 0) continue;

            g = GameObject.Instantiate(
                gameSettingsMenuButtonPrefab, gameSettingsMenusParent);
            n = parameters.GetParametersName();
            g.transform.Find("Text (TMP)").GetComponent<TMPro.TextMeshProUGUI>().text = n;
            _AddGameSettingsPanelMenuListener(g.GetComponent<Button>(), n);
            availableMenus.Add(n);
        }

        // if possible, set the first menu as the currently active one
        if (availableMenus.Count > 0)
            _SetGameSettingsContent(availableMenus[0]);
    }

    private void _AddGameSettingsPanelMenuListener(Button b, string menu)
    {
        b.onClick.AddListener(() => _SetGameSettingsContent(menu));
    }

    private void _SetGameSettingsContent(string menu)
    {
        gameSettingsContentName.text = menu;

        foreach (Transform child in gameSettingsContentParent)
            Destroy(child.gameObject);

        GameParameters parameters = _gameParameters[menu];
        System.Type ParametersType = parameters.GetType();
        GameObject gWrapper, gEditor;
        RectTransform rtWrapper, rtEditor;
        int i = 0;
        float contentWidth = 534f;
        float parameterNameWidth = 200f;
        float fieldHeight = 32f;
        foreach (string fieldName in parameters.FieldsToShowInGame)
        {
            gWrapper = GameObject.Instantiate(
                gameSettingsParameterPrefab, gameSettingsContentParent);
            gWrapper.transform.Find("Text (TMP)").GetComponent<TMPro.TextMeshProUGUI>().text =
                Utils.CapitalizeWords(fieldName);

            gEditor = null;
            FieldInfo field = ParametersType.GetField(fieldName);
            if (field.FieldType == typeof(bool))
            {
                gEditor = Instantiate(togglePrefab);
                Toggle t = gEditor.GetComponent<Toggle>();
                t.isOn = (bool) field.GetValue(parameters);
                t.onValueChanged.AddListener(delegate {
                    _OnGameSettingsToggleValueChanged(parameters, field, fieldName, t);
                });
            }
            else if (field.FieldType == typeof(int) || field.FieldType == typeof(float))
            {
                bool isRange = System.Attribute.IsDefined(field, typeof(RangeAttribute), false);
                if (isRange)
                {
                    RangeAttribute attr = (RangeAttribute)System.Attribute.GetCustomAttribute(field, typeof(RangeAttribute));
                    gEditor = Instantiate(sliderPrefab);
                    Slider s = gEditor.GetComponent<Slider>();
                    s.minValue = attr.min;
                    s.maxValue = attr.max;
                    s.wholeNumbers = field.FieldType == typeof(int);
                    s.value = field.FieldType == typeof(int)
                        ? (int) field.GetValue(parameters)
                        : (float)field.GetValue(parameters);
                    s.onValueChanged.AddListener(delegate
                    {
                        _OnGameSettingsSliderValueChanged(parameters, field, fieldName, s);
                    });
                }
            }
            rtWrapper = gWrapper.GetComponent<RectTransform>();
            rtWrapper.anchoredPosition = new Vector2(0f, -i * fieldHeight);
            rtWrapper.sizeDelta = new Vector2(contentWidth, fieldHeight);

            if (gEditor != null)
            {
                gEditor.transform.SetParent(gWrapper.transform);
                rtEditor = gEditor.GetComponent<RectTransform>();
                rtEditor.anchoredPosition = new Vector2((parameterNameWidth + 16f), 0f);
                rtEditor.sizeDelta = new Vector2(rtWrapper.sizeDelta.x - (parameterNameWidth + 16f), fieldHeight);
            }

            i++;
        }

        RectTransform rt = gameSettingsContentParent.GetComponent<RectTransform>();
        Vector2 size = rt.sizeDelta;
        size.y = i * fieldHeight;
        rt.sizeDelta = size;
    }

    private void _OnGameSettingsToggleValueChanged(
        GameParameters parameters,
        FieldInfo field,
        string gameParameter,
        Toggle change
    )
    {
        field.SetValue(parameters, change.isOn);
        EventManager.TriggerEvent($"UpdateGameParameter:{gameParameter}", change.isOn);
    }

    private void _OnGameSettingsSliderValueChanged(
        GameParameters parameters,
        FieldInfo field,
        string gameParameter,
        Slider change
    )
    {
        if (field.FieldType == typeof(int))
            field.SetValue(parameters, (int) change.value);
        else
            field.SetValue(parameters, change.value);
        EventManager.TriggerEvent($"UpdateGameParameter:{gameParameter}", change.value);
    }
}