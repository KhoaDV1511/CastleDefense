using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Castle : MonoBehaviour
{
    [SerializeField] private Image spriteHealth;
    [SerializeField] private TextMeshProUGUI valueTxt;
    private int _healthCastle = 100;
    private int _healthCastleMax;
    // Start is called before the first frame update
    void Awake()
    {
        _healthCastleMax = _healthCastle;
        Signals.Get<DameEnemy>().AddListener(ReceiveDameEnemy);
    }

    private void OnDestroy()
    {
        Signals.Get<DameEnemy>().RemoveListener(ReceiveDameEnemy);
    }

    private void ReceiveDameEnemy(int dame)
    {
        _healthCastle -= dame;
        valueTxt.text = _healthCastle.ToString();
        spriteHealth.fillAmount -= (float)dame / _healthCastleMax;
        if (spriteHealth.fillAmount <= 0)
        {
            spriteHealth.fillAmount = 0;
            Signals.Get<OnStopGame>().Dispatch();
        }
    }
}
