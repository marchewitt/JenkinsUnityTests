using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Scenes;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace Editor.Build
{
    /// <summary>
    /// Used by a batch script (-executeMethod) to automate building process for unity project
    /// from commandline with our Continuous Integration setup.
    /// </summary>
    public static class UnityBuild
    {
        //DEV NOTES: 
        //* Unity Documentation - https://docs.unity3d.com/Manual/EditorCommandLineArguments.html
        //* This class must be in an editor folder
        //* Method must be static
        //* Throw exceptions to fail build and send errors to console & jenkin logs
        //* You can use #define regions for Windows vs Linux vs Mac vs AR
        //* Have seen people complain online that linux bash commands require -projectPath={path} instead of the space like docs say and batch files use.  
        
        //Example batch
        //CMD: cd C:\Unity3d\2020.3.26f1\Editor
        //CMD: Unity -quit -batchmode -logFile C:/Builds/logs/logname.log -projectPath C:\Code\AlLProjects\SpecificUnityFolder -executeMethod Editor.Build.UnityBuild.StartBuild
        //CMD: For this demo project  -projectPath C:\Code\Jenkins\Jenkins

        private const string EXE = ".exe";
        
        //All Args should be Key/Value paired up
        private const string ARG_BUILD_DIR = "--builddir";
        private const string ARG_BUILD_TYPE = "--buildtype";
        
        //Variables - all should have a value in the BuildServer's local workspace file build_server_config.json
        private const string DEFAULT_ROOT_BUILD_DIR = "C:/Builds/"; //TODO: Move Default to Local Config file
        private static string RootBuildDirectory;
        private static uint BuildNumber;
        
        enum BuildType
        {
            Unknown = 0,
            Dev = 1,
            RC = 2,
            Release = 3
        }
        
        
        /// <summary>
        /// Pulls console args or sets defaults for build process
        /// </summary>
        /// <exception cref="Exception"></exception>
        private static void HandleCmdArgs()
        {
            //Get args
            string[] args = System.Environment.GetCommandLineArgs();

            Debug.Log($"Arg length total: {args.Length}");

            Dictionary<string, string> arg_dict = new Dictionary<string, string>();
            for (int i = 0; i < args.Length -1; i+=2)
            {
                //Key/Value args
                arg_dict.Add(args[i], args[i+1]);
            }

            string key_checker = null; //AKA: typo catcher

            //BUILD TYPE ARG
            key_checker = ARG_BUILD_TYPE;
            App.Suffix = string.Empty; //Default
            BuildType build_type = BuildType.Unknown; 
            if (arg_dict.ContainsKey(key_checker))
            {
                string value = arg_dict[key_checker];

                if (value == "dev") build_type = BuildType.Dev; 
                else if (value == "rc") build_type = BuildType.RC;
                else if (value == "release") build_type = BuildType.Release;
                else throw new Exception($"Unknown {key_checker} {value}");
            }
            if(build_type != BuildType.Unknown) App.Suffix = build_type.ToString();
            
            //BUILD DIR ARG
            key_checker = ARG_BUILD_DIR;
            RootBuildDirectory = DEFAULT_ROOT_BUILD_DIR; //Default
            if (arg_dict.ContainsKey(key_checker))
            {
                RootBuildDirectory = arg_dict[key_checker];
            }
        }
        
        /// <summary>
        /// What we call from command line
        /// </summary>
        public static void StartBuild()
        {
            //Get and Increment Build Number
            //TODO: Read Build Number from a Local Config File
            //TODO: Increment that Build Number
            //App.BuildNumber = build_number;
            //TODO: Write updated BuildNumber to Config File (note: Move to after BuildPlayer() with an IF Fail, when incrementing only on successful builds)
            
            HandleCmdArgs();
            
            //Get list of scenes that are enabled in the Unity Build Settings
            List<string> enabled_scene_paths = new List<string>();
            foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
            {
                if (scene.enabled) enabled_scene_paths.Add(scene.path);
            }
            
            
            //Define the EXE path we want and confirm the directory exists
            //string build_date_time = System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"); //optional date-time 
            string exe_dir_path = RootBuildDirectory + "/Windows/" + App.AppName.NoWhiteSpace();
            if (false == Directory.Exists(exe_dir_path)) Directory.CreateDirectory(exe_dir_path);

            string full_build_path = exe_dir_path + App.GetFileName() + EXE;
            
            Debug.Log("Starting to Unity Window Build");
            BuildPlayerOptions bpo = new BuildPlayerOptions();
            bpo.scenes = enabled_scene_paths.ToArray();
            bpo.locationPathName = full_build_path;
            bpo.target = BuildTarget.StandaloneWindows64;
            bpo.options = BuildOptions.None; //Could add .Development or ConnectWithProfiler here


            //Build it
            BuildReport report = BuildPipeline.BuildPlayer(bpo);
            
            //Log out the Build Report
            Debug.Log($"{report.summary.platform} Build Complete." +
                      $"\nResult: {report.summary.result}" +
                      $"\nBuild Time: {report.summary.totalTime}" +
                      $"\nOutput Path: {report.summary.outputPath}" +
                      $"\n" +
                      $"\nTotal Size: {report.summary.totalSize}" +
                      $"\nTotal File Count: {report.files.Length}" +
                      $"\n" +
                      $"\nTotal Warnings: {report.summary.totalWarnings}" +
                      $"\nTotal Errors: {report.summary.totalErrors}"
            );
        }

        
        
    }
}