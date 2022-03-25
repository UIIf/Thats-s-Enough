using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dropWeaponScript : MonoBehaviour
{
    Rigidbody rb;
    public float speed = 18f, angle = 0f;
    private bool delay = false;
    private bool grounded = false;
    private float startTime;

    ParticleSystem _particleSystem;
    rayWeapon _rW;

    float lifetime = 2;

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
        startTime = 0;

        _rW = GetComponent<rayWeapon>();
        _particleSystem = GetComponent<ParticleSystem>();
    }

    void FixedUpdate()
    {

        if(grounded && _rW.GetAmmo() <= 0)
        {
            if (!_particleSystem.isPlaying)
                _particleSystem.Play();

            lifetime -= Time.fixedDeltaTime;
            if (lifetime <= 0)
                Destroy(gameObject);
        }
        else if(delay && rb == null)
        {
            Destroy(gameObject.GetComponent<dropWeaponScript>());
        }

        //после того, как прошел delay после выкидывания
        if ( delay &&
            rb != null &&
            Mathf.Abs(rb.velocity.x) < 0.02f &&
            Mathf.Abs(rb.velocity.y) < 0.02f &&
            Mathf.Abs(rb.velocity.z) < 0.02f)
        {
            Destroy(rb);
            rb = null;
            grounded = true;

            if (_rW.GetAmmo() <= 0){

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
            }
        }

        //delay сразу после выкидывания чтобы оружие успело отлететь
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
