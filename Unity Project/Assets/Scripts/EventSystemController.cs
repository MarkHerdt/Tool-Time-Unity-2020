using System;
using UnityEngine;

public class EventSystemController : MonoBehaviour
{
    // Eventsystem
    public static EventSystemController self;
    private void Awake()
    {
        self = this;
    }


    // Random Object breaks
    public event Action onRandomObjectBreaks;
    public void RandomObjectBreaks()
    {
        if (onRandomObjectBreaks != null)
        {
            onRandomObjectBreaks();
        }
    }


    // Object is repaired
    public event Action onObjectRepaired;
    public void ObjectRepaired()
    {
        if (onObjectRepaired != null)
        {
            onObjectRepaired();
        }
    }


    // Broadcast total damage
    public event Action<float> onBroadcastTotalDamage;
    public void BroadcastTotalDamage(float damage)
    {
        if (onBroadcastTotalDamage != null)
        {
            onBroadcastTotalDamage(damage);
            //Debug.Log("Gesamtschaden Broadcast: " + (int)(damage * 100) + "%");
        }
    }


    // Game could be lost at the moment
    public event Action<bool> onVictoryIsInDanger;
    public void VictoryIsInDanger(bool inDanger)
    {
        if (onVictoryIsInDanger != null)
        {
            onVictoryIsInDanger(inDanger);
        }
    }


    // Change value of total damage bar in UI
    public event Action<float> onChangeDamageBar;
    public void ChangeDamageBar(float damage)
    {
        if (onChangeDamageBar != null)
        {
            onChangeDamageBar(damage);
        }
    }


    // Countdown ends
    public event Action onGlobalTimerEnd;
    public void GlobalTimerEnd()
    {
        if (onGlobalTimerEnd != null)
        {
            onGlobalTimerEnd();
        }
    }

}
