//reuse//code//ApplyAnEffectFxFile//
uniform extern float4x4 WorldViewProj : WORLDVIEWPROJECTION;

struct VS_OUTPUT
{
    float4 position : POSITION;
    float4 color : COLOR0;
};

VS_OUTPUT Transform(
    float4 Pos  : POSITION, 
    float4 Color : COLOR0 )
{
    VS_OUTPUT Out = (VS_OUTPUT)0;

    Out.position = mul(Pos, WorldViewProj);
    Out.color = Color;

    return Out;
}

float4 ApplyAPixelShader( VS_OUTPUT vsout ) : COLOR
{
	return vsout.color;
}

technique TransformTechnique
{
    pass P0
    {
        vertexShader = compile vs_2_0 Transform();
        pixelShader = compile ps_2_0 ApplyAPixelShader();
    }
}
//reuse//code//ApplyAnEffectFxFile//

