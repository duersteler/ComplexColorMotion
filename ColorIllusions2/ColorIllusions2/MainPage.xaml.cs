using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Shader.Effects;

namespace ColorIllusions2
{
    public partial class MainPage : UserControl
    {
        private bool bStarted = false;
        private double centerRotationSpeed = 12; //6
        private double MaskRotationSpeed = 0;
        private double cFreq = 0.4; //0.2;
        private double MaskScalingFreq = 0.0;
        private double cSmallScale;
        private double Zoom = 100.0;
        private double bscale;
        private double MaskSize = 100.0;
        private ImageBrush brCenter;
        private Storyboard sb = null;

        private DoubleAnimation aniShear;
        private DoubleAnimation aniShearMarker;
        //private PointAnimation aniShearFlicker;

        private ColorFromMapEffect colorMapPS;
        private RealShearEffect realShearPS;
        private TextureShearingEffect textureShearPS;
        private ShearingMarkerEffect shearingMarkerPS;


        public MainPage()
        {
            InitializeComponent();
            realShearPS = new RealShearEffect();
            shearingMarkerPS = new ShearingMarkerEffect();
            textureShearPS = new TextureShearingEffect();
        }

        private void LayoutRoot_LayoutUpdated(object sender, EventArgs e)
        {
            double width = LayoutRoot.ColumnDefinitions[1].ActualWidth;
            double height = LayoutRoot.RowDefinitions[1].ActualHeight;
            if (width > 0.0 && height > 0.0)
            {
                bscale = width / 512.0;
                if (height / 512.0 < bscale)
                    bscale = height / 512.0;
            }
            IllusionPanel.Width = width;
            IllusionPanel.Height = height;
            IllusionClip.Rect = new Rect(0, 0, width, height);
            double x = 0.5 * (width - 1024.0); //center width and height not yet available
            double y = 0.5 * (height - 1024.0);

            //borderCnvMask.Margin = new Thickness(x, y, 0.0, 0.0);
            borderDepthMarker2.Margin = new Thickness(x, y, 0.0, 0.0);
            //Nabel.Width = 105.0 * bscale;
            //Nabel.Height = 105.0 * bscale;
            Nabel.Width = 20 * bscale;
            Nabel.Height = 20 * bscale;
            SolidColorBrush nableBr = new SolidColorBrush(Color.FromArgb(255,165,95, 0));
            Nabel.Fill = nableBr;

            double xe = 0.5 * (width - Nabel.Width) ;
            double ye = 0.5 * (height - Nabel.Height);
            Nabel.Margin = new Thickness(xe + 6, ye, 0.0, 0.0);
            brCenter = new ImageBrush();
            wNabel.Width = 105 * bscale;
            wNabel.Height = 105.0 * bscale;
            double xew = 0.5 * (width - wNabel.Width);
            double yew = 0.5 * (height - wNabel.Height);
            wNabel.Margin = new Thickness(xew + 6, yew, 0.0, 0.0);
            wNabel.Fill = nableBr;

            //CenterPS.Green = (double)nudWheelGreen.Value * 0.00390625;
            CenterPS.MaskScale = 100.0 / nudMaskScale.Value;
            Marker2PS.WheelZoom = MarkerPS.WheelZoom = nudZoom.Value;
            CenterPS.WheelZoom = nudZoom.Value;
            CenterPS.WheelFromColor = Color.FromArgb(255, (byte)nudWheelFromRed.Value, (byte)nudWheelFromGreen.Value, (byte)nudWheelFromBlue.Value);
            CenterPS.WheelToColor = Color.FromArgb(255, (byte)nudWheelToRed.Value, (byte)nudWheelToGreen.Value, (byte)nudWheelToBlue.Value);
            CenterPS.MaskColor = Color.FromArgb(255, (byte)nudMaskRed.Value, (byte)nudMaskGreen.Value, (byte)nudMaskBlue.Value);
            MaskRotPS.Radius = 0.25 * 100.0 / nudMaskScale.Value * nudMaskSize.Value * 0.01;
            MaskScale.ScaleX = bscale * 1.0;
            MaskScale.ScaleY = bscale * 1.0;
 
            //taken from paradigm with ring construction using 80 rings with a beta of 0.2 and a outer radius of 512
            //cSmallScale = GetSmallScale(80, 2,0.2);
            cSmallScale = 0.668; //tried out  0.6725; //measured from pictures
            //cSmallScale = 1.0 / Math.Pow(Math.E, 0.9);
            if (sb == null)
            {
                sb = new Storyboard();
                aniShear = new DoubleAnimation();
                aniShear.From = 1.0;
                aniShear.To = nudShearingFactor.Value;
                //aniShear.Duration = dur;
                aniShear.EasingFunction = new SineEase { EasingMode = EasingMode.EaseInOut };
                aniShear.AutoReverse = true;
                aniShear.RepeatBehavior = RepeatBehavior.Forever;
                sb.Children.Add(aniShear);
                Storyboard.SetTarget(aniShear, realShearPS);
                Storyboard.SetTargetProperty(aniShear, new PropertyPath("ShearFactor"));

                //aniShearFlicker = new PointAnimation();
                //aniShearFlicker.RepeatBehavior = RepeatBehavior.Forever;
                //sb.Children.Add(aniShearFlicker);
                //Storyboard.SetTarget(aniShearFlicker, realShearPS);
                //Storyboard.SetTargetProperty(aniShearFlicker, new PropertyPath("MaskTranslation"));

                aniShearMarker = new DoubleAnimation();
                aniShearMarker.From = 0.0;
                aniShearMarker.To = nudShearingFactor.Value;
                aniShearMarker.EasingFunction = new SineEase { EasingMode = EasingMode.EaseInOut };
                aniShearMarker.AutoReverse = true;
                aniShearMarker.RepeatBehavior = RepeatBehavior.Forever;
                sb.Children.Add(aniShearMarker);
                Storyboard.SetTarget(aniShearMarker, shearingMarkerPS);
                Storyboard.SetTargetProperty(aniShearMarker, new PropertyPath("ShearFactor"));

                LayoutRoot.Resources.Add("unique_id", sb);
            }
            bStarted = true;
        }

