
//--------------------------------------------------------------------------------------
// File: MakeRotation.fx
//
// The effect file for Silverlight4 applications by MRD to rotate b/w bitmasks and filter them
// 
//--------------------------------------------------------------------------------------

//-----------------------------------------------------------------------------------------
// Shader constant register mappings (scalars - float, double, Point, Color, Point3D, etc.)
//-----------------------------------------------------------------------------------------
float1 Angle : register(C0);
float1 Radius : register(C1);
float1 BW: register(C2);
float2 Scale: register(C3);

//--------------------------------------------------------------------------------------
// Sampler Inputs (Brushes, including ImplicitInput)
//--------------------------------------------------------------------------------------

sampler2D  implicitInputSampler : register(S0);

//--------------------------------------------------------------------------------------
// This shader produces a transparent image 
//--------------------------------------------------------------------------------------
float4 main(float2 uv : TEXCOORD) : COLOR 
{
	float2 uvd = Scale * (uv - 0.5);
	float p = atan2(uvd.x, uvd.y);
	float a = p + radians(Angle);
	float r = sqrt(uvd.x * uvd.x + uvd.y * uvd.y);
	float2 uva;
	if (r <= Radius)
		uva = r * float2(sin(a), cos(a)) + 0.5;
	else
		uva = r * float2(sin(p), cos(p)) + 0.5;
	float4 mask = tex2D(implicitInputSampler,uva);
	float4 destColor = mask;
	if (BW <= 0.0)
	{
		if (mask.r  >= 0.5)
			destColor.rgb = 1.0;
		else
			destColor.rgb = 0.0;
	}
	return destColor;
};
