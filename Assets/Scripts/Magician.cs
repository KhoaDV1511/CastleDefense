using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Magician : Character
{
    private const int MANA_COMBATANT = 10;
    private const string MAGICIAN = "Magician";
    private GamePlayModel _gamePlayModel = GamePlayModel.Instance;
    private const int TIME_COOLDOWN_SKILL = 10;

    private float _timeReduce;
    // Start is called before the first frame update
    void Awake()
    {
        _timeReduce = 3;
        _isCoolDown = false;
        Signals.Get<MagicianSkills>().AddListener(SkillReduceCoolDown);
        Signals.Get<OnStopGame>().AddListener(StopGame);
    }

    private void OnMouseDown()
    {
        if(_gamePlayModel.isPlaying && !_isCoolDown)
            UseSkill(MAGICIAN, MANA_COMBATANT);
    }

    private void SkillReduceCoolDown()
    {
        Signals.Get<CoolDownBarMagician>().Dispatch(TIME_COOLDOWN_SKILL);
        Signals.Get<TimeReduce>().Dispatch(_timeReduce);
        _isCoolDown = true;
        _coolDownSkill = DOVirtual.DelayedCall(TIME_COOLDOWN_SKILL, () =>
        {
            _isCoolDown = false;
        });
        skill.SetActive(true);
    }

    private void StopGame()
    {
        StopCoolDownSkill();
    }
}
