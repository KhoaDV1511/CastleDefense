using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class BarManaMagician : Character
{
    // Start is called before the first frame update
    void Awake()
    {
        Signals.Get<CoolDownBarMagician>().AddListener(ManaArcherCoolDown);
        Signals.Get<OnStopGame>().AddListener(OnStopGame);
    }
    
    private void ManaArcherCoolDown(int coolDown)
    {
        RestoreMana(coolDown);
    }
}
