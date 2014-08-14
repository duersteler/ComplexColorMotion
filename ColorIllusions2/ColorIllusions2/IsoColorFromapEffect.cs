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
    public class IsoColorFromMapEffect : ShaderEffect
    {
        private static PixelShader _pixelshader = new PixelShader()
        {
            UriSource = new Uri("/ColorIllusions2;component/IsoColorFromMap.ps",
                UriKind.Relative)
        };

        public IsoColorFromMapEffect()
        {
            PixelShader = _pixelshader;

            UpdateShaderValue(RdDotInputProp);
            UpdateShaderValue(DepthMapProp);
            UpdateShaderValue(AngleProp);
            UpdateShaderValue(MaskScaleProp);
            UpdateShaderValue(WheelScaleProp);
            UpdateShaderValue(WheelZoomProp);
            UpdateShaderValue(GreenProp);
            UpdateShaderValue(WheelTranslationProp);
        }

        public static DependencyProperty RdDotInputProp =
            ShaderEffect.RegisterPixelShaderSamplerProperty("RdDotInput", typeof(IsoColorFromMapEffect), 0);

        public Brush RdDotInput
        {
            get { return ((Brush)base.GetValue(RdDotInputProp)); }
            set { base.SetValue(RdDotInputProp, value); }
        }

        public static readonly DependencyProperty DepthMapProp =
            ShaderEffect.RegisterPixelShaderSamplerProperty("ColorMap", typeof(IsoColorFromMapEffect), 1);

        public Brush DepthMap
        {
            get { return (Brush)GetValue(DepthMapProp); }
            set { SetValue(DepthMapProp, value); }
        }
        public static readonly DependencyProperty AngleProp =
            DependencyProperty.Register("Angle", typeof(double), typeof(IsoColorFromMapEffect), new PropertyMetadata(0.0, PixelShaderConstantCallback(0)));

        public double Angle
        {
            get { return (double)GetValue(AngleProp); }
            set { SetValue(AngleProp, value); }
        }

        public static readonly DependencyProperty MaskScaleProp =
            DependencyProperty.Register("MaskScale", typeof(double), typeof(IsoColorFromMapEffect), new PropertyMetadata(0.0, PixelShaderConstantCallback(1)));

        public double MaskScale
        {
            get { return (double)GetValue(MaskScaleProp); }
            set {  SetValue(MaskScaleProp, value); }
        }

        public static readonly DependencyProperty WheelScaleProp =
           DependencyProperty.Register("WheelScale", typeof(double), typeof(IsoColorFromMapEffect), new PropertyMetadata(0.0, PixelShaderConstantCallback(2)));

        public double WheelScale
        {
            get { return (double)GetValue(WheelScaleProp); }
            set { SetValue(WheelScaleProp, value); }
        }

        public static readonly DependencyProperty WheelZoomProp =
          DependencyProperty.Register("WheelZoom", typeof(double), typeof(IsoColorFromMapEffect), new PropertyMetadata(0.0, PixelShaderConstantCallback(3)));

        public double WheelZoom
        {
            get { return (double)GetValue(WheelZoomProp); }
            set { SetValue(WheelZoomProp, value); }
        }

        public static readonly DependencyProperty GreenProp =
            DependencyProperty.Register("Green", typeof(double), typeof(IsoColorFromMapEffect), new PropertyMetadata(0.0, PixelShaderConstantCallback(4)));

        public double Green
        {
            get { return (double)GetValue(GreenProp); }
            set { SetValue(GreenProp, value); }
        }

        public static readonly DependencyProperty WheelTranslationProp =
            DependencyProperty.Register("WheelTranslation", typeof(Point), typeof(IsoColorFromMapEffect), new PropertyMetadata(new Point(0.0, 0.0), PixelShaderConstantCallback(5)));

        public Point WheelTranslation
        {
            get { return (Point)GetValue(WheelTranslationProp); }
            set { SetValue(WheelTranslationProp, value); }
        }



    }
}
