using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using TMPro;

[RequireComponent(typeof(LineRenderer))]
public class lidarcamera : MonoBehaviour
{
    public float maxRayDistance = 100f;
    public float markerScale = 0.05f;
    public Color markerColor = Color.black;
    private List<(Vector3, Vector3)> markerPairs = new List<(Vector3, Vector3)>();
    private Vector3? lastMarker = null;
    private Material mat;
    private Camera cam;
    List<GameObject> activeLabels = new List<GameObject>();


    void Start()
    {
        mat = Resources.Load<Material>("camera/distance");
        if (mat == null)
        {
            Debug.LogError("リソースフォルダ　camera/distance.matが読み込めません");
        }
        cam = Camera.main;
        if (cam == null)
        {
            Debug.LogError("MainCamera が見つかりません。カメラに 'MainCamera' タグを付けてください。");
        }
    }


    void Update()
    {
        foreach (var label in activeLabels)
        {
            if (label != null)
            {
                label.transform.forward = cam.transform.forward;
            }
        }
    }


    public void MeasureDistance(Vector2 pos)
    {


        Physics.queriesHitBackfaces = true;

        Ray ray = cam.ScreenPointToRay(pos);
        RaycastHit hit;



        if (Physics.Raycast(ray, out hit, maxRayDistance))
        {

            GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Cube);
            sphere.transform.position = hit.point;
            sphere.transform.localScale = Vector3.one * markerScale;

            Renderer renderer = sphere.GetComponent<Renderer>();
            renderer.material = mat;
            renderer.material.color = markerColor;
            Destroy(sphere.GetComponent<Collider>());


            if (lastMarker.HasValue)
            {
                Vector3 a = lastMarker.Value;
                Vector3 b = hit.point;

                markerPairs.Add((a, b));

                float distance = Vector3.Distance(a, b);

                DrawLineBetween(a, b, distance);
                lastMarker = null;
            }
            else
            {
                lastMarker = hit.point;
            }
        }
        




    }
    void DrawLineBetween(Vector3 start, Vector3 end, float distance)
    {

        GameObject lineObj = new GameObject("Line");
        LineRenderer lr = lineObj.AddComponent<LineRenderer>();
        lr.positionCount = 2;
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);
        lr.material = mat;
        lr.startColor = lr.endColor = Color.yellow;
        lr.startWidth = lr.endWidth = 0.02f;

        Vector3 midPoint = (start + end) / 2;
        midPoint.y += 0.3f;


        GameObject textObj = new GameObject("DistanceLabel");
        textObj.transform.position = midPoint;

        TextMeshPro textMesh = textObj.AddComponent<TextMeshPro>();
        textMesh.font = Resources.Load<TMP_FontAsset>("Fonts & Materials/LiberationSans SDF");
        textMesh.text = $"{distance:F2} m";
        textMesh.fontSize = 1.5f;
        textMesh.alignment = TextAlignmentOptions.Center;
        textMesh.color = Color.white;
        textMesh.enableCulling = true;



        GameObject background = GameObject.CreatePrimitive(PrimitiveType.Quad);
        background.name = "LabelBackground";
        background.transform.SetParent(textObj.transform);
        background.transform.localPosition = new Vector3(0, 0, 0.01f);
        background.transform.localScale = new Vector3(1f, 0.3f, 0.2f);

        Material bgMat = mat;
        bgMat.color = Color.black;
        background.GetComponent<MeshRenderer>().material = bgMat;

        Destroy(background.GetComponent<Collider>());


        activeLabels.Add(textObj);
    }


}
