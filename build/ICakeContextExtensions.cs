using Cake.Core;
using System;
using Cake.Common.Diagnostics;
using Cake.Common.Xml;
using Cake.Common.IO;

namespace Build;

// ReSharper disable once InconsistentNaming
public static class ICakeContextExtensions
{
    public static void WriteWarning(this ICakeContext context, string message)
    {
        var previousColor = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Yellow;
        context.Warning(message);
        Console.ForegroundColor = previousColor;
    }

    public static void SetAppSettings(this ICakeContext context, string key, string value, string configFilePath)
    {
        context.Information($"Setting appSettings key: \u001b[36m{key}\u001b[0m value: \u001b[36m{value}\u001b[0m");

        var file = context.File(configFilePath);
        context.XmlPoke(file, $"/configuration/appSettings/add[@key = '{key}']/@value", value);
    }

    public static void SetConnectionString(this ICakeContext context, string name, string connectionString, string configFilePath)
    {
        context.Information($"Setting connectionStrings name: \u001b[36m{name}\u001b[0m connectionString: \u001b[36m{connectionString}\u001b[0m");

        var file = context.File(configFilePath);
        context.XmlPoke(file, $"/configuration/connectionStrings/add[@name = '{name}']/@connectionString", connectionString);
    }
}
