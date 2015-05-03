using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.PackageManagement.SwidTag.Utility {
    using System.Diagnostics;
    using System.Globalization;

    public static class ExceptionExtensions {
        public static void Dump(this Exception e) {
            var text = string.Format(CultureInfo.CurrentCulture, "{0}/{1}\r\n{2}", e.GetType().Name, e.Message, e.StackTrace);
            Debug.WriteLine(text);
        }

#if DETAILED_DEBUG
        private static DateTime startTime = DateTime.Now;
        public static T DumpTime<T>(this T nothing) {
            StackTrace s = new StackTrace(true);
            var f = s.GetFrame(1);

            Console.WriteLine("      OFFSET: \r\n          {0}:{1}:\r\n          {2}\r\n          Time:[{3}] ",f.GetFileName(), f.GetFileLineNumber(), f.GetMethod(), DateTime.Now.Subtract(startTime).TotalMilliseconds);
            return nothing;
        }
#endif
    }
}
