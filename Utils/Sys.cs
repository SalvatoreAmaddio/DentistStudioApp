using System.Windows;

namespace DentistStudioApp.Utils
{
    public class Sys
    { 
        public static ResourceDictionary GetAppImagesDictionary() => GetAppDictionary("Images");
        public static ResourceDictionary GetAppDictionary(string name) =>
        new()
        {
            Source = new Uri($"pack://application:,,,/DentistStudioApp;component/Themes/{name}.xaml")
        };
    }
}
