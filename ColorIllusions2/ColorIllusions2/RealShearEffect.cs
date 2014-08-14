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

namespace Shader.Effects
{
    public class RealShearEffect: ShaderEffect
    {
        private static PixelShader _pixelshader = new PixelShader()
        {
            UriSource = new Uri("/ColorIllusions2;component/RealShear.ps",
                UriKind.Relative)
        };

        public RealShearEffect()
        {
            PixelShader = _pixelshader;

            UpdateShaderValue(RdDotInputProp);
            UpdateShaderValue(DepthMapProp);
            UpdateShaderValue(ScaleProp);
            UpdateShaderValue(ShearAngleProp);
            UpdateShaderValue(ShearFactorProp);
            UpdateShaderValue(IsStatTextureProp);
            UpdateShaderValue(MaskColorProp);
            //UpdateShaderValue(MaskTranslationProp);
            UpdateShaderValue(IsStatWheelProp);
            UpdateShaderValue(WheelColorProp1);
            UpdateShaderValue(WheelColorProp2);
        }

        public static DependencyProperty RdDotInputProp =
            ShaderEffect.RegisterPixelShaderSamplerProperty("RdDotInput", typeof(RealShearEffect), 0);

        public Brush RdDotInput
        {
            get { return ((Brush)base.GetValue(RdDotInputProp)); }
            set { base.SetValue(RdDotInputProp, value); }
        }

        public static readonly DependencyProperty DepthMapProp =
            ShaderEffect.RegisterPixelShaderSamplerProperty("ColorMap", typeof(RealShearEffect), 1);

        public Brush DepthMap
        {
            get { return (Brush)GetValue(DepthMapProp); }
            set { SetValue(DepthMapProp, value); }
        }

        public static readonly DependencyProperty ScaleProp =
            DependencyProperty.Register("MaskScale", typeof(double), typeof(RealShearEffect), new PropertyMetadata(0.0, PixelShaderConstantCallback(0)));

        public double MaskScale
        {
            get { return (double)GetValue(ScaleProp); }
            set { SetValue(ScaleProp, value); }
        }
        public static readonly DependencyProperty ShearAngleProp =
            DependencyProperty.Register("ShearAngle", typeof(double), typeof(RealShearEffect), new PropertyMetadata(0.0, PixelShaderConstantCallback(1)));

        public double ShearAngle
        {
            get { return (double)GetValue(ShearAngleProp); }
            set { SetValue(ShearAngleProp, value); }
        }

        public static readonly DependencyProperty ShearFactorProp =
            DependencyProperty.Register("ShearFactor", typeof(double), typeof(RealShearEffect), new PropertyMetadata(0.0, PixelShaderConstantCallback(2)));

        public double ShearFactor
        {
            get { return (double)GetValue(ShearFactorProp); }
            set {  SetValue(ShearFactorProp, value); }
        }

        public static readonly DependencyProperty IsStatTextureProp =
           DependencyProperty.Register("IsStatTexture", typeof(double), typeof(RealShearEffect), new PropertyMetadata(0.0, PixelShaderConstantCallback(3)));

        public double IsStatTexture
        {
            get { return (double)GetValue(IsStatTextureProp); }
            set { SetValue(IsStatTextureProp, value); }
        }

        public static readonly DependencyProperty MaskColorProp =
           DependencyProperty.Register("MaskColor", typeof(Color), typeof(RealShearEffect),
               new PropertyMetadata(new Color(), PixelShaderConstantCallback(4)));

        public Color MaskColor
        {
            get { return (Color)GetValue(MaskColorProp); }
            set { SetValue(MaskColorProp, value); }
        }

        public static readonly DependencyProperty MaskTranslationProp =
          DependencyProperty.Register("MaskTranslation", typeof(Point), typeof(ColorFromMapEffect), new PropertyMetadata(new Point(0.0, 0.0), PixelShaderConstantCallback(5)));

        public Point MaskTranslation
        {
            get { return (Point)GetValue(MaskTranslationProp); }
            set { SetValue(MaskTranslationProp, value); }
        }

        public static readonly DependencyProperty IsStatWheelProp =
           DependencyProperty.Register("IsStatWheel", typeof(double), typeof(RealShearEffect), new PropertyMetadata(0.0, PixelShaderConstantCallback(6)));

        public double IsStatWheel
        {
            get { return (double)GetValue(IsStatWheelProp); }
            set { SetValue(IsStatWheelProp, value); }
        }

         public static readonly DependencyProperty WheelColorProp1 =
          DependencyProperty.Register("WheelColor1", typeof(Color), typeof(RealShearEffect),
              new PropertyMetadata(new Color(), PixelShaderConstantCallback(7)));

        public Color WheelColor1
        {
            get { return (Color)GetValue(WheelColorProp1); }
            set { SetValue(WheelColorProp1, value); }
        }
        public static readonly DependencyProperty WheelColorProp2 =
         DependencyProperty.Register("WheelColor2", typeof(Color), typeof(RealShearEffect),
             new PropertyMetadata(new Color(), PixelShaderConstantCallback(8)));

        public Color WheelColor2
        {
            get { return (Color)GetValue(WheelColorProp2); }
            set { SetValue(WheelColorProp2, value); }
        }


    }
}
