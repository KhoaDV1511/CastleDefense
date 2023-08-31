using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnEnemy : MonoBehaviour
{
    [SerializeField] private GameObject objEnemy;
    [SerializeField] private GameObject castle;
    [SerializeField] private GameObject[] castleMoveAround;
    private Vector2 _posSpawn;
    private int _spawnQuantity;
    void Start()
    {
        _spawnQuantity = 10;
        
        for (int i = 0; i <= _spawnQuantity; i++)
        {
            float randomY = Random.Range(-3, 0);
            _posSpawn = new Vector2(objEnemy.transform.position.x, randomY);
            var obj = Instantiate(objEnemy, _posSpawn, Quaternion.identity);
            obj.transform.SetParent(transform);
            obj.SetActive(true);
        }
        Signals.Get<CastlePos>().Dispatch(castle.transform.position);
        Signals.Get<PosAIMove>().Dispatch(castleMoveAround[0].transform.position);
        Signals.Get<PosAIMove>().Dispatch(castleMoveAround[1].transform.position);
    }
}
