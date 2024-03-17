using System;
using UnityEngine;
using Unity.Muse.Behavior;
using Action = Unity.Muse.Behavior.Action;

[Serializable]
[NodeDescription(name: "Instantiate", story: "Spawn [GameObject] at [Position] with [Rotation]", category: "Action", id: "08bf4d3a4b6d4d74032876fa54076b55")]
public class Instantiate : Action
{
    public BlackboardVariable<GameObject> GameObject;
    public BlackboardVariable<Vector3> Position;
    public BlackboardVariable<Vector3> Rotation;

    protected override Status OnStart()
    {
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        return Status.Success;
    }

    protected override void OnEnd()
    {
    }
}

