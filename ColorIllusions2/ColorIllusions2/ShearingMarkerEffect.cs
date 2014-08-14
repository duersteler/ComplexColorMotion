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
    public class ShearingMarkerEffect : ShaderEffect
    {
       private static PixelShader _pixelshader = new PixelShader()
        {
            UriSource = new Uri("/ColorIllusions2;component/ShearingMarker.ps",
                UriKind.Relative)
        };

        public ShearingMarkerEffect()
        {
            PixelShader = _pixelshader;

            UpdateShaderValue(DepthMapProp);
            UpdateShaderValue(MarkerThresholdProp);
            UpdateShaderValue(ShearAngleProp);
            UpdateShaderValue(ShearFactorProp);

        }

        public static DependencyProperty RdDotInputProp =
            ShaderEffect.RegisterPixelShaderSamplerProperty("RdDotInput", typeof(ShearingMarkerEffect), 0);


        
        public Brush RdDotInput
        {
            get { return ((Brush)base.GetValue(RdDotInputProp)); }
            set { base.SetValue(RdDotInputProp, value); }
        }

        public static readonly DependencyProperty DepthMapProp =
            ShaderEffect.RegisterPixelShaderSamplerProperty("ColorMap", typeof(ShearingMarkerEffect), 1);

        public Brush DepthMap
        {
            get { return (Brush)GetValue(DepthMapProp); }
            set { SetValue(DepthMapProp, value); }
        }

        public static readonly DependencyProperty MarkerThresholdProp =
            DependencyProperty.Register("MarkerThreshold", typeof(double), typeof(ShearingMarkerEffect), new PropertyMetadata(0.0, PixelShaderConstantCallback(0)));

        public double MarkerThreshold
        {
            get { return (double)GetValue(MarkerThresholdProp); }
            set { SetValue(MarkerThresholdProp, value); }
        }


        public static readonly DependencyProperty ShearAngleProp =
            DependencyProperty.Register("ShearAngle", typeof(double), typeof(ShearingMarkerEffect), new PropertyMetadata(0.0, PixelShaderConstantCallback(1)));

        public double ShearAngle
        {
            get { return (double)GetValue(ShearAngleProp); }
            set { SetValue(ShearAngleProp, value); }
        }

        public static readonly DependencyProperty ShearFactorProp =
           DependencyProperty.Register("ShearFactor", typeof(double), typeof(ShearingMarkerEffect), new PropertyMetadata(0.0, PixelShaderConstantCallback(2)));

        public double ShearFactor
        {
            get { return (double)GetValue(ShearFactorProp); }
            set { SetValue(ShearFactorProp, value); }
        }

    }
}
