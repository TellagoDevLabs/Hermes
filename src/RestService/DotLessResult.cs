using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using dotless.Core.configuration;
using dotless.Core.Loggers;

namespace TellagoStudios.Hermes.RestService
{
    public class DotLessResult : ActionResult
    {
        public IDictionary<string, string> Parameters { get; set; }
        public string Less { get; set; }
        public bool Minify { get; set; }

        public DotLessResult(string less, IDictionary<string, string> parameters = null, bool minify = false)
        {
            Less = less;
            Parameters = parameters ?? new Dictionary<string, string>();
            Minify = minify;
        }

        public DotLessResult(Stream stream, IDictionary<string, string> parameters = null, bool minify = false)
            : this(new StreamReader(stream).ReadToEnd(), parameters, minify) { }

        public override void ExecuteResult(ControllerContext context)
        {
            var output = Less;
            foreach (var key in Parameters.Keys)
            {
                output = Regex.Replace(output, @"^\s*@" + key + @":\s*\S+;", "@" + key + ":" + Parameters[key] + ";");
            }
            var css = dotless.Core.Less.Parse(output, new DotlessConfiguration { CacheEnabled  = false, MinifyOutput = Minify, Logger = typeof(Foo)});
            context.HttpContext.Response.ContentType = "text/css";
            using (var writer = new StreamWriter(context.HttpContext.Response.OutputStream, Encoding.UTF8))
            {
                writer.Write(css);
                writer.Flush();
            }
        }

        public class Foo : ILogger
        {

            public void Debug(string message)
            {
                throw new NotImplementedException(message);
            }

            public void Error(string message)
            {
                throw new NotImplementedException(message);
            }

            public void Info(string message)
            {
                throw new NotImplementedException(message);
            }

            public void Log(LogLevel level, string message)
            {
                throw new NotImplementedException(message);
            }

            public void Warn(string message)
            {
                throw new NotImplementedException(message);
            }
        }
    }

}