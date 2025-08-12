Shader "Custom/DepthMask"
{
    SubShader
    {
        // Skyboxより後に描画されるように設定
        Tags { "Queue" = "Transparent+51" "RenderType"="Opaque" }

        Cull Off       // ← 追加：裏面も描画対象に
        ColorMask 0    // 色は描画しない（完全に透明）
        ZWrite On      // 深度には描く
        ZTest LEqual   // 通常の深度テスト

        Pass { }
    }
}
