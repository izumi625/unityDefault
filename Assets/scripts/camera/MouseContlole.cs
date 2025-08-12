using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;


public class MouseContlole : MouseActions
{
    IPointerDevice pointer;
    [SerializeField] private UnityEvent DoubleClickEvent;
    [SerializeField] private UnityEvent<Vector2> LeftClickEvent;
    [SerializeField] private UnityEvent<Vector2> RightClickEvent;


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

            LeftClickEvent?.Invoke(pointer.GetPosition());
        }
        if (pointer.RightClickheld())
        {
            RightClickEvent?.Invoke(pointer.GetDelta());
        }

        if (pointer.RightClick())
        {

        }
        if (pointer.DoubleClicke())
        {
            DoubleClickEvent?.Invoke();

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
