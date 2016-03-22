using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Web;
using System.Web.Hosting;

namespace GF.UCenter.Common
{
    public static class TraceLogger
    {
        private static DateTime currentTime;
        public static void TraceInformation(ITraceIdentifier traceIdentifier, string message, params object[] args)
        {
            Trace.TraceInformation(GenerateMessage(traceIdentifier, message), args);
        }

        public static void TraceError(ITraceIdentifier traceIdentifier, string message, params object[] args)
        {
            Trace.TraceError(GenerateMessage(traceIdentifier, message), args);
        }

        public static void TraceWarning(ITraceIdentifier traceIdentifier, string message, params object[] args)
        {
            Trace.TraceWarning(GenerateMessage(traceIdentifier, message), args);
        }

        private static string GenerateMessage(ITraceIdentifier traceIdentifier, string message)
        {
            InitTraceEnvironment();
            return string.Format("[{0}].{1}", traceIdentifier.TraceIdentifier, message);
        }

        private static void InitTraceEnvironment()
        {
            // split the trace file by date;
            if ((DateTime.Now - currentTime).TotalDays > 1)
            {
                string rootPath;
                if (HttpContext.Current != null)
                {
                    // MVC Controller
                    rootPath = HttpContext.Current.Server.MapPath("~/logs");
                }
                else
                {
                    // Api Controller
                    rootPath = HostingEnvironment.MapPath("~/logs"); var path = HostingEnvironment.MapPath("~");
                    if (rootPath == null)
                    {
                        // Self-host owin
                        var uriPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase);
                        rootPath = new Uri(uriPath).LocalPath;
                    }
                }
                if (!Directory.Exists(rootPath))
                {
                    Directory.CreateDirectory(rootPath);
                }
                currentTime = DateTime.Now.Date;
                string file = Path.Combine(rootPath, "trace-{0:MM-dd-yyyy}.log".FormatInvariant(currentTime));
                Trace.Listeners.Clear();
                Trace.AutoFlush = true;
                Trace.Listeners.Add(new XmlWriterTraceListener(file));
            }
        }
    }
}
