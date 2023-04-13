using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class BuildingManager : UnitManager
{
    private Building _building = null;
    private int _nCollisions = 0;

    protected override Unit Unit
    {
        get { return _building; }
        set { _building = value is Building ? (Building)value : null; }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Terrain") return;
        _nCollisions++;
        CheckPlacement();
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Terrain") return;
        _nCollisions--;
        CheckPlacement();
    }

    public bool CheckPlacement()
    {
        if (_building == null) return false;
        if (_building.IsFixed) return false;
        bool validPlacement = HasValidPlacement();
        if (!validPlacement)
        {
            _building.SetMaterials(BuildingPlacementState.INVALID);
        }
        else
        {
            _building.SetMaterials(BuildingPlacementState.VALID);
        }
        return validPlacement;
    }

    public bool HasValidPlacement()
    {
        return _nCollisions == 0;
    }

    protected override bool IsActive()
    {
        return _building.IsFixed;
    }
}