using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Enemy : MonoBehaviour
{
    [SerializeField] private GameObject objEnemy;
    [SerializeField] private GameObject castle;
    [SerializeField] private GameObject[] castleMoveAround;
    [SerializeField] private Button btnBattle;
    private Vector2 _posSpawn;
    private int _spawnQuantity;
    protected int _healthAddEnemyOne;
    protected int _dameAddEnemyOne;

    protected virtual void Awake()
    {
        btnBattle.onClick.AddListener(EnemyAttack);
        Signals.Get<OnStopGame>().AddListener(BattleStop);
    }

    protected virtual void OnDestroy()
    {
        Signals.Get<OnStopGame>().RemoveListener(BattleStop);
    }

    protected void LevelUpEnemyOne(int health = 10, int dame = 1)
    {
        _healthAddEnemyOne += health;
        _dameAddEnemyOne += dame;
    }

    private void BattleStop()
    {
        btnBattle.interactable = true;
    }
    private void EnemyAttack()
    {
        btnBattle.interactable = false;
        GamePlayModel.Instance.isPlaying = true;
        _spawnQuantity = 10;
        
        for (int i = 1; i <= _spawnQuantity; i++)
        {
            float randomY = Random.Range(-3, 0);
            _posSpawn = new Vector2(objEnemy.transform.position.x, randomY);
            var obj = Instantiate(objEnemy, _posSpawn, Quaternion.identity);
            obj.transform.SetParent(transform);
            obj.SetActive(true);
        }
        Signals.Get<QuantityEnemy>().Dispatch(_spawnQuantity);
        Signals.Get<StartFindEnemy>().Dispatch();
        Signals.Get<PosAIMove>().Dispatch(castleMoveAround[0].transform.position);
        Signals.Get<PosAIMove>().Dispatch(castleMoveAround[1].transform.position);
        Signals.Get<PosAIMove>().Dispatch(castleMoveAround[2].transform.position);
        Signals.Get<CastlePos>().Dispatch(castle.transform.position);
    }
}
