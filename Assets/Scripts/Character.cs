
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

public class Character : MonoBehaviour
{
    [SerializeField] private Transform mana;
    [SerializeField] protected GameObject skill;
    [SerializeField] private Transform objShoot;
    [SerializeField] protected float radius;
    [SerializeField] LayerMask mask;
    
    protected Collider2D[] _enemysInsideArea;
    
    private List<Vector3> _enemyPos = new List<Vector3>();
    private List<float> _distanceEnemy = new List<float>();
    
    public Vector3 enemyPosMin;

    private Tween _coolDownMana;
    protected Tween _coolDownSkill;
    
    private float _sweepFrequency = 0.005f;
    private float _startTime;
    private float _timeCoolDownRemain;
    private float _timeCoolDown;
    
    protected int _spawnQuantity;
    
    private Coroutine _sweep;
    
    private bool _startInvoke = false;
    protected bool _isCoolDown = false;

    protected void StopInvoke()
    {
        if(_sweep != null) StopCoroutine(_sweep);
        CancelInvoke();
    }
    protected void StopCoolDownSkill()
    {
        _coolDownSkill?.Kill();
        _isCoolDown = false;
    }
    protected void StopBarMana()
    {
        _coolDownMana?.Kill();
        mana.localScale = new Vector3(1, 1, 0);
    }

    protected void StartTimeCooldown(float coolDown)
    {
        _timeCoolDown = coolDown;
        _startTime = Time.time;
    }

    protected void RestoreMana(float coolDown)
    {
        mana.localScale = new Vector3(0, 1, 0);
        _coolDownMana = mana.DOScaleX(1, coolDown).SetEase(Ease.Linear);
    }

    protected void ReduceTimeCooldown(float reduce)
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

    protected void UseSkill(string name, int mana)
    {
        Signals.Get<ManaUse>().Dispatch(name, mana);
    }

    protected void InitPlayer()
    {
        _timeCoolDownRemain = 0;
        _startTime = 0;
        _isCoolDown = false;
    }

    protected void InitBot()
    {
        _spawnQuantity = 4;
    }

    protected void InitStartInvoke()
    {
        _startInvoke = false;
    }
    protected void InitTimeCoolDownSkill()
    {
        _timeCoolDown = 0;
    }
    protected void DetectEnemy()
    {
        if(_sweep != null) StopCoroutine(_sweep);
        _sweep = StartCoroutine(EnemyPos());
        _startInvoke = false;
    }
    private IEnumerator EnemyPos()
    {
        _enemyPos.Clear();
        _enemysInsideArea = Physics2D.OverlapCircleAll(transform.position, radius, mask);
        //if(_quantityHealthCastle <= 0) yield break;
        if(_enemysInsideArea.Length > 0)
            PosEnemyMin();
 
        yield return new WaitForSeconds(_sweepFrequency);
        if(_sweep != null) StopCoroutine(_sweep);
        _sweep = StartCoroutine(EnemyPos());
    }

    private void PosEnemyMin()
    {
        foreach (Collider2D collider2D in _enemysInsideArea)
        {
            _enemyPos.Add(collider2D.transform.position);
        }

        _distanceEnemy.Clear();
        foreach (var dis in _enemyPos)
        {
            float dist = Vector3.Distance(dis, transform.position);
            _distanceEnemy.Add(dist);
        }
        var min_dis = _distanceEnemy.AsQueryable().Min();
        var indexInList = _distanceEnemy.IndexOf(min_dis);
        if(!_startInvoke)
        {
            InvokeRepeating(nameof(SpawnObj), 0, 1f);
            _startInvoke = true;
        }
        Signals.Get<EnemyPosProjectile>().Dispatch(_enemyPos[indexInList]);
        enemyPosMin = _enemyPos[indexInList];
    }
    protected void SpawnObj()
    {
        var obj = Instantiate(objShoot, objShoot.transform.position, Quaternion.identity).gameObject;
        obj.transform.SetParent(transform);
        obj.SetActive(true);
    }
    protected void SkillAfterReduce(float reduce, int timeCoolDown)
    {
        _coolDownSkill?.Kill();
        _timeCoolDownRemain = timeCoolDown - (Time.time - _startTime) + reduce;
        _coolDownSkill = DOVirtual.DelayedCall(_timeCoolDownRemain, () => _isCoolDown = false);
    }

    protected void TimeUseSkill(int timeCoolDown)
    {
        _isCoolDown = true;
        _startTime = Time.time;
        _coolDownSkill = DOVirtual.DelayedCall(timeCoolDown, () =>
        {
            _isCoolDown = false;
        });
        
    }
}
