using Azure;
using Azure.Storage.Blobs;
using Microsoft.Maui.Layouts;
using Newtonsoft.Json;
using Saplin.xOPS.UI.Misc;
using System.Diagnostics;
using System.Text;

namespace DopeTestMaui;

public partial class MainPage : ContentPage
{

    private static readonly CancellationTokenSource _cts = new CancellationTokenSource(1500);

    public MainPage()
	{
		InitializeComponent();
	}

    volatile bool breakTest = false;
    const int max = 600;

    void StartTestMT()
    {
        var rand = new Random2(0);

        breakTest = false;

        var width = absolute.Width;
        var height = absolute.Height;

        var i = 0;
        var processed = 0;

        var thread = new Thread(() =>
        {
            while (true)
            {
                if (processed < i - 20)
                {
                    Thread.Sleep(1);
                    continue;
                }

                var label = new Label()
                {
                    Text = "Dope",
                    TextColor = new Color((float)rand.NextDouble(), (float)rand.NextDouble(), (float)rand.NextDouble()),
                    Rotation = rand.NextDouble() * 360
                };

                AbsoluteLayout.SetLayoutFlags(label, AbsoluteLayoutFlags.PositionProportional);
                AbsoluteLayout.SetLayoutBounds(label, new Rect(rand.NextDouble(), rand.NextDouble(), 80, 24));

                absolute.Dispatcher.Dispatch(() =>
                {
                    if (i > max)
                    {
                        absolute.Children.RemoveAt(0);
                    }

                    absolute.Children.Add(label);

                    processed++;
                });

                if (breakTest) break;

                i++;
            }
        });

        thread.IsBackground = true;
        thread.Priority = ThreadPriority.Lowest;
        thread.Start();

        var sw = new Stopwatch();
        sw.Start();
        long prevTicks = 0;
        int prevProcessed = 0;
        double avgSum = 0;
        int avgN = 0;

        Device.StartTimer(TimeSpan.FromMilliseconds(500), () =>
        {
            if (stop.IsVisible)
            {
                var avg = avgSum / avgN;
                dopes.Text = string.Format("{0:0.00} Dopes/s (AVG)", avg).PadLeft(21);
                return false;
            }

            var r = (double)(processed - prevProcessed) / ((double)(sw.ElapsedTicks - prevTicks) / Stopwatch.Frequency);
            dopes.Text = string.Format("{0:0.00} Dopes/s", r).PadLeft(15);
            prevTicks = sw.ElapsedTicks;
            prevProcessed = processed;

            if (i > max)
            {
                avgSum += r;
                avgN++;
            }

            return true;
        });
    }

