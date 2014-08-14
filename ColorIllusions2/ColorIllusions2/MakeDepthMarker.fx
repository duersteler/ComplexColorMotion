
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
float1 WheelAngle : register(C1); //rotation of 3d-figure
float1 WheelScale : register(C2); //Expansion/contraction (for rings only)
float1 WheelZoom : register(C3); //Zoomfactor for 3d-Figure
float1 InnerEnd: register(C4); //use 0.03
float1 OuterEnd: register(C5); //use 0.12



//--------------------------------------------------------------------------------------
// Sampler Inputs (Brushes, including ImplicitInput)
//--------------------------------------------------------------------------------------

sampler2D  implicitInputSampler : register(S0);
sampler2D  depthMapSampler	: register(S1);


//--------------------------------------------------------------------------------------
// This shader produces an yellow mark on highest depth map values
//--------------------------------------------------------------------------------------
float4 main(float2 uv : TEXCOORD) : COLOR 
{
	float4 destColor = tex2D(implicitInputSampler, uv);
	if (Threshold > 1.0 || destColor.r < 0.01)
		return destColor;
	float2 uvd = 50.0 / WheelZoom  * WheelScale  * (uv - 0.5);
	float a = atan2(uvd.x, uvd.y);
	float aw = a + radians(WheelAngle);
	float r = sqrt(uvd.x * uvd.x + uvd.y * uvd.y);
	float2 uvw =  r * float2(cos(aw), sin(aw)) + 0.5;
	float4 depthMapColor = tex2D(depthMapSampler, uvw);
	if (depthMapColor.r > 0.95 &&  r < OuterEnd * WheelScale  && r > InnerEnd * WheelScale ) 
	   destColor.rg =0.75;
	return destColor;
};