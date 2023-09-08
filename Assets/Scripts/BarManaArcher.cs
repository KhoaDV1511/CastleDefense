using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class BarManaArcher : MonoBehaviour
{
    [SerializeField] private Transform mana;
    private float _startTime;
    private float _timeCoolDownRemain;
    private float _timeCoolDown;
    private Tween _coolDownMana;
    // Start is called before the first frame update
    void Awake()
    {
        InitTime();
        Signals.Get<CoolDownBarArcher>().AddListener(ManaArcherCoolDown);
        Signals.Get<OnStopGame>().AddListener(OnStopGame);
        Signals.Get<TimeReduce>().AddListener(ReduceTime);
    }
    private void OnStopGame()
    {
        _coolDownMana?.Kill();
        mana.localScale = new Vector3(1, 1, 0);
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
    private void StartTimeCooldown(float coolDown)
    {
        _timeCoolDown = coolDown;
        _startTime = Time.time;
    }
    private void RestoreMana(float coolDown)
    {
        mana.localScale = new Vector3(0, 1, 0);
        _coolDownMana = mana.DOScaleX(1, coolDown).SetEase(Ease.Linear);
    }
    private void ReduceTimeCooldown(float reduce)
    {
        _coolDownMana?.Kill();
        _timeCoolDownRemain = _timeCoolDown - (Time.time - _startTime) + reduce;
        if(_timeCoolDownRemain - reduce <= 0)
            mana.localScale = new Vector3(1, 1, 0);
        else
        {
            var barXCurrent = (1 / _timeCoolDown) * ((Time.time - _startTime) + reduce);
            mana.localScale = new Vector3(barXCurrent, 1, 0);
            _coolDownMana = mana.DOScaleX(1, _timeCoolDownRemain).SetEase(Ease.Linear);
        }
    }
    private void InitTime()
    {
        _timeCoolDownRemain = 0;
        _startTime = 0;
        _timeCoolDown = 0;
    }
}
