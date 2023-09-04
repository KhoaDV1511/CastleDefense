using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

public class SpawnBot : MonoBehaviour
{
    [SerializeField] private GameObject objBot;
    private int _spawnQuantity;
    private Vector3 _posSpawn;
    private GamePlayModel _gamePlayModel = GamePlayModel.Instance;
    private const int MANA_COMBATANT = 10;
    private const string COMBATANT = "combatant";
    private const int TIME_COOLDOWN_SKILL = 10;
    private bool _isCoolDown = false;
    private Tween _coolDownSkill;
    // Start is called before the first frame update
    private void Awake()
    {
        _spawnQuantity = 5;
        _isCoolDown = false;
        Signals.Get<CombatantSkills>().AddListener(CombatantSkill);
        Signals.Get<OnStopGame>().AddListener(StopSpawn);
    }

    void OnMouseDown()
    {
        if(_gamePlayModel.isPlaying && !_isCoolDown)
            Signals.Get<ManaUse>().Dispatch(COMBATANT, MANA_COMBATANT);
    }

    private void CombatantSkill()
    {
        _isCoolDown = true;
        Signals.Get<CoolDownBarCombatant>().Dispatch(TIME_COOLDOWN_SKILL);
        _coolDownSkill = DOVirtual.DelayedCall(TIME_COOLDOWN_SKILL, () => _isCoolDown = false);
        var posObj = objBot.transform.position;
        for (int i = 1; i <= _spawnQuantity; i++)
        {
            float randomY = posObj.y;
            _posSpawn = new Vector3(posObj.x, randomY + 0.35f * i, 0);
            var obj = Instantiate(objBot, _posSpawn, Quaternion.identity);
            obj.transform.SetParent(transform);
            obj.SetActive(true);
        }
    }
    private void StopSpawn()
    {
        _coolDownSkill?.Kill();
        _isCoolDown = false;
    }
}
