using System.Diagnostics;

namespace dopetest_blazor.Shared
{
    public partial class MainLayout
    {
        private const int Max = 600;

        private int Height => 800; // TODO: calculate
        private int Width => 1000; // TODO: calculate

        private readonly Random2 _rand = new Random2(0);

        private readonly Stopwatch _sw = new Stopwatch();

        private long? _prevElapsed;
        private int _processed = 0;
        private int _prevCount;
        private bool _started = false;
        private bool _startedOnce = false;
        private double _accum;
        private int _accumN;
        private string _dopes = "";

        private bool _isUpdatePending = false;

        private readonly List<string> _labels = new List<string>();

        private string GetDope() => $"<span style=\"position:absolute;top:{GetTop()}px;left:{GetLeft()}px; transform:{GetTransform()}; color:{GetColor()};\">Dope</span>";

        private int GetTop() => (int)Math.Round(_rand.NextDouble() * Height);
        private int GetLeft() => (int)Math.Round(_rand.NextDouble() * Width);

        private string GetTransform() => $"rotate({GetAngle()}deg)";

        private int GetAngle() => (int)Math.Round(_rand.NextDouble() * 360);

        private string GetColor() => $"rgb({GetColorByte()},{GetColorByte()},{GetColorByte()})";

        private int GetColorByte() => (int)Math.Round(_rand.NextDouble() * 255);

        private string[] _texts = new[]{ "dOpe", "Dope", "doPe", "dopE" };

        private async void StartBuildClicked()
        {
            _started = !_started;

            if (_started)
            {
                _dopes = "Warming up..";
                _sw.Start();
                _prevElapsed = null;
                _accum = 0;
                _accumN = 0;
            }
            else
            {
                _dopes = $"{_accum / _accumN} Dopes/s (AVG)";
            }

            while (_started)
            {
                var now = _sw.ElapsedMilliseconds;
                var processedPreLoop = _processed;

                //60hz, 16ms to build the frame
                while (_started && !_isUpdatePending && _sw.ElapsedMilliseconds - now < 16 && _processed - processedPreLoop < Max)
                {
                    var label = GetDope();
                    _processed++;

                    if (_processed > Max)
                    {
                        _labels.RemoveAt(0);

                        if (_prevElapsed == null)
                        {
                            _prevElapsed = _sw.ElapsedMilliseconds;
                            _prevCount = _processed;
                        }

                        var diff = _sw.ElapsedMilliseconds - _prevElapsed;

                        if (diff > 500)
                        {
                            _prevElapsed = _sw.ElapsedMilliseconds;
                            var val = ((_processed - _prevCount) / (double)diff) * 1000;
                            _dopes = $"{val} Dopes/s";
                            _accum += val;
                            _accumN++;
                            _prevElapsed = _sw.ElapsedMilliseconds;
                            _prevCount = _processed;
                        }
                    }

                    _labels.Add(label);

                    StateHasChanged();
                }
                await Task.Delay(1);
            }
        }

        private async void StartChangeClicked()
        {
            _started = !_started;

            if (_started)
            {
                _dopes = "Warming up..";
                _sw.Start();
                _prevElapsed = null;
                _accum = 0;
                _accumN = 0;
            }
            else
            {
                _dopes = $"{_accum / _accumN} Dopes/s (AVG)";
            }

            while (_started)
            {
                var now = _sw.ElapsedMilliseconds;
                var processedPreLoop = _processed;

                //60hz, 16ms to build the frame
                while (_started && !_isUpdatePending && _sw.ElapsedMilliseconds - now < 16 && _processed - processedPreLoop < Max)
                {
                    _processed++;

                    if (_processed > Max)
                    {
                        var index = _processed % Max;
                        var changeLabel = _labels[index];
                        _labels[index] = changeLabel.Replace("dope", _texts[(int)Math.Floor(_rand.NextDouble() * 4)], StringComparison.InvariantCultureIgnoreCase);
                    }
                    else
                    {
                        var label = GetDope();
                        _labels.Add(label);
                    }

                    if (_prevElapsed == null)
                    {
                        _prevElapsed = _sw.ElapsedMilliseconds;
                        _prevCount = _processed;
                    }

                    var diff = _sw.ElapsedMilliseconds - _prevElapsed;

                    if (diff > 500)
                    {
                        _prevElapsed = _sw.ElapsedMilliseconds;
                        var val = ((_processed - _prevCount) / (double)diff) * 1000;
                        _dopes = $"{val} Dopes/s";
                        _accum += val;
                        _accumN++;
                        _prevElapsed = _sw.ElapsedMilliseconds;
                        _prevCount = _processed;
                    }

                    StateHasChanged();
                }
                await Task.Delay(1);
            }
        }

        protected override void OnAfterRender(bool firstRender)
        {
            _isUpdatePending = false;
            base.OnAfterRender(firstRender);
        }
    }
}
