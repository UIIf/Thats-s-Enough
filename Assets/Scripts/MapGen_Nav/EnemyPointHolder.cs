using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPointHolder : MonoBehaviour
{
    [SerializeField] GameObject[] pointParent = null;
    [SerializeField] GameObject[] spawnPoint = null;

    //[SerializeField] Vector3[] points = null;

    public Vector3[] GetPatrolPoints()
    {
        Vector3[] temp = pointParent[(int)Mathf.Floor(Random.Range(0, pointParent.Length - 0.5f))].GetComponent<routPoints>().GetPoints();
        return temp;
    }

    public Vector3[] GetSpawnPoints(){
        Vector3[] temp = new Vector3[spawnPoint.Length];
        for( int i = 0; i < spawnPoint.Length; i++){
            temp[i] = spawnPoint[i].transform.position;
        } 
        return temp;
    }
}
