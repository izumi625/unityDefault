using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;


public static class UIQuickMake
{

    static Animator anima;

    [MenuItem("Tools/UI/Do Something With Animator")]
    public static void DoSomething()
    {
        var go = Selection.activeGameObject;

        if (go == null || EditorUtility.IsPersistent(go) || go.GetComponent<Animator>() == null)
        {
            EditorUtility.DisplayDialog(
                "Selection Required",
                "Please select a GameObject in the scene that has an Animator component attached.",
                "OK"
            );
            return;
        }

        anima = go.GetComponent<Animator>();

        CreateSliderSmart();
    }

    public static void CreateSliderSmart()
    {
        // 1) �Ώ� Canvas ������i�I�𒆂̐e �� �V�[�����̍ŏ��j
        Canvas canvas = Selection.activeGameObject
            ? Selection.activeGameObject.GetComponentInParent<Canvas>()
            : null;
        canvas ??= Object.FindFirstObjectByType<Canvas>();

        // 2) Canvas ��������΍쐬�iEventSystem ���p�Ӂj
        if (!canvas)
        {
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
        }

        // 3) ���ɂ��� Canvas �z���� Slider ������Ȃ��炸�I��
        var existing = canvas.GetComponentInChildren<Slider>(true);
        if (existing)
        {
            Selection.activeGameObject = existing.gameObject;
            EditorGUIUtility.PingObject(existing);
            Debug.Log("Existing Slider found. Selected it.", existing);
            return;
        }

        // 4) Canvas ��I�� �� �������j���[�� Slider �𐶐�
        Selection.activeGameObject = canvas.gameObject;
        EditorApplication.ExecuteMenuItem("GameObject/UI/Slider");

        // 5) �d�グ�i�����z�u�E�T�C�Y�j
        var sliderGO = Selection.activeGameObject;
        if (sliderGO)
        {
            var rt = sliderGO.GetComponent<RectTransform>();
            if (rt)
            {
                rt.anchorMin = new Vector2(0.5f, 0f);
                rt.anchorMax = new Vector2(0.5f, 0f);

                rt.pivot = new Vector2(0.5f, 0f);

                rt.anchoredPosition = new Vector2(0f, 20f);
                rt.sizeDelta = new Vector2(220, 20);
            }
        }

        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        Debug.Log("Created Slider under Canvas.", sliderGO);


        var slider = canvas.GetComponentInChildren<UnityEngine.UI.Slider>(true);
        var sync = slider.GetComponent<SliderAnimatorSync>();
        if (!sync) sync = Undo.AddComponent<SliderAnimatorSync>(slider.gameObject);
        sync.slider = slider;
        sync.animator = anima;
        sync.layer = 0;

        EditorUtility.SetDirty(sync);
        EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());

        // �Ō�ɑI�����X���C�_�[��
        Selection.activeGameObject = slider.gameObject;
        EditorGUIUtility.PingObject(slider);

    }

}




