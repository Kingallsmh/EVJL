using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementScript : MonoBehaviour {
    PlayerControllerScript control;
    Animator anim;

    public float speed;
    public float jumpSpeed;
    public int availableJumps, totalJumps = 2;
    Vector3 input;

    Rigidbody rb;
    public float currentGravity = 0, constGravity = 0.1f, maxVelChange = 1;
    public bool isGrounded = true, touchingGround = true;

    bool targeting = false;
    public GameObject currentTarget;
    Camera mainCam;
    //MainCameraScript camScript;
    GameObject newTrans;

    //For testing
    public Vector3 currentVelocity;

	// Use this for initialization
	void Start () {
        anim = GetComponent<Animator>();
        newTrans = new GameObject();
        newTrans.transform.parent = transform;
        control = GetComponent<PlayerControllerScript>();
        mainCam = Camera.main;
        //camScript = mainCam.GetComponent<MainCameraScript>();
        rb = GetComponent<Rigidbody>();
        StartCoroutine(PlayerLoop());
	}
    
    IEnumerator PlayerLoop()
    {
        while (true)
        {
            InputRegister();
            GravityChange();
            Movement();
            FaceTarget(control.GetBtnPressed(3));
            if (control.GetBtnPressed(2))
            {
                anim.SetTrigger("Dodge");
                yield return Dodge(newTrans.transform.TransformDirection(input));
            }
            if (control.GetBtnPressed(1))
            {
                anim.SetTrigger("Atk1");
            }
            currentVelocity = rb.velocity;
            yield return new WaitForEndOfFrame();
        }
    }

    IEnumerator Dodge(Vector3 direction)
    {
        float cTime = 0, time = 0.55f;
        while (true)
        {
            if (time < cTime)
            {
                break;
            }
            else
            {
                if(direction.magnitude < 0.1)
                {
                    rb.velocity = transform.forward * Time.deltaTime * speed * 3.5f;
                }
                else
                {
                    rb.velocity = direction * 4;
                }
                FaceTowardsDirection(rb.velocity);
                cTime += Time.deltaTime;
            }
            yield return new WaitForEndOfFrame();
        }
    }

    void InputRegister()
    {
        input = control.MoveInput;
        input *= speed * Time.deltaTime;
    }

    void Jump()
    {
        if (isGrounded)
        {
            availableJumps = totalJumps;
            if (control.GetBtnPressed(4))
            {
                input.y += jumpSpeed;
                availableJumps -= 1;
                isGrounded = false;
            }
        }
        else
        {
            if (control.GetBtnPressed(4) && availableJumps > 0)
            {
                input.y = jumpSpeed;
                availableJumps -= 1;
            }
            if(rb.velocity.y > 0 && control.GetBtnHeld(4))
            {
                currentGravity /= 2;
            }
        }
    }

    void Movement()
    {
        newTrans.transform.position = mainCam.transform.position;
        newTrans.transform.rotation = mainCam.transform.rotation;
        newTrans.transform.eulerAngles = new Vector3(0, newTrans.transform.rotation.eulerAngles.y, 0);
        input.y = rb.velocity.y;
        Jump();
        if (input.y > -maxVelChange)
        {
            input.y += currentGravity;
        }
        rb.velocity = newTrans.transform.TransformDirection(input);
        input.y = 0;
        if (input != Vector3.zero)
        {
            Vector3 tempVel = rb.velocity;
            tempVel.y = 0;
            //Use FaceTowardsMovement
            FaceTowardsDirection(tempVel);
        }
            
        anim.SetBool("Moving", input.magnitude > 0.1);
        //isGrounded = false;
    }

    void FaceTowardsDirection(Vector3 direction)
    {
        //If rotation in minute in difference, don't bother rotating
        if (direction != Vector3.zero)
        {
            if(Quaternion.Angle(transform.rotation, Quaternion.LookRotation(rb.velocity.normalized)) > 0.01)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction.normalized), 0.2f);
            }
        }
    }

    void FaceTowardsRotation(Quaternion rot)
    {
        if(Quaternion.Angle(transform.rotation, rot) > 0.01)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation,
                rot, 0.2f);
        }
    }
    
    void FaceTarget(bool engageT)
    {
        if (engageT)
        {
            targeting = !targeting;
            //camScript.IsTargeting = targeting;
        }
    }

    void GravityChange()
    {
        if (isGrounded)
        {
            currentGravity = 0;
        }
        else
        {
            currentGravity = -1;
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if(touchingGround)
        isGrounded = true;
    }

    private void OnTriggerStay(Collider other)
    {
        touchingGround = true;
    }

    private void OnTriggerExit(Collider other)
    {
        touchingGround = false;
        isGrounded = false;
    }
}
