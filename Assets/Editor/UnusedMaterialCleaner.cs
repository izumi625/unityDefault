using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class UnusedMaterialChecker
{
    [MenuItem("Tools/Check Unused Materials In Selected Folder")]
    public static void CheckUnusedMaterials()
    {
        // 選択フォルダ取得
        string[] selectedFolders = Selection.assetGUIDs != null
            ? System.Array.ConvertAll(Selection.assetGUIDs, AssetDatabase.GUIDToAssetPath)
            : new string[0];

        if (selectedFolders.Length == 0)
        {
            Debug.LogWarning("フォルダを選択してください。");
            return;
        }

        // 選択フォルダ内のマテリアル一覧
        string[] guids = AssetDatabase.FindAssets("t:Material", selectedFolders);
        List<string> materialPaths = guids.Select(AssetDatabase.GUIDToAssetPath).ToList();
        Debug.Log($"Get Mat {materialPaths.Count}Count");

        // プロジェクト内の全アセット（マテリアル以外も含む）
        string[] allGuids = AssetDatabase.FindAssets("");
        List<string> allPaths = allGuids.Select(AssetDatabase.GUIDToAssetPath).ToList();

        List<string> unusedMaterials = new List<string>();

        foreach (string matPath in materialPaths)
        {
            bool isUsed = false;
            foreach (string assetPath in allPaths)
            {
                if (assetPath == matPath) continue; // 自分自身はスキップ

                if (assetPath.EndsWith(".fbx", System.StringComparison.OrdinalIgnoreCase))
                    continue;

                // このアセットの依存ファイルを取得
                var deps = AssetDatabase.GetDependencies(assetPath, true);
                if (deps.Contains(matPath))
                {
                    isUsed = true;
                    Debug.Log(assetPath);
                    break;
                }
            }

            if (!isUsed)
            {
                unusedMaterials.Add(matPath);
                Debug.Log($"未使用: {matPath}");
            }
        }

        Debug.Log($"未使用マテリアル数: {unusedMaterials.Count}");
    }
}
