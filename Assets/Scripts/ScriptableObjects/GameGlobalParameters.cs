using UnityEngine;

[CreateAssetMenu(fileName = "Global Parameters", menuName = "Scriptable Objects/Game Global Parameters", order = 10)]
public class GameGlobalParameters : GameParameters
{
    public override string GetParametersName() => "Global";
    
    [Header("Initialization")]
    public BuildingData initialBuilding;

    [Header("FOV")]
    public bool enableFOV;

    public int baseGoldProduction;
    public int baseWoodProduction;
    public int baseStoneProduction;
}