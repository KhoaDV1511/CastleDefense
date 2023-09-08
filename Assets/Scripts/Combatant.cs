using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

public class Combatant : Character
{
    private Vector3 _posSpawn;
    private GamePlayModel _gamePlayModel = GamePlayModel.Instance;
    private const int MANA_COMBATANT = 10;
    private const string COMBATANT = "combatant";
    private const int TIME_COOLDOWN_SKILL = 10;
    // Start is called before the first frame update
    private void Awake()
    {
        InitPlayer();
        InitBot();
        Signals.Get<CombatantSkills>().AddListener(CombatantSkill);
        Signals.Get<OnStopGame>().AddListener(OnStopGame);
        Signals.Get<TimeReduce>().AddListener(TimeRemainSkill);
        Signals.Get<CoolDownBarCombatant>().AddListener(ManaCoolDown);
        Signals.Get<TimeReduce>().AddListener(ReduceTime);
    }

    void OnMouseDown()
    {
        if(_gamePlayModel.isPlaying && !_isCoolDown)
            UseSkill(COMBATANT, MANA_COMBATANT);
    }
    private void ManaCoolDown(int coolDown)
    {
        StartTimeCooldown(coolDown);
        RestoreMana(coolDown);
    }
    private void ReduceTime(float reduce)
    {
        ReduceTimeCooldown(reduce);
    }
    private void TimeRemainSkill(float reduce)
    {
        SkillAfterReduce(reduce, TIME_COOLDOWN_SKILL);
    }
    private void CombatantSkill()
    {
        Signals.Get<CoolDownBarCombatant>().Dispatch(TIME_COOLDOWN_SKILL);
        TimeUseSkill(TIME_COOLDOWN_SKILL);
        var posObj = skill.transform.position;
        for (int i = 1; i <= _spawnQuantity; i++)
        {
            float randomY = posObj.y;
            _posSpawn = new Vector3(posObj.x, randomY + 0.35f * i, 0);
            var obj = Instantiate(skill, _posSpawn, Quaternion.identity);
            obj.transform.SetParent(transform);
            obj.SetActive(true);
        }
    }
    private void OnStopGame()
    {
        StopCoolDownSkill();
        StopBarMana();
    }
}