        private double GetSmallScale(int nRings, int nPeriods, double beta)
        {
            double rFac = 0.5 / Math.Exp(beta * (double)nRings);
            double secondLast = rFac * Math.Exp(beta * (double)(nRings - nPeriods));
            return 1.0 - secondLast;
        }
        //use for continuous speed only
        private TimeSpan velToTime(double vel)
        {
            double ptime;
            if (vel < 0.0) vel = -vel;
            if (vel != 0.0)
            {

                if (vel < 0.00001)
                    vel = 0.00001;
                ptime = 360.0 / vel * (double)TimeSpan.TicksPerSecond;
                return new TimeSpan((long)ptime);
            }
            else
            {
                return new TimeSpan(0L);
            }
        }
        private void velToAnimation(double vel, DoubleAnimation ani)
        {
            if (vel >= 0.0)
            {
                ani.From = 0.0;
                ani.To = 360.0;
                ani.Duration = new Duration(velToTime(vel));
            }
            else
            {
                ani.From = 360.0;
                ani.To = 0.0;
                ani.Duration = new Duration(velToTime(-vel));
            }
        }
        private TimeSpan freqToTime(double freq)
        {
            double ptime;
            if (freq < 0.0) freq = -freq;
            if (freq != 0.0)
            {

                if (freq < 0.00001)
                    freq = 0.00001;
                //use cycles /sec
                ptime = 1 / freq * (double)TimeSpan.TicksPerSecond;
                return new TimeSpan((long)ptime);
            }
            else
            {
                return new TimeSpan(0L);
            }
        }
        private void timeToAnimation(double scale, double freq, double nPeriods, double smallScale, DoubleAnimation ani)
        {
            if (bStarted)
            {
                if (freq >= 0.0)
                {
                    ani.From = scale * smallScale;
                    ani.To = scale;
                    ani.Duration = new Duration(freqToTime(freq));
                }
                else
                {
                    ani.From = scale;
                    ani.To = scale * smallScale;
                    ani.Duration = new Duration(freqToTime(-freq));
                }
            }
        }

