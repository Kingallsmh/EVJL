using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementScript : MonoBehaviour
{
    Camera mainCam;
    PlayerControllerScript control;
    Rigidbody rb;
    public Animator anim;
    Vector3 previousVel;

    GameObject newTrans;
    public float speed, jump, turnAmount;

    // Start is called before the first frame update
    void Start()
    {
        newTrans = new GameObject();
        newTrans.transform.parent = transform;
        rb = GetComponent<Rigidbody>();
        control = GetComponent<PlayerControllerScript>();
        if (GetComponent<Animator>())
        {
            anim = GetComponent<Animator>();
        }
        mainCam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        Movement();
    }

    void Movement()
    {
        newTrans.transform.position = mainCam.transform.position;
        newTrans.transform.rotation = mainCam.transform.rotation;
        newTrans.transform.eulerAngles = new Vector3(0, newTrans.transform.rotation.eulerAngles.y, 0);
        float fall = rb.velocity.y;
        if (control.GetBtnPressed(1))
        {
            fall += jump;
        }
        rb.velocity = newTrans.transform.TransformDirection(control.MoveInput) * speed;
        rb.velocity = new Vector3(rb.velocity.x, fall, rb.velocity.z);
        if (control.MoveInput != Vector3.zero)
        {
            Vector3 tempVel = rb.velocity;
            tempVel.y = 0;
            //Use FaceTowardsMovement
            FaceTowardsDirection(tempVel);
        }
        if (control.GetBtnHeld(2))
        {
            rb.velocity = rb.velocity / 2;
        }


        
        anim.SetBool("Slowed", control.GetBtnHeld(2));
        anim.SetBool("Woo", control.GetBtnPressed(3));
        //FixStuckWall();

        anim.SetBool("Moving", control.MoveInput.magnitude > 0.1);
        //isGrounded = false;
        previousVel = rb.velocity;
    }

    void FaceTowardsDirection(Vector3 direction)
    {
        //If rotation in minute in difference, don't bother rotating
        if (direction != Vector3.zero)
        {
            if (Quaternion.Angle(transform.rotation, Quaternion.LookRotation(rb.velocity.normalized)) > 0.01)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(direction.normalized), turnAmount);
            }
        }
    }

    void FixStuckWall()
    {
        // Get the velocity
        Vector3 horizontalMove = rb.velocity;
        // Don't use the vertical velocity
        horizontalMove.y = 0;
        // Calculate the approximate distance that will be traversed
        float distance = horizontalMove.magnitude * Time.fixedDeltaTime;
        // Normalize horizontalMove since it should be used to indicate direction
        horizontalMove.Normalize();
        RaycastHit hit;

        // Check if the body's current velocity will result in a collision
        if (rb.SweepTest(horizontalMove, out hit, distance))
        {
            // If so, stop the movement
            rb.velocity = new Vector3(0, rb.velocity.y, 0);
        }
    }
}
