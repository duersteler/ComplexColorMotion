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
    public class ColorFromMapEffect : ShaderEffect
    {
        private static PixelShader _pixelshader = new PixelShader()
        {
            UriSource = new Uri("/ColorIllusions2;component/ColorFromMap.ps",
                UriKind.Relative)
        };

        public ColorFromMapEffect()
        {
            PixelShader = _pixelshader;

            UpdateShaderValue(RdDotInputProp);
            UpdateShaderValue(DepthMapProp);
            UpdateShaderValue(AngleProp);
            UpdateShaderValue(MaskScaleProp);
            UpdateShaderValue(MaskColorProp);
            UpdateShaderValue(WheelScaleProp);
            UpdateShaderValue(WheelZoomProp);
            UpdateShaderValue(WheelTranslationProp);
            UpdateShaderValue(WheelFromColorProp);
            UpdateShaderValue(WheelToColorProp);
        }

        public static DependencyProperty RdDotInputProp =
            ShaderEffect.RegisterPixelShaderSamplerProperty("RdDotInput", typeof(ColorFromMapEffect), 0);

        public Brush RdDotInput
        {
            get { return ((Brush)base.GetValue(RdDotInputProp)); }
            set { base.SetValue(RdDotInputProp, value); }
        }

        public static readonly DependencyProperty DepthMapProp =
            ShaderEffect.RegisterPixelShaderSamplerProperty("ColorMap", typeof(ColorFromMapEffect), 1);

        public Brush DepthMap
        {
            get { return (Brush)GetValue(DepthMapProp); }
            set { SetValue(DepthMapProp, value); }
        }
        public static readonly DependencyProperty AngleProp =
            DependencyProperty.Register("Angle", typeof(double), typeof(ColorFromMapEffect), new PropertyMetadata(0.0, PixelShaderConstantCallback(0)));

        public double Angle
        {
            get { return (double)GetValue(AngleProp); }
            set { SetValue(AngleProp, value); }
        }

        public static readonly DependencyProperty MaskScaleProp =
            DependencyProperty.Register("MaskScale", typeof(double), typeof(ColorFromMapEffect), new PropertyMetadata(0.0, PixelShaderConstantCallback(1)));

        public double MaskScale
        {
            get { return (double)GetValue(MaskScaleProp); }
            set {  SetValue(MaskScaleProp, value); }
        }

        public static readonly DependencyProperty MaskColorProp =
           DependencyProperty.Register("MaskColor", typeof(Color), typeof(ColorFromMapEffect),
               new PropertyMetadata(new Color(), PixelShaderConstantCallback(2)));

        public Color MaskColor
        {
            get { return (Color)GetValue(MaskColorProp); }
            set { SetValue(MaskColorProp, value); }
        }
        
        public static readonly DependencyProperty WheelScaleProp =
           DependencyProperty.Register("WheelScale", typeof(double), typeof(ColorFromMapEffect), new PropertyMetadata(0.0, PixelShaderConstantCallback(3)));

        public double WheelScale
        {
            get { return (double)GetValue(WheelScaleProp); }
            set { SetValue(WheelScaleProp, value); }
        }

        public static readonly DependencyProperty WheelZoomProp =
          DependencyProperty.Register("WheelZoom", typeof(double), typeof(ColorFromMapEffect), new PropertyMetadata(0.0, PixelShaderConstantCallback(4)));

        public double WheelZoom
        {
            get { return (double)GetValue(WheelZoomProp); }
            set { SetValue(WheelZoomProp, value); }
        }

        public static readonly DependencyProperty WheelTranslationProp =
           DependencyProperty.Register("WheelTranslation", typeof(Point), typeof(ColorFromMapEffect), new PropertyMetadata(new Point(0.0, 0.0), PixelShaderConstantCallback(5)));

        public Point WheelTranslation
        {
            get { return (Point)GetValue(WheelTranslationProp); }
            set { SetValue(WheelTranslationProp, value); }
        }
         
        public static readonly DependencyProperty WheelFromColorProp =
             DependencyProperty.Register("WheelColor1", typeof(Color), typeof(ColorFromMapEffect),
                 new PropertyMetadata(new Color(), PixelShaderConstantCallback(6)));

        public Color WheelFromColor
        {
            get { return (Color)GetValue(WheelFromColorProp); }
            set { SetValue(WheelFromColorProp, value); }
        }

        public static readonly DependencyProperty WheelToColorProp =
             DependencyProperty.Register("WheelColor2", typeof(Color), typeof(ColorFromMapEffect),
                 new PropertyMetadata(new Color(), PixelShaderConstantCallback(7)));


        public Color WheelToColor
        {
            get { return (Color)GetValue(WheelToColorProp); }
            set { SetValue(WheelToColorProp, value); }
        }


    }
}
