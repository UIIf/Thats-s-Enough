using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamScript : MonoBehaviour
{
    [SerializeField] private float camY;
    [SerializeField] private Camera cam;
    [SerializeField] private float maxShotDist;
    [SerializeField] private float percntsCamDist;

    private float newCamX, newCamZ;
    private float hitDistance;
    public float VectoroTEmpo;

    private Vector3 mousePose = Vector3.zero;
    private Transform _transform;
    // Start is called before the first frame update
    void Awake()
    {
        _transform = GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        mousePose = cam.WorldToScreenPoint(Input.mousePosition);
    }

    private void FixedUpdate()
    {
        LookingLogic();
    }

    private void CamMove(Vector3 targetPoint)
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

        if (plane.Raycast(ray, out hitDistance))
        {
            Vector3 targetPoint = ray.GetPoint(hitDistance);

            Quaternion targetRotation = Quaternion.LookRotation(targetPoint - _transform.position);
            targetRotation.x = 0;
            targetRotation.z = 0;
            _transform.rotation = targetRotation;
            //if (targetPoint.sqrMagnitude > maxShotDist * maxShotDist)
            //{
            //    targetPoint = (targetPoint - transform.position).normalized * maxShotDist;
            //}
            CamMove(targetPoint);

        }

    }
}