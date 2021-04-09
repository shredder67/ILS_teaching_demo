using System;
using System.Collections.Generic;

using OxyPlot;
using OxyPlot.Series;
using OxyPlot.Axes;


namespace MarkersDemonstration
{

    //Статический контейнер, хранящий модели всех графиков
    class SignalPlotCollection : Dictionary<string, SignalPlotModel>
    {

        public void Update()
        {
            foreach (KeyValuePair<string, SignalPlotModel> kvp in this)
            {
                kvp.Value.Update();
                kvp.Value.Model.InvalidatePlot(true);
            }
        }
    }

    //Base plot class, implements function Inner_Amp*cos(i_w*t)*[1 + Amp/Inner_Amp * cos(w*t)], where w = 2 pi * freq
    public class SignalPlotModel
    {

        private Func<double, double> curve;

        public PlotModel Model { get; private set; }

        public double Amp { get; set; }
        public double Freq { get; set; }

        private double Inner_Amp { get; set; }
        private double Inner_Freq { get; set; }

        public SignalPlotModel()
        {
            //default values
            Model = new PlotModel();
            Model.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Bottom,
                Minimum = 0,
                Maximum = 0.3,
                Title = "t"
            });
            Model.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Left,
                Minimum = -10,
                Maximum = 10,
                Title = "U",
            });

            Model.TitleFontSize = 14;
            Model.Axes[0].LabelFormatter = new Func<double, string>((v) => "");
            Model.Axes[0].TitlePosition = 1;
            Model.Axes[0].AxisTitleDistance = 0;
            Model.Axes[1].TitlePosition = 1;
            Model.Axes[1].AxisTitleDistance = 0;

            this.Inner_Amp = 8;
            this.Inner_Freq = 80;
        }

        public SignalPlotModel(double freq, double amp, String title = "") : this()
        {
            this.Freq = freq;
            this.Amp = amp;
            Model.Title = title;


            //Inner_Amp*cos(i_w*t)*[1 + Amp/Inner_Amp * cos(w*t)], where w = 2 pi * freq
            this.curve = (x) => Inner_Amp * Math.Cos(Inner_Freq * 2 * Math.PI * x) * (1 + Amp/Inner_Amp * Math.Cos((2 * Math.PI * Freq)*x));
            FunctionSeries funcVals = new FunctionSeries(curve, 0, 0.5, 0.001);
            funcVals.TrackerFormatString = Model.Title;
            Model.Series.Add(funcVals);
        }

        public void Update()
        {
            //Inner_Amp*cos(i_w*t)*[1 + Amp/Inner_Amp * cos(w*t)], where w = 2 pi * freq
            this.curve = (x) => Inner_Amp * Math.Cos(Inner_Freq * 2 * Math.PI * x) * (1 + Amp / Inner_Amp * Math.Cos((2 * Math.PI * Freq) * x));
            FunctionSeries funcVals = new FunctionSeries(curve, 0, 0.5, 0.001);
            funcVals.TrackerFormatString = Model.Title;
            Model.Series[0] = funcVals;
        }

    }
}
