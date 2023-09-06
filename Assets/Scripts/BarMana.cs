
using DG.Tweening;
using UnityEngine;

public class BarMana : MonoBehaviour
{
    [SerializeField] private Transform mana;

    private Tween _coolDown;
    private float _startTime;
    private float _timeCoolDownRemain;
    private float _timeCoolDown;

    protected void StopCoolDown()
    {
        _coolDown?.Kill();
        mana.localScale = new Vector3(1, 1, 0);
    }
    protected void InitTime()
    {
        _timeCoolDownRemain = 0;
        _startTime = 0;
        _timeCoolDown = 0;
    }

    protected void StartTimeCooldown(float coolDown)
    {
        _timeCoolDown = coolDown;
        _startTime = Time.time;
    }

    protected void RestoreMana(float coolDown)
    {
        mana.localScale = new Vector3(0, 1, 0);
        _coolDown = mana.DOScaleX(1, coolDown).SetEase(Ease.Linear);
    }

    protected void ReduceTimeCooldown(float reduce)
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
