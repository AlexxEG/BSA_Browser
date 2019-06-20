using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace BSA_Browser.Classes
{
    public class LimitedAction
    {
        static Dictionary<int, Timer> _timers = new Dictionary<int, Timer>();

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
    }
}
