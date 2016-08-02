float AbberationAmount;
float RadialBlurAmount;
sampler s0;

float4 PixelShaderFunction(float2 coords: TEXCOORD0) : COLOR0
{
	float4 Original/* = tex2D(s0, float2(coords.x, coords.y))*/;
	//float4 Alpha = tex2D(s0, float2(coords.x, coords.y));

	float4 Red = tex2D(s0, float2(coords.x + AbberationAmount, coords.y));
	Red.gb = 0;

	float4 Gre = tex2D(s0, float2(coords.x, coords.y));
	Gre.rb = 0;

	float4 Blu = tex2D(s0, float2(coords.x - AbberationAmount, coords.y));
	Blu.rg = 0;

	//if (Original.r < 0.2f && Original.g < 0.2f)
	//{
	//	Red *= 2;
	//}

	//if (Original.b < 0.2f && Original.g < 0.2f)
	//{
	//	Blu *= 2;
	//}

	Original = Blu;
	Original += Red;
	Original += Gre;
	Original /= 3;

	return Original;
}

technique Technique1
{
    pass Pass1
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