    void StartTestMT2()
    {
        var rand = new Random2(0);

        breakTest = false;

        var width = absolute.Width;
        var height = absolute.Height;

        const int step = 75;

        var processed = 0;

        long prevTicks = 0;
        long prevMs = 0;
        int prevProcessed = 0;
        double avgSum = 0;
        int avgN = 0;
        var sw = new Stopwatch();

        var bankA = new Label[step];
        var bankB = new Label[step];

        Action<Label[]> addLabels = (Label[] labels) =>
        {
            for (int k = 0; k < step; k++)
            {
                var label = new Label()
                {
                    Text = "Dope",
                    TextColor = new Color((float)rand.NextDouble(), (float)rand.NextDouble(), (float)rand.NextDouble()),
                    Rotation = rand.NextDouble() * 360
                };

                AbsoluteLayout.SetLayoutFlags(label, AbsoluteLayoutFlags.PositionProportional);
                AbsoluteLayout.SetLayoutBounds(label, new Rect(rand.NextDouble(), rand.NextDouble(), 80, 24));

                labels[k] = label;
            }
        };

        addLabels(bankA);
        addLabels(bankB);

        var bank = bankA;

        Action loop = null;

        var i = 0;
        Task task = null;

        loop = () =>
        {
            if (breakTest)
            {
                var avg = avgSum / avgN;
                dopes.Text = string.Format("{0:0.00} Dopes/s (AVG)", avg).PadLeft(21);
                return;
            }

            if (processed > max)
            {
                absolute.Children.RemoveAt(0);
            }

            absolute.Children.Add(bank[i]);
            i++;

            if (i == step)
            {
                if (task != null && task.Status != TaskStatus.RanToCompletion) task.Wait();
                task = Task.Run(() => addLabels(bank));
                if (bank == bankA) bank = bankB; else bank = bankA;
                i = 0;
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

            Device.BeginInvokeOnMainThread(loop);
        };

        sw.Start();


        Device.BeginInvokeOnMainThread(loop);
    }

    void StartTestST()
    {
        var rand = new Random2(0);

        breakTest = false;

        var width = absolute.Width;
        var height = absolute.Height;

        const int step = 20;
        var labels = new Label[step * 2];

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
            while (sw.ElapsedMilliseconds - now < 16)
            {
                var label = new Label()
                {
                    Text = "Dope",
                    TextColor = new Color((float)rand.NextDouble(), (float)rand.NextDouble(), (float)rand.NextDouble()),
                    Rotation = rand.NextDouble() * 360
                };

                AbsoluteLayout.SetLayoutFlags(label, AbsoluteLayoutFlags.PositionProportional);
                AbsoluteLayout.SetLayoutBounds(label, new Rect(rand.NextDouble(), rand.NextDouble(), 80, 24));

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

            Device.BeginInvokeOnMainThread(loop);
        };

        sw.Start();

        Device.BeginInvokeOnMainThread(loop);
    }

    void StartTestGridST()
    {
        var rand = new Random2(0);

        breakTest = false;

        var width = grid.Width;
        var height = grid.Height;

        const int step = 20;
        var labels = new Label[step * 2];

        var processed = 0;

        long prevTicks = 0;
        long prevMs = 0;
        int prevProcessed = 0;
        double avgSum = 0;
        int avgN = 0;
        var sw = new Stopwatch();

        Action loop = null;

        loop = async () =>
        {
            await Task.Delay(1);

            if (breakTest)
            {
                var avg = avgSum / avgN;
                dopes.Text = string.Format("{0:0.00} Dopes/s (AVG)", avg).PadLeft(21);
                return;
            }

            var now = sw.ElapsedMilliseconds;

            //60hz, 16ms to build the frame
            while (sw.ElapsedMilliseconds - now < 16)
            {
                var label = new Label()
                {
                    Text = "Dope",
                    TextColor = new Color((float)rand.NextDouble(), (float)rand.NextDouble(), (float)rand.NextDouble()),
                    Rotation = rand.NextDouble() * 360,
                    TranslationX = rand.NextDouble() * width,
                    TranslationY = rand.NextDouble() * height
                };


                if (processed > max)
                {
                    grid.Children.RemoveAt(0);
                }

                grid.Children.Add(label);

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

            Device.BeginInvokeOnMainThread(loop);
        };

        sw.Start();

        Device.BeginInvokeOnMainThread(loop);
    }

    void StartTestChangeST()
    {
        var rand = new Random2(0);

        breakTest = false;

        var width = grid.Width;
        var height = grid.Height;

        const int step = 20;
        var labels = new Label[step * 2];

        var processed = 0;

        long prevTicks = 0;
        long prevMs = 0;
        int prevProcessed = 0;
        double avgSum = 0;
        int avgN = 0;
        var sw = new Stopwatch();

        var texts = new string[] { "dOpe", "Dope", "doPe", "dopE" };

        Action loop = null;

        var sw2 = Stopwatch.StartNew();

        loop = async () =>
        {
            // await Task.Delay(1);

            if (breakTest || sw2.Elapsed > TimeSpan.FromSeconds(15))
            {
                var avg = avgSum / avgN;
                dopes.Text = string.Format("{0:0.00} Dopes/s (AVG)", avg).PadLeft(21);
                return;
            }


            var now = sw.ElapsedMilliseconds;

            //60hz, 16ms to build the frame
            while (sw.ElapsedMilliseconds - now < 16)
            {
                if (processed > max)
                {
                    (absolute.Children[processed % max] as Label).Text = texts[(int)Math.Floor(rand.NextDouble() * 4)];
                }
                else
                {
                    var label = new Label()
                    {
                        Text = "Dope",
                        TextColor = new Color((float)rand.NextDouble(), (float)rand.NextDouble(), (float)rand.NextDouble()),
                        Rotation = rand.NextDouble() * 360
                    };

                    AbsoluteLayout.SetLayoutFlags(label, AbsoluteLayoutFlags.PositionProportional);
                    AbsoluteLayout.SetLayoutBounds(label, new Rect(rand.NextDouble(), rand.NextDouble(), 80, 24));

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

            this.Dispatcher.Dispatch(loop);
        };

        sw.Start();

        this.Dispatcher.Dispatch(loop);
    }

    private void SetControlsAtStart()
    {
        startChangeST.IsVisible = startST.IsVisible = startGridST.IsVisible = false;
        stop.IsVisible = dopes.IsVisible = true;
        absolute.Children.Clear();
        grid.Children.Clear();
        dopes.Text = "Warming up..";
    }

    void startMT_Clicked(System.Object sender, System.EventArgs e)
    {
        SetControlsAtStart();
        StartTestMT2();
    }

    void startST_Clicked(System.Object sender, System.EventArgs e)
    {
        SetControlsAtStart();
        StartTestST();
    }

    void startGridST_Clicked(System.Object sender, System.EventArgs e)
    {
        SetControlsAtStart();
        StartTestGridST();
    }

    void startChangeST_Clicked(System.Object sender, System.EventArgs e)
    {
        SetControlsAtStart();
        StartTestChangeST();
    }

    void Stop_Clicked(System.Object sender, System.EventArgs e)
    {
        breakTest = true;
        stop.IsVisible = false;
        startChangeST.IsVisible = startST.IsVisible = startGridST.IsVisible = true;
    }

    private async void startAll_Clicked(object sender, EventArgs e)
    {
        int testLengthMs = 60000;
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

        //startGridST_Clicked(default, default);
        //await Task.Delay(testLengthMs);
        //Stop_Clicked(default, default);
        //await Task.Delay(pauseLengthMs);
        //_ = Decimal.TryParse(dopes.Text.Replace(" Dopes/s (AVG)", "").Trim(), out var resultGridST);

        var platformVersion = "Maui 6.0.200-preview.12.2441";

#if ANDROID
            var operatingSystem = "Android";
#elif IOS
            var operatingSystem = "iOS";
#elif MACCATALYST
            var operatingSystem = "MacCatalyst";
#elif WINDOWS
        var operatingSystem = "WinUI";
#else
        var operatingSystem = "Unknown";
#endif

        var results = new { OS = operatingSystem, Platform = platformVersion, Build = resultST, Change = resultChangeST, Reuse = 0, Grid = 0 };
        string jsonString = JsonConvert.SerializeObject(results);

        dopes.Text = $"Build: {results.Build}; Change: {results.Change}";
        Console.WriteLine(jsonString);

#if false// !DEBUG
        try
        {
            var client = new BlobServiceClient(new Uri(Config.StorageUrl), new AzureSasCredential(Config.StorageSasToken));
            var blobContainerClient = client.GetBlobContainerClient("results");
            await blobContainerClient.CreateIfNotExistsAsync();

            var filename = $"{operatingSystem}-{platformVersion}-{DateTime.UtcNow.ToString("yyyy-MM-dd-hh-mm-ss")}.json";

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

