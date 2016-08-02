sampler s0;

float4 PixelShaderFunction(float2 coords: TEXCOORD0) : COLOR0
{
	float4 Original;

	Original = tex2D(s0, float2(coords.x, coords.y));
	Original += tex2D(s0, float2(coords.x - 0.000625, coords.y - 0.000625));
	Original += tex2D(s0, float2(coords.x + 0.000625, coords.y + 0.000625));
	Original /= 4;

	Original += tex2D(s0, float2(coords.x + 0.000625, coords.y - 0.000625));
	Original += tex2D(s0, float2(coords.x - 0.000625, coords.y + 0.000625));
	Original /= 4;

    return Original;
}

technique Technique1
{
	pass Pass0
	{
		PixelShader = compile ps_2_0 PixelShaderFunction();
	}
}
