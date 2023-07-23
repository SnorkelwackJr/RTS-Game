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

    private BuildingBT _bt;
    private float _constructionRatio;
    private bool _isAlive;

    private MeshFilter _rendererMesh;
    private Mesh[] _constructionMeshes;

    public Building(BuildingData data, int owner) : this(data, owner, new List<ResourceValue>() { }) { }
    public Building(BuildingData data, int owner, List<ResourceValue> production) :
        base(data, owner, production)
    {
        _buildingManager = _transform.GetComponent<BuildingManager>();
        _bt = _transform.GetComponent<BuildingBT>();
        _bt.enabled = false;

        _constructionRatio = 0f;
        _isAlive = false;
        
        Transform mesh = _transform.Find("Mesh");
        
        _materials = new List<Material>();
        foreach (Material material in _transform.GetComponent<Renderer>().materials)
        {
            _materials.Add(new Material(material));
        }

        SetMaterials();
        _placement = BuildingPlacementState.VALID;

        _rendererMesh = mesh.GetComponent<MeshFilter>();
        _constructionMeshes = data.constructionMeshes;

        if (data.ambientSound != null)
        {
            if (_buildingManager.ambientSource != null)
            {
                _buildingManager.ambientSource.clip = data.ambientSound;
                _buildingManager.ambientSource.enabled = false;
            }
            else
                Debug.LogWarning($"'{data.unitName}' prefab is missing an ambient audio source!");
        }
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
        // change building construction ratio
        SetConstructionRatio(0);

        Globals.CURRENT_PLACED_BUILDING = null;
    }

    public void SetConstructionRatio(float constructionRatio)
    {
        if (_isAlive) return;
        
        _constructionRatio = constructionRatio;
        Debug.Log("Construction at: " + (_constructionRatio * 100) + "%");

        int meshIndex = Mathf.Max(
            0,
            (int)(_constructionMeshes.Length * constructionRatio) - 1);
        Mesh m = _constructionMeshes[meshIndex];
        _rendererMesh.sharedMesh = m;
        
        if (_constructionRatio >= 1)
            _SetAlive();
    }

    private void _SetAlive()
    {
        _isAlive = true;
        _bt.enabled = true;
        ComputeProduction();
        
        _buildingManager.ambientSource.enabled = true;
        _buildingManager.ambientSource.Play();
        EventManager.TriggerEvent("PlaySoundByName", "onBuildingPlacedSound");
        
        //Globals.UpdateNavMeshSurface();
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
    public float ConstructionRatio { get => _constructionRatio; }
    public override bool IsAlive { get => _isAlive; set { _isAlive = value; } }
}