# Copilot Instructions for AI Agents

## プロジェクト概要
このリポジトリはUnityプロジェクトです。主な構成は `Assets/` ディレクトリ内のスクリプト・シーン・リソースと、`ProjectSettings/` の設定ファイル群です。C#スクリプトは主に `Assets/Scripts/` に配置されています。

## 主要コンポーネント
- **カメラ操作**: `Assets/Scripts/camera/MouseActions.cs` では、マウスによるカメラ移動・回転・ズームを制御しています。
- **シーン/リソース**: `Assets/Scenes/` や `Assets/Resources/` にはシーンや動的ロード用リソースが格納されています。
- **マテリアル/モデル**: 3Dモデルやマテリアルは `Assets/3DMODEL/`, `Assets/Materials/` にあります。

## ビルド・テスト・デバッグ
- **ビルド**: Unity Editorから通常通りビルド可能。特別なコマンドやスクリプトは現状なし。
- **テスト**: テストコードや自動テストのディレクトリは未発見。テスト追加時は `Assets/Tests/` などを推奨。
- **デバッグ**: Unity EditorのPlayモードでデバッグ。`Debug.Log`やGizmos活用例あり（例: `OnDrawGizmosSelected`）。

## コーディング規約・パターン
- **MonoBehaviour継承**: 全てのUnityスクリプトは `MonoBehaviour` を継承。
- **SerializeField/Inspector**: パラメータは `[Header]` や `[SerializeField]` でInspectorから編集可能。
- **InputSystem**: `UnityEngine.InputSystem` を利用（例: `MouseActions.cs`）。
- **命名規則**: PascalCase（クラス・メソッド）、camelCase（変数）。
- **レイヤー/マスク**: Raycast等で `LayerMask` を活用。

## 外部連携・依存
- **TextMesh Pro**: `Assets/TextMesh Pro/` ディレクトリあり。必要に応じてパッケージ管理。
- **Packages/manifest.json**: Unity Package Managerによる依存管理。

## 参考ファイル
- `Assets/Scripts/camera/MouseActions.cs`: カメラ操作の実装例
- `Packages/manifest.json`: 依存パッケージ一覧
- `ProjectSettings/ProjectSettings.asset`: プロジェクト設定

## 注意事項
- Unity Editorのバージョンに依存する場合があるため、`ProjectVersion.txt` を参照。
- `Library/`, `Temp/`, `obj/` ディレクトリはビルド生成物・一時ファイル。編集不要。

---

この内容で不明点や追加したい情報があればご指摘ください。
