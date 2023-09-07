using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BarManaArcher : Character
{
    // Start is called before the first frame update
    void Awake()
    {
        InitPlayer();
        Signals.Get<CoolDownBarArcher>().AddListener(ManaArcherCoolDown);
        Signals.Get<OnStopGame>().AddListener(OnStopGame);
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
