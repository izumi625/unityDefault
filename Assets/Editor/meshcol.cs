using UnityEngine;
using UnityEditor;

public static class MeshColliderTool
{
    [MenuItem("Tools /Attach Mesh Collider")]
    public static void AddMeshColliderToSelection()
    {
        GameObject root = Selection.activeGameObject;

        int count = 0;
        foreach (Transform t in root.GetComponentsInChildren<Transform>(true))
        {
            GameObject go = t.gameObject;
            var mf = go.GetComponent<MeshFilter>();
            var mr = go.GetComponent<MeshRenderer>();

            if (mf != null && mr != null)
            {
                var collider = go.GetComponent<MeshCollider>();
                if (collider == null)
                {
                    collider = go.AddComponent<MeshCollider>();
                }

                collider.sharedMesh = mf.sharedMesh;
                count++;
            }
        }

        Debug.Log($"Added Mesh Collider to {count} items");
    }

    [MenuItem("Tools /Attach a Mesh Collider", true)]
    public static bool ValidateAddMeshColliderToSelection()
    {
        return Selection.activeGameObject != null;
    }
}
