using UnityEngine;

[CreateAssetMenu(fileName = "Global Parameters", menuName = "Scriptable Objects/Game Global Parameters", order = 10)]
public class GameGlobalParameters : GameParameters
{
    [Header("Initialization")]
    public BuildingData initialBuilding;

    [Header("FOV")]
    public bool enableFOV;
}