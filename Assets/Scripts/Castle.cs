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

    private void CheckMana(int mana)
    {
        amountMana = spriteMana.fillAmount;
        amountMana -= mana / _manaCastleMax;
        if(amountMana <= 0) return;
        _manaCastle -= mana;
        spriteMana.fillAmount = amountMana;
        valueManaTxt.text = _manaCastle.ToString();
        Signals.Get<ArcherSkill>().Dispatch();
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
