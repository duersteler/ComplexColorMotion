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
    public class MakeDepthMarker2Effect : ShaderEffect
    {
        private static PixelShader _pixelshader = new PixelShader()
        {
            UriSource = new Uri("/ColorIllusions2;component/MakeDepthMarker2.ps",
                UriKind.Relative)
        };

        public MakeDepthMarker2Effect()
        {
            PixelShader = _pixelshader;
            UpdateShaderValue(RdDotInputProp);
            UpdateShaderValue(DepthMapProp);
            UpdateShaderValue(ThresholdProp);
            UpdateShaderValue(WheelScaleProp);
            UpdateShaderValue(WheelZoomProp);
            UpdateShaderValue(OuterEndProp);

        }
        public static readonly DependencyProperty ThresholdProp =
            DependencyProperty.Register("Threshold", typeof(double), typeof(MakeDepthMarker2Effect), new PropertyMetadata(0.0, PixelShaderConstantCallback(0)));


        public double Threshold
        {
            get { return (double)GetValue(ThresholdProp); }
            set { SetValue(ThresholdProp, value); }
        }

        public static DependencyProperty RdDotInputProp =
            ShaderEffect.RegisterPixelShaderSamplerProperty("RdDotInput", typeof(MakeDepthMarker2Effect), 0);

        public Brush RdDotInput
        {
            get { return ((Brush)base.GetValue(RdDotInputProp)); }
            set { base.SetValue(RdDotInputProp, value); }
        }

        public static readonly DependencyProperty DepthMapProp =
            ShaderEffect.RegisterPixelShaderSamplerProperty("ColorMap", typeof(MakeDepthMarker2Effect), 1);

        public Brush DepthMap
        {
            get { return (Brush)GetValue(DepthMapProp); }
            set { SetValue(DepthMapProp, value); }
        }


        public static readonly DependencyProperty WheelScaleProp =
           DependencyProperty.Register("WheelScale", typeof(double), typeof(MakeDepthMarker2Effect), new PropertyMetadata(0.0, PixelShaderConstantCallback(1)));

        public double WheelScale
        {
            get { return (double)GetValue(WheelScaleProp); }
            set { SetValue(WheelScaleProp, value); }
        }

        public static readonly DependencyProperty WheelZoomProp =
          DependencyProperty.Register("WheelZoom", typeof(double), typeof(MakeDepthMarker2Effect), new PropertyMetadata(0.0, PixelShaderConstantCallback(2)));

        public double WheelZoom
        {
            get { return (double)GetValue(WheelZoomProp); }
            set { SetValue(WheelZoomProp, value); }
        }


        public static readonly DependencyProperty OuterEndProp =
          DependencyProperty.Register("OuterEnd", typeof(double), typeof(MakeDepthMarker2Effect), new PropertyMetadata(0.0, PixelShaderConstantCallback(3)));

        public double OuterEnd
        {
            get { return (double)GetValue(OuterEndProp); }
            set { SetValue(OuterEndProp, value); }
        }


    }
}
