using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameGlobalParameters gameGlobalParameters;
    public GameSoundParameters gameSoundParameters;
    public GameInputParameters gameInputParameters;
    private Ray _ray;
    private RaycastHit _raycastHit;
    public static GameManager instance;
    public Vector3 startPosition;

    [HideInInspector]
    public bool gameIsPaused;
    public GamePlayersParameters gamePlayersParameters;

    [HideInInspector]
    public List<Unit> ownedProducingUnits = new List<Unit>();

    [HideInInspector]
    public float producingRate = 0.5f; // in seconds

    [HideInInspector]
    public bool waitingForInput;
    [HideInInspector]
    public string pressedKey;

    //TEST
    public TestScriptableObject testScriptableObject;

    public void Start()
    {
        instance = this;
    }
    
    private void Awake()
    {
        DataHandler.LoadGameData();
        gameIsPaused = false;

        _GetStartPosition();

        Globals.InitializeGameResources(gamePlayersParameters.players.Length);

        //TEST
        //testScriptableObject.SaveToFile();
        testScriptableObject.LoadFromFile();
    }

    private void Update()
    {
        if (Input.anyKeyDown)
        {
            if (waitingForInput)
            {
                if (Input.GetMouseButtonDown(0))
                    pressedKey = "mouse 0";
                else if (Input.GetMouseButtonDown(1))
                    pressedKey = "mouse 1";
                else if (Input.GetMouseButtonDown(2))
                    pressedKey = "mouse 2";
                else
                    pressedKey = Input.inputString;
                waitingForInput = false;
            }
            else
                gameInputParameters.CheckForInput();
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

    private void _GetStartPosition()
    {
        startPosition = Utils.MiddleOfScreenPointToWorld();
    }

#if UNITY_EDITOR
    private void OnGUI()
    {
        GUILayout.BeginArea(new Rect(0f, 40f, 100f, 100f));

        int newMyPlayerId = GUILayout.SelectionGrid(
            gamePlayersParameters.myPlayerId,
            gamePlayersParameters.players.Select((p, i) => i.ToString()).ToArray(),
            gamePlayersParameters.players.Length
        );

        GUILayout.EndArea();
        
        if (newMyPlayerId != gamePlayersParameters.myPlayerId)
        {
            gamePlayersParameters.myPlayerId = newMyPlayerId;
            EventManager.TriggerEvent("SetPlayer", newMyPlayerId);
        }
    }
#endif
}