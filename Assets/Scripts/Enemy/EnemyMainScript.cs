using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMainScript : MonoBehaviour, Humanoid
{
    [SerializeField] private float _HP;
    [SerializeField] GameObject deathParticl;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Death()
    {
        Instantiate(deathParticl,transform.position,deathParticl.transform.rotation);
        Destroy(gameObject);
    }

    public void GetDamage(float dmg)
    {
        _HP -= dmg;
        print("Ouch");
        if(_HP <= 0)
        {
            Death();
        }
    }

    public float GetHP()
    {
        return _HP;
    }
}
