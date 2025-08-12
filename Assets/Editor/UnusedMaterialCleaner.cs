using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class UnusedMaterialCleaner
{
    [MenuItem("Tools/Clean Unused Materials in Scene")]
    public static void CleanUnusedMaterials()
    {
        HashSet<Material> usedMaterials = new HashSet<Material>();

        // Renderer 系
        foreach (Renderer renderer in Object.FindObjectsByType<Renderer>(FindObjectsSortMode.None))
        {
            foreach (var mat in renderer.sharedMaterials)
            {
                if (mat != null)
                    usedMaterials.Add(mat);
            }
        }

        // UI Graphic 系 (Image, RawImage, Text など)
        foreach (Graphic graphic in Object.FindObjectsByType<Graphic>(FindObjectsSortMode.None))
        {
            if (graphic.material != null)
                usedMaterials.Add(graphic.material);
        }

        Debug.Log(usedMaterials.Count);

        // プロジェクト内の全マテリアルを取得
        string[] guids = AssetDatabase.FindAssets("t:Material");
        List<string> unusedPaths = new List<string>();
        Debug.Log(guids.Length);
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            Material mat = AssetDatabase.LoadAssetAtPath<Material>(path);

            if (!usedMaterials.Contains(mat))
            {
                unusedPaths.Add(path);
            }
        }

        if (unusedPaths.Count == 0)
        {
            EditorUtility.DisplayDialog("Unused Material Cleaner", "未使用のマテリアルは見つかりませんでした。", "OK");
            return;
        }

        if (EditorUtility.DisplayDialog(
            "Unused Material Cleaner",
            $"{unusedPaths.Count} 個の未使用マテリアルが見つかりました。削除しますか？",
            "削除する", "キャンセル"))
        {
            foreach (string path in unusedPaths)
            {
                AssetDatabase.DeleteAsset(path);
            }
            AssetDatabase.Refresh();
            EditorUtility.DisplayDialog("Unused Material Cleaner", "削除が完了しました。", "OK");
        }
    }
}
