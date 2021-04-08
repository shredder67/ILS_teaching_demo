using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using OxyPlot;
using OxyPlot.Wpf;

namespace MarkersDemonstration
{
    public partial class MarkersWindow : Window
    {

        Rectangle planeYX;
        Rectangle planeZX;

        double planeHeight = 15;
        double planeWidth = 35;

        //PFD Lines
        Line lVertical;
        Line lHorizontal;
        
        //Local Canvases for planes
        Canvas localYXCanvas;
        Canvas localZXCanvas;

        //Diamonds (follow PFD lines on sub-canvases)
        Polygon VDiamond;
        Polygon HDiamond;

        //Plot collection
        SignalPlotCollection SPC;

        public MarkersWindow()
        {
            //Planes for YX and ZX Canvases (local)

            planeYX = new Rectangle
            {
                Width = planeWidth,
                Height = planeHeight,
                Fill = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/Resources/top-down-icon-v2.png")))
            };
            planeYX.PreviewMouseDown += Canvas_PreviewMouseLeftButtonDown;

            planeZX = new Rectangle
            {
                Width = planeWidth,
                Height = planeHeight,
                Fill = new ImageBrush(new BitmapImage(new Uri("pack://application:,,,/Resources/side-view-icon-v2.png")))
            };
            planeZX.PreviewMouseDown += Canvas_PreviewMouseLeftButtonDown;

            //Diamonds on PFD sub-panels

            VDiamond = new Polygon
            {
                Stroke = Brushes.Violet,
                Fill = Brushes.Pink,
                HorizontalAlignment = System.Windows.HorizontalAlignment.Center
            };

            HDiamond = new Polygon
            {
                Stroke = Brushes.Violet,
                Fill = Brushes.LightPink,
                HorizontalAlignment = System.Windows.HorizontalAlignment.Center
            };

            //Lines on PFD

            lVertical = new Line();
            lHorizontal = new Line();
            lVertical.Stroke = Brushes.Violet;
            lVertical.StrokeThickness = 3;
            lHorizontal.Stroke = Brushes.Violet;
            lHorizontal.StrokeThickness = 3;

            localYXCanvas = new Canvas
            {
                Height = 100,
                Width = 100,
                Background = Brushes.OrangeRed
            };
            localYXCanvas.PreviewMouseMove += Canvas_PreviewMouseMove;
            localYXCanvas.PreviewMouseLeftButtonUp += Canvas_PreviewLeftButtonUp;

            localZXCanvas = new Canvas
            {
                Height = 100,
                Width = 100,
                Background = Brushes.Orange
            };
            localZXCanvas.PreviewMouseMove += Canvas_PreviewMouseMove;
            localZXCanvas.PreviewMouseLeftButtonUp += Canvas_PreviewLeftButtonUp;

            //Plots data
            SPC = new SignalPlotCollection();
            SPC.Add("L150", new SignalPlotModel(150, 5, "КРМ 150Hz"));
            SPC.Add("L90", new SignalPlotModel(90, 5, "КРМ 90Hz"));
            SPC.Add("GS150", new SignalPlotModel(150, 5, "ГРМ 150Hz"));
            SPC.Add("GS90", new SignalPlotModel(90, 5, "ГРМ 90Hz"));

            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            //Get the slider values (1/2*maxs by default)
            double x0 = xSlider.Value;
            double y0 = ySlider.Value;
            double z0 = zSlider.Value;

            //Draw the "plane" on ZX and YX canvases
            Canvas.SetLeft(localYXCanvas, 163);
            Canvas.SetTop(localYXCanvas, 75);
            Canvas.SetLeft(localZXCanvas, 163);
            Canvas.SetTop(localZXCanvas, 101);

            Canvas.SetLeft(planeYX, x0 - planeWidth/2);
            Canvas.SetTop(planeYX, y0 - planeHeight/2);
            Canvas.SetLeft(planeZX, x0 - planeWidth/2);
            Canvas.SetTop(planeZX, z0 - planeHeight/2);

            yxCanvas.Children.Add(localYXCanvas);
            zxCanvas.Children.Add(localZXCanvas);
            localYXCanvas.Children.Add(planeYX);
            localZXCanvas.Children.Add(planeZX);

            //Lines on PFD
            lVertical.X1 = pfdCanvas.Width / 2;
            lVertical.Y1 = 0;
            lVertical.X2 = pfdCanvas.Width / 2;
            lVertical.Y2 = pfdCanvas.Height;

            lHorizontal.X1 = 0;
            lHorizontal.Y1 = pfdCanvas.Height / 2;
            lHorizontal.X2 = pfdCanvas.Width;
            lHorizontal.Y2 = pfdCanvas.Height / 2;

            pfdCanvas.Children.Add(lVertical);
            pfdCanvas.Children.Add(lHorizontal);

            //Diamonds on PFD sub-panels

            //Vertical Line Diamond
            PointCollection points = new PointCollection();
            points.Add(new Point(VDCanvas.Width / 2, 0));
            points.Add(new Point(VDCanvas.Width / 2 + 10, VDCanvas.Height / 2));
            points.Add(new Point(VDCanvas.Width / 2, VDCanvas.Height));
            points.Add(new Point(VDCanvas.Width / 2 - 10, VDCanvas.Height / 2));

            VDiamond.Points = points;
            VDCanvas.Children.Add(VDiamond);

            //Horizontal Line Diamond
            points = new PointCollection();
            points.Add(new Point(HDCanvas.Width / 2, HDCanvas.Height / 2 - 10));
            points.Add(new Point(HDCanvas.Width, HDCanvas.Height / 2));
            points.Add(new Point(HDCanvas.Width / 2, HDCanvas.Height / 2 + 10));
            points.Add(new Point(0, HDCanvas.Height / 2));

            HDiamond.Points = points;
            HDCanvas.Children.Add(HDiamond);

            //Render the plot column
            foreach (KeyValuePair<string, SignalPlotModel> kvp in SPC)
            {
                PlotColumn.Children.Add( new PlotView {
                    Model = kvp.Value.Model,
                    Height = 150,
                    Width = PlotColumn.Width,
                    Margin = new Thickness(0, 0, 10, 0),
                });
            }
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!this.IsLoaded) return;

