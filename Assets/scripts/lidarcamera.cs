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
            Debug.LogError("default.mat �� Resources �Ɍ�����܂���I");
        }
    }

    void Update()
    {
        // �E�N���b�N�h���b�O�Ŏ��_�ړ�
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
                // �}�[�J�[�ݒu
                GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Cube);
                sphere.transform.position = hit.point;
                sphere.transform.localScale = Vector3.one * markerScale;

                Renderer renderer = sphere.GetComponent<Renderer>();
                renderer.material = mat;
                renderer.material.color = markerColor;
                Destroy(sphere.GetComponent<Collider>());

                // 2�_�ڂȂ�y�A��ۑ�
                if (lastMarker.HasValue)
                {
                    Vector3 a = lastMarker.Value;
                    Vector3 b = hit.point;

                    markerPairs.Add((a, b));

                    float distance = Vector3.Distance(a, b);
                    Debug.Log($"����: {distance:F3}m");

                    DrawLineBetween(a, b, distance);
                    lastMarker = null; // ���̃y�A�ɔ����ă��Z�b�g
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
        // ����`��
        GameObject lineObj = new GameObject("Line");
        LineRenderer lr = lineObj.AddComponent<LineRenderer>();
        lr.positionCount = 2;
        lr.SetPosition(0, start);
        lr.SetPosition(1, end);
        lr.material = mat;
        lr.startColor = lr.endColor = Color.yellow;
        lr.startWidth = lr.endWidth = 0.02f;

        // ���x���̈ʒu�͐��̒��_
        Vector3 midPoint = (start + end) / 2;
        midPoint.y += 0.3f;

        // TextMeshPro �̕\���I�u�W�F�N�g�쐬
        GameObject textObj = new GameObject("DistanceLabel");
        textObj.transform.position = midPoint;

        TextMeshPro textMesh = textObj.AddComponent<TextMeshPro>();
        textMesh.font = Resources.Load<TMP_FontAsset>("Fonts & Materials/LiberationSans SDF");
        textMesh.text = $"{distance:F2} m";
        textMesh.fontSize = 1.5f;
        textMesh.alignment = TextAlignmentOptions.Center;
        textMesh.color = Color.white;
        textMesh.enableCulling = true;


        // �w�i�p Quad ���쐬
        GameObject background = GameObject.CreatePrimitive(PrimitiveType.Quad);
        background.name = "LabelBackground";
        background.transform.SetParent(textObj.transform); // Text �ɒǏ]������
        background.transform.localPosition = new Vector3(0, 0, 0.01f); // �������ցiZ�����j
        background.transform.localScale = new Vector3(1f, 0.3f, 0.2f); // �T�C�Y����

        // �w�i�̃}�e���A��������
        Material bgMat = mat;
        bgMat.color = Color.black;
        background.GetComponent<MeshRenderer>().material = bgMat;

        // �s�v�ȃR���C�_�[���폜�iQuad �̓R���C�_�[�t���j
        Destroy(background.GetComponent<Collider>());


        activeLabels.Add(textObj);
    }



}
