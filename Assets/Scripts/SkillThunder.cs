using System;
using System.Collections;
using System.Collections.Generic;
using System.Resources;
using DG.Tweening;
using UnityEngine;

public class SkillThunder : MonoBehaviour
{
    [SerializeField] private GameObject skill;
    private const int MANA_COMBATANT = 10;
    private const string THUNDER = "Thunder";
    private GamePlayModel _gamePlayModel = GamePlayModel.Instance;
    private const int TIME_COOLDOWN_SKILL = 10;
    private bool _isCoolDown = false;
    private Tween _coolDownSkill;
    // Start is called before the first frame update
    void Awake()
    {
        _isCoolDown = false;
        Signals.Get<ThunderSkills>().AddListener(Thunder);
        Signals.Get<OnStopGame>().AddListener(StopSpawn);
    }

    private void OnMouseDown()
    {
        if(_gamePlayModel.isPlaying && !_isCoolDown)
            Signals.Get<ManaUse>().Dispatch(THUNDER, MANA_COMBATANT);
    }

    private void Thunder()
    {
        _isCoolDown = true;
        Signals.Get<CoolDownBarThunder>().Dispatch(TIME_COOLDOWN_SKILL);
        _coolDownSkill = DOVirtual.DelayedCall(TIME_COOLDOWN_SKILL, () =>
        {
            _isCoolDown = false;
        });
        skill.SetActive(true);
    }
    private void StopSpawn()
    {
        _coolDownSkill?.Kill();
        _isCoolDown = false;
    }
}
