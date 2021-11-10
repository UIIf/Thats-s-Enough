using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dropWeaponScript : MonoBehaviour
{
    Rigidbody rb;
    public float speed = 5f, angle = 15f;

    void Awake()
    {
        rb = gameObject.AddComponent(typeof(Rigidbody)) as Rigidbody;
        Vector3 rot = transform.rotation.eulerAngles;
        rb.AddForce(new Vector3(-Mathf.Cos(rot.y), Mathf.Sin(angle), Mathf.Sin(rot.y)) * speed, ForceMode.Impulse);
        BoxCollider[] boxcol  = gameObject.GetComponents<BoxCollider>();
        for(int i = 0; i < boxcol.Length; i++)
        {
            if (boxcol[i].isTrigger)
            {
                boxcol[i].enabled = false;
            }
            else
            {
                boxcol[i].enabled = true;
            }
        }
    }

    void Update()
    {
        if( Mathf.Abs(rb.velocity.x) < 0.02f &&
            Mathf.Abs(rb.velocity.y) < 0.02f &&
            Mathf.Abs(rb.velocity.z) < 0.02f)
        {
            Destroy(rb);
            BoxCollider[] boxcol = gameObject.GetComponents<BoxCollider>();
            for (int i = 0; i < boxcol.Length; i++)
            {
                if (boxcol[i].isTrigger)
                {
                    boxcol[i].enabled = true;
                }
                else
                {
                    boxcol[i].enabled = false;
                }

            }
            //переместил дестрой скрипта в конец... логично же
            Destroy(gameObject.GetComponent<dropWeaponScript>());
        }
    }
}
