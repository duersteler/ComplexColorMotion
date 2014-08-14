//--------------------------------------------------------------------------------------
// File: RealShearfx
//
// The effect file for Silverlight4 applications by MRD for shearing color pictures
// 
//--------------------------------------------------------------------------------------

//-----------------------------------------------------------------------------------------
// Shader constant register mappings (scalars - float, double, Point, Color, Point3D, etc.)
//-----------------------------------------------------------------------------------------------
float1 MaskScale : register(C0); //scale of mask
float1 ShearAngle : register(C1); //angle shearing
float1 ShearFactor : register(C2); //shearing factor (expansion for one, contraction for the axis orthgonal to it)
float1 IsStatTexture : register(C3); // 0: random dot texture is shearing too, 1: random dots texture move only with disparity
float4 MaskColor: register(C4); // used for color of mask pixel
float2 MaskTranslation: register(C5); // used for flicker
float1 IsStatWheel: register(C6); //0: wheel is shearing, 1: wheel is not shearing
float4 WheelColor1: register(C7); // wheel  start color
float4 WheelColor2: register(C8); // wheel end color


//--------------------------------------------------------------------------------------
// Sampler Inputs (Brushes, including ImplicitInput)
//--------------------------------------------------------------------------------------

sampler2D  implicitInputSampler : register(S0);
sampler2D  depthMap	: register(S1);


//--------------------------------------------------------------------------------------
// This shader produces an color shearing stereo image 
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
	float2 uvw =  r * float2(sin(aw) * ShearFactor, cos(aw) * 1.0 / ShearFactor ) + 0.5;

	float2 uvm;
	if (IsStatTexture == 1.0)
		uvm = MaskScale * (uv - 0.5) + 0.5;// + MaskTranslation;
	else
		uvm = MaskScale * (uvw - 0.5) + 0.5;// + MaskTranslation;
	destColor = tex2D(implicitInputSampler, uvm);
	if (destColor.r < 0.05) 
		return MaskColor;
	float4 depthMapColor;
	if (IsStatWheel == 0.0)
		depthMapColor =  tex2D(depthMap, uvw) - 0.5; //-0.75
	else
		depthMapColor = tex2D(depthMap, uvd)- 0.5;
	if (destColor.r < 0.05) 
			return MaskColor;
		destColor.r = WheelColor1.r +(depthMapColor.r + 0.5) * (WheelColor2.r - WheelColor1.r);
		destColor.g = WheelColor1.g + (depthMapColor.g + 0.5) * (WheelColor2.g - WheelColor1.g);
		destColor.b = WheelColor1.b;
	return destColor;
}
