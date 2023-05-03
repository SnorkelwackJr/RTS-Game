using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameGlobalParameters gameGlobalParameters;
    public GameSoundParameters gameSoundParameters;
    private Ray _ray;
    private RaycastHit _raycastHit;
    public static GameManager instance;

    [HideInInspector]
    public bool gameIsPaused;
    public GamePlayersParameters gamePlayersParameters;

    [HideInInspector]
    public List<Unit> ownedProducingUnits = new List<Unit>();
    private float _producingRate = 1f; // in seconds
    private Coroutine _producingResourcesCoroutine = null;

    public void Start()
    {
        instance = this;

        _producingResourcesCoroutine = StartCoroutine("_ProducingResources");

        GameParameters[] gameParametersList =
          Resources.LoadAll<GameParameters>("ScriptableObjects/Parameters");
        foreach (GameParameters parameters in gameParametersList)
        {
            Debug.Log(parameters.GetParametersName());
            Debug.Log("> Fields shown in-game:");
            foreach (string fieldName in parameters.FieldsToShowInGame)
                Debug.Log($"    {fieldName}");
        }
    }
    
    private void Awake()
    {
        DataHandler.LoadGameData();
        gameIsPaused = false;
    }

    private void Update()
    {
        if (gameIsPaused) return;
        _CheckUnitsNavigation();
    }

    private void _CheckUnitsNavigation()
    {
        if (Globals.SELECTED_UNITS.Count > 0 && Input.GetMouseButtonUp(1))
        {
            _ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(
                _ray,
                out _raycastHit,
                1000f,
                Globals.TERRAIN_LAYER_MASK
            ))
            {
                foreach (UnitManager um in Globals.SELECTED_UNITS)
                    if (um.GetType() == typeof(CharacterManager))
                        ((CharacterManager)um).MoveTo(_raycastHit.point);
            }
        }
    }

    private void OnEnable()
    {
        EventManager.AddListener("PauseGame", _OnPauseGame);
        EventManager.AddListener("ResumeGame", _OnResumeGame);
    }

    private void OnDisable()
    {
        EventManager.RemoveListener("PauseGame", _OnPauseGame);
        EventManager.RemoveListener("ResumeGame", _OnResumeGame);
    }

    private void _OnPauseGame()
    {
        gameIsPaused = true;
        Time.timeScale = 0;

        if (_producingResourcesCoroutine != null)
        {
            StopCoroutine(_producingResourcesCoroutine);
            _producingResourcesCoroutine = null;
        }
    }

    private void _OnResumeGame()
    {
        gameIsPaused = false;
        Time.timeScale = 1;

        if (_producingResourcesCoroutine == null)
            _producingResourcesCoroutine = StartCoroutine("_ProducingResources");
    }

    private void OnApplicationQuit()
    {
#if !UNITY_EDITOR
        DataHandler.SaveGameData();
#endif
    }

    private IEnumerator _ProducingResources()
    {
        while (true)
        {
            foreach (Unit unit in ownedProducingUnits)
                unit.ProduceResources();
            EventManager.TriggerEvent("UpdateResourceTexts");
            yield return new WaitForSeconds(_producingRate);
        }
    }
}