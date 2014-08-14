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
    public class MakeRotationEffect : ShaderEffect
    {
        private static PixelShader _pixelshader = new PixelShader()
        {
            UriSource = new Uri("/ColorIllusions2;component/MakeRotation.ps",
                UriKind.Relative)
        };

        public MakeRotationEffect()
        {
            PixelShader = _pixelshader;
            UpdateShaderValue(AngleProp);
            UpdateShaderValue(RadiusProp);

        }
        public static DependencyProperty ImageInputProp =
            ShaderEffect.RegisterPixelShaderSamplerProperty("ImageInput", typeof(MakeRotationEffect), 0);

        public Brush ImageInput
        {
            get
            {
                return ((Brush)base.GetValue(ImageInputProp));
            }
            set
            {
                base.SetValue(ImageInputProp, value);
            }
        }

        public static readonly DependencyProperty AngleProp =
            DependencyProperty.Register("Angle", typeof(double), typeof(MakeRotationEffect), new PropertyMetadata(0.0, PixelShaderConstantCallback(0)));

        public double Angle
        {
            get { return (double)GetValue(AngleProp); }
            set { SetValue(AngleProp, value); }
        }


        public static readonly DependencyProperty RadiusProp =
           DependencyProperty.Register("Radius", typeof(double), typeof(MakeRotationEffect), new PropertyMetadata(0.0, PixelShaderConstantCallback(1)));

        public double Radius
        {
            get { return (double)GetValue(RadiusProp); }
            set { SetValue(RadiusProp, value); }
        }

        public static readonly DependencyProperty BWProp =
           DependencyProperty.Register("BW", typeof(double), typeof(MakeRotationEffect), new PropertyMetadata(0.0, PixelShaderConstantCallback(2)));

        public double BW
        {
            get { return (double)GetValue(BWProp); }
            set { SetValue(BWProp, value); }
        }

        public static readonly DependencyProperty ScaleProp =
           DependencyProperty.Register("Scale", typeof(double), typeof(MakeRotationEffect), new PropertyMetadata(0.0, PixelShaderConstantCallback(3)));

        public double Scale
        {
            get { return (double)GetValue(ScaleProp); }
            set { SetValue(ScaleProp, value); }
        }
    }
}
