using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCameraScript : MonoBehaviour {

    public Transform lookAt, target;
    Transform camTransform;

    float Y_ANGLE_MIN = 0;
    float Y_ANGLE_MAX = 80;

    public float distance = 5;
    public float lookAboveOffset = 2f;
    public float currentX = 0;
    public float currentY = 25;
    public float sensitivityX = 2;
    public float sensitivityY = 1;

    bool isTargeting = false;

    private void Start()
    {
        camTransform = transform;
    }

    private void FixedUpdate()
    {
        if(IsTargeting)
         AdjustToLookAtTarget();
    }

    private void LateUpdate()
    {
       
        ControlCameraOperations();
        AdjustCamera();
    }

    public void ControlCameraOperations()
    {
        //currentX += Input.GetAxis("Second Horizontal") * sensitivityX;
        //currentY += Input.GetAxis("Second Vertical") * sensitivityY;

        currentY = Mathf.Clamp(currentY, Y_ANGLE_MIN, Y_ANGLE_MAX);

        //isAiming = Input.GetKey(KeyCode.L);
    }

    public void AdjustToLookAtTarget()
    {
        //Debug.Log("Difference Vector3: " + (target.position - transform.position).normalized);

        Vector3 normTargetX = (target.position - transform.position).normalized;
        normTargetX.y = 0;

        Vector3 normTrans = transform.forward;
        normTrans.y = 0;

        float dotR = Vector3.Dot(transform.right, normTargetX);
        float dotL = Vector3.Dot(-transform.right, normTargetX);
        if (dotR < dotL)
        {
            //float angle = Vector3.Angle(normTrans, normTargetX);
            currentX = Mathf.Lerp(currentX, currentX - Vector3.Angle(normTrans, normTargetX), 1);            
        }
        else if (dotR > dotL)
        {
            //float angle = Vector3.Angle(normTrans, normTargetX);
            currentX = Mathf.Lerp(currentX, currentX + Vector3.Angle(normTrans, normTargetX), 1);            
        }
        else
        {
            currentX = currentX + Vector3.Angle(transform.forward, (target.position - transform.position).normalized);
        }
    }

    public void AdjustCamera()
    {
        Vector3 dir = new Vector3(0, 0, -distance);
        Quaternion rotation = Quaternion.Euler(currentY, currentX, 0);
        camTransform.position = lookAt.position + rotation * dir;
        Vector3 desiredSpot = lookAt.transform.position;
        desiredSpot.y += lookAboveOffset;
        camTransform.LookAt(desiredSpot);

        //if (isAiming)
        //{
        //    if (r == null)
        //    {
        //        CreateReticle();
        //    }
        //    SetupReticle();
        //}
        //else
        //{
        //    if (r != null)
        //    {
        //        Destroy(r);
        //    }
        //}

    }

    public bool IsTargeting
    {
        get
        {
            return isTargeting;
        }

        set
        {
            isTargeting = value;
        }
    }
}
