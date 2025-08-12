using UnityEngine;
using UnityEditor;

public static class Standard2502
{
    [MenuItem("Tools/Change to Standard2502")]
    public static void AddMeshColliderToSelection()
    {
        GameObject root = Selection.activeGameObject;
        int count = 0;
        foreach (Transform t in root.GetComponentsInChildren<Transform>(true))
        {
            GameObject go = t.gameObject;
            var renderer = go.GetComponent<Renderer>();
            if (renderer != null)
            {
                foreach (var mat in renderer.sharedMaterials)
                {
                    if (mat != null)
                    {
                        mat.shader = Shader.Find("Custom/StandardShader2502");
                    }
                }

                count++;
            }
        }

        Debug.Log($"Change Mat {count}");
    }

    [MenuItem("Tools/Change to Standard2502", true)]
    public static bool ValidateAddMeshColliderToSelection()
    {
        return Selection.activeGameObject != null;
    }
}
