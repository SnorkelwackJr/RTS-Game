using UnityEngine;

using BehaviorTree;

public class TaskBuild: Node
{
    int _buildPower;

    public TaskBuild(UnitManager manager) : base()
    {
        _buildPower = ((CharacterData) manager.Unit.Data).buildPower;
    }

    public override NodeState Evaluate()
    {
        Debug.Log("In TaskBuild");
        object currentTarget = GetData("currentTarget");
        BuildingManager bm = ((Transform)currentTarget).GetComponent<BuildingManager>();
        bool finishedBuilding = bm.Build(_buildPower);
        if (finishedBuilding)
        {
            ClearData("currentTarget");
            ClearData("currentTargetOffset");
        }

        _state = NodeState.SUCCESS;
        return _state;
    }
}