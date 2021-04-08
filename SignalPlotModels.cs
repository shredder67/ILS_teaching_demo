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

    //Base plot class, implements function A * cos(w * x), where w = 2 pi * freq
    public class SignalPlotModel
    {

        private Func<double, double> curve;

        public PlotModel Model { get; private set; }

        public double Amp { get; set; }
        public double Freq { get; set; }

        public SignalPlotModel()
        {
            //default values
            Model = new PlotModel();
            Model.Axes.Add(new LinearAxis
            {
                Position = AxisPosition.Bottom,
                Minimum = 0,
                Maximum = 0.1,
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
        }

        public SignalPlotModel(double freq, double amp, String title = "") : this()
        {
            this.Freq = freq;
            this.Amp = amp;
            Model.Title = title;

            this.curve = (x) => amp * Math.Cos((2 * Math.PI * freq) * x);
            FunctionSeries funcVals = new FunctionSeries(curve, 0, 0.1, 0.001);
            funcVals.TrackerFormatString = Model.Title;
            Model.Series.Add(funcVals);
        }

        public void Update()
        {
            this.curve = (x) => Amp * Math.Cos((2 * Math.PI * Freq) * x);
            FunctionSeries funcVals = new FunctionSeries(curve, 0, 0.1, 0.001);
            funcVals.TrackerFormatString = Model.Title;
            Model.Series[0] = funcVals;
        }

    }
}
