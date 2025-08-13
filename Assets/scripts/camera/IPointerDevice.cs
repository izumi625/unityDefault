using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Utilities;
using System.Collections.Generic;

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
    private bool singleclick = false;
    private float clickDownTime;
    private bool isHolding;
    private bool doublecheck = false;

    Vector2 pos;
    Vector2 delta;
    private bool pinching;
    private float prevPinchDist;

    public TouchWrapper(ReadOnlyArray<TouchControl> touches) => this.touches = touches;

    public bool DoubleClicke() => doublecheck;
    public Vector2 GetPosition() => pos;
    public Vector2 GetDelta() => delta;
    public bool RightClick() => isHolding;
    public bool RightClickheld() => isHolding;




    private float doubleDownTime;

    public bool LeftClick()
    {
        doublecheck = false;
        if (singleclick && (Time.time - doubleDownTime) > doubletime)
        {
            singleclick = false;
            return true;
        }

        if (clickIndex < 0)
        {
            for (int i = 0; i < touches.Count; i++)
            {
                if (touches[i].press.wasPressedThisFrame)
                {
                    pos = touches[i].position.ReadValue();

                    if(IsOverUI(pos)) return false;
                         
        
                    clickIndex = i;
                    isHolding = false;
                    clickDownTime = Time.time; // このタップの押下開始時刻
                    touchcount = 1;         // この指を追う
                    break;
                }
            }
            if (clickIndex < 0) return false;   // 押下なし
        }




        var t = touches[clickIndex];



        // --- 4) ドラッグ/長押し ---
        if (t.press.isPressed && (Time.time - clickDownTime) >= holdThreshold && touchcount == 1)
        {
            isHolding = true;
            delta = t.delta.ReadValue();
        }
        else
        {
            delta = Vector2.zero;
        }

        if (t.press.wasReleasedThisFrame)
        {

            bool isTap = !isHolding && (Time.time - clickDownTime) < holdThreshold;

            if (isTap)
            {
                if (singleclick && (Time.time - doubleDownTime) <= doubletime)
                {
                    doublecheck = true;
                    singleclick = false;
                }
                else
                {
                    singleclick = true;
       
                }
            }
            doubleDownTime = Time.time;
            clickIndex = -1;
            isHolding = false;


            return false;
        }

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

    static bool IsOverUI(Vector2 screenPos)
    {
        if (EventSystem.current == null) return false;

        var data = new PointerEventData(EventSystem.current) { position = screenPos };
        var results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(data, results);
        return results.Count > 0;  // 1つでもUIに当たれば true
    }

}

