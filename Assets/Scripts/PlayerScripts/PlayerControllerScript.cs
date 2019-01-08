using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerScript : MonoBehaviour {

    //Change the button to check for held, on press and let go
    Vector3 lookInput, moveInput;
    Button btn1, btn2, btn3, btn4;

    private void Awake()
    {
        btn1 = new Button();
        btn2 = new Button();
        btn3 = new Button();
        btn4 = new Button();
    }

    void Start () {
        StartCoroutine(GatherInput());
	}

    private void FixedUpdate()
    {
        
    }

    IEnumerator GatherInput()
    {
        while (this)
        {
            //Gather input for moving. Possibly use y to jump.
            moveInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
            //Gather input for button presses
            btn1.CheckAll(Input.GetAxis("Btn1") > 0.1);
            btn2.CheckAll(Input.GetAxis("Btn2") > 0.1);
            btn3.CheckAll(Input.GetAxis("Btn3") > 0.1);
            btn4.CheckAll(Input.GetAxis("Btn4") > 0.1);
            yield return new WaitForEndOfFrame();
        }
        yield return null;
    }

    public Vector3 LookInput
    {
        get
        {
            return lookInput;
        }

        set
        {
            lookInput = value;
        }
    }

    public Vector3 MoveInput
    {
        get
        {
            return moveInput;
        }

        set
        {
            moveInput = value;
        }
    }


    public bool GetBtnPressed(int num)
    {
        switch (num)
        {
            case 1: return btn1.Pressed;
            case 2: return btn2.Pressed;
            case 3: return btn3.Pressed;
            case 4: return btn4.Pressed;
        }
        return false;
    }

    public bool GetBtnHeld(int num)
    {
        switch (num)
        {
            case 1: return btn1.Held;
            case 2: return btn2.Held;
            case 3: return btn3.Held;
            case 4: return btn4.Held;
        }
        return false;
    }
}

public class Button
{
    bool pressed, held, release;

    void isPressed(bool isPressed)
    {
        pressed = false;
        if (!pressed && isPressed && !held)
        {
            pressed = true;
        }
    }

    void isHeld(bool isPressed)
    {
        held = isPressed;
    }

    void isReleased(bool isPressed)
    {
        release = false;
        if(held && !isPressed && !release)
        {
            release = true;
        }
    }

    public void CheckAll(bool btn)
    {
        isPressed(btn);
        isHeld(btn);
        isReleased(btn);
    }

    public bool Pressed
    {
        get
        {
            return pressed;
        }

        set
        {
            pressed = value;
        }
    }

    public bool Held
    {
        get
        {
            return held;
        }

        set
        {
            held = value;
        }
    }

    public bool Release
    {
        get
        {
            return release;
        }

        set
        {
            release = value;
        }
    }
}

