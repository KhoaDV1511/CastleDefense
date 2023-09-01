using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PosManaAcher : MonoBehaviour
{
    [SerializeField] private GameObject baMana;

    private List<float> posXAcher = new List<float>();

    private const string ACHER = "Acher";
    // Start is called before the first frame update
    private void Awake()
    {
        SetPosBamana();
    }

    private void SetPosBamana()
    {
        foreach (Transform child in transform)
        {
            if(child.gameObject.activeInHierarchy && child.CompareTag(ACHER))
                posXAcher.Add(child.position.x);
        }

        float totalX = 0;
        foreach (var posX in posXAcher)
        {
            totalX += posX;
        }

        var posManaX = totalX / posXAcher.Count;
        baMana.transform.position = new Vector3(posManaX - 0.25f, baMana.transform.position.y, 0);
    }
}
