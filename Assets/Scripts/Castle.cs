using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Castle : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Signals.Get<CastlePos>().Dispatch(transform.position);
    }
}
