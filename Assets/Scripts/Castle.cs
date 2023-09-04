using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class Castle : MonoBehaviour
{
    [SerializeField] private Image spriteHealth;
    [SerializeField] private TextMeshProUGUI valueHPTxt;
    [SerializeField] private Image spriteMana;
    [SerializeField] private TextMeshProUGUI valueManaTxt;
    private DameEnemy _dameEnemy = Signals.Get<DameEnemy>();
    private ManaUse _manaUse = Signals.Get<ManaUse>();
    private int _healthCastle = 100;
    private int _healthCastleMax;
    private int _manaCastle = 50;
    private float _manaCastleMax;

    private float amountMana;
    private const string ACHER = "Archer";
    private const string COMBATANT = "combatant";
    private const string THUNDER = "Thunder";
    private const string MAGICIAN = "Magician";
    // Start is called before the first frame update
    void Awake()
    {
        _healthCastleMax = _healthCastle;
        _manaCastleMax = _manaCastle;
        _dameEnemy.AddListener(ReceiveDameEnemy);
        _manaUse.AddListener(CheckMana);
    }

    private void OnDestroy()
    {
        _dameEnemy.RemoveListener(ReceiveDameEnemy);
        _manaUse.RemoveListener(CheckMana);
    }

    private void CheckMana(string typeUse, int mana)
    {
        amountMana = spriteMana.fillAmount;
        amountMana -= mana / _manaCastleMax;
        if(amountMana <= 0) return;
        _manaCastle -= mana;
        spriteMana.fillAmount = amountMana;
        valueManaTxt.text = _manaCastle.ToString();
        switch (typeUse)
        {
            case ACHER:
                Signals.Get<ArcherSkills>().Dispatch();
                break;
            case COMBATANT:
                Signals.Get<CombatantSkills>().Dispatch();
                break;
            case THUNDER:
                Signals.Get<ThunderSkills>().Dispatch();
                break;
            case MAGICIAN:
                Signals.Get<MagicianSkills>().Dispatch();
                break;
            default:
                break;
        }
        
    }

    private void ReceiveDameEnemy(int dame)
    {
        _healthCastle -= dame;
        valueHPTxt.text = _healthCastle.ToString();
        spriteHealth.fillAmount -= (float)dame / _healthCastleMax;
        if (spriteHealth.fillAmount <= 0)
        {
            spriteHealth.fillAmount = 0;
            Signals.Get<OnStopGame>().Dispatch();
        }
    }
}
