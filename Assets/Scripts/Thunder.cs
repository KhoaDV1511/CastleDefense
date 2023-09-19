using UnityEngine;

public class Thunder : Character
{
    private const int MANA_THUNDER = 10;
    private const string THUNDER = "Thunder";
    private GamePlayModel _gamePlayModel = GamePlayModel.Instance;
    private const int TIME_COOLDOWN_SKILL = 10;



    // Start is called before the first frame update
    void Awake()
    {
        InitPlayer();
        InitStartInvoke();
        InitTimeCoolDownSkill();
        Signals.Get<ThunderSkills>().AddListener(SkillThunder);
        Signals.Get<OnStopGame>().AddListener(OnStopGame);
        Signals.Get<StartFindEnemy>().AddListener(DetectEnemy);
        Signals.Get<TimeReduce>().AddListener(TimeRemainSkill);
        Signals.Get<CoolDownBarThunder>().AddListener(ManaCoolDown);
        Signals.Get<TimeReduce>().AddListener(ReduceTime);
    }
    private void OnMouseDown()
    {
        if(_gamePlayModel.isPlaying && _enemysInsideArea.Length > 0 && !_isCoolDown)
            UseSkill(THUNDER, MANA_THUNDER);
    }

    private void OnStopGame()
    {
        StopBarMana();
        StopCoolDownSkill();
        StopInvoke();
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
    private void SkillThunder()
    {
        Signals.Get<CoolDownBarThunder>().Dispatch(TIME_COOLDOWN_SKILL);
        TimeUseSkill(TIME_COOLDOWN_SKILL);
        skill.transform.position = enemyPosMin;
        skill.SetActive(true);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
