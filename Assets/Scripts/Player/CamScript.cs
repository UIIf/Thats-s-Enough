using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamScript : MonoBehaviour
{
    [SerializeField] private float camY;
    [SerializeField] private Camera cam;
    [SerializeField] private float maxShotDist;
    [SerializeField] private float percntsCamDist;
    [SerializeField] private float minimalDist = 1f;

    private float newCamX, newCamZ;
    private float hitDistance;
    public float VectoroTEmpo;

    public Vector3 targetPoint = Vector3.zero;

    private Transform _transform;

    void Awake()
    {
        _transform = GetComponent<Transform>();
    }

    private void FixedUpdate()
    {
        LookingLogic();
    }

    private void CamMove()
    {
        if (Mathf.Abs(targetPoint.x - _transform.position.x) < maxShotDist)
        {
            newCamX = (targetPoint.x - _transform.position.x);
        }
        else
        {
            if (targetPoint.x - _transform.position.x < 0)
                newCamX = -maxShotDist;
            else
                newCamX = maxShotDist;

        }
        newCamX = newCamX * percntsCamDist + _transform.position.x;

        if (Mathf.Abs(targetPoint.z - _transform.position.z) < maxShotDist)
        {
            newCamZ = (targetPoint.z - _transform.position.z);
        }
        else
        {
            if (targetPoint.z - _transform.position.z < 0)
                newCamZ = -maxShotDist;
            else
                newCamZ = maxShotDist;
        }
        newCamZ = newCamZ * percntsCamDist + _transform.position.z;


        cam.transform.position = Vector3.Lerp(cam.transform.position,
                                              new Vector3(
                                                  newCamX,
                                                  camY,
                                                  newCamZ),
                                              VectoroTEmpo);
    }
    private void LookingLogic()
    {
        Plane plane = new Plane(Vector3.up, _transform.position);
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (!plane.Raycast(ray, out hitDistance))
        {
            return;
        }

        targetPoint = ray.GetPoint(hitDistance);

        if (Vector3.Distance(targetPoint, transform.position) < minimalDist)
        {
            targetPoint = (targetPoint - transform.position).normalized * minimalDist + transform.position;
        }

        Vector3 targetRotation = _transform.eulerAngles;
        targetRotation.y = Quaternion.LookRotation(targetPoint - _transform.position).eulerAngles.y; 
        _transform.eulerAngles = targetRotation;

        CamMove();

    }

}
