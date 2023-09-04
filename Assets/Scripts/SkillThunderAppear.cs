using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class SkillThunderAppear : MonoBehaviour
{
    private Tween _coolDownAppear;
    private void OnEnable()
    {
        Signals.Get<OnStopGame>().AddListener(StopCoolDown);
        _coolDownAppear = DOVirtual.DelayedCall(1.5f, () => gameObject.SetActive(false));
    }

    private void OnDisable()
    {
        Signals.Get<OnStopGame>().RemoveListener(StopCoolDown);
    }
    private void StopCoolDown()
    {
        _coolDownAppear?.Kill();
        gameObject.SetActive(false);
    }
}
