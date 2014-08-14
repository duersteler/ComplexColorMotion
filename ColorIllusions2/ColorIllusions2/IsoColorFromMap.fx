
//--------------------------------------------------------------------------------------
// File:IsoColorFromMap.fx
//
// The effect file for Silverlight4 applications by MRD for color encoding
// Uses an about isoluminant mask
//--------------------------------------------------------------------------------------

//-----------------------------------------------------------------------------------------
// Shader constant register mappings (scalars - float, double, Point, Color, Point3D, etc.)
//-----------------------------------------------------------------------------------------
float1 WheelAngle : register(C0); //rotation of 3d-figure
float1 MaskScale : register(C1); //scale of mask
float1 WheelScale : register(C2); //Expansion/contraction (for rings only)
float1 WheelZoom : register(C3); //Zoomfactor for 3d-Figure
float1 Green: register(C4); // give value for green intensity
float2 Translation: register(C5); // used for flicker


//--------------------------------------------------------------------------------------
// Sampler Inputs (Brushes, including ImplicitInput)
//--------------------------------------------------------------------------------------

sampler2D  implicitInputSampler : register(S0);
sampler2D  depthMapSampler	: register(S1);


//--------------------------------------------------------------------------------------
// This shader produces an red-green Image according the depth map
//--------------------------------------------------------------------------------------
float4 main(float2 uv : TEXCOORD) : COLOR 
{
	
	float2 uvm = MaskScale * (uv - 0.5) + 0.5  + Translation;
	float4 destColor = tex2D(implicitInputSampler, uvm);
	float2 uvd = 50.0 / WheelZoom  * WheelScale  * (uv - 0.5);
	//float2 uvd = 1.0 * (uv - 0.5);
	float a = atan2(uvd.x, uvd.y);
	float aw = a + radians(WheelAngle);
	float r = length(uvd);
	float2 uvw =  r * float2(cos(aw), sin(aw)) + 0.5;
	//float4 depthMapColor =  2 * (tex2D(depthMapSampler, uvw) - 0.5);
	//float4 depthMapColor =  4 * (tex2D(depthMapSampler, uvw) - 0.75);
	float4 depthMapColor =  tex2D(depthMapSampler, uvw) - 0.5; //adapted for map using half width only
	
	if (r > 0.125 * WheelScale)
	{
		destColor.rgb = 0.3333;
	}
	else
	{
		if (destColor.r < 0.05) 
			//return destColor;
		{

			return float4(0.4833984375,0.2724609375,0,1);
		}

		destColor.b = 0;
		destColor.r = 0.64453125 * (depthMapColor.r + 0.5); //adapted for map using half width only
		destColor.g = Green * (1 - depthMapColor.r); 
	}
	return destColor;
	

};
