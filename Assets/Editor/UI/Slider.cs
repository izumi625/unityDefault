#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

public class SliderCreate
{
    private Canvas canvas;
    private Animator anima;
    public SliderCreate(Canvas canvas)
    {
        this.canvas = canvas;
        var animator = Object.FindFirstObjectByType<Animator>();

        if (animator == null)
        {
            EditorUtility.DisplayDialog(
                "Animator Not Found",
                "No GameObject with an Animator component was found in the scene.",
                "OK"
            );
            return;
        }

        // 見つかった Animator をセット
        anima = animator;
    }
    public void Create()
    {
        var existing = canvas.GetComponentInChildren<Slider>(true);
        if (existing)
        {
            Selection.activeGameObject = existing.gameObject;
            EditorGUIUtility.PingObject(existing);
            Debug.Log("Existing Slider found. Selected it.", existing);
            return;
        }

        Selection.activeGameObject = canvas.gameObject;
        EditorApplication.ExecuteMenuItem("GameObject/UI/Slider");

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

        Selection.activeGameObject = slider.gameObject;
        EditorGUIUtility.PingObject(slider);
    }


}
#endif