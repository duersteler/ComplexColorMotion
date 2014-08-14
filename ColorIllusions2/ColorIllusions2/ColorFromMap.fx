
//--------------------------------------------------------------------------------------
// File:ColorFromMap.fx
//
// The effect file for Silverlight4 applications by MRD for color encoding
// Must by fx compiled by using /Vd compile option to disenable limits imposed by PS_2 shadel model
// 
//--------------------------------------------------------------------------------------

//-----------------------------------------------------------------------------------------
// Shader constant register mappings (scalars - float, double, Point, Color, Point3D, etc.)
//-----------------------------------------------------------------------------------------
float1 WheelAngle : register(C0); //rotation of 3d-figure
float1 MaskScale : register(C1); //scale of mask
float4 MaskColor: register(C2); // used for color of mask pixel
float1 WheelScale : register(C3); //Expansion/contraction (for rings only)
float1 WheelZoom : register(C4); //Zoomfactor for 3d-Figure
float2 Translation: register(C5); // used for flicker
float4 WheelColor1: register(C6); // wheel  start color
float4 WheelColor2: register(C7); // wheel end color




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
	//float4 depthMapColor =  tex2D(depthMapSampler, uvw);
	if (r > 0.125 * WheelScale)
	{
		destColor.rgb = 0.3333;
	}
	else
	{
		if (destColor.r < 0.05) 
			return MaskColor;

		destColor.r = WheelColor1.r +(depthMapColor.r + 0.5) * (WheelColor2.r - WheelColor1.r);
		destColor.g = WheelColor1.g + (depthMapColor.g + 0.5) * (WheelColor2.g - WheelColor1.g);
		destColor.b = WheelColor1.b;
		//destColor.r = WheelColor1.r * (depthMapColor.r + 0.5);  //+75
		//destColor.g = WheelColor1.g *  (1 - depthMapColor.r); 
	}
	return destColor;
	

};
