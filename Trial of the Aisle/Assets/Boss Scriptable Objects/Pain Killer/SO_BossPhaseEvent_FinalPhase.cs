
using UnityEngine;

[CreateAssetMenu(fileName = "Painkiller Final Phase Event", menuName = "Boss Scriptable Objects/Boss Phase Events/PainKiller/Final Phase Event")]

public class SO_BossPhaseEvent_FinalPhase : BossPhaseEvent
{
    public override void OnHealthChange()
    {
        Debug.Log("Play Event");
        eventPlayed = true;
    }
}
