using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BarManaArcher : MonoBehaviour
{
    [SerializeField] private Transform mana;

    private Tween _coolDown;
    // Start is called before the first frame update
    void Awake()
    {
        Signals.Get<CoolDownBarArcher>().AddListener(ManaArcherCoolDown);
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
