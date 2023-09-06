using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BarManaArcher : BarMana
{
    // Start is called before the first frame update
    void Awake()
    {
        InitTime();
        Signals.Get<CoolDownBarArcher>().AddListener(ManaArcherCoolDown);
        Signals.Get<OnStopGame>().AddListener(StopCoolDown);
        Signals.Get<TimeReduce>().AddListener(ReduceTime);
    }

    private void ManaArcherCoolDown(int coolDown)
    {
        StartTimeCooldown(coolDown);
        RestoreMana(coolDown);
    }
    private void ReduceTime(float reduce)
    {
        ReduceTimeCooldown(reduce);
    }
}
