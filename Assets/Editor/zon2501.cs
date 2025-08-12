using UnityEngine;
using UnityEditor;

public static class Mat2501
{
    [MenuItem("Tools/Change Mat zon")]
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
                Material zonMaterial = Resources.Load<Material>("zon");
                if (zonMaterial != null)
                {

                    var materials = renderer.materials;
                    for (int i = 0; i < materials.Length; i++)
                    {
                        materials[i] = zonMaterial;
                    }
                    renderer.materials = materials;

                    count++;
                }
                else
                {
                    Debug.Log("ZON.mat is not in the Resources folder.");
                }
            }
        }

        Debug.Log($"Changed {count} items to the ZON material.");
    }

    [MenuItem("Tools/Change Mat zon", true)]
    public static bool ValidateAddMeshColliderToSelection()
    {
        return Selection.activeGameObject != null;
    }
}
