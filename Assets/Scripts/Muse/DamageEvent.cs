using System;
using Graph;
using Unity.Muse.Behavior;
using UnityEngine;

#if UNITY_EDITOR
[CreateAssetMenu(menuName = "Muse Behavior/Event Channels/DamageEvent")]
#endif
[Serializable]
[EventChannelDescription(name: "DamageEvent", message: "[Self] damages [Player] for [Damage]", category: "Events", id: "9a27eca20c367cf83a4767a33ebb7b04")]
public class DamageEvent : EventChannelBase
{
    public delegate void DamageEventEventHandler(GameObject Self, GameObject Player, int Damage);
    public event DamageEventEventHandler Event; 

    public void SendEventMessage(GameObject Self, GameObject Player, int Damage)
    {
        Event?.Invoke(Self, Player, Damage);
    }

    public override void SendEventMessage(BlackboardVariable[] messageData)
    {
        BlackboardVariable<GameObject> SelfBlackboardVariable = messageData[0] as BlackboardVariable<GameObject>;
        var Self = SelfBlackboardVariable != null ? SelfBlackboardVariable.Value : default(GameObject);

        BlackboardVariable<GameObject> PlayerBlackboardVariable = messageData[1] as BlackboardVariable<GameObject>;
        var Player = PlayerBlackboardVariable != null ? PlayerBlackboardVariable.Value : default(GameObject);

        BlackboardVariable<int> DamageBlackboardVariable = messageData[2] as BlackboardVariable<int>;
        var Damage = DamageBlackboardVariable != null ? DamageBlackboardVariable.Value : default(int);

        Event?.Invoke(Self, Player, Damage);
    }

    public override Delegate CreateEventHandler(BlackboardVariable[] vars, System.Action callback)
    {
        DamageEventEventHandler del = (Self, Player, Damage) =>
        {
            BlackboardVariable<GameObject> var0 = vars[0] as BlackboardVariable<GameObject>;
            if(var0 != null)
                var0.Value = Self;

            BlackboardVariable<GameObject> var1 = vars[1] as BlackboardVariable<GameObject>;
            if(var1 != null)
                var1.Value = Player;

            BlackboardVariable<int> var2 = vars[2] as BlackboardVariable<int>;
            if(var2 != null)
                var2.Value = Damage;

            callback();
        };
        return del;
    }

    public override void RegisterListener(Delegate del)
    {
        Event += del as DamageEventEventHandler;
    }

    public override void UnregisterListener(Delegate del)
    {
        Event -= del as DamageEventEventHandler;
    }
}