            String senderName = (sender as Slider).Name;

            //коэфециенты прямой для расчета смещения по Z/X
            double k = 1/Math.Sqrt(3);
            double b = 67;

            double d = 110; // коэф. сжатия параметров для pfd

            if(senderName.Equals("zSlider"))
            {
                double z1 = zSlider.Value;
                Canvas.SetTop(planeZX, z1 - planeHeight/2);

                double x1 = xSlider.Value;
                double dist = Math.Abs((z1 - planeHeight/2) + k * (x1 - planeWidth/2) - b) / Math.Sqrt(1 + k); //from 0 to 1/2 of diagonal
                double K = 1.05 + x1 / 3000; //поправочный коэф. (для корректировки отображения прямой)
                double normDist = Math.Round(dist * pfdCanvas.Height / d) * K;
                bool isLower = z1 - planeHeight / 2 >= -k * (x1 - planeWidth / 2) + b;

                double newPosition;
                if (isLower)
                {
                    //plane LOWER then signal, line is up
                    newPosition = pfdCanvas.Height / 2 - normDist;
                }
                else
                {
                    //plane Higher then signal
                    newPosition = pfdCanvas.Height / 2 + normDist;
                }

                if (newPosition <= pfdCanvas.Height - 10 && newPosition >= 10)
                {

                    lHorizontal.Y1 = lHorizontal.Y2 = newPosition;

                    //Update Diamond
                    HDiamond.Points[0] = new Point(HDCanvas.Width / 2, newPosition - 10);
                    HDiamond.Points[1] = new Point(HDCanvas.Width, newPosition);
                    HDiamond.Points[2] = new Point(HDCanvas.Width / 2, newPosition + 10);
                    HDiamond.Points[3] = new Point(0, newPosition);

                    //Update Plots (z change - GS150/GS90)
                    double deltaAmp = normDist * 5 / (pfdCanvas.Height / 2);
                    if (isLower)
                    {
                        SPC["GS150"].Amp = 5 + deltaAmp;
                        SPC["GS90"].Amp = 5 - deltaAmp;
                    } else
                    {
                        SPC["GS150"].Amp = 5 - deltaAmp;
                        SPC["GS90"].Amp = 5 + deltaAmp;
                    }
                    SPC.Update();
                }

            } else if (senderName.Equals("ySlider"))
            {
                double y1 = ySlider.Value;
                Canvas.SetTop(planeYX, y1 - planeHeight/2);

                //Update Line
                double normY = Math.Round(((y1 + planeHeight/2) * pfdCanvas.Width / 120));
                if (normY >= 10 && normY <= pfdCanvas.Width - 10)
                {
                    lVertical.X1 = lVertical.X2 = normY;

                    //Update Diamond
                    VDiamond.Points[0] = new Point(normY, 0);
                    VDiamond.Points[1] = new Point(normY + 10, VDCanvas.Height / 2);
                    VDiamond.Points[2] = new Point(normY, VDCanvas.Height);
                    VDiamond.Points[3] = new Point(normY - 10, VDCanvas.Height / 2);

                    //Update Plots (z change - L150/L90)
                    double deltaAmp = Math.Abs(normY - pfdCanvas.Width/2) * 5 / (pfdCanvas.Width / 2);
                    if (normY > pfdCanvas.Width / 2)
                    {
                        SPC["L150"].Amp = 5 - deltaAmp;
                        SPC["L90"].Amp = 5 + deltaAmp;
                    }
                    else
                    {
                        SPC["L150"].Amp = 5 + deltaAmp;
                        SPC["L90"].Amp = 5 - deltaAmp;
                    }
                    SPC.Update();
                }

            } else if (senderName.Equals("xSlider"))
            {
                double x1 = xSlider.Value;
                Canvas.SetLeft(planeYX, x1 - planeWidth/2);
                Canvas.SetLeft(planeZX, x1 - planeWidth/2);

                double z1 = zSlider.Value;
                double dist = Math.Abs((z1 - planeHeight/2) + k * (x1 - planeWidth/2) - b) / Math.Sqrt(1 + k); //from 0 to 1/2 of diagonal
                double K = 1 + x1 / 400; //поправочный коэф. (для корректировки отображения прямой)
                double normDist = Math.Round(dist * pfdCanvas.Height / d) * K;
                bool isLower = z1 - planeHeight / 2 >= -k * (x1 - planeWidth / 2) + b;

                double newPosition;
                if (z1 - planeHeight / 2 >= -k * (x1 - planeWidth / 2) + b)
                {
                    //plane LOWER then signal, line is up
                    newPosition = pfdCanvas.Height / 2 - normDist;
                }
                else
                {
                    //plane Higher then signal, line is lower
                    newPosition = pfdCanvas.Height / 2 + normDist;
                }

                if (newPosition <= pfdCanvas.Height - 10 && newPosition >= 10)
                {

                        lHorizontal.Y1 = lHorizontal.Y2 = newPosition;

                    //Update Diamond
                    HDiamond.Points[0] = new Point(HDCanvas.Width / 2, newPosition - 10);
                    HDiamond.Points[1] = new Point(HDCanvas.Width, newPosition);
                    HDiamond.Points[2] = new Point(HDCanvas.Width / 2, newPosition + 10);
                    HDiamond.Points[3] = new Point(0, newPosition);

                    //Update Plots (z change - GS150/GS90)
                    double deltaAmp = normDist * 5 / (pfdCanvas.Height / 2);
                    if (isLower)
                    {
                        SPC["GS150"].Amp = 5 + deltaAmp;
                        SPC["GS90"].Amp = 5 - deltaAmp;
                    }
                    else
                    {
                        SPC["GS150"].Amp = 5 - deltaAmp;
                        SPC["GS90"].Amp = 5 + deltaAmp;
                    }
                    SPC.Update();
                }
            }
        }

