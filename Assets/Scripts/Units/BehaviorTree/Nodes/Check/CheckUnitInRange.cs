using UnityEngine;

using BehaviorTree;

public class CheckUnitInRange : Node
{
    UnitManager _manager;
    float _range;
    Transform _lastTarget;
    float _targetSize;

    public CheckUnitInRange(UnitManager manager, bool checkAttack) : base()
    {
        _manager = manager;
        _range = checkAttack
            ? _manager.Unit.AttackRange
            : ((CharacterData)_manager.Unit.Data).buildRange;
        _lastTarget = null;
    }

    public override NodeState Evaluate()
    {
        object currentTarget = Parent.GetData("currentTarget");
        if (currentTarget == null)
        {
            _state = NodeState.FAILURE;
            return _state;
        }

        Transform target = (Transform)currentTarget;
        if (target != _lastTarget)
        {
            Vector3 s = target
                .Find("Mesh")
                .GetComponent<MeshFilter>()
                .sharedMesh.bounds.size / 2;
            _targetSize = Mathf.Max(s.x, s.z);
            _lastTarget = target;
        }

        // (in case the target object is gone - for example it died
        // and we haven't cleared it from the data yet)
        if (!target)
        {
            Parent.ClearData("currentTarget");
            Parent.ClearData("currentTargetOffset");
            _state = NodeState.FAILURE;
            return _state;
        }

        float d = Vector3.Distance(_manager.transform.position, target.position);
        bool isInRange = (d - _targetSize) <= _range;
        _state = isInRange ? NodeState.SUCCESS : NodeState.FAILURE;
        return _state;
    }
}