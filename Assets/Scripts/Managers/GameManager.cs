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

    [HideInInspector]
    public float producingRate = 3f; // in seconds

    public void Start()
    {
        instance = this;
    }
    
    private void Awake()
    {
        DataHandler.LoadGameData();
        gameIsPaused = false;
    }

    private void Update()
    {
        if (gameIsPaused) return;
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
    }

    private void _OnResumeGame()
    {
        gameIsPaused = false;
        Time.timeScale = 1;
    }

    private void OnApplicationQuit()
    {
#if !UNITY_EDITOR
        DataHandler.SaveGameData();
#endif
    }
}