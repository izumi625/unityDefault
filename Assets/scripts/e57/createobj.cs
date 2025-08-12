using UnityEngine;
using UnityEngine.UI;

public class CanvasSetup : MonoBehaviour
{
    public static void CreateCanvas()
    {
        GameObject canvasObj = new GameObject("Canvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.worldCamera = Camera.main;
        canvasObj.AddComponent<CanvasScaler>();
        canvasObj.AddComponent<GraphicRaycaster>();

        // Image�i�����̓_�j�쐬
        GameObject imageObj = new GameObject("CenterDot");
        imageObj.transform.SetParent(canvasObj.transform);

        Image image = imageObj.AddComponent<Image>();
        image.color = Color.white; // �F�w��i���_�ɂ�������� Color.white�j

        RectTransform rt = image.GetComponent<RectTransform>();
        rt.sizeDelta = new Vector2(10, 10);
        rt.anchoredPosition = Vector2.zero;
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
    }
}
