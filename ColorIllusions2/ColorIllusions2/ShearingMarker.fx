//--------------------------------------------------------------------------------------
// File: ShearingMarker.fx
//
// The effect file for Silverlight4 applications by MRD for  shearing anaglyphic stereopictures
// 
//--------------------------------------------------------------------------------------

//-----------------------------------------------------------------------------------------
// Shader constant register mappings (scalars - float, double, Point, Color, Point3D, etc.)
//-----------------------------------------------------------------------------------------
float1 MarkerThreshold : register(C0); //threshold fo depth to be collored (range 0 to 1.0, put above 1 to have no effect)
float1 ShearAngle : register(C1); //expanding/contraction factor: area constant
float1 ShearFactor: register(C2); 






//--------------------------------------------------------------------------------------
// Sampler Inputs (Brushes, including ImplicitInput)
//--------------------------------------------------------------------------------------

sampler2D  implicitInputSampler : register(S0);
sampler2D  depthMapSampler	: register(S1);


//--------------------------------------------------------------------------------------
// This shader produces an anglyphic stereo image 
//--------------------------------------------------------------------------------------
float4 main(float2 uv : TEXCOORD) : COLOR 
{
	float4 destColor = tex2D(implicitInputSampler, uv);
	if (MarkerThreshold > 1.0 || destColor.r > 0.95)
		return destColor;

	float2 uvd = uv - 0.5;
	float a = atan2(uvd.x, uvd.y);
	float aw = a + radians(ShearAngle);
	float r = length(uvd);
	float2 uvw =  r * float2(sin(aw) * ShearFactor, cos(aw) * 1.0 / ShearFactor ) + 0.5;
	float4 depthMapColor = tex2D(depthMapSampler, uvw);

	if (depthMapColor.r > MarkerThreshold && r < 0.25)
	{
		destColor.rg = 0.75;
	}
	return destColor;
};
