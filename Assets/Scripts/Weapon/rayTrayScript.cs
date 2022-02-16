using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rayTrayScript : MonoBehaviour, BulletInterface
{
    [Tooltip("How much units trail travel at 1 second")]
    [SerializeField] private float rayTraySpeed;
    [SerializeField] private float damage;

    public void BulletShootCoroutine(Vector3 start, Vector3 end, GameObject target = null)
    {
        StartCoroutine(DrawRay(start, end, target));
    }

    private IEnumerator DrawRay(Vector3 start, Vector3 end, GameObject target = null)
    {
        end.y = start.y;

        float x = end.x - start.x;
        float z = end.z - start.z;

        //float sin = x / Mathf.Sqrt(x * x + z * z);
        float Angle = Mathf.Asin(x / Mathf.Sqrt(x * x + z * z)) * Mathf.Rad2Deg;
        if (z < 0)
        {
            Angle = 180 - Angle;
        }

        Vector3 prevRot = transform.eulerAngles;

        prevRot.y = Angle - 180;

        Transform curTr = transform;
        curTr.position = end;
        curTr.localEulerAngles = prevRot;

        float dist = Vector3.Distance(start, end);

        if (dist < 0.1f)
        {
            Destroy(curTr.gameObject);
            yield break;
        }

        float trayScalePersent = 1;
        Vector3 newScale = Vector3.one;

        float dt = trayScalePersent / dist * rayTraySpeed * Time.fixedDeltaTime;

        if(target != null)
        {
            target.GetComponent<Humanoid>().GetDamage(damage);
        }

        for (; trayScalePersent > 0.1; trayScalePersent -= dt)
        {
            newScale.y = trayScalePersent * dist;
            curTr.localScale = newScale;
            yield return new WaitForFixedUpdate();
        }

        Destroy(curTr.gameObject);

        yield break;
    }
}
