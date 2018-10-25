using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;

namespace CMK
{
    public class ATOM
    {
        public interface ICallable
        {
            void Call();
        }

        public interface ICallableWithParameters
        {
            void Call(IEnumerable<object> parameters);
        }

        public interface ICallableWithParametersAndReturn
        {
            object Call(IEnumerable<object> parameters);
        }

        public static Assembly GetAssemblyFromUrl(string url)
        {
            return GetAssemblyFromUrl(url, null);
        }

        public static Assembly GetAssemblyFromUrl(string url, Func<byte[], byte[]> intermediateStep)
        {
            using (var webClient = new WebClient())
            {
                var bytes = webClient.DownloadData(url);
                if(intermediateStep != null)
                    bytes = intermediateStep.Invoke(bytes);
                return GetAssemblyFromBytes(bytes);
            }
        }

        public static Assembly GetAssemblyFromBytes(byte[] bytes)
        {
            return Assembly.Load(bytes);
        }

        public static Assembly GetAssemblyFromStream(Stream stream)
        {
            using (var reader = new StreamReader(stream))
                return Assembly.Load(Encoding.ASCII.GetBytes(reader.ReadToEnd()));
        }

        public static Assembly GetAssemblyFromFile(string filePath)
        {
            return Assembly.Load(filePath);
        }

        public static T Get<T>(string nameSpace, string className, byte[] array, object[] args) where T : class
        {
            var a = Assembly.Load(array);
            return Get<T>(nameSpace, className, a, args);
        }

        public static T Get<T>(string nameSpace, string className, Assembly a, object[] args) where T : class
        {
            return (T)a.CreateInstance(
                $"{nameSpace}.{className}",
                true,
                BindingFlags.CreateInstance,
                null,
                args,
                CultureInfo.CurrentCulture,
                null);
        }
    }
}
