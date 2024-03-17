using System;
using Graph;
using Unity.Muse.Behavior;
using UnityEngine;

#if UNITY_EDITOR
[CreateAssetMenu(menuName = "Muse Behavior/Event Channels/Complete Attack Event")]
#endif
[Serializable]
[EventChannelDescription(name: "Complete Attack Event", message: "Agent completes [Attack]", category: "Events", id: "679b3bc375aa1c717e0a4a5db4f8d291")]
public class CompleteAttackEvent : EventChannelBase
{
    public delegate void CompleteAttackEventEventHandler(string Attack);
    public event CompleteAttackEventEventHandler Event; 

    public void SendEventMessage(string Attack)
    {
        Event?.Invoke(Attack);
    }

    public override void SendEventMessage(BlackboardVariable[] messageData)
    {
        BlackboardVariable<string> AttackBlackboardVariable = messageData[0] as BlackboardVariable<string>;
        var Attack = AttackBlackboardVariable != null ? AttackBlackboardVariable.Value : default(string);

        Event?.Invoke(Attack);
    }

    public override Delegate CreateEventHandler(BlackboardVariable[] vars, System.Action callback)
    {
        CompleteAttackEventEventHandler del = (Attack) =>
        {
            BlackboardVariable<string> var0 = vars[0] as BlackboardVariable<string>;
            if(var0 != null)
                var0.Value = Attack;

            callback();
        };
        return del;
    }

    public override void RegisterListener(Delegate del)
    {
        Event += del as CompleteAttackEventEventHandler;
    }

    public override void UnregisterListener(Delegate del)
    {
        Event -= del as CompleteAttackEventEventHandler;
    }
}

