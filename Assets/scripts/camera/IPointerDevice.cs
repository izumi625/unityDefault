using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Utilities;

public interface IPointerDevice
{
    Vector2 GetPosition();
    Vector2 GetDelta();
    float ScrollY();
    bool LeftClick();
    bool RightClick();
    bool RightClickheld();

    bool DoubleClicke();

}

public class MouseWrapper : IPointerDevice
{
    Mouse mouse;

    private float clickTime = Time.time;
    bool DoubleCheck = false;
    private const float threshold = 0.2f;
    public MouseWrapper(Mouse mouse) => this.mouse = mouse;
    public Vector2 GetPosition() => mouse.position.ReadValue();

    public bool DoubleClicke() => DoubleCheck;

    private bool singleEmitted = false;
    public bool LeftClick()
    {

        if (mouse.leftButton.wasPressedThisFrame)
        {

            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())//on ui
            {
                return false;
            }
            float now = Time.time;

            if (clickTime > 0f && (now - clickTime) <= threshold)
            {
                DoubleCheck = true;
                clickTime = -1f;
                singleEmitted = false;
                return false;
            }


            clickTime = now;
            singleEmitted = false;
            DoubleCheck = false;
            return false;
        }


        if (clickTime > 0f && !singleEmitted && (Time.time - clickTime) > threshold)
        {
            singleEmitted = true;
            DoubleCheck = false;
            clickTime = -1f;
            return true;
        }
        DoubleCheck = false;
        return false;
    }

    public bool RightClick() => mouse.rightButton.wasPressedThisFrame;
    public Vector2 GetDelta() => mouse.delta.ReadValue();
    public bool RightClickheld() => mouse.rightButton.isPressed;
    public　float ScrollY() => mouse.scroll.ReadValue().y;
}

public class TouchWrapper : IPointerDevice
{
    private ReadOnlyArray<TouchControl> touches;
    private float holdThreshold = 0.15f;
    private float doubletime = 0.2f;
    private int touchcount = 0;

    private int clickIndex = -1;
    private float clickDownTime;
    private bool isHolding;

    private bool doublecheck = false;

    Vector2 pos;
    Vector2 delta;
    private bool pinching;
    private float prevPinchDist;

    public TouchWrapper(ReadOnlyArray<TouchControl> touches) => this.touches = touches;

    public bool DoubleClicke()
    {
        return doublecheck;
    }
    public Vector2 GetPosition() => pos;
    public Vector2 GetDelta() => delta;
    public bool RightClick() => isHolding;
    public bool RightClickheld() => isHolding;



    public bool LeftClick()
    {
        if (clickIndex < 0)
        {
            for (int i = 0; i < touches.Count; i++)
            {
                if (touches[i].press.wasPressedThisFrame)
                {
                    if(Time.time - clickDownTime < doubletime)
                    {
                        doublecheck = true;
                    }
       
                    clickIndex = i;
                    clickDownTime = Time.time;
                    isHolding = false;
                    touchcount++;

                }
            }
            return false;
        }
        if (clickIndex >= touches.Count)
        {
            clickIndex = -1;
            touchcount = 0;
            isHolding = false;
            return false;
        }

        var t = touches[clickIndex];

        if (t.press.isPressed && (Time.time - clickDownTime) >= holdThreshold && touchcount == 1)
        {
            isHolding = true;
            delta = touches[clickIndex].delta.ReadValue();
        }
        else
        {
            delta = Vector2.zero;
        }

        if (t.press.wasReleasedThisFrame)
        {
            bool isTap = !isHolding && (Time.time - clickDownTime) < holdThreshold;
            pos = t.position.ReadValue();

            clickIndex = -1;
            touchcount = 0;
            doublecheck = false;
            return isTap;
        }
        doublecheck = false;
        return false;
    }

    public float ScrollY()
    {
        int a = -1, b = -1;
        for (int i = 0; i < touches.Count; i++)
        {
            if (touches[i].press.isPressed)
            {
                if (a == -1) a = i;
                else { b = i; break; }
            }
        }

        if (a == -1 || b == -1)
        {
            pinching = false;
            return 0f;
        }


        var p0 = touches[a].position.ReadValue();
        var p1 = touches[b].position.ReadValue();

        float dist = Vector2.Distance(p0, p1);

        if (!pinching)
        {
            pinching = true;
            prevPinchDist = dist;
            return 0f; // 開始フレームは差分 0
        }

        float delta = dist - prevPinchDist; // +広がる / -縮む
        prevPinchDist = dist;
        touchcount = 0;
        return delta;
    }



}

