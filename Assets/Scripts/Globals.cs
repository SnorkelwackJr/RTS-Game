using System.Collections.Generic;

public enum InGameResource
{
    Gold,
    Wood,
    Stone
}

public enum UnitFormationType
{
    None,
    Line,
    Grid,
    XCross
}

public class Globals
{
    public static int TERRAIN_LAYER_MASK = 1 << 8;
    public static int UNIT_MASK = 1 << 0;
    public static List<UnitManager> SELECTED_UNITS = new List<UnitManager>();
    public static UnitFormationType UNIT_FORMATION_TYPE = UnitFormationType.None;

    public static BuildingData[] BUILDING_DATA;
    public static Dictionary<string, CharacterData> CHARACTER_DATA =
        new Dictionary<string, CharacterData>();
    public static Building CURRENT_PLACED_BUILDING = null;

    public static Dictionary<InGameResource, GameResource>[] GAME_RESOURCES;
    public static void InitializeGameResources(int nPlayers)
    {
        GAME_RESOURCES = new Dictionary<InGameResource, GameResource>[nPlayers];
        for (int i = 0; i < nPlayers; i++)
            GAME_RESOURCES[i] = new Dictionary<InGameResource, GameResource>()
                {
                    { InGameResource.Gold, new GameResource("Gold", 1000) },
                    { InGameResource.Wood, new GameResource("Wood", 1000) },
                    { InGameResource.Stone, new GameResource("Stone", 1000) }
                };
    }

    public static bool CanBuy(List<ResourceValue> cost)
    {
        return CanBuy(GameManager.instance.gamePlayersParameters.myPlayerId, cost);
    }
    public static bool CanBuy(int playerId, List<ResourceValue> cost)
    {
        foreach (ResourceValue resource in cost)
            if (GAME_RESOURCES[playerId][resource.code].Amount < resource.amount)
                return false;
        return true;
    }
}