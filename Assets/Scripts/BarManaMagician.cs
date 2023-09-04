using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class BarManaMagician : MonoBehaviour
{
    [SerializeField] private Transform mana;

    private Tween _coolDown;
    // Start is called before the first frame update
    void Awake()
    {
        Signals.Get<CoolDownBarMagician>().AddListener(ManaArcherCoolDown);
        Signals.Get<OnStopGame>().AddListener(StopCoolDown);
    }

    private void StopCoolDown()
    {
        _coolDown?.Kill();
        mana.localScale = new Vector3(1, 1, 0);
    }
    private void ManaArcherCoolDown(int coolDown)
    {
        mana.localScale = new Vector3(0, 1, 0);
        _coolDown = mana.DOScaleX(1, coolDown);
    }
}
