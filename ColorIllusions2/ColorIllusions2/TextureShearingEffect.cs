using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Media.Effects;

namespace ColorIllusions2
{
    public class TextureShearingEffect: ShaderEffect
    {
        private static PixelShader _pixelshader = new PixelShader()
        {
           UriSource = new Uri("/ColorIllusions2;component/TextureShearing.ps",
                UriKind.Relative)
        };

        public TextureShearingEffect()
        {
            PixelShader = _pixelshader;

            UpdateShaderValue(RdDotInputProp);
            UpdateShaderValue(ColorMapProp);
            UpdateShaderValue(ScaleProp);
            UpdateShaderValue(ShearFactorProp);
            //UpdateShaderValue(GreenProp);
            UpdateShaderValue(WheelColorProp);
            UpdateShaderValue(MaskColorProp);

       }

        public static DependencyProperty RdDotInputProp =
            ShaderEffect.RegisterPixelShaderSamplerProperty("RdDotInput", typeof(TextureShearingEffect), 0);

        public Brush RdDotInput
        {
            get { return ((Brush)base.GetValue(RdDotInputProp)); }
            set { base.SetValue(RdDotInputProp, value); }
        }

        public static readonly DependencyProperty ColorMapProp =
            ShaderEffect.RegisterPixelShaderSamplerProperty("ColorMap", typeof(TextureShearingEffect), 1);

        public Brush ColorMap
        {
            get { return (Brush)GetValue(ColorMapProp); }
            set { SetValue(ColorMapProp, value); }
        }
        public static readonly DependencyProperty ScaleProp =
            DependencyProperty.Register("MaskScale", typeof(double), typeof(TextureShearingEffect), new PropertyMetadata(0.0, PixelShaderConstantCallback(0)));

        public double MaskScale
        {
            get { return (double)GetValue(ScaleProp); }
            set { SetValue(ScaleProp, value); }
        }
        public static readonly DependencyProperty ShearAngleProp =
            DependencyProperty.Register("ShearAngle", typeof(double), typeof(TextureShearingEffect), new PropertyMetadata(0.0, PixelShaderConstantCallback(1)));

        public double ShearAngle
        {
            get { return (double)GetValue(ShearAngleProp); }
            set { SetValue(ShearAngleProp, value); }
        }
        public static readonly DependencyProperty ShearFactorProp =
            DependencyProperty.Register("ShearFactor", typeof(double), typeof(TextureShearingEffect), new PropertyMetadata(0.0, PixelShaderConstantCallback(2)));

        public double ShearFactor
        {
            get { return (double)GetValue(ShearFactorProp); }
            set {  SetValue(ShearFactorProp, value); }
        }
        /**********************************
        public static readonly DependencyProperty GreenProp =
            DependencyProperty.Register("Green", typeof(double), typeof(TextureShearingEffect), new PropertyMetadata(0.0, PixelShaderConstantCallback(3)));

        public double Green
        {
            get { return (double)GetValue(GreenProp); }
            set { SetValue(GreenProp, value); }
        }
        ********************************/
        public static readonly DependencyProperty WheelColorProp =
             DependencyProperty.Register("WheelColor", typeof(Color), typeof(TextureShearingEffect),
                 new PropertyMetadata(new Color(), PixelShaderConstantCallback(3)));

        public Color WheelColor
        {
            get { return (Color)GetValue(WheelColorProp); }
            set { SetValue(WheelColorProp, value); }
        }

        public static readonly DependencyProperty MaskColorProp =
            DependencyProperty.Register("MaskColor", typeof(Color), typeof(TextureShearingEffect),
                new PropertyMetadata(new Color(), PixelShaderConstantCallback(4)));

        public Color MaskColor
        {
            get { return (Color)GetValue(MaskColorProp); }
            set { SetValue(MaskColorProp, value); }
        }

    }
}
