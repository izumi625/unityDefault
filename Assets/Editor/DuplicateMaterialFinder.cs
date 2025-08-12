using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class DuplicateMaterialFinder
{
    [MenuItem("Tools/Find Duplicate Materials (Shader + Colors)")]
    public static void FindDuplicates()
    {

        string[] selectedFolders = Selection.assetGUIDs != null
            ? System.Array.ConvertAll(Selection.assetGUIDs, AssetDatabase.GUIDToAssetPath)
            : new string[0];

        if (selectedFolders.Length == 0)
        {
            Debug.LogWarning("フォルダを選択してください。");
            return;
        }

        string[] guids = AssetDatabase.FindAssets("t:Material", selectedFolders);
        var map = new Dictionary<string, List<Material>>();

        int countLoaded = 0;
        foreach (var guid in guids)
        {
            var path = AssetDatabase.GUIDToAssetPath(guid);
            var mat = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (mat == null || mat.shader == null) continue;

            // 署名キー作成：シェーダー名 + 全Colorプロパティ（名前=値）
            string key = BuildSignature(mat);
            if (!map.TryGetValue(key, out var list))
            {
                list = new List<Material>();
                map[key] = list;//空リスト　キー名だけ登録
            }
            list.Add(mat);
            countLoaded++;
        }

        var dupGroups = map.Where(kv => kv.Value.Count > 1)
                           .OrderByDescending(kv => kv.Value.Count)
                           .ToList();

        if (dupGroups.Count == 0)
        {
            Debug.Log($"[DuplicateMaterialFinder] 重複なし。チェック対象：{countLoaded} 個のMaterial。");
            return;
        }

        Debug.Log($"[DuplicateMaterialFinder] 重複候補グループ数：{dupGroups.Count}（合計 {countLoaded} 個をスキャン）");

        int g = 1;
        foreach (var (signature, mats) in dupGroups)
        {
            Debug.Log($"─── 重複グループ #{g}  件数: {mats.Count}\n{signature}");
            foreach (var m in mats)
            {
                // オブジェクトを渡すとクリックでPing可能
                Debug.Log($"   • {AssetDatabase.GetAssetPath(m)}", m);
            }
            g++;
        }

        if (!EditorUtility.DisplayDialog(
            "重複マテリアルの処理確認",
            $"重複候補 {dupGroups.Count} グループが見つかりました。\n" +
            "このまま置き換え・削除を実行しますか？",
            "はい（実行）",
            "いいえ（中止）"))
        {
            // ユーザーが「いいえ」を押した場合は処理中断
            return;
        }
        MatDelete(dupGroups);


    }

    // 署名作成：Shader名 + Colorプロパティ（名前=R,G,B,A）をソートして連結
    private static string BuildSignature(Material mat)
    {
        var shader = mat.shader;
        var props = GetColorProperties(shader);
        var items = new List<string>(props.Count);

        // Colorプロパティが1つも無い場合、Material.color も一応見る（存在すれば）
        if (props.Count == 0)
        {
            // Unityの標準Colorプロパティ候補
            var fallbackNames = new[] { "_Color", "_BaseColor", "_Tint" };
            foreach (var name in fallbackNames)
            {
                if (mat.HasProperty(name))
                {
                    var c = mat.GetColor(name);
                    items.Add($"{name}={Fmt(c)}");
                }
            }
        }
        else
        {
            foreach (var name in props)
            {
                if (mat.HasProperty(name))
                {
                    var c = mat.GetColor(name);
                    items.Add($"{name}={Fmt(c)}");
                }
                else
                {
                    // 念のため存在しない場合も記録（差分になる）
                    items.Add($"{name}=<missing>");
                }
            }
        }

        // プロパティ名で安定ソート
        items.Sort(StringComparer.Ordinal);

        return $"Shader:{shader.name}\n" + string.Join("\n", items);
    }

    // Shader内のColor型プロパティ名一覧を取得（Editor専用API）
    private static List<string> GetColorProperties(Shader shader)
    {

        var names = new List<string>();
        int count = ShaderUtil.GetPropertyCount(shader);
        for (int i = 0; i < count; i++)
        {
            var type = ShaderUtil.GetPropertyType(shader, i);
            if (type == ShaderUtil.ShaderPropertyType.Color)
            {
                names.Add(ShaderUtil.GetPropertyName(shader, i));
            }
        }
        return names;
    }

    // Colorの値を丸めて安定化（浮動小数の微妙な差を無視）
    private static string Fmt(Color c)
    {
        // 完全一致が要件なら丸めなしでもOK。微小誤差で弾かれるのを避けるなら丸め推奨。
        return $"{Round4(c.r)},{Round4(c.g)},{Round4(c.b)},{Round4(c.a)}";
    }

    private static string Round4(float v) => Math.Round(v, 4).ToString("0.####");

    private static void MatDelete(List<KeyValuePair<string, List<Material>>> dupGroups)
    {
        foreach (var kv in dupGroups)
        {
            var signature = kv.Key;
            var mats = kv.Value.Where(m => m != null).Distinct().ToList();
            if (mats.Count <= 1) continue;

            var master = mats.First();
            foreach (var dup in mats.Skip(1))
            {
                var dupPath = AssetDatabase.GetAssetPath(dup);
                if (!string.IsNullOrEmpty(dupPath))
                {
                    AssetDatabase.DeleteAsset(dupPath);
                    Debug.Log($"削除: {dupPath}");
                }
            }
        }
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }


}


