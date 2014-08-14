//--------------------------------------------------------------------------------------
// File: TextureShearing.fx
//
// The effect file for Silverlight4 applications by MRD for shaering textures over color
// 
//--------------------------------------------------------------------------------------

//-----------------------------------------------------------------------------------------
// Shader constant register mappings (scalars - float, double, Point, Color, Point3D, etc.)
//-----------------------------------------------------------------------------------------------
float1 MaskScale : register(C0); //scale of mask
float1 ShearAngle : register(C1); //angle shearing
float1 ShearFactor : register(C2); //shearing factor (expansion for one, contraction for the axis orthgonal to it)
//float1 Green: register(C3);
float4 WheelColor: register(C3); // wheel color
float4 MaskColor: register(C4); // used for color of mask pixel
//--------------------------------------------------------------------------------------
// Sampler Inputs (Brushes, including ImplicitInput)
//--------------------------------------------------------------------------------------

sampler2D  implicitInputSampler : register(S0);
sampler2D  depthMap	: register(S1);


//--------------------------------------------------------------------------------------
// This shader produces an iso luminant image a with shearing texture
//--------------------------------------------------------------------------------------

float4 main(float2 uv : TEXCOORD) : COLOR 
{
	float2 uvd = uv - 0.5;
	float a = atan2(uvd.x, uvd.y);
	float aw = a + radians(ShearAngle);
	float4 destColor = 1;
	destColor.rgb = 0.3333;	
	float r = length(uvd);
	if ( r > 0.25) 
		return destColor;
	//float2 uvs =  r * float2(sin(a), cos(a));

	float2 uvt =  MaskScale * r * float2(sin(aw) * ShearFactor, cos(aw) * 1.0 / ShearFactor ) + 0.5;
	float4 depthMapColor = tex2D(depthMap, MaskScale * uvd + 0.5) - 0.5;

	destColor = tex2D(implicitInputSampler, uvt);
	if (destColor.r < 0.05) 
			return MaskColor;
	destColor.b = WheelColor.b;
	destColor.r = WheelColor.r * (depthMapColor.r + 0.5); 
	destColor.g = WheelColor.g *  (1 - depthMapColor.r); 
	return destColor;
}