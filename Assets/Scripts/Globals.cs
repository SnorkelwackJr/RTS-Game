using System.Collections.Generic;

public enum InGameResource
{
    Gold,
    Wood,
    Stone
}

public class Globals
{
    public static int TERRAIN_LAYER_MASK = 1 << 8;
    public static int UNIT_MASK = 1 << 0;
    public static List<UnitManager> SELECTED_UNITS = new List<UnitManager>();

    public static BuildingData[] BUILDING_DATA;
    public static Building CURRENT_PLACED_BUILDING = null;

    public static Dictionary<InGameResource, GameResource> GAME_RESOURCES =
        new Dictionary<InGameResource, GameResource>()
    {
        { InGameResource.Gold, new GameResource("Gold", 1000) },
        { InGameResource.Wood, new GameResource("Wood", 1000) },
        { InGameResource.Stone, new GameResource("Stone", 1000) }
    };
}