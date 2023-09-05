using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class BarManaThunder : MonoBehaviour
{
    [SerializeField] private Transform mana;

    private Tween _coolDown;
    private float _startTime;
    private float _timeCoolDownRemain;
    private float _timeCoolDown;
    // Start is called before the first frame update
    void Awake()
    {
        _timeCoolDownRemain = 0;
        _startTime = 0;
        _timeCoolDown = 0;
        Signals.Get<CoolDownBarThunder>().AddListener(ManaArcherCoolDown);
        Signals.Get<OnStopGame>().AddListener(StopCoolDown);
        Signals.Get<TimeReduce>().AddListener(ReduceTime);
    }

    private void StopCoolDown()
    {
        _coolDown?.Kill();
        mana.localScale = new Vector3(1, 1, 0);
    }
    private void ManaArcherCoolDown(int coolDown)
    {
        _timeCoolDown = coolDown;
        _startTime = Time.time;
        mana.localScale = new Vector3(0, 1, 0);
        _coolDown = mana.DOScaleX(1, coolDown).SetEase(Ease.Linear);
    }
    private void ReduceTime(float reduce)
    {
        _coolDown?.Kill();
        _timeCoolDownRemain = _timeCoolDown - (Time.time - _startTime) + reduce;
        if(_timeCoolDownRemain - reduce <= 0)
            mana.localScale = new Vector3(1, 1, 0);
        else
        {
            var barXCurrent = (1 / _timeCoolDown) * ((Time.time - _startTime) + reduce);
            mana.localScale = new Vector3(barXCurrent, 1, 0);
            _coolDown = mana.DOScaleX(1, _timeCoolDownRemain).SetEase(Ease.Linear);
        }
    }
}
