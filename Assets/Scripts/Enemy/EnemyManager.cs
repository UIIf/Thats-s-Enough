using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [Header("Enemys")]
    [SerializeField] GameObject[] enemy;
    [SerializeField] GameObject[] guns;
    [Header("Enemy view")]

    [SerializeField] Transform enemyTarget;
    public Transform trarget
    {
        get => enemyTarget;
    }

    [Tooltip("Count of fixedUpdate before next view")]
    [SerializeField] int fixedUpdateCount = 100;
    public int maxFixedUpdateCount
    {
        get => fixedUpdateCount;
    }

    int fixedUpdateCounter = 0;
    public int fUCounter
    {
        get => fixedUpdateCounter;
    }

    [Min(0.1f)]
    [SerializeField] float lookingAroundTimer;
    public float lookArT
    {
        get => lookingAroundTimer;
    }

    int[] fixedCounts;

    public void Initialise(){
        fixedCounts = new int[maxFixedUpdateCount];
        for(int i = 0; i < fixedCounts.Length; i++){
            fixedCounts[i] = 0;
        }
    }
    private void FixedUpdate()
    {
        fixedUpdateCounter = ++fixedUpdateCounter % fixedUpdateCount;
    }

    public void SpawnEnemys(float cx, float cz){
        Collider[] col = Physics.OverlapBox(new Vector3(cx, 0, cz), new Vector3(0.01f, 10f, 0.01f), Quaternion.identity, LayerMask.GetMask("floor"));
        if(col.Length > 0){
            EnemyPointHolder tEnemyPHolder = col[0].transform.parent.GetChild(4).GetComponent<EnemyPointHolder>();
            if(tEnemyPHolder == null)
                return;

            Vector3[] spPoint = tEnemyPHolder.GetSpawnPoints();
            for(int i = 0; i < spPoint.Length; i++){
                SpawnEnemy(spPoint[i]);
            }
        }
    }

    private void SpawnEnemy(Vector3 vec){
        GameObject tempEnemy = GetRandomEnemy();
        
        vec.y = tempEnemy.transform.position.y;
        
        tempEnemy.GetComponent<EnemyMainScript>().curManager = transform.GetComponent<EnemyManager>();
        tempEnemy.GetComponent<EnemyMainScript>().startToWatch = GetIndexFixedCounts();
        tempEnemy = Instantiate(tempEnemy, vec, Quaternion.Euler(new Vector3(0, Random.Range(0,360) ,0)));
        tempEnemy.GetComponent<EnemyMainScript>().GetGun(Instantiate(GetRandomGun(), tempEnemy.transform));
    }

    private GameObject GetRandomEnemy(){
        return enemy[Random.Range(0, enemy.Length)];
    }

    private GameObject GetRandomGun(){
        return guns[Random.Range(0, guns.Length)];
    }

    private int GetIndexFixedCounts(){
        int index = 0;
        for(int i = 1; i < fixedCounts.Length; i++){
            if(fixedCounts[index] > fixedCounts[i]){
                index = i;
            }
        }
        fixedCounts[index]++;
        return index;
    }

    public void ClearIndexFixedCounts(int index){
        fixedCounts[index] --;
    }

}
