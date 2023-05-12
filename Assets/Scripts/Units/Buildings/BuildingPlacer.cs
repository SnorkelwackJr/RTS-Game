using UnityEngine;
using UnityEngine.EventSystems;

public class BuildingPlacer : MonoBehaviour
{
    private Building _placedBuilding = null;
    private Ray _ray;
    private RaycastHit _raycastHit;
    private Vector3 _lastPlacementPosition;

    private void Start()
    {
        // instantiate headquarters at the beginning of the game
        _placedBuilding = new Building(
            GameManager.instance.gameGlobalParameters.initialBuilding,
            0
        );
        _placedBuilding.SetPosition(GameManager.instance.startPosition);

        // link the data into the manager
        _placedBuilding.Transform.GetComponent<BuildingManager>().Initialize(_placedBuilding);
        _PlaceBuilding();
        
        // make sure we have no building selected when the player starts
        // to play
        _CancelPlacedBuilding();
    }

    void Update()
    {
        if (GameManager.instance.gameIsPaused) return;
        
        if (_placedBuilding != null)
        {
            Globals.CURRENT_PLACED_BUILDING = _placedBuilding;
            if (Input.GetKeyUp(KeyCode.Escape))
            {
                _CancelPlacedBuilding();
                return;
            }

            _ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(
                _ray,
                out _raycastHit,
                1000f,
                Globals.TERRAIN_LAYER_MASK
            ))
            {
                _placedBuilding.SetPosition(_raycastHit.point);
                if (_lastPlacementPosition != _raycastHit.point)
                {
                    _placedBuilding.CheckValidPlacement();
                }
                _lastPlacementPosition = _raycastHit.point;
            }

            if (_placedBuilding.HasValidPlacement && Input.GetMouseButtonUp(0) && !EventSystem.current.IsPointerOverGameObject())
            {
                _PlaceBuilding();
            }
        }
    }

    public void SelectPlacedBuilding(int buildingDataIndex)
    {
        _PreparePlacedBuilding(buildingDataIndex);
    }

    void _PreparePlacedBuilding(int buildingDataIndex)
    {
        // destroy the previous "phantom" if there is one
        if (_placedBuilding != null  && !_placedBuilding.IsFixed)
        {
            Destroy(_placedBuilding.Transform.gameObject);
        }

        Building building = new Building(
            Globals.BUILDING_DATA[buildingDataIndex],
            GameManager.instance.gamePlayersParameters.myPlayerId
        );

        _placedBuilding = building;
        _lastPlacementPosition = Vector3.zero;
    }

    void _PlaceBuilding()
    {
        _placedBuilding.ComputeProduction();
        _placedBuilding.Place();
        
        if (_placedBuilding.CanBuy())
        {
            _PreparePlacedBuilding(_placedBuilding.DataIndex);
        }
        else
        {
            _placedBuilding = null;
            Globals.CURRENT_PLACED_BUILDING = null;
        }
        EventManager.TriggerEvent("UpdateResourceTexts");
        EventManager.TriggerEvent("CheckBuildingButtons");
        EventManager.TriggerEvent("PlaySoundByName", "onBuildingPlacedSound");
    }

    void _CancelPlacedBuilding()
    {
        // destroy the "phantom" building
        Destroy(_placedBuilding.Transform.gameObject);
        _placedBuilding = null;
        Globals.CURRENT_PLACED_BUILDING = null;
    }

    private void OnEnable()
    {
        EventManager.AddListener("<Input>Build", _OnBuildInput);
    }

    private void OnDisable()
    {
        EventManager.RemoveListener("<Input>Build", _OnBuildInput);
    }

    private void _OnBuildInput(object data)
    {
        string buildingCode = (string)data;
        for (int i = 0; i < Globals.BUILDING_DATA.Length; i++)
        {
            if (Globals.BUILDING_DATA[i].code == buildingCode)
            {
                SelectPlacedBuilding(i);
                return;
            }
        }
    }
}