using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UCenter.Common
{
    public static class DisposableExtensions
    {
        public static void DisposeOnException(this IDisposable obj, Action action)
        {
            bool success = false;
            try
            {
                action();
                success = true;
            }
            finally
            {
                if (!success)
                {
                    obj.Dispose();
                }
            }
        }
    }
}
