float4x4 World;
float4x4 View;
float4x4 Projection;

// TODO: add effect parameters here.

uniform float3 ambLight;
uniform float  ambIntensity;
uniform float3 dirLight1;
uniform float3 dirColor1;
uniform float  dirIntensity1;

texture Terrain;
sampler TerrainSampler = 
sampler_state
{
    Texture = <Terrain>;
    
    //The MinFilter describes how the texture sampler will read for pixels
    //larger than one texel.
    MinFilter = Linear;
    //The MagFilter describes how the texture sampler will read for pixels
    //smaller than one texel.
    MagFilter = Linear;
    //The MipFilter describes how the texture sampler will combine different
    //mip levels of the texture.
    MipFilter = Linear;

    //The AddressU and AddressV values describe how the texture sampler will treat
    //a texture coordinate that is outside the range of [0-1].

    AddressU = Wrap;
    AddressV = Wrap;
};

struct VertexShaderInput
{
    float4 Position : POSITION0;
	float4 Normal : NORMAL0;
	float2 TexCoord: TEXCOORD0;

    // TODO: add input channels such as texture
    // coordinates and vertex colors here.
};

struct VertexShaderOutput
{
    float4 Position : POSITION0;
	float4 Normal : TEXCOORD0;
	float2 TexCoord: TEXCOORD1;

    // TODO: add vertex shader outputs such as colors and texture
    // coordinates here. These values will automatically be interpolated
    // over the triangle, and provided as input to your pixel shader.
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;

    float4 worldPosition = mul(input.Position, World);
    float4 viewPosition = mul(worldPosition, View);
    output.Position = mul(viewPosition, Projection);
	output.Normal = mul(mul(mul(input.Normal, World),View),Projection);
	output.Normal = mul(input.Normal, World);
	output.Normal = input.Normal;
	output.TexCoord = input.TexCoord;

    // TODO: add your vertex shader code here.

    return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
	float4 amb = ambIntensity * float4(ambLight,1);
	float4 texcolor = ambIntensity * tex2D(TerrainSampler, input.TexCoord);
	float3 norm = input.Normal;
	float4 color = float4(texcolor + amb + dirIntensity1 * dirColor1 * saturate(dot(-1 * norm, dirLight1)), 1);

	//color.w =1;
    return color;
}

technique Technique1
{
    pass Pass1
    {
        // TODO: set renderstates here.

        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
