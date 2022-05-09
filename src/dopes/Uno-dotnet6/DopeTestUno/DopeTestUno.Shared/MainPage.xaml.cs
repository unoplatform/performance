using Azure;
using Azure.Storage.Blobs;
using Newtonsoft.Json;
using Saplin.xOPS.UI.Misc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Xamarin.Essentials;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Uno.Foundation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace DopeTestUno
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class MainPage : Page
	{
		public MainPage()
		{
			this.InitializeComponent();
		}

        volatile bool breakTest = false;
        const int max = 600;

        public static Brush ConvertColor(Color c) => new SolidColorBrush(c);

        //void StartTestMT()
        //{
        //    var rand = new Random2(0);

        //    breakTest = false;

        //    var width = absolute.ActualWidth;
        //    var height = absolute.ActualHeight;

        //    var i = 0;
        //    var processed = 0;

        //    async void Loop()
        //    {
        //        while (true)
        //        {
        //            if (processed < i - 20)
        //            {
        //                Thread.Sleep(1);
        //                continue;
        //            }

        //            var label = new TextBlock()
        //            {
        //                Text = "Dope",
        //                Foreground = new SolidColorBrush(Color.FromArgb(1, (byte)(rand.NextDouble()*255), (byte)(rand.NextDouble() * 255), (byte)(rand.NextDouble() * 255)))
        //            };

        //            label.RenderTransform = new RotateTransform() { Angle = rand.NextDouble() * 360 };

        //            Canvas.SetLeft(label, rand.NextDouble());
        //            Canvas.SetTop(label, rand.NextDouble());

        //            if (i > max)
        //            {
        //                absolute.Children.RemoveAt(0);
        //            }

        //            absolute.Children.Add(label);

        //            await Task.Yield();

        //            if (breakTest) break;

        //            i++;
        //        }
        //    });

        //    Loop();

        //    var sw = new Stopwatch();
        //    sw.Start();
        //    long prevTicks = 0;
        //    int prevProcessed = 0;
        //    double avgSum = 0;
        //    int avgN = 0;

        //    var timer = new DispatcherTimer();
        //    timer.Interval = TimeSpan.FromMilliseconds(500);
        //    timer.Tick += (s, e) =>
        //    {
        //        if (stop.Visibility == Visiblity.Visible)
        //        {
        //            var avg = avgSum / avgN;
        //            dopes.Text = string.Format("{0:0.00} Dopes/s (AVG)", avg).PadLeft(21);
        //            return false;
        //        }

        //        var r = (double)(processed - prevProcessed) / ((double)(sw.ElapsedTicks - prevTicks) / Stopwatch.Frequency);
        //        dopes.Text = string.Format("{0:0.00} Dopes/s", r).PadLeft(15);
        //        prevTicks = sw.ElapsedTicks;
        //        prevProcessed = processed;

        //        if (i > max)
        //        {
        //            avgSum += r;
        //            avgN++;
        //        }

        //        return true;
        //    });
        //}

        //   void StartTestMT2()
        //   {
        //       var rand = new Random2(0);

        //       breakTest = false;

        //       var width = absolute.ActualWidth;
        //       var height = absolute.ActualHeight;

        //       const int step = 75;

        //       var processed = 0;

        //       long prevTicks = 0;
        //       long prevMs = 0;
        //       int prevProcessed = 0;
        //       double avgSum = 0;
        //       int avgN = 0;
        //       var sw = new Stopwatch();

        //       var bankA = new TextBlock[step];
        //       var bankB = new TextBlock[step];

        //       Action<TextBlock[]> addLabels = (TextBlock[] labels) =>
        //       {
        //           for (int k = 0; k < step; k++)
        //           {
        //var label = new TextBlock()
        //{
        //	Text = "Dope",
        //	Foreground = new SolidColorBrush(Color.FromArgb(1, (byte)(rand.NextDouble() * 255), (byte)(rand.NextDouble() * 255), (byte)(rand.NextDouble() * 255)))
        //};

        //label.RenderTransform = new RotateTransform() { Angle = rand.NextDouble() * 360 };

        //Canvas.SetLeft(label, rand.NextDouble());
        //Canvas.SetTop(label, rand.NextDouble());

        //               labels[k] = label;
        //           }
        //       };

        //       addLabels(bankA);
        //       addLabels(bankB);

        //       var bank = bankA;

        //       Action loop = null;

        //       var i = 0;
        //       Task task = null;

        //       loop = () =>
        //       {
        //           if (breakTest)
        //           {
        //               var avg = avgSum / avgN;
        //               dopes.Text = string.Format("{0:0.00} Dopes/s (AVG)", avg).PadLeft(21);
        //               return;
        //           }

        //           if (processed > max)
        //           {
        //               absolute.Children.RemoveAt(0);
        //           }

        //           absolute.Children.Add(bank[i]);
        //           i++;

        //           if (i == step)
        //           {
        //               if (task != null && task.Status != TaskStatus.RanToCompletion) task.Wait();
        //               task = Task.Run(() => addLabels(bank));
        //               if (bank == bankA) bank = bankB; else bank = bankA;
        //               i = 0;
        //           }

        //           processed++;

        //           if (sw.ElapsedMilliseconds - prevMs > 500)
        //           {

        //               var r = (double)(processed - prevProcessed) / ((double)(sw.ElapsedTicks - prevTicks) / Stopwatch.Frequency);
        //               prevTicks = sw.ElapsedTicks;
        //               prevProcessed = processed;

        //               if (processed > max)
        //               {
        //                   dopes.Text = string.Format("{0:0.00} Dopes/s", r).PadLeft(15);
        //                   avgSum += r;
        //                   avgN++;
        //               }

        //               prevMs = sw.ElapsedMilliseconds;
        //           }

        //           _ = Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.High, () => loop());
        //       };

        //       sw.Start();


        //       Device.BeginInvokeOnMainThread(loop);
        //   }

        void StartTestST()
        {
            var rand = new Random2(0);

            breakTest = false;

            var width = absolute.ActualWidth;
            var height = absolute.ActualHeight;

            const int step = 20;
            var labels = new TextBlock[step * 2];

            var processed = 0;

            long prevTicks = 0;
            long prevMs = 0;
            int prevProcessed = 0;
            double avgSum = 0;
            int avgN = 0;
            var sw = new Stopwatch();

            Action loop = null;

            loop = () =>
            {
                var now = sw.ElapsedMilliseconds;

                if (breakTest)
                {
                    var avg = avgSum / avgN;
                    dopes.Text = string.Format("{0:0.00} Dopes/s (AVG)", avg).PadLeft(21);
                    return;
                }

                //60hz, 16ms to build the frame
                while (sw.ElapsedMilliseconds - now < 16 && !breakTest)
                {
					var label = new TextBlock()
					{
						Text = "Dope",
						Foreground = new SolidColorBrush(Color.FromArgb(0xFF, (byte)(rand.NextDouble() * 255), (byte)(rand.NextDouble() * 255), (byte)(rand.NextDouble() * 255)))
					};

					label.RenderTransform = new RotateTransform() { Angle = rand.NextDouble() * 360 };

					Canvas.SetLeft(label, rand.NextDouble() * width);
					Canvas.SetTop(label, rand.NextDouble() * height);

					if (processed > max)
                    {
                        absolute.Children.RemoveAt(0);
                    }

                    absolute.Children.Add(label);

                    processed++;

                    if (sw.ElapsedMilliseconds - prevMs > 500)
                    {

                        var r = (double)(processed - prevProcessed) / ((double)(sw.ElapsedTicks - prevTicks) / Stopwatch.Frequency);
                        prevTicks = sw.ElapsedTicks;
                        prevProcessed = processed;

                        if (processed > max)
                        {
                            dopes.Text = string.Format("{0:0.00} Dopes/s", r).PadLeft(15);
                            avgSum += r;
                            avgN++;
                        }

                        prevMs = sw.ElapsedMilliseconds;
                    }
                }

                _ = Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => loop());
            };

            sw.Start();

            _ = Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () => loop());
        }

        void StartTestReuseST()
        {
            var rand = new Random2(0);

            breakTest = false;

            var width = absolute.ActualWidth;
            var height = absolute.ActualHeight;

            const int step = 20;
            var labels = new TextBlock[step * 2];

            var processed = 0;

            long prevTicks = 0;
            long prevMs = 0;
            int prevProcessed = 0;
            double avgSum = 0;
            int avgN = 0;
            var sw = new Stopwatch();

            Action loop = null;

            Stack<TextBlock> _cache = new Stack<TextBlock>();

            loop = () =>
            {
                var now = sw.ElapsedMilliseconds;

                if (breakTest)
                {
                    var avg = avgSum / avgN;
                    dopes.Text = string.Format("{0:0.00} Dopes/s (AVG)", avg).PadLeft(21);
                    return;
                }

                //60hz, 16ms to build the frame
                while (sw.ElapsedMilliseconds - now < 16 && !breakTest)
                {
                    var label = _cache.Count == 0 ? new TextBlock() { Foreground = new SolidColorBrush() } : _cache.Pop();

                    label.Text = "Dope";
                    (label.Foreground as SolidColorBrush).Color = Color.FromArgb(0xFF, (byte)(rand.NextDouble() * 255), (byte)(rand.NextDouble() * 255), (byte)(rand.NextDouble() * 255));

                    label.RenderTransform = new RotateTransform() { Angle = rand.NextDouble() * 360 };

                    Canvas.SetLeft(label, rand.NextDouble() * width);
                    Canvas.SetTop(label, rand.NextDouble() * height);

                    if (processed > max)
                    {
                        _cache.Push(absolute.Children[0] as TextBlock);
                        absolute.Children.RemoveAt(0);
                    }

                    absolute.Children.Add(label);

                    processed++;

                    if (sw.ElapsedMilliseconds - prevMs > 500)
                    {

                        var r = (double)(processed - prevProcessed) / ((double)(sw.ElapsedTicks - prevTicks) / Stopwatch.Frequency);
                        prevTicks = sw.ElapsedTicks;
                        prevProcessed = processed;

                        if (processed > max)
                        {
                            dopes.Text = string.Format("{0:0.00} Dopes/s", r).PadLeft(15);
                            avgSum += r;
                            avgN++;
                        }

                        prevMs = sw.ElapsedMilliseconds;
                    }
                }

                _ = Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Low, () => loop());
            };

            sw.Start();

            _ = Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Low, () => loop());
        }

        //void StartTestGridST()
        //{
        //    var rand = new Random2(0);

        //    breakTest = false;

        //    var width = grid.Width;
        //    var height = grid.Height;

        //    const int step = 20;
        //    var labels = new Label[step * 2];

        //    var processed = 0;

        //    long prevTicks = 0;
        //    long prevMs = 0;
        //    int prevProcessed = 0;
        //    double avgSum = 0;
        //    int avgN = 0;
        //    var sw = new Stopwatch();

        //    Action loop = null;

        //    loop = () =>
        //    {
        //        if (breakTest)
        //        {
        //            var avg = avgSum / avgN;
        //            dopes.Text = string.Format("{0:0.00} Dopes/s (AVG)", avg).PadLeft(21);
        //            return;
        //        }

        //        var now = sw.ElapsedMilliseconds;

        //        //60hz, 16ms to build the frame
        //        while (sw.ElapsedMilliseconds - now < 16)
        //        {
        //            var label = new Label()
        //            {
        //                Text = "Dope",
        //                TextColor = new Color(rand.NextDouble(), rand.NextDouble(), rand.NextDouble()),
        //                Rotation = rand.NextDouble() * 360,
        //                TranslationX = rand.NextDouble() * width,
        //                TranslationY = rand.NextDouble() * height
        //            };


        //            if (processed > max)
        //            {
        //                grid.Children.RemoveAt(0);
        //            }

        //            grid.Children.Add(label);

        //            processed++;

        //            if (sw.ElapsedMilliseconds - prevMs > 500)
        //            {

        //                var r = (double)(processed - prevProcessed) / ((double)(sw.ElapsedTicks - prevTicks) / Stopwatch.Frequency);
        //                prevTicks = sw.ElapsedTicks;
        //                prevProcessed = processed;

        //                if (processed > max)
        //                {
        //                    dopes.Text = string.Format("{0:0.00} Dopes/s", r).PadLeft(15);
        //                    avgSum += r;
        //                    avgN++;
        //                }

        //                prevMs = sw.ElapsedMilliseconds;
        //            }
        //        }

        //        Device.BeginInvokeOnMainThread(loop);
        //    };

        //    sw.Start();

        //    Device.BeginInvokeOnMainThread(loop);
        //}

        void StartTestBindings()
        {
            var rand = new Random2(0);

            breakTest = false;

            var width = absolute.ActualWidth;
            var height = absolute.ActualHeight;

            const int step = 20;
            var labels = new TextBlock[step * 2];

            var processed = 0;

            long prevTicks = 0;
            long prevMs = 0;
            int prevProcessed = 0;
            double avgSum = 0;
            int avgN = 0;
            var sw = new Stopwatch();

            var source = Enumerable.Range(0, max).Select(i => new BindingItem() { Color = Colors.Red }).ToArray();
            items.ItemsSource = source;

            Action loop = null;
            var current = 0;

            loop = () =>
            {
                var now = sw.ElapsedMilliseconds;

                if (breakTest)
                {
                    var avg = avgSum / avgN;
                    dopes.Text = string.Format("{0:0.00} Dopes/s (AVG)", avg).PadLeft(21);
                    return;
                }

                //60hz, 16ms to build the frame
                while (sw.ElapsedMilliseconds - now < 16 && !breakTest)
                {
                    var index = current++ % source.Length;

                    source[index].Color = Color.FromArgb(0xFF, (byte)(rand.NextDouble() * 255), (byte)(rand.NextDouble() * 255), (byte)(rand.NextDouble() * 255));
                    source[index].Rotation = rand.NextDouble() * 360;
                    source[index].Top = rand.NextDouble() * height;
                    source[index].Left = rand.NextDouble() * width;

                    processed++;

                    if (sw.ElapsedMilliseconds - prevMs > 500)
                    {

                        var r = (double)(processed - prevProcessed) / ((double)(sw.ElapsedTicks - prevTicks) / Stopwatch.Frequency);
                        prevTicks = sw.ElapsedTicks;
                        prevProcessed = processed;

                        if (processed > max)
                        {
                            dopes.Text = string.Format("{0:0.00} Dopes/s", r).PadLeft(15);
                            avgSum += r;
                            avgN++;
                        }

                        prevMs = sw.ElapsedMilliseconds;
                    }
                }

                _ = Dispatcher.RunIdleAsync(_ => loop());
            };

            sw.Start();

            _ = Dispatcher.RunIdleAsync(_ => loop());
        }

		public void StartTestChangeST()
        {
            var rand = new Random2(0);

            breakTest = false;

            var width = grid.ActualWidth;
            var height = grid.ActualHeight;

            const int step = 20;
            var labels = new TextBlock[step * 2];

            var processed = 0;

            long prevTicks = 0;
            long prevMs = 0;
            int prevProcessed = 0;
            double avgSum = 0;
            int avgN = 0;
            var sw = new Stopwatch();

            var texts = new string[] { "dOpe", "Dope", "doPe", "dopE" };

            Action loop = null;

            loop = () =>
            {
                if (breakTest)
                {
                    var avg = avgSum / avgN;
                    dopes.Text = string.Format("{0:0.00} Dopes/s (AVG)", avg).PadLeft(21);
                    return;
                }

                var now = sw.ElapsedMilliseconds;

                //60hz, 16ms to build the frame
                while (sw.ElapsedMilliseconds - now < 16 && !breakTest)
                {
                    if (processed > max)
                    {
                        (absolute.Children[processed % max] as TextBlock).Text = texts[(int)Math.Floor(rand.NextDouble() * 4)];
                    }
                    else
                    {
                        var label = new TextBlock()
                        {
                            Text = "Dope",
                            Foreground = new SolidColorBrush(Color.FromArgb(0xFF, (byte)(rand.NextDouble() * 255), (byte)(rand.NextDouble() * 255), (byte)(rand.NextDouble() * 255)))
                        };

                        label.RenderTransform = new RotateTransform() { Angle = rand.NextDouble() * 360 };

                        Canvas.SetLeft(label, rand.NextDouble() * width);
                        Canvas.SetTop(label, rand.NextDouble() * height);

                        absolute.Children.Add(label);
                    }

                    processed++;

                    if (sw.ElapsedMilliseconds - prevMs > 500)
                    {

                        var r = (double)(processed - prevProcessed) / ((double)(sw.ElapsedTicks - prevTicks) / Stopwatch.Frequency);
                        prevTicks = sw.ElapsedTicks;
                        prevProcessed = processed;

                        if (processed > max)
                        {
                            dopes.Text = string.Format("{0:0.00} Dopes/s", r).PadLeft(15);
                            avgSum += r;
                            avgN++;
                        }

                        prevMs = sw.ElapsedMilliseconds;
                    }
                }

                _ = Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Low, () => loop());
            };

            sw.Start();

            _ = Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Low, () => loop());
        }

        private void SetControlsAtStart()
        {
            startChangeST.Visibility = startST.Visibility = startGridST.Visibility = Visibility.Collapsed;
            stop.Visibility = dopes.Visibility = Visibility.Visible;
            absolute.Children.Clear();
            grid.Children.Clear();
            dopes.Text = "Warming up..";
        }

        void startST_Clicked(System.Object sender, object e)
        {
            SetControlsAtStart();
            StartTestST();
        }

        void startGridST_Clicked(System.Object sender, object e)
        {
            SetControlsAtStart();
            StartTestBindings();
        }

        void startChangeST_Clicked(System.Object sender, object e)
        {
            SetControlsAtStart();
            StartTestChangeST();
        }

        void startChangeReuse_Clicked(System.Object sender, object e)
        {
            SetControlsAtStart();
            StartTestReuseST();
        }

		void Stop_Clicked(System.Object sender, object e)
        {
            breakTest = true;
            stop.Visibility = Visibility.Collapsed;
            startChangeST.Visibility = startST.Visibility = startGridST.Visibility = Visibility.Visible;
        }

        async void startAll_Clicked(System.Object sender, object e)
        {
#if HAS_UNO_SKIA
            var deviceFamilyInfo = Windows.System.Profile.AnalyticsInfo.VersionInfo;
            var deviceInfo = new
            {
                OS = "Skia",
                OSVersion = deviceFamilyInfo.DeviceFamily,
                DeviceModel = deviceFamilyInfo.DeviceFamily,
                //DeviceManufacturer = DeviceInfo.Manufacturer,
                //DeviceName = DeviceInfo.Name,
                //DeviceIdiom = DeviceInfo.Idiom.ToString(),
                //DeviceType = DeviceInfo.DeviceType.ToString()
            };
#elif HAS_UNO_WASM
            var deviceFamilyInfo = Windows.System.Profile.AnalyticsInfo.VersionInfo;
            var browserDeviceIdiom = Windows.System.Profile.AnalyticsInfo.DeviceForm;
            var browserUserAgent = WebAssemblyRuntime.InvokeJS("navigator.userAgent;");
            var deviceInfo = new
            {
                OS = "WebAssembly",
                //OSVersion = DeviceInfo.VersionString,
                DeviceModel = browserUserAgent,
                //DeviceManufacturer = DeviceInfo.Manufacturer,
                //DeviceName = DeviceInfo.Name,
                DeviceIdiom = browserDeviceIdiom,
                //DeviceType = DeviceInfo.DeviceType.ToString()
            };
#else
            var deviceInfo = new
            {
                OS = "mac catalyst",
                OSVersion = string.Empty,
                DeviceModel = string.Empty,
                DeviceManufacturer = string.Empty,
                DeviceName = string.Empty,
                DeviceIdiom = string.Empty,
                DeviceType = string.Empty
            };
            try
            {
                deviceInfo = new
                {
                    OS = DeviceInfo.Platform.ToString(),
                    OSVersion = DeviceInfo.VersionString,
                    DeviceModel = DeviceInfo.Model,
                    DeviceManufacturer = DeviceInfo.Manufacturer,
                    DeviceName = DeviceInfo.Name,
                    DeviceIdiom = DeviceInfo.Idiom.ToString(),
                    DeviceType = DeviceInfo.DeviceType.ToString()
                };
            } catch { }
#endif

#if DEBUG
            int testLengthMs = 5000;
#else
            int testLengthMs = 60000;
#endif
            int pauseLengthMs = 100;

            startST_Clicked(default, default);
            await Task.Delay(testLengthMs);
            Stop_Clicked(default, default);
            await Task.Delay(pauseLengthMs);
            _ = Decimal.TryParse(dopes.Text.Replace(" Dopes/s (AVG)", "").Trim(), out var resultST);

            startChangeST_Clicked(default, default);
            await Task.Delay(testLengthMs);
            Stop_Clicked(default, default);
            await Task.Delay(pauseLengthMs);
            _ = Decimal.TryParse(dopes.Text.Replace(" Dopes/s (AVG)", "").Trim(), out var resultChangeST);

            //startChangeReuse_Clicked(default, default);
            //await Task.Delay(testLengthMs);
            //Stop_Clicked(default, default);
            //await Task.Delay(pauseLengthMs);
            //_ = Decimal.TryParse(dopes.Text.Replace(" Dopes/s (AVG)", "").Trim(), out var resultReuseST);

            //startGridST_Clicked(default, default);
            //await Task.Delay(testLengthMs);
            //Stop_Clicked(default, default);
            //await Task.Delay(pauseLengthMs);
            //_ = Decimal.TryParse(dopes.Text.Replace(" Dopes/s (AVG)", "").Trim(), out var resultGridST);

            var platformVersion = "Uno Platform 4.2.0-dev.601 .NET 6";

            var results = new {
                Date = DateTime.Today,
                DeviceInfo = deviceInfo,
                Platform = platformVersion,
                Build = resultST,
                Change = resultChangeST,
                //Reuse = resultReuseST,
                //Grid = resultGridST
            };
            string jsonString = JsonConvert.SerializeObject(results);

            Console.WriteLine(jsonString);
            dopes.Text = $"Build: {results.Build}; Change: {results.Change}";

#if !DEBUG
           try
           {
                //var client = new BlobServiceClient(Config.StorageConnectionString);
                var client = new BlobServiceClient(new Uri(Config.StorageUrl), new AzureSasCredential(Config.StorageSasToken));
                var blobContainerClient = client.GetBlobContainerClient("results");
                await blobContainerClient.CreateIfNotExistsAsync();

                var filename = $"{deviceInfo.OS}-{platformVersion}-{DateTime.UtcNow.ToString("yyyy-MM-dd-hh-mm-ss")}.json";

                using (MemoryStream memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(jsonString)))
                    await blobContainerClient.UploadBlobAsync(filename, memoryStream);

                Console.WriteLine("Uploaded.");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
#endif
        }
    }

    public class BindingItem : INotifyPropertyChanged
    {
        private double top;
        private double left;
        private double rotation;
        private Color color;

        public double Top
        {
            get => top; set
            {
                top = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Top)));
            }
        }
        public double Left
        {
            get => left; set
            {
                left = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Left)));
            }
        }
        public double Rotation
        {
            get => rotation; set
            {
                rotation = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Rotation)));
            }
        }
        public Color Color
        {
            get => color; set
            {
                color = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Color)));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }

}
