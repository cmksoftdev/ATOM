# Execute assemblies from webserver
Extend your app by assemblies loaded from a webserver at runtime.

## Introduction
Maybe you want to extend your app by assemblies loaded from a webspace at runtime. 
This is possible by using reflection. In this article I will show you how to achieve this.

### Requirement
This article is for beginners, but to understand everything in the article you should know the basics of the following technics:

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
```code
public interface IStart
{
    void EntryPoint();
}
```

### The executing app
Now we will implement the app to load and execute the assemblies.
