using UnityEngine;
using System.Collections.Generic;
using TMPro;

[RequireComponent(typeof(LineRenderer))]
public class RayVisualizer : MonoBehaviour
{
    public float maxRayDistance = 100f;
    public float markerScale = 0.05f;
    public Color markerColor = Color.black;
    public float sensitivity = 3f;
    private float rotationX = 0f;
    private float rotationY = 0f;

    private List<(Vector3, Vector3)> markerPairs = new List<(Vector3, Vector3)>();
    private Vector3? lastMarker = null;

    private Material mat;

    List<GameObject> activeLabels = new List<GameObject>();
    void Start()
    {
        Vector3 angles = transform.eulerAngles;
        rotationX = angles.x;
        rotationY = angles.y;
        mat = Resources.Load<Material>("default");
        if (mat == null)
        {
            Debug.LogError("default.mat が Resources に見つかりません！");
        }
    }

    void Update()
    {
        // 右クリックドラッグで視点移動
        if (Input.GetMouseButton(1))
        {
            float mouseX = Input.GetAxis("Mouse X") * sensitivity;
            float mouseY = Input.GetAxis("Mouse Y") * sensitivity;

            rotationY += mouseX;
            rotationX -= mouseY;
            rotationX = Mathf.Clamp(rotationX, -90f, 90f);

            transform.rotation = Quaternion.Euler(rotationX, rotationY, 0f);
        }

        if (Input.GetMouseButtonDown(2))
        {
            Physics.queriesHitBackfaces = true;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, maxRayDistance))
            {
                // マーカー設置
                GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Cube);
                sphere.transform.position = hit.point;
                sphere.transform.localScale = Vector3.one * markerScale;

                Renderer renderer = sphere.GetComponent<Renderer>();
                renderer.material = mat;
                renderer.material.color = markerColor;
                Destroy(sphere.GetComponent<Collider>());

                // 2点目ならペアを保存
                if (lastMarker.HasValue)
                {
                    Vector3 a = lastMarker.Value;
                    Vector3 b = hit.point;

                    markerPairs.Add((a, b));

                    float distance = Vector3.Distance(a, b);
                    Debug.Log($"距離: {distance:F3}m");

                    DrawLineBetween(a, b, distance);
                    lastMarker = null; // 次のペアに備えてリセット
                }
                else
                {
                    lastMarker = hit.point;
                }
            }
        }

        foreach (var label in activeLabels)
        {
            if (label != null)
            {
                label.transform.forward = Camera.main.transform.forward;
            }
        }


    }
    void DrawLineBetween(Vector3 start, Vector3 end, float distance)
    {
        // 線を描画
        GameObject lineObj = new GameObject("Line");
        LineRenderer lr = lineObj.AddComponent<LineRenderer>();
        lr.positionCount = 2;
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);
        lr.material = mat;
        lr.startColor = lr.endColor = Color.yellow;
        lr.startWidth = lr.endWidth = 0.02f;

        // ラベルの位置は線の中点
        Vector3 midPoint = (start + end) / 2;
        midPoint.y += 0.3f;

        // TextMeshPro の表示オブジェクト作成
        GameObject textObj = new GameObject("DistanceLabel");
        textObj.transform.position = midPoint;

        TextMeshPro textMesh = textObj.AddComponent<TextMeshPro>();
        textMesh.font = Resources.Load<TMP_FontAsset>("Fonts & Materials/LiberationSans SDF");
        textMesh.text = $"{distance:F2} m";
        textMesh.fontSize = 1.5f;
        textMesh.alignment = TextAlignmentOptions.Center;
        textMesh.color = Color.white;
        textMesh.enableCulling = true;


        // 背景用 Quad を作成
        GameObject background = GameObject.CreatePrimitive(PrimitiveType.Quad);
        background.name = "LabelBackground";
        background.transform.SetParent(textObj.transform); // Text に追従させる
        background.transform.localPosition = new Vector3(0, 0, 0.01f); // 少し後ろへ（Z方向）
        background.transform.localScale = new Vector3(1f, 0.3f, 0.2f); // サイズ調整

        // 背景のマテリアルを黒に
        Material bgMat = mat;
        bgMat.color = Color.black;
        background.GetComponent<MeshRenderer>().material = bgMat;

        // 不要なコライダーを削除（Quad はコライダー付き）
        Destroy(background.GetComponent<Collider>());


        activeLabels.Add(textObj);
    }



}
