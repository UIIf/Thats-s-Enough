using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bulletScript : MonoBehaviour
{
    [SerializeField] float speed;
    [SerializeField] float dmg;
   
    void Update()
    {
        
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        //if (other.gameObject.layer == LayerMask.GetMask("enemy")) 
        //{
        //    print(1);
        //    if (other.GetComponent<Ñreature>())
        //    {
        //        print(2);
        //        other.GetComponent<Ñreature>().GetDamage(dmg);
        //    }
        //}
        //Destroy(gameObject);
    }
}