        //Drag and Drop handling
        FrameworkElement dragObj = null;
        Canvas parentCanvas = null;
        private void Canvas_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            dragObj = sender as FrameworkElement;
            parentCanvas = LogicalTreeHelper.GetParent(dragObj) as Canvas;
            parentCanvas.CaptureMouse();
        }

        private void Canvas_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            if (this.dragObj == null) return;
            Point pos = e.GetPosition(sender as IInputElement);

            //Local coordinate
            double newLeft = pos.X - dragObj.ActualWidth/2;
            double newTop = pos.Y - dragObj.ActualHeight/2;

            //newLeft inside canvas left-right-border
            if (newLeft > - planeYX.Width / 3 && newLeft < parentCanvas.ActualWidth - planeYX.Width / 3)
            {
                Canvas.SetLeft(dragObj, newLeft);
                xSlider.Value = newLeft + planeWidth/2;
            }
            else if (newLeft <= - planeYX.Width / 3)
                Canvas.SetLeft(dragObj, -planeYX.Width / 3);
            else
                Canvas.SetLeft(dragObj, parentCanvas.ActualWidth - planeYX.Width / 3);

            //newTop inside canvas top-bottom-border
            if (newTop > 0 && newTop < parentCanvas.ActualHeight - planeHeight/2)
            {
                Canvas.SetTop(dragObj, newTop);
                if (parentCanvas.Equals(localZXCanvas))
                    zSlider.Value = newTop + planeHeight/2;
                else
                    ySlider.Value = newTop + planeHeight/2;
            }
            else if (newTop <= 0)
                Canvas.SetTop(dragObj, 0);
            else
                Canvas.SetTop(dragObj, parentCanvas.ActualHeight - planeHeight/2);
        }

        private void Canvas_PreviewLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (dragObj != null)
            {
                parentCanvas.ReleaseMouseCapture();
                parentCanvas = null;
                dragObj = null;
            }
        }

    }
}
