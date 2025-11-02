using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

using Benchmarks.Shared.Controls;

namespace Benchmarks
{
    public sealed partial class MainPage : Page
    {
        private UserControl _benchmarkDotNetControl = new BenchmarkDotNetControl();
        private UserControl _asyncUIBenchmarkControl = new AsyncUIBenchmarkControl();

        public MainPage()
        {
            this.InitializeComponent();
        }

        private void HandleCheck(object sender, RoutedEventArgs e)
        {
            var type = ((RadioButton)sender).Content;

            switch (type)
            {
                case "Async UI Benchmarks" :
                    BenchmarkControl.Content = _asyncUIBenchmarkControl;
                    break;
                default :
                    BenchmarkControl.Content = _benchmarkDotNetControl;
                    break;
            }
        }
    }
}
