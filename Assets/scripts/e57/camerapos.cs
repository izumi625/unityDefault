using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ScanPoseLoader : MonoBehaviour
{
    [System.Serializable]
    public class ScanPose
    {
        public int index;
        public Vector3 position;
    }

    private int i = 0;
    private List<ScanPose> poses;
    private float maxRayDistance = 1000f;


    private Texture front;
    private Texture back;
    private Texture left;
    private Texture right;
    private Texture up;
    private Texture down;

    [Header("Resorceのフォルダ名を入力")]
    [Header("sample (himezisouko)")]
    public String foldername;

    private Dictionary<int, float> rotMap;

    void Start()
    {
        if (foldername == null)
        {
            Debug.LogError("folodernameをインスペクターで入力してください");
            Debug.Break(); // エディタの再生を一時停止
        }
            
        LoadScanPosesFromString();
        poses = LoadScanPoses();
        if (poses.Count > 0)
        {
            transform.position = poses[i].position;
            SetSkybox(i);
        }
    }

    public List<ScanPose> LoadScanPoses()
    {
        var poses = new List<ScanPose>();

        TextAsset pos_csv = Resources.Load<TextAsset>($"{foldername}/pose_log");

        string[] lines = pos_csv.text.Split('\n');

        for (int i = 1; i < lines.Length; i++) // 先頭はヘッダー行
        {
            string line = lines[i].Trim();
            if (string.IsNullOrEmpty(line)) continue;

            string[] tokens = line.Split(',');

            ScanPose pose = new ScanPose
            {
                index = int.Parse(tokens[0]),
                position = new Vector3(
                    float.Parse(tokens[1]),
                    float.Parse(tokens[3]),
                    float.Parse(tokens[2])
                )
            };

            poses.Add(pose);
        }

        return poses;
    }
    public void LoadScanPosesFromString()
    {
        rotMap = new Dictionary<int, float>();

        TextAsset rot_csv = Resources.Load<TextAsset>($"{foldername}/pose_rot");


        var lines = rot_csv.text.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
        foreach (var line in lines)
        {
            var parts = line.Split(',');
            if (parts.Length == 2 &&
                int.TryParse(parts[0], out int index) &&
                float.TryParse(parts[1], out float angle))
            {
                rotMap[index] = angle;
            }
        }
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3? vector3 = Raycascheck();
            if (vector3 != null)
            {

                ScanPose closestPose = poses
                 .OrderBy(p => Vector3.Distance(p.position, vector3.Value))
                 .First();

                Debug.Log($"最も近いScanPose: index={closestPose.index}, position={closestPose.position}");
                SetSkybox(closestPose.index);
            }
        }
    }

    public void SetSkybox(int i)
    {

        transform.position = poses[i].position;

        front = Resources.Load<Texture>($"{foldername}/images/image{i * 6 + 3}");
        back = Resources.Load<Texture>($"{foldername}/images/image{i * 6 + 1}");
        right = Resources.Load<Texture>($"{foldername}/images/image{i * 6 + 2}");
        left = Resources.Load<Texture>($"{foldername}/images/image{i * 6 + 4}");
        up = Resources.Load<Texture>($"{foldername}/images/image{i * 6 + 0}");
        down = Resources.Load<Texture>($"{foldername}/images/image{i * 6 + 5}");


        RenderSettings.skybox.SetTexture("_FrontTex", front);
        RenderSettings.skybox.SetTexture("_BackTex", back);
        RenderSettings.skybox.SetTexture("_LeftTex", left);
        RenderSettings.skybox.SetTexture("_RightTex", right);
        RenderSettings.skybox.SetTexture("_UpTex", up);
        RenderSettings.skybox.SetTexture("_DownTex", down);

        RenderSettings.skybox.SetFloat("_Rotation", rotMap[i]);
    }

    public Vector3? Raycascheck()
    {
        Physics.queriesHitBackfaces = true;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, maxRayDistance)){
            return hit.point;
        }
        else
        {
            return null;
        }

    }

}
