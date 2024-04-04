using Kenwood.PlaylistOrganizer.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Kenwood.PlaylistOrganizer.Helpers
{
    internal class TimeHelper
    {
        private const int interval = 1000;
        
        private readonly Stopwatch _stopwatch;

        private readonly LayoutWriter _layoutWriter;

        private protected bool _status = true;

        private protected long _timeCheck = 0;

        internal TimeHelper(ref LayoutWriter layoutWriter, ref Stopwatch stopwatch)
        {
            _layoutWriter = layoutWriter;
            _stopwatch = stopwatch;
        }

        internal void Run()
        {
            while (_status)
            {
                if (_stopwatch.ElapsedMilliseconds > interval)
                {
                    _layoutWriter.UpdateTime($"{_stopwatch.Elapsed.ToString("hh\\:mm\\:ss")}");

                    _timeCheck += _stopwatch.ElapsedMilliseconds;
                }
            }
        }

        internal void Stop()
        {
            _status = false;
        }
    }
}
