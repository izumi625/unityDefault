Shader "Custom/DepthMask"
{
    SubShader
    {
        // Skybox����ɕ`�悳���悤�ɐݒ�
        Tags { "Queue" = "Transparent+51" "RenderType"="Opaque" }

        Cull Off       // �� �ǉ��F���ʂ��`��Ώۂ�
        ColorMask 0    // �F�͕`�悵�Ȃ��i���S�ɓ����j
        ZWrite On      // �[�x�ɂ͕`��
        ZTest LEqual   // �ʏ�̐[�x�e�X�g

        Pass { }
    }
}
