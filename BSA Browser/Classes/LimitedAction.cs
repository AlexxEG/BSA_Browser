using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace BSA_Browser.Classes
{
    public class LimitedAction
    {
        static int _incrementingId = 0;
        static Dictionary<int, Timer> _timers = new Dictionary<int, Timer>();

        /// <summary>
        /// Returns unique id, incrementing from 0.
        /// </summary>
        public static int GenerateId() => ++_incrementingId;

        /// <summary>
        /// Delays executing of <paramref name="action"/> everytime it's called, delaying by <paramref name="ms"/>.
        /// Example use is performing search after typing, instead of everytime a key is pressed.
        /// </summary>
        public static void RunAfter(int id, int ms, Action action)
        {
            if (_timers.ContainsKey(id))
            {
                _timers[id].Stop();
            }
            else
            {
                var newTimer = new Timer() { Interval = ms };
                newTimer.Tick += delegate (object timer, EventArgs e) { action(); (timer as Timer).Stop(); };
                _timers.Add(id, newTimer);
            }

            _timers[id].Start();
        }

        /// <summary>
        /// Stops timer for given id.
        /// </summary>
        public static void Stop(int id)
        {
            if (_timers.ContainsKey(id))
                _timers[id].Stop();
        }
    }
}
