using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class BarManaMagician : BarMana
{
    // Start is called before the first frame update
    void Awake()
    {
        Signals.Get<CoolDownBarMagician>().AddListener(ManaArcherCoolDown);
        Signals.Get<OnStopGame>().AddListener(StopCoolDown);
    }
    
    private void ManaArcherCoolDown(int coolDown)
    {
        RestoreMana(coolDown);
    }
}
