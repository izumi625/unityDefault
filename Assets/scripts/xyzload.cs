using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class xyzload : MonoBehaviour
{
    public string fileName = "merged_clean.xyz";  // StreamingAssets �ɂ���t�@�C����
    public Material pointMaterial;          // �V�F�[�_�[��ݒ肵���}�e���A��
    public float pointSize = 0.02f;         // �_�̃T�C�Y

    private ComputeBuffer pointBuffer;
    private int pointCount = 0;

    void Start()
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, fileName);
        var points = LoadXYZ(filePath);
        pointCount = points.Length;

        pointBuffer = new ComputeBuffer(pointCount, sizeof(float) * 3);
        pointBuffer.SetData(points);

        pointMaterial.SetBuffer("_points", pointBuffer);
        pointMaterial.SetFloat("_pointSize", pointSize);
    }

    void OnRenderObject()
    {
        pointMaterial.SetPass(0);
        Graphics.DrawProceduralNow(MeshTopology.Points, pointCount);
    }

    void OnDestroy()
    {
        pointBuffer?.Release();
    }

    public Vector3[] LoadXYZ(string filePath)
    {
        var lines = File.ReadAllLines(filePath);
        return lines.Select(line =>
        {
            var tokens = line.Split(' ');
            return new Vector3(
                float.Parse(tokens[0]),
                float.Parse(tokens[1]),
                float.Parse(tokens[2])
            );

        }).ToArray();
    }
}
