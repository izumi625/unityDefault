#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

public class UIQuickMakeWindow : EditorWindow
{
    // 任意で手動指定したい場合に使う
    [SerializeField] private Canvas canvasRef;

    [MenuItem("Tools/UI/Canvas Quick Make")]
    private static void Open()
    {
        var win = GetWindow<UIQuickMakeWindow>("Canvas Quick Make");
        win.minSize = new Vector2(360, 180);
        win.Show();
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("Canvas", EditorStyles.boldLabel);


        EditorGUILayout.HelpBox(
        "If the Canvas is null, it will be created automatically.",
            MessageType.Info
        );
        canvasRef = (Canvas)EditorGUILayout.ObjectField("Target Canvas", canvasRef, typeof(Canvas), true);
        FindCanvasInScene();

        EditorGUILayout.Space();

        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("Slider linked to animation");

        using (new EditorGUI.DisabledScope(false))
        {
            if (GUILayout.Button("Create Slider", GUILayout.Height(32)))
            {
                var canvas = canvasRef ? canvasRef : EnsureCanvas();
                if (canvas == null)
                {
                    EditorUtility.DisplayDialog("Canvas Not Found", "Could not find or create a Canvas.", "OK");
                    return;
                }
                var creator = new SliderCreate(canvas);
                creator.Create();
                EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            }
        }
        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("Camera Position Control");
        if (GUILayout.Button("UpDownButton Create", GUILayout.Height(32)))
        {
            var canvas = canvasRef ? canvasRef : EnsureCanvas();
            if (canvas == null)
            {
                EditorUtility.DisplayDialog("Canvas Not Found", "Could not find or create a Canvas.", "OK");
                return;
            }
            var creator = new UpDownBottom(canvas);
            creator.Create();
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        }
    }

    // シーン内から最初の Canvas を探す
    private static Canvas FindCanvasInScene()
    {
        var canvas = Object.FindFirstObjectByType<Canvas>();
        return canvas;
    }

    private static Canvas EnsureCanvas()
    {
        var canvas = Object.FindFirstObjectByType<Canvas>();
        if (canvas) return canvas;

        var canvasGO = new GameObject("Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        Undo.RegisterCreatedObjectUndo(canvasGO, "Create Canvas");
        canvas = canvasGO.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;

        var scaler = canvasGO.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;

        if (!Object.FindFirstObjectByType<EventSystem>())
        {
            var esGO = new GameObject("EventSystem", typeof(EventSystem));
            esGO.AddComponent<InputSystemUIInputModule>();
            Undo.RegisterCreatedObjectUndo(esGO, "Create EventSystem");
        }

        Selection.activeGameObject = canvasGO;
        EditorGUIUtility.PingObject(canvasGO);
        return canvas;
    }
}
#endif
