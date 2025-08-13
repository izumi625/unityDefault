#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Events;
using UnityEngine;
using UnityEngine.UI;

public class UpDownBottom
{
    private Canvas canvas;
    public UpDownBottom(Canvas canvas)
    {
        this.canvas = canvas;
    }
    public void Create()
    {
        if (!canvas) { Debug.LogError("Canvas is null"); return; }

        var camT = Camera.main;
        if (!camT) { Debug.LogError("No camera found."); return; }

        var controller = camT.GetComponent<MouseContlole>();
        if (!controller) controller = Undo.AddComponent<MouseContlole>(camT.gameObject);

        var upBtnGO = CreateUIButton(canvas.transform, "UpButton", "Up", new Vector2(-50, 30));
        var downBtnGO = CreateUIButton(canvas.transform, "DownButton", "Down", new Vector2(-50, -30));

        var up = upBtnGO.GetComponent<Button>();
        var down = downBtnGO.GetComponent<Button>();

        UnityEventTools.AddPersistentListener(up.onClick, controller.Up);
        UnityEventTools.AddPersistentListener(down.onClick, controller.Down);

        EditorUtility.SetDirty(up);
        EditorUtility.SetDirty(down);
        EditorUtility.SetDirty(controller);
    }

    static GameObject CreateUIButton(Transform parent, string name, string text, Vector2 anchoredPos)
    {
        var go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image), typeof(Button));
        go.transform.SetParent(parent, false);

        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = rt.anchorMax = new Vector2(1f, 0.5f);
        rt.pivot = new Vector2(1f, 0.5f);
        rt.sizeDelta = new Vector2(80, 40);
        rt.anchoredPosition = anchoredPos;

        var textGO = new GameObject("Text", typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
        textGO.transform.SetParent(go.transform, false);
        var txt = textGO.GetComponent<Text>();
        txt.text = text;
        txt.alignment = TextAnchor.MiddleCenter;
        txt.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        txt.color = Color.black;
        txt.fontSize = 20;

        var trt = textGO.GetComponent<RectTransform>();
        trt.anchorMin = Vector2.zero; trt.anchorMax = Vector2.one;
        trt.offsetMin = Vector2.zero;  trt.offsetMax = Vector2.zero;

        return go;
    }
}
#endif
