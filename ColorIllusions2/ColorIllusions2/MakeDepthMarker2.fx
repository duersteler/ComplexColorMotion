
//--------------------------------------------------------------------------------------
// File: MakeDepthMark.fx
//
// The effect file for Silverlight4 applications by MRD for stting depth marks
// 
//--------------------------------------------------------------------------------------

//-----------------------------------------------------------------------------------------
// Shader constant register mappings (scalars - float, double, Point, Color, Point3D, etc.)
//-----------------------------------------------------------------------------------------
float1 Threshold : register(C0); //threshold fo depth to be collored (range 0 to 1.0, put above 1 to have no effect)
float1 WheelScale : register(C1); //Expansion/contraction (for rings only)
float1 WheelZoom : register(C2); //Zoomfactor for 3d-Figure
float1 OuterEnd: register(C3); //use 0.12



//--------------------------------------------------------------------------------------
// Sampler Inputs (Brushes, including ImplicitInput)
//--------------------------------------------------------------------------------------

sampler2D  implicitInputSampler : register(S0);
sampler2D  depthMapSampler	: register(S1);


//--------------------------------------------------------------------------------------
// This shader produces yellow marks on highest ring depth map values
//--------------------------------------------------------------------------------------
float4 main(float2 uv : TEXCOORD) : COLOR 
{
	float4 destColor = tex2D(implicitInputSampler, uv);
	if (Threshold > 1.0 || destColor.r < 0.05)
		return destColor;
	float2 uvd = 50.0 / WheelZoom  * WheelScale  * (uv - 0.5);
	float a = atan2(uvd.x, uvd.y);
	float r = sqrt(uvd.x * uvd.x + uvd.y * uvd.y);
	float4 depthMapColor = tex2D(depthMapSampler, uvd + 0.5);
	float1 ma = abs(a) % 0.78539816339744830961566084581988; //Pi / 4
	if (depthMapColor.r > 0.8 && (ma < 0.1 || ma > 0.68539816339744830961566084581988) &&  r < OuterEnd * WheelScale)  //0.8
	   destColor.rg =0.75;
	return destColor;
};
