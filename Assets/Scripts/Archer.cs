using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using DG.Tweening;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Archer : Character
{
    public static Archer Instance;
    private Archer _instance;
    private float _repeatRate = 1;
    private const int TIME_PROCESS_SKILL = 2;
    private const int TIME_COOLDOWN_SKILL = 10;
    
    private const int MANA_ACHER = 10;
    private const string ACHER = "Archer";
    private Tween _doneSkill;

    private GamePlayModel _gamePlayModel = GamePlayModel.Instance;
    // Start is called before the first frame update
    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            Instance = _instance;
        }
        else
        {
            Instance = _instance;
        }

        InitPlayer();
        Signals.Get<OnStopGame>().AddListener(StopSpawn);
        Signals.Get<StartFindEnemy>().AddListener(DetectEnemy);
        Signals.Get<ArcherSkills>().AddListener(SkillArcher);
        Signals.Get<TimeReduce>().AddListener(TimeRemainSkill);
    }

    private void OnMouseDown()
    {
        if(_gamePlayModel.isPlaying && _enemysInsideArea.Length > 0 && !_isCoolDown)
        {
            UseSkill(ACHER, MANA_ACHER);
        }
        
    }

    private void TimeRemainSkill(float reduce)
    {
        SkillAfterReduce(reduce, TIME_COOLDOWN_SKILL);
    }

    private void SkillArcher()
    {
        CancelInvoke();
        _repeatRate = 0.2f;
        InvokeRepeating(nameof(SpawnProjectiles), 0, _repeatRate);
        Signals.Get<CoolDownBarArcher>().Dispatch(TIME_COOLDOWN_SKILL);
        TimeUseSkill(TIME_COOLDOWN_SKILL);
        _doneSkill = DOVirtual.DelayedCall(TIME_PROCESS_SKILL,() =>
        {
            CancelInvoke();
            _repeatRate = 1f;
            InvokeRepeating(nameof(SpawnProjectiles), 0, _repeatRate);
        });
    }
    private void StopSpawn()
    {
        _gamePlayModel.isPlaying = false;
        _doneSkill?.Kill();
        OnStopGame();
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
