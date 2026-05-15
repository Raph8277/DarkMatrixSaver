// Prototype shader CRT : à compiler en .ps avec fxc/dxc si tu veux brancher un ShaderEffect WPF.
// Le projet WPF contient déjà un fallback scanlines/vignette sans compilation shader.

sampler2D input : register(s0);
float intensity : register(c0);

float4 main(float2 uv : TEXCOORD) : COLOR
{
    float2 centered = uv - 0.5;
    float distortion = dot(centered, centered) * 0.12 * intensity;
    float2 warped = uv + centered * distortion;
    float4 color = tex2D(input, warped);
    float scan = sin(uv.y * 900.0) * 0.04 * intensity;
    color.rgb -= scan;
    color.rgb *= 1.0 - dot(centered, centered) * 0.55 * intensity;
    return color;
}
