using System.Collections.Generic;
using UnityEngine;

public enum BuildingPlacementState
{
    VALID,
    INVALID,
    FIXED
};

public class Building : Unit
{

    private BuildingManager _buildingManager;
    private BuildingPlacementState _placement;
    private List<Material> _materials;

    public Building(BuildingData data, int owner) : this(data, owner, new List<ResourceValue>() { }) { }
    public Building(BuildingData data, int owner, List<ResourceValue> production) :
        base(data, owner, production)
    {
        _buildingManager = _transform.GetComponent<BuildingManager>();
        _materials = new List<Material>();
        foreach (Material material in _transform.GetComponent<Renderer>().materials)
        {
            _materials.Add(new Material(material));
        }

        _placement = BuildingPlacementState.VALID;
        SetMaterials();
    }

    public void SetMaterials() { SetMaterials(_placement); }
    public void SetMaterials(BuildingPlacementState placement)
    {
        List<Material> materials;
        if (placement == BuildingPlacementState.VALID)
        {
            Material refMaterial = Resources.Load("Materials/Valid") as Material;
            materials = new List<Material>();
            for (int i = 0; i < _materials.Count; i++)
                materials.Add(refMaterial);
        }
        else if (placement == BuildingPlacementState.INVALID)
        {
            Material refMaterial = Resources.Load("Materials/Invalid") as Material;
            materials = new List<Material>();
            for (int i = 0; i < _materials.Count; i++)
                materials.Add(refMaterial);
        }
        else if (placement == BuildingPlacementState.FIXED)
            materials = _materials;
        else
            return;
        _transform.Find("Mesh").GetComponent<Renderer>().materials = materials.ToArray();
    }

    public override void Place()
    {
        base.Place();
        // set placement state
        _placement = BuildingPlacementState.FIXED;
        // change building materials
        SetMaterials();
    }

    public void CheckValidPlacement()
    {
        if (_placement == BuildingPlacementState.FIXED) return;
        _placement = _buildingManager.CheckPlacement()
            ? BuildingPlacementState.VALID
            : BuildingPlacementState.INVALID;
    }

    public bool HasValidPlacement { get => _placement == BuildingPlacementState.VALID; }
    public bool IsFixed { get => _placement == BuildingPlacementState.FIXED; }
    public int DataIndex
    {
        get
        {
            for (int i = 0; i < Globals.BUILDING_DATA.Length; i++)
            {
                if (Globals.BUILDING_DATA[i].code == _data.code)
                    return i;
            }
            return -1;
        }
    }
}