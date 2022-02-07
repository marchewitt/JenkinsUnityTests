using System.Runtime.CompilerServices;

namespace Scenes
{
    public static class App
    {
        //Manually set and put in git repo
        public static string AppName { get; private set; } = "Testing Jenkins";
        public static string Version { get; private set; } = "0.1.0";
        
        
        //Local config values
        //TODO: Config Read and cache
        public static uint BuildNumber { get; set; } = 0; //default to say "none set"
        public static string Suffix { get; set; } = string.Empty; //default to say "none set"
        
        private static bool HasBuildNumber => BuildNumber != 0;
        private static bool HasSuffix => false == string.IsNullOrEmpty(Suffix);
        
        //Formatters
        public static string NoWhiteSpace(this string str)
        {
            return str.NoWhiteSpace('_');
        }
        public static string NoWhiteSpace(this string str, char newChar)
        {
            return str.Replace(' ', newChar);
        }
        
        
        
        //TODO: Gotta Write/Read build number from a config file 
        
        

        /// <summary>
        /// Returns the desired format based on applied App settings with intent for .exe name
        /// </summary>
        /// <returns></returns>
        public static string GetFileName()
        {
            
            string name = $"{AppName.NoWhiteSpace()}_v{Version}";
            if (HasBuildNumber) name += $"({BuildNumber})";
            if (HasSuffix) name += $"_{Suffix}";
            return name;
        }

        public static string GetDisplayFullName()
        {
            string name = $"{AppName} v{Version}";
            if (HasBuildNumber) name += $"({BuildNumber})";
            if (HasSuffix) name += $"- {Suffix}";
            return name;
        }

        public static string GetDisplayVersion()
        {
            string name = $"v{Version}";
            if (HasBuildNumber) name += $"({BuildNumber})";
            if (HasSuffix) name += $"{Suffix}";
            return name;
        }
    }
}