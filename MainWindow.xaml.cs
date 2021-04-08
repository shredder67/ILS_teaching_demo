using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MarkersDemonstration
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Markers_Button_Click(object sender, RoutedEventArgs e)
        {
            var MarkWindow = new MarkersWindow();
            MarkWindow.Closed += (s, eArgs) => this.MarkerButton.IsEnabled = true;
            MarkWindow.Show();
            this.MarkerButton.IsEnabled = false;
        }

        private void TheoryButton_Click(object sender, RoutedEventArgs e)
        {
            WebBrowser pdfViewer = new WebBrowser();
            Uri uri = new Uri("pack://siteoforigin:,,,/Resources/ILS.pdf");
            pdfViewer.Navigate(uri);
        }

        private void DemoButton_Click(object sender, RoutedEventArgs e)
        {
            var DemoWindow = new LandingDemoWindow();
            DemoWindow.Closed += (s, eArgs) => this.DemoButton.IsEnabled = true;
            DemoWindow.Show();
            this.DemoButton.IsEnabled = false;
        }
    }
}
