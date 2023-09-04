using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class SkillMagician : MonoBehaviour
{
    private const int MANA_COMBATANT = 10;
    private const string MAGICIAN = "Magician";
    private GamePlayModel _gamePlayModel = GamePlayModel.Instance;
    private const int TIME_COOLDOWN_SKILL = 10;
    private Tween _coolDownSkill;

    private bool _isCoolDown = false;

    private float _timeReduce;
    // Start is called before the first frame update
    void Awake()
    {
        _timeReduce = 3;
        _isCoolDown = false;
        Signals.Get<MagicianSkills>().AddListener(ReduceCoolDown);
        Signals.Get<OnStopGame>().AddListener(StopGame);
    }

    private void OnMouseDown()
    {
        if(_gamePlayModel.isPlaying && !_isCoolDown)
            Signals.Get<ManaUse>().Dispatch(MAGICIAN, MANA_COMBATANT);
    }

    private void ReduceCoolDown()
    {
        _isCoolDown = true;
        Signals.Get<CoolDownBarMagician>().Dispatch(TIME_COOLDOWN_SKILL);
        Signals.Get<TimeReduce>().Dispatch(_timeReduce);
        _coolDownSkill = DOVirtual.DelayedCall(TIME_COOLDOWN_SKILL, () =>
        {
            _isCoolDown = false;
        });
    }

    private void StopGame()
    {
        _isCoolDown = false;
        _coolDownSkill?.Kill();
    }
}
