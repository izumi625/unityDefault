using UnityEngine;
using UnityEngine.InputSystem;


public class MouseContlole : MouseActions
{
    IPointerDevice pointer;

    protected override void Awake()
    {
        base.Awake();//Camera.main 
#if UNITY_IOS || UNITY_ANDROID
                pointer = new TouchWrapper(Touchscreen.current.touches);
#else
        pointer = new MouseWrapper(Mouse.current);
                #endif
    }



    void Update()
    {
        if (pointer.LeftClick())
        {
            Move(pointer.GetPosition());
        }
        if (pointer.RightClickheld())
        {
            Direction(pointer.GetDelta());
        }

        if (pointer.RightClick())
        {

        }

        float scroll = pointer.ScrollY();
          
        if(Mathf.Abs(scroll) > 0.0001f)
        {
            Zoomby(scroll);
        }


    }

    void LateUpdate()
    {
        transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref moveVelocity, moveSmoothTime);
        transform.rotation = Quaternion.Euler(pitch, yaw, 0f);
    }
}
