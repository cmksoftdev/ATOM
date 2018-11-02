# Execute assemblies from webserver
Extend your app by assemblies loaded from a webserver at runtime.

## Introduction
Maybe you want to extend your app by assemblies loaded from a webspace at runtime. 
This is possible by using reflection. In this article I will show you how to achieve this.

### Requirement
This article is for beginners, but to understand everything, you should know the basics of the following technics:

* Reflection
* Interfaces
* Zip-Archiv
* XML-Config
* WebClient

### Description
The concept is to write a small app, loading a zip archiv, containing a XML-Config file and some assemblies. 
One assemlby implements the base interface with our entry point. The config tells us which assembly contains the entry point.
With this informations we can execute the code without saving it on disk.

### Base interface
First we will implement our base interface. This interface impements only the entry point. On this way, 
the executing program knows where to start without knowing the assembly complettely.

This interface should look like this:
```csharp
public interface IStart
{
    void EntryPoint();
}
```

### The executing app
Now we will implement the app to load and execute the assemblies. For the first test it should be enouth to implement a console app for it. If you want, you can use the shown technics in every kind of project.

This is how your Main should look like:
```csharp
namespace TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            foreach (var url in args)
                ExecuteCodeFromUrl(url);
            Console.ReadLine();
        }
    }
}
```

How you can see, we are using the static Method ExecuteCodeFromUrl(string). This method will load and execute the assemblies loaded from a webserver.

### Service class
Now we have to implement the service class containing this method. This service class should do all the steps, mentioned in the description. So we need to load the zip archive using a web- or http client, map it, load config, create an instance of the class, implementing the IStart interface and last it should execute the EntryPoint method. It sound like a lot of work, but it is just combining some already existing services.

Later our service class should look like this:
```csharp
    public class AssemblyLoader
    {
        public static Assembly GetAssemblyFromUrl(string url)
        {
            return GetAssemblyFromUrl(url, null);
        }

        public class Config
        {
            [XmlArray]
            [XmlArrayItem("a")]
            public List<string> assemblies { get; set; }
            public string classToCall { get; set; }
        }

        public static Assembly GetAssemblyFromUrl(string url, Func<byte[], byte[]> intermediateStep)
        {
            using (var webClient = new WebClient())
            {
                var bytes = webClient.DownloadData(url);
                if (intermediateStep != null)
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
                    if (entry.Name.EndsWith(".dll"))
                        asms.Add(GetAssemblyFromStream(entry.Open(), intermediateStep));
                var namespaceClass = cfg.classToCall.Split('.');
                IStart callableClass = Get<IStart>(namespaceClass[0], namespaceClass[1], asms.FirstOrDefault(x => x.FullName.Contains(namespaceClass[0])), null);
                callableClass.EntryPoint();
            }
        }
    }
```
The function intermediateStep is to decode some parts. It gets the whole byte array and has to return a byte array.

### Creating assembly package
Now we need to create a zip archiv, containing the assemblies we want to execute, and the config file.

#### Config
This is the config DTO:
```csharp
public class Config
{
    [XmlArray]
    [XmlArrayItem("a")]
    public List<string> assemblies { get; set; }
    public string classToCall { get; set; }
}
```

This is how the XML config file should look like:
```xml
<?xml version="1.0"?>
<Config>
    <assemblies>
        <a>TestAssembly.Class1</a>
    </assemblies>
    <classToCall>TestAssembly.Class1</classToCall>
</Config>
```
