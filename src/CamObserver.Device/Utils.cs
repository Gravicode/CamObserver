using System;
using System.Collections;
using System.Text;
using System.Threading;

namespace CamObserver.Device
{
    public static class Utils
    {
        public static bool StartsWith(this string s, string value)
        {
            return s.IndexOf(value) == 0;
        }

        public static bool Contains(this string s, string value)
        {
            return s.IndexOf(value) > 0;
        }
    }
}
