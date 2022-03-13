using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dropWeaponScript : MonoBehaviour
{
    Rigidbody rb;
    public float speed = 18f, angle = 0f;
    private bool delay;
    private float startTime;

    void Start()
    {
        rb = gameObject.AddComponent(typeof(Rigidbody)) as Rigidbody;
        //rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        Vector3 rot = transform.rotation.eulerAngles;

        BoxCollider[] boxcol = gameObject.GetComponents<BoxCollider>();
        for (int i = 0; i < boxcol.Length; i++)
        {
            boxcol[i].enabled = false;
            //if (boxcol[i].isTrigger)
            //{
            //    boxcol[i].enabled = false;
            //}
            //else
            //{
            //    boxcol[i].enabled = false;
            //}
        }

        rb.AddForce(new Vector3(-Mathf.Cos(rot.y * Mathf.Deg2Rad), Mathf.Sin(angle), Mathf.Sin(rot.y * Mathf.Deg2Rad)) * speed, ForceMode.Impulse);
        //rb.AddForce(transform.rotation.eulerAngles.normalized * speed , ForceMode.Impulse);
        delay = false;
        startTime = 0;
    }

    void FixedUpdate()
    {
        if( delay &&
            Mathf.Abs(rb.velocity.x) < 0.02f &&
            Mathf.Abs(rb.velocity.y) < 0.02f &&
            Mathf.Abs(rb.velocity.z) < 0.02f)
        {
            Destroy(rb);
            if(GetComponent<WeaponInterface>().GetAmmo() <= 0){
                Destroy(gameObject);
            }
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
            //���������� ������� ������� � �����... ������� ��
            Destroy(gameObject.GetComponent<dropWeaponScript>());
        }

        if (!delay)
        {
            startTime += Time.fixedDeltaTime;
            if (startTime > 0.01f)
            {
                delay = true;
                BoxCollider[] boxcol = gameObject.GetComponents<BoxCollider>();
                for (int i = 0; i < boxcol.Length; i++)
                {
                    boxcol[i].enabled = false;
                    if (!boxcol[i].isTrigger)
                    {
                        boxcol[i].enabled = true;
                    }
                }
            }
        }
    }
}