        private void timeToAnimationMask(double freq, double smallScale, DoubleAnimation ani)
        {
            if (bStarted)
            {
                if (freq >= 0.0)
                {
                    ani.From = 100.0 * smallScale / nudMaskScale.Value;
                    ani.To = 100.0 / nudMaskScale.Value;
                    ani.Duration = new Duration(freqToTime(freq));
                }
                else
                {
                    ani.From = 100.0 / nudMaskScale.Value;
                    ani.To = 100.0 * smallScale / nudMaskScale.Value;
                    ani.Duration = new Duration(freqToTime(-freq));
                }
            }
        }
        private void nudCenterRotationSpeed_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (bStarted)
            {
                MotionGenerator.Stop();
                velToAnimation(e.NewValue, aniWheelRot);
                velToAnimation(nudMaskRotationSpeed.Value, aniMaskRot);
                if (cbMarker.IsChecked == false || cbFlicker.IsChecked == false)
                {
                    nudMarkerSpeed.Value = nudCenterSpeed.Value;
                }
                velToAnimation(nudMarkerSpeed.Value, aniMarkerRot);
                MotionGenerator.Begin();
            }
        }
        private void nudMarkerRotationSpeed_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (bStarted)
            {
                velToAnimation(nudMarkerSpeed.Value, aniMarkerRot);
            }


        }
        private void nudMaskRotationSpeed_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (bStarted)
            {
                MotionGenerator.Stop();
                velToAnimation(e.NewValue, aniMaskRot);
                velToAnimation(nudCenterSpeed.Value, aniWheelRot);
                velToAnimation(nudMarkerSpeed.Value, aniMarkerRot);
                if (TextureMap.SelectedIndex == 0 && (e.NewValue > 0.01 || e.NewValue < -0.01) && (cbFlicker.IsChecked == false))
                {
                    spMaskOffset.Visibility = System.Windows.Visibility.Visible;
                }
                else
                {
                    spMaskOffset.Visibility = System.Windows.Visibility.Collapsed;
                    nudMaskXOffset.Value = nudMaskYOffset.Value = 0.0;
                    nudMaskOffset_ValueChanged(this, null);
                }
                MotionGenerator.Begin();
            }
        }

        private void cnvImageGround_Loaded(object sender, RoutedEventArgs e)
        {
            Stimulus.SelectedIndex = 0;
            TextureMap.SelectedIndex = 0;
            LayoutRoot_LayoutUpdated(this, null);
            nudCenterSpeed.Value = centerRotationSpeed;
            velToAnimation(centerRotationSpeed, aniWheelRot);
            velToAnimation(nudCenterSpeed.Value, aniMarkerRot);
            nudMaskRotationSpeed.Value = MaskRotationSpeed;
            velToAnimation(nudMaskRotationSpeed.Value, aniMaskRot);
             nudCenterScalingFrequency.Value = cFreq;
            nudMaskScalingFrequency.Value = MaskScalingFreq;
            nudMaskSize.Value = MaskSize;
            nudZoom.Value = Zoom;
            //nudWheelGreen.Value = green;
            colorMapPS = CenterPS;
            Stimulus_SelectionChanged(this, null);

            TextureMap_SelectionChanged(this, null);
        }

        private void nudWheelColor_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (bStarted)
            {
                //CenterPS.Green = (double)e.NewValue * 0.00390625;
                CenterPS.WheelFromColor = Color.FromArgb(255, (byte)nudWheelFromRed.Value, (byte)nudWheelFromGreen.Value, (byte)nudWheelFromBlue.Value);
                CenterPS.WheelToColor = Color.FromArgb(255, (byte)nudWheelToRed.Value, (byte)nudWheelToGreen.Value, (byte)nudWheelToBlue.Value);
                if (Stimulus.SelectedIndex >= 10) //shearing?
                {
                    realShearPS.WheelColor1 = Color.FromArgb(255, (byte)nudWheelFromRed.Value, (byte)nudWheelToGreen.Value, (byte)nudWheelFromBlue.Value);
                    realShearPS.WheelColor2 = Color.FromArgb(255, (byte)nudWheelToRed.Value, (byte)nudWheelToGreen.Value, (byte)nudWheelToBlue.Value);
                    textureShearPS.WheelColor = Color.FromArgb(255, (byte)nudWheelFromRed.Value, (byte)nudWheelToGreen.Value, (byte)nudWheelFromBlue.Value);
                    Stimulus_SelectionChanged(this, null);
                }
            }
        }

        private void nudMaskScale_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (bStarted)
            {
                if (Stimulus.SelectedIndex < 10)
                {
                    CenterPS.MaskScale = 100.0 / (double)e.NewValue;
                }
                else
                    realShearPS.MaskScale = 100.0 / (double)e.NewValue;
                MaskRotPS.Radius = 0.25 * 100.0 / (double) e.NewValue * 0.01;
                double size = nudMaskSize.Value;
                nudFrequency_ValueChanged(this, null); //resets MaskSize
                nudMaskSize.Value = size;
                nudMaskOffset_ValueChanged(this, null);
            }

        }

        private void nudMaskSize_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (bStarted)
            {
                //nudMaskSize.Value = e.NewValue;
                MaskRotPS.Radius = 0.25 * 100.0 / nudMaskScale.Value * nudMaskSize.Value * 0.01;
                nudMaskOffset_ValueChanged(this, null);
            }
        }

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            nudMarkerSpeed.Value = nudCenterSpeed.Value = centerRotationSpeed;
            nudMaskRotationSpeed.Value = MaskRotationSpeed;
            nudMaskSize.Value = MaskSize;
            nudMaskXOffset.Value = 0.0;
            nudMaskYOffset.Value = 0.0;
            nudZoom.Value = Zoom;
            cbFlicker.IsChecked = false;
            cbFlicker_Click(false, null);
            cbMarker.IsChecked = false;
            cbMarker_Click(false, null);
            cnvImageGround_Loaded(this, null);
        }

        private void Stimulus_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (bStarted)
                sb.Stop();
            brCenter = new ImageBrush();
            borderDepthMarker.Effect = MarkerPS;
            MarkerPS.OuterEnd = 0.12;
            MarkerPS.InnerEnd = 0.1;
            Marker2PS.Threshold = MarkerPS.Threshold = 1.1;
            CenterPS.WheelFromColor = Color.FromArgb(255, (byte)nudWheelFromRed.Value, (byte)nudWheelFromGreen.Value, (byte)nudWheelFromBlue.Value);
            CenterPS.WheelToColor = Color.FromArgb(255, (byte)nudWheelToRed.Value, (byte)nudWheelToGreen.Value, (byte)nudWheelToBlue.Value);
            CenterPS.MaskColor = Color.FromArgb(255, (byte)nudMaskRed.Value, (byte)nudMaskGreen.Value, (byte)nudMaskBlue.Value);
            //shearingMarkerPS.MarkerThreshold = 1.1;
            if (cbMarker.IsChecked == true) //shearing from 12
            {
                if (Stimulus.SelectedIndex < 10)
                {
                    borderDepthMarker.Effect = MarkerPS;
                    borderDepthMarker2.Effect = Marker2PS;
                    Marker2PS.Threshold = 1.1;
                    MarkerPS.Threshold = 0.4;
                }
                else
                {
                    borderDepthMarker.Effect = shearingMarkerPS;
                    //shearingMarkerPS.MarkerThreshold = 0.4;
                }
            }
            borderCnvMask.Effect = CenterPS;
            spCenterSpeed.Visibility = System.Windows.Visibility.Collapsed;
            spFrequency.Visibility = System.Windows.Visibility.Collapsed;
            spRingCB.Visibility = System.Windows.Visibility.Collapsed;
            spShearing.Visibility = System.Windows.Visibility.Collapsed;
            spMaskRotation.Visibility = System.Windows.Visibility.Visible;
            spTextureScale.Visibility = System.Windows.Visibility.Visible;
            spMaskSize.Visibility = System.Windows.Visibility.Visible;
            spMaskScaling.Visibility = System.Windows.Visibility.Visible;
            spFlicker.Visibility = System.Windows.Visibility.Visible;
            if ((cbMarker.IsChecked == true) || (cbRings.IsChecked == true))
                spMarkerSpeed.Visibility = System.Windows.Visibility.Visible;
            else
                spMarkerSpeed.Visibility = System.Windows.Visibility.Collapsed;
            CenterPS.DepthMap = brCenter;
            nudMarkerSpeed.Value = nudCenterSpeed.Value;
            nudCenterScalingFrequency.Value = 0.0;
            if (nudCenterSpeed.Value == 0.0)
                nudCenterSpeed.Value = centerRotationSpeed;


            switch (Stimulus.SelectedIndex)
            {
                case 0:
                    brCenter.ImageSource = imgSectors.Source;
                    MarkerPS.DepthMap = brCenter;
                   spCenterSpeed.Visibility = System.Windows.Visibility.Visible;
                    lbMainTitle.Text = "Color Rotation Standstill Illusion";
                    break;
                case 1:
                    brCenter.ImageSource = imgLHSpirals.Source;
                    MarkerPS.DepthMap = brCenter;
                    spCenterSpeed.Visibility = System.Windows.Visibility.Visible;
                    lbMainTitle.Text = "Color Spiraling Motion Standstill";
                    break;
                case 2:
                    brCenter.ImageSource = imgRHSpirals.Source;
                    MarkerPS.DepthMap = brCenter;
                    spCenterSpeed.Visibility = System.Windows.Visibility.Visible;
                    lbMainTitle.Text = "Color Spiraling Motion Standstill";
                    break;
                case 3:
                    brCenter.ImageSource = imgRings.Source;
                    spFrequency.Visibility = System.Windows.Visibility.Visible;
                    spRingCB.Visibility = System.Windows.Visibility.Visible;
                    CenterPS.DepthMap = brCenter;               
                    MarkerPS.DepthMap = brCenter;
                    Marker2PS.DepthMap = brCenter;
                    if (nudCenterScalingFrequency.Value == 0.0)
                        nudCenterScalingFrequency.Value = cFreq;
                    nudCenterSpeed.Value = 0.0;
                    lbMainTitle.Text = "Color Scaling Standstill Illusion";
                    MarkerPS.InnerEnd = 0.01;
                    Marker2PS.OuterEnd = 0.125;

                    if (cbMarker.IsChecked == true)
                    {
                        Marker2PS.Threshold = 0.96; 
                        MarkerPS.Threshold = 1.1;
                        spMarkerSpeed.Visibility = System.Windows.Visibility.Visible;
                    }
                    else if (cbRings.IsChecked == true)
                    {
                        Marker2PS.Threshold = 1.1;
                        MarkerPS.Threshold =0.96; //0.96
                        spMarkerSpeed.Visibility = System.Windows.Visibility.Visible;
                    }
                    break;
                case 4:
                    brCenter.ImageSource = imgWheel4.Source;
                    MarkerPS.DepthMap = brCenter;
                    spCenterSpeed.Visibility = System.Windows.Visibility.Visible;
                    lbMainTitle.Text = "Color Rotation Standstill Illusion";
                    MarkerPS.OuterEnd = 0.11;
                    MarkerPS.InnerEnd = 0.027;
                    break;
                case 5:
                    brCenter.ImageSource = imgWheel8.Source;
                    MarkerPS.DepthMap = brCenter;
                   spCenterSpeed.Visibility = System.Windows.Visibility.Visible;
                    lbMainTitle.Text = "Color Rotation Standstill Illusion";
                    MarkerPS.OuterEnd = 0.11;
                    MarkerPS.InnerEnd = 0.027;
                    break;
                case 6:
                    brCenter.ImageSource = imgWheel16.Source;
                    MarkerPS.DepthMap = brCenter;
                    spCenterSpeed.Visibility = System.Windows.Visibility.Visible;
                    lbMainTitle.Text = "Color Rotation Standstill Illusion";
                    MarkerPS.OuterEnd = 0.11;
                    MarkerPS.InnerEnd = 0.027;
                    break;
                case 7:
                    brCenter.ImageSource = imgHSinus128.Source;
                    MarkerPS.DepthMap = brCenter;
                    Nabel.Visibility = System.Windows.Visibility.Collapsed;
                    spCenterSpeed.Visibility = System.Windows.Visibility.Visible;
                    lbMainTitle.Text = "Color Rotation Standstill Illusion";
                    MarkerPS.OuterEnd = 0.11;
                    MarkerPS.InnerEnd = 0.027;
                    break;
                case 8:
                    brCenter.ImageSource = imgHSinus256.Source;
                    MarkerPS.DepthMap = brCenter;
                   //Nabel.Visibility = System.Windows.Visibility.Collapsed;
                    spCenterSpeed.Visibility = System.Windows.Visibility.Visible;
                    lbMainTitle.Text = "Color Rotation Standstill Illusion";
                    MarkerPS.OuterEnd = 0.11;
                    MarkerPS.InnerEnd = 0.027;
                    break;
                case 9:
                    brCenter.ImageSource = imgSteppedSectors.Source;
                    MarkerPS.DepthMap = brCenter;
                    spCenterSpeed.Visibility = System.Windows.Visibility.Visible;
                    lbMainTitle.Text = "Color Rotation Standstill Illusion";
                    MarkerPS.OuterEnd = 0.11;
                    MarkerPS.InnerEnd = 0.027;
                    MarkerPS.Threshold = 0.51;
                    break;

                case 10:
                    MakeShearing(imgSectors.Source);
                    break;
                case 11:
                    MakeShearing(imgRings.Source);
                    break;
                case 12:
                    MakeShearing(imgLHSpirals.Source);
                    break;
                case 13:
                    MakeShearing(imgRHSpirals.Source);
                    break;
                case 14:
                    MakeShearing(imgVSinus128.Source);
                    break;
                case 15:
                    MakeShearing(imgVSinus256.Source);
                    break;
                case 16:
                    MakeShearing(imgHSinus128.Source);
                    break;
                case 17:
                    MakeShearing(imgHSinus256.Source);
                    break;
            }

        }

        private void MakeShearing(ImageSource imageSource)
        {
            try
            {
                brCenter.ImageSource = imageSource;
                spMarkerSpeed.Visibility = Visibility.Collapsed;
                spMaskSize.Visibility = System.Windows.Visibility.Collapsed;
                spTextureScale.Visibility = System.Windows.Visibility.Visible;
                spMaskRotation.Visibility = System.Windows.Visibility.Collapsed;
                spMaskScaling.Visibility = System.Windows.Visibility.Collapsed;
                spFlicker.Visibility = System.Windows.Visibility.Visible;
                spShearing.Visibility = System.Windows.Visibility.Visible;
                spStatShearDot.Visibility = System.Windows.Visibility.Visible;
                //Nabel.Visibility = System.Windows.Visibility.Visible;
                //wNabel.Visibility = System.Windows.Visibility.Collapsed;
                if (cbStatWheel.IsChecked == false && cbStatDot.IsChecked == true)
                    lbMainTitle.Text = "Color Shearing Standstill Illusion";
                else if (cbStatWheel.IsChecked == true && cbStatDot.IsChecked == false)
                    lbMainTitle.Text = "Color Shearing Capture Illusion";
                else if (cbStatWheel.IsChecked == false && cbStatDot.IsChecked == false)
                    lbMainTitle.Text = "Common Shearing";
                else lbMainTitle.Text = "No motion";
                borderCnvMask.Effect = realShearPS;
                //MotionGenerator.Stop();
                realShearPS.DepthMap = brCenter;
                realShearPS.WheelColor1 = Color.FromArgb(255, (byte)nudWheelFromRed.Value, (byte)nudWheelFromGreen.Value, (byte)nudWheelFromBlue.Value);
                realShearPS.WheelColor2 = Color.FromArgb(255, (byte)nudWheelToRed.Value, (byte)nudWheelToGreen.Value, (byte)nudWheelToBlue.Value);
                realShearPS.MaskColor = Color.FromArgb(255, (byte)nudMaskRed.Value, (byte)nudMaskGreen.Value, (byte)nudMaskBlue.Value);
                realShearPS.ShearAngle = (double)nudShearingAngle.Value;
                realShearPS.MaskScale = CenterPS.MaskScale;
                //realShearPS.MaskTranslation = new Point(0.0, 0.0);
                if (cbStatWheel.IsChecked == true)
                    realShearPS.IsStatWheel = 1.0;
                else
                    realShearPS.IsStatWheel = 0.0;
                if (cbStatDot.IsChecked == true)
                    realShearPS.IsStatTexture = 1.0;
                else
                    realShearPS.IsStatTexture = 0.0;

                Duration dur = new Duration(freqToTime(nudShearingFreq.Value));
                aniShear.Duration = dur;

                aniShear.To = 1.0 * nudShearingFactor.Value;
                aniShear.From = 1.0 / nudShearingFactor.Value;
                aniShear.EasingFunction = new SineEase { EasingMode = EasingMode.EaseInOut };
                aniShear.RepeatBehavior = RepeatBehavior.Forever;
                Storyboard.SetTarget(aniShear, realShearPS);
                Storyboard.SetTargetProperty(aniShear, new PropertyPath("ShearFactor"));


                if (cbMarker.IsChecked == true)
                {
                    borderDepthMarker.Effect = shearingMarkerPS;
                    shearingMarkerPS.DepthMap = brCenter;
                    shearingMarkerPS.MarkerThreshold = 0.96;
                    shearingMarkerPS.ShearAngle = (double)nudShearingAngle.Value;

                    aniShearMarker.Duration = dur;
                    if (cbStatWheel.IsChecked == false)
                    {
                        aniShearMarker.To = 1.0 * nudShearingFactor.Value;
                        aniShearMarker.From = 1.0 / nudShearingFactor.Value;
                    }
                    else
                    {
                        aniShearMarker.To = 1.0;
                        aniShearMarker.From = 1.0;

                    }
                    aniShearMarker.EasingFunction = new SineEase { EasingMode = EasingMode.EaseInOut };
                    aniShearMarker.RepeatBehavior = RepeatBehavior.Forever;
                    Storyboard.SetTarget(aniShearMarker, shearingMarkerPS);
                    Storyboard.SetTargetProperty(aniShearMarker, new PropertyPath("ShearFactor"));
                }
                else
                {
                    shearingMarkerPS.MarkerThreshold = 1.1;
                    //borderDepthMarker.Effect = MarkerPS;
                }
                sb.Begin();
            }
            catch (Exception exp)
            {
                MessageBox.Show("MakeShearing Shit: "+ exp.Message);
            }

        }
        /************************************************
        private void MakeTextureShearing(ImageSource imageSource)
        {
            try
            {
                brCenter.ImageSource = imageSource;
                spMarkerSpeed.Visibility = Visibility.Collapsed;
                spMaskSize.Visibility = System.Windows.Visibility.Collapsed;
                spTextureScale.Visibility = System.Windows.Visibility.Collapsed;
                spMaskRotation.Visibility = System.Windows.Visibility.Collapsed;
                spMaskScaling.Visibility = System.Windows.Visibility.Collapsed;
                spFlicker.Visibility = System.Windows.Visibility.Collapsed;
                spShearing.Visibility = System.Windows.Visibility.Visible;
                spStatShearDot.Visibility = System.Windows.Visibility.Collapsed;
                lbMainTitle.Text = "Color Shearing Capture Illusion";
                borderCnvMask.Effect = textureShearPS;
                textureShearPS.ColorMap = brCenter;
                textureShearPS.WheelColor = Color.FromArgb(255, (byte)nudWheelFromRed.Value, (byte)nudWheelToGreen.Value, (byte)nudWheelFromBlue.Value);
                textureShearPS.MaskColor = Color.FromArgb(255, (byte)nudMaskRed.Value, (byte)nudMaskGreen.Value, (byte)nudMaskBlue.Value);
                textureShearPS.MaskScale = CenterPS.MaskScale;
                textureShearPS.ShearAngle = (double)nudShearingAngle.Value;
                Duration dur = new Duration(freqToTime(nudShearingFreq.Value));
                aniShear.Duration = dur;

                aniShear.To = 1.0 * nudShearingFactor.Value;
                aniShear.From = 1.0 / nudShearingFactor.Value;
                aniShear.EasingFunction = new SineEase { EasingMode = EasingMode.EaseInOut };
                aniShear.RepeatBehavior = RepeatBehavior.Forever;
                Storyboard.SetTarget(aniShear, textureShearPS);
                Storyboard.SetTargetProperty(aniShear, new PropertyPath("ShearFactor"));
                if (cbMarker.IsChecked == true)
                {
                    borderDepthMarker.Effect = shearingMarkerPS;
                    shearingMarkerPS.DepthMap = brCenter;
                    shearingMarkerPS.MarkerThreshold = 0.96;
                    shearingMarkerPS.ShearAngle = (double)nudShearingAngle.Value;
                    aniShearMarker.Duration = dur;
                    aniShearMarker.To = 1.0;
                    aniShearMarker.From = 1.0;
                    aniShearMarker.EasingFunction = new SineEase { EasingMode = EasingMode.EaseInOut };
                    aniShearMarker.RepeatBehavior = RepeatBehavior.Forever;
                    Storyboard.SetTarget(aniShearMarker, shearingMarkerPS);
                    Storyboard.SetTargetProperty(aniShearMarker, new PropertyPath("ShearFactor"));
                }
                else
                {
                    shearingMarkerPS.MarkerThreshold = 1.1;
                    //borderDepthMarker.Effect = MarkerPS;
                }
                sb.Begin();
            }
            catch (Exception exp)
            {
                MessageBox.Show("MakeTextureShearing Shit: " + exp.Message);
            }

        }
 ***************************************/
        private void TextureMap_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //spMaskRotation.Visibility = System.Windows.Visibility.Visible;
            imgRdDots1.Visibility = System.Windows.Visibility.Collapsed;
            imgRDRings.Visibility = System.Windows.Visibility.Collapsed;
            imgNoDots.Visibility = System.Windows.Visibility.Collapsed;
            imgHalfRdDots.Visibility = System.Windows.Visibility.Collapsed;
            imgRDSpirals.Visibility = System.Windows.Visibility.Collapsed;
            imgTiltedLines0.Visibility = System.Windows.Visibility.Collapsed;
            imgTiltedLines45.Visibility = System.Windows.Visibility.Collapsed;
            imgTiltedLines90.Visibility = System.Windows.Visibility.Collapsed;
            Nabel.Visibility = System.Windows.Visibility.Collapsed;
            wNabel.Visibility = System.Windows.Visibility.Collapsed;
            spMaskScaling.Visibility = System.Windows.Visibility.Collapsed;
            spMaskOffset.Visibility = System.Windows.Visibility.Collapsed;
            wNabel.Width = 80 * bscale;
            wNabel.Height = 80.0 * bscale;
            double xew = 0.5 * (LayoutRoot.ColumnDefinitions[1].ActualWidth - wNabel.Width);
            double yew = 0.5 * (LayoutRoot.ColumnDefinitions[1].ActualWidth - wNabel.Height);
            wNabel.Margin = new Thickness(xew, yew, 0.0, 0.0);
            Nabel.Width = 20 * bscale;
            Nabel.Height = 20.0 * bscale;
            double xen = 0.5 * (LayoutRoot.ColumnDefinitions[1].ActualWidth - Nabel.Width);
            double yen = 0.5 * (LayoutRoot.ColumnDefinitions[1].ActualWidth - Nabel.Height);
            Nabel.Margin = new Thickness(xen, yen, 0.0, 0.0);

            switch (TextureMap.SelectedIndex)
            {
                case 0:
                    {
                        if (cbFlicker.IsChecked == true)
                            cbFlicker_Click(true, null);
                        nudMarkerSpeed.Value = nudCenterSpeed.Value; 
                        nudMaskScale.Value = 100.0; // random dot size
                        spTextureScale.Visibility = System.Windows.Visibility.Visible;
                        spFlicker.Visibility = System.Windows.Visibility.Visible;
                        imgRdDots1.Visibility = System.Windows.Visibility.Visible;
                        Nabel.Visibility = System.Windows.Visibility.Visible; 
                        nudMaskScalingFrequency.Value = 0.0;
                        MaskRotPS.BW = 0.0; //black and white mask
                        MaskScale.ScaleX = 4;
                        MaskScale.ScaleY = 4;
                        break;
                    }
                case 1:
                    {
                        if (cbFlicker.IsChecked == true)
                        {
                            cbFlicker.IsChecked = false;
                            cbFlicker_Click(false, null);
                        }
                        //nudMaskScale.Value = 50.0; // 1 pixel minimal size
                        spTextureScale.Visibility = System.Windows.Visibility.Collapsed;
                        spFlicker.Visibility = System.Windows.Visibility.Collapsed;
                        imgRDRings.Visibility = System.Windows.Visibility.Visible;
                        Nabel.Visibility = System.Windows.Visibility.Collapsed;
                        wNabel.Visibility = System.Windows.Visibility.Visible;
                        spMaskScaling.Visibility = System.Windows.Visibility.Visible;
                        nudMaskXOffset.Value = 0.0;
                        nudMaskYOffset.Value = 0.0;
                        MaskRotPS.BW = 0.0; //black and white mask
                        break;
                    }
                case 2:
                    {
                        if (cbFlicker.IsChecked == true)
                        {
                            cbFlicker.IsChecked = false;
                            cbFlicker_Click(false, null);
                        }
                        spTextureScale.Visibility = System.Windows.Visibility.Visible;
                        spFlicker.Visibility = System.Windows.Visibility.Collapsed;
                        //nudMaskScale.Value = 100.0; // 1 pixel minimal size
                        imgNoDots.Visibility = System.Windows.Visibility.Visible;
                        Nabel.Visibility = System.Windows.Visibility.Visible;
                        //Nabel.Visibility = System.Windows.Visibility.Collapsed;
                        wNabel.Visibility = System.Windows.Visibility.Collapsed;
                        nudMaskScalingFrequency.Value = 0.0;
                         nudMaskXOffset.Value = 0.0;
                        nudMaskYOffset.Value = 0.0;
                        MaskRotPS.BW = 1.0; //Color mask
                        break;
                    }
                case 3:
                    {
                        if (cbFlicker.IsChecked == true)
                        {
                            cbFlicker.IsChecked = false;
                            cbFlicker_Click(false, null);
                        }
                        spTextureScale.Visibility = System.Windows.Visibility.Visible;
                        spFlicker.Visibility = System.Windows.Visibility.Collapsed;
                        //nudMaskScale.Value = 100.0; // 1 pixel minimal size
                        Nabel.Visibility = System.Windows.Visibility.Visible;
                        imgHalfRdDots.Visibility = System.Windows.Visibility.Visible;
                        nudMaskScalingFrequency.Value = 0.0;
                        nudMaskXOffset.Value = 0.0;
                        nudMaskYOffset.Value = 0.0;
                        MaskRotPS.BW = 1.0; //Color mask
                        break;
                    }
                case 4:
                    {
                        if (cbFlicker.IsChecked == true)
                        {
                            cbFlicker.IsChecked = false;
                            cbFlicker_Click(false, null);
                        }
                        spTextureScale.Visibility = System.Windows.Visibility.Visible;
                        spFlicker.Visibility = System.Windows.Visibility.Collapsed;
                        //nudMaskScale.Value = 100.0; // 1 pixel minimal size
                        imgRDSpirals.Visibility = System.Windows.Visibility.Visible;
                        nudMaskScalingFrequency.Value = 0.0;
                        nudMaskXOffset.Value = 0.0;
                        nudMaskYOffset.Value = 0.0;
                        MaskRotPS.BW = 1.0; //Color mask
                        break;
                    }
                case 5:
                    {
                        if (cbFlicker.IsChecked == true)
                            cbFlicker_Click(true, null);
                        nudMarkerSpeed.Value = nudCenterSpeed.Value; 
                        //nudMaskScale.Value = 100.0; // random dot size
                        spTextureScale.Visibility = System.Windows.Visibility.Visible;
                        spFlicker.Visibility = System.Windows.Visibility.Visible;
                        imgTiltedLines0.Visibility = System.Windows.Visibility.Visible;
                        nudMaskScalingFrequency.Value = 0.0;
                        MaskRotPS.BW = 0.0; //black and white mask
                        MaskScale.ScaleX = 4;
                        MaskScale.ScaleY = 4;
                        break;
                    }
                case 6:
                    {
                        if (cbFlicker.IsChecked == true)
                            cbFlicker_Click(true, null);
                        nudMarkerSpeed.Value = nudCenterSpeed.Value; 
                        //nudMaskScale.Value = 100.0; // random dot size
                        spTextureScale.Visibility = System.Windows.Visibility.Visible;
                        spFlicker.Visibility = System.Windows.Visibility.Visible;
                        imgTiltedLines45.Visibility = System.Windows.Visibility.Visible;
                        nudMaskScalingFrequency.Value = 0.0;
                        MaskRotPS.BW = 0.0; //black and white mask
                        MaskScale.ScaleX = 4;
                        MaskScale.ScaleY = 4;
                        break;
                    }
                case 7:
                    {
                        if (cbFlicker.IsChecked == true)
                            cbFlicker_Click(true, null);
                        nudMarkerSpeed.Value = nudCenterSpeed.Value; 
                        //nudMaskScale.Value = 100.0; // random dot size
                        spTextureScale.Visibility = System.Windows.Visibility.Visible;
                        spFlicker.Visibility = System.Windows.Visibility.Visible;
                        imgTiltedLines90.Visibility = System.Windows.Visibility.Visible;
                        nudMaskScalingFrequency.Value = 0.0;
                        MaskRotPS.BW = 0.0; //black and white mask
                        MaskScale.ScaleX = 4;
                        MaskScale.ScaleY = 4;
                        break;
                    }

            }
        }

        private void nudFrequency_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (bStarted)
            {
                MotionGenerator.Stop();
                timeToAnimation(4.0, nudCenterScalingFrequency.Value, 1.0, cSmallScale, aniWheelScale);
                timeToAnimation(4.0, nudCenterScalingFrequency.Value, 1.0, cSmallScale, aniMarkerScale);
                timeToAnimation(4.0, nudCenterScalingFrequency.Value, 1.0, cSmallScale, aniMarker2Scale);
                if (nudMaskScalingFrequency.Value > 0.01 || nudMaskScalingFrequency.Value < -0.01)
                {
                    nudMaskSize.Value = 200.0;
                    spMaskSize.Visibility = System.Windows.Visibility.Collapsed;
                }
                else
                {
                    nudMaskSize.Value = 100.0;
                    spMaskSize.Visibility = System.Windows.Visibility.Visible;
                }
                timeToAnimationMask(nudMaskScalingFrequency.Value, cSmallScale, aniMaskScale);
                MotionGenerator.Begin();
            }

        }

        private void cbFlicker_Click(object sender, RoutedEventArgs e)
        {
            if (bStarted)
            {

                if (Stimulus.SelectedIndex < 10)
                {
                    if (cbFlicker.IsChecked == true)
                    {
                        //MotionGenerator.Stop();
                        //*****************************
                        if (TextureMap.SelectedIndex != 0 && TextureMap.SelectedIndex != 1)
                        {
                            TextureMap.SelectedIndex = 1;
                            TextureMap_SelectionChanged(1, null);
                        }
                        nudMaskSize.Value = 200.0;
                        nudMaskXOffset.Value = 0.0;
                        nudMaskYOffset.Value = 0.0;
                        nudMaskRotationSpeed.Value = 971.0;
                        spMaskRotation.Visibility = System.Windows.Visibility.Collapsed;
                        if (cbMarker.IsChecked == true && Stimulus.SelectedIndex != 3)
                        {
                            spMarkerSpeed.Visibility = System.Windows.Visibility.Visible;
                            nudMarkerSpeed.Value = nudCenterSpeed.Value;
                        }
                        if (TextureMap.SelectedIndex == 1)
                        {
                            nudMaskScalingFrequency.Value = 999.0;
                            aniFlicker.Duration = new Duration(new TimeSpan(0L));
                            aniFlicker.From = new Point(0.0, 0.0);
                            aniFlicker.To = new Point(0.0, 0.0);
                        }
                        else
                        {
                            nudMaskScalingFrequency.Value = 999.0;
                            aniFlicker.From = new Point(-0.1, -0.1);
                            aniFlicker.To = new Point(0.1, 0.1); ;
                            aniFlicker.Duration = new Duration(new TimeSpan(0, 0, 0, 0, 100));
                            aniFlicker.AutoReverse = false;
                            aniFlicker.RepeatBehavior = RepeatBehavior.Forever;
                        }

                        //MotionGenerator.Begin();
                    }
                    else
                    {
                        //MotionGenerator.Stop();
                        //************************************************
                        spMarkerSpeed.Visibility = System.Windows.Visibility.Collapsed;
                        nudMarkerSpeed.Value = nudCenterSpeed.Value;
                        nudMaskSize.Value = MaskSize;
                        nudMaskRotationSpeed.Value = MaskRotationSpeed;
                        spMaskRotation.Visibility = System.Windows.Visibility.Visible;
                        nudMaskScalingFrequency.Value = MaskScalingFreq;
                        //**************************************/
                        aniFlicker.Duration = new Duration(new TimeSpan(0L));
                        nudMaskOffset_ValueChanged(this, null);
                    }
                }
                /**********************************************
               else
                {
                    if (cbFlicker.IsChecked == true)
                    {
                        if (TextureMap.SelectedIndex != 0 && TextureMap.SelectedIndex != 1)
                        {
                            TextureMap.SelectedIndex = 1;
                            TextureMap_SelectionChanged(1, null);
                        }
                        nudMaskSize.Value = 200.0;
                        nudMaskXOffset.Value = 0.0;
                        nudMaskYOffset.Value = 0.0;
                        nudMaskRotationSpeed.Value = 971.0;
                        spMaskRotation.Visibility = System.Windows.Visibility.Collapsed;
                        if (cbMarker.IsChecked == true && Stimulus.SelectedIndex != 3)
                        {
                            spMarkerSpeed.Visibility = System.Windows.Visibility.Visible;
                            nudMarkerSpeed.Value = nudCenterSpeed.Value;
                        }
                        if (TextureMap.SelectedIndex == 1)
                        {
                            nudMaskScalingFrequency.Value = 999.0;
                            aniShearFlicker.Duration = new Duration(new TimeSpan(0L));
                            aniShearFlicker.From = new Point(0.0, 0.0);
                            aniShearFlicker.To = new Point(0.0, 0.0);
                        }
                        else
                        {
                            nudMaskScalingFrequency.Value = 999.0;
                            aniShearFlicker.From = new Point(-0.1, -0.1);
                            aniShearFlicker.To = new Point(0.1, 0.1); ;
                            aniShearFlicker.Duration = new Duration(new TimeSpan(0, 0, 0, 0, 100));
                            aniShearFlicker.AutoReverse = false;
                            aniShearFlicker.RepeatBehavior = RepeatBehavior.Forever;
                        }
                        //Storyboard.SetTarget(aniShearFlicker, realShearPS);
                        //Storyboard.SetTargetProperty(aniShearFlicker, new PropertyPath("MaskTranslation"));
                        //MotionGenerator.Begin();
                    }
                    else
                    {
                        //MotionGenerator.Stop();
                        spMarkerSpeed.Visibility = System.Windows.Visibility.Collapsed;
                        nudMarkerSpeed.Value = nudCenterSpeed.Value;
                        nudMaskSize.Value = MaskSize;
                        nudMaskRotationSpeed.Value = MaskRotationSpeed;
                        spMaskRotation.Visibility = System.Windows.Visibility.Visible;
 
                        aniShearFlicker.Duration = new Duration(new TimeSpan(0L));
                        nudMaskOffset_ValueChanged(this, null);
                    }
                }
                ***************************************/

            }
        }

        
        private void cbMarker_Click(object sender, RoutedEventArgs e)
        {
            if (cbMarker.IsChecked == true)
            {
                cbRings.IsChecked = false;
                MarkerPS.Threshold = 0.99;
                
                Marker2PS.Threshold = 1.1;
                MarkerPS.OuterEnd = 0.12;
                MarkerPS.InnerEnd = 0.005;
                spMarkerSpeed.Visibility = System.Windows.Visibility.Visible;
                if (Stimulus.SelectedIndex >= 9) //Shearing?
                {
                    Stimulus_SelectionChanged(this, null);
                }

                //cbRings_Click(false, null);
                else if (Stimulus.SelectedIndex == 3)
                {
                    MarkerPS.Threshold = 1.1;
                    Marker2PS.Threshold = 0.99;
                }
                else if (Stimulus.SelectedIndex > 3)
                {
                    MarkerPS.OuterEnd = 0.11;
                    MarkerPS.InnerEnd = 0.027;
                }
                if ((cbFlicker.IsChecked == true) && (Stimulus.SelectedIndex != 3))
                {
                    spMarkerSpeed.Visibility = System.Windows.Visibility.Visible;
                    nudMarkerSpeed.Value = nudCenterSpeed.Value;
                }
            }
            else
            {
                Marker2PS.Threshold = MarkerPS.Threshold = 1.1;
                spMarkerSpeed.Visibility = System.Windows.Visibility.Collapsed;
                nudMarkerSpeed.Value = nudCenterSpeed.Value;
                if (Stimulus.SelectedIndex >= 9) //shearing
                {
                    Stimulus_SelectionChanged(this, null);
                }
           }
           Stimulus_SelectionChanged(this, null);
        }
        private void cbRings_Click(object sender, RoutedEventArgs e)
        {
            if (cbRings.IsChecked == true)
            {
                cbMarker.IsChecked = false;
                MarkerPS.InnerEnd = 0.005;
                MarkerPS.OuterEnd = 0.12;
                MarkerPS.Threshold = 0.96;
                Marker2PS.Threshold = 1.1;
            }
            else
            {
                MarkerPS.InnerEnd = 0.09;
                MarkerPS.OuterEnd = 0.12;
                MarkerPS.Threshold = 1.1;
                Marker2PS.Threshold = 1.1;
            }
            Stimulus_SelectionChanged(this, null);
        }

        private void nudMaskOffset_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (bStarted && (cbFlicker.IsChecked == false))
            {
                Point pt = new Point(nudMaskXOffset.Value * -0.005 * 100.0 / nudMaskScale.Value,
                    nudMaskYOffset.Value * 0.005 * 100.0 / nudMaskScale.Value);
                aniFlicker.From = pt;
                aniFlicker.To = pt;
            }
        }

        private void nudZoom_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (bStarted)
            {
                timeToAnimationMask(nudMaskScalingFrequency.Value, cSmallScale, aniMaskScale);
            }

        }
        private void nudShearingAmplitude_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (bStarted)
                //shearingPS.ShearAmplitude = (double)e.NewValue;
                Stimulus_SelectionChanged(this, null);

        }

        private void nudShearingAngle_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (bStarted)
                //shearingPS.ShearAngle = (double)e.NewValue;
                Stimulus_SelectionChanged(this, null);

        }


        private void nudShearingFreq_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (bStarted)
                Stimulus_SelectionChanged(this, null);
                
        }

        private void cbStatDot_Click(object sender, RoutedEventArgs e)
        {
            if (bStarted)
                Stimulus_SelectionChanged(this, null);


        }

        private void nudMaskColor_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (bStarted)
                Stimulus_SelectionChanged(this, null);

        }

        private void cbStatWheel_Click(object sender, RoutedEventArgs e)
        {
            if (bStarted)
                Stimulus_SelectionChanged(this, null);

        }
    }
}
