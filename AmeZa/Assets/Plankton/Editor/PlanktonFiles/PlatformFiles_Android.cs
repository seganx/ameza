using System;
using System.IO;
using UnityEngine;
using System.Collections.Generic;
using UnityEditor;


namespace Plankton.Editor.Files
{
    public class PlatformFiles_Android : PlatformFiles
    {
        public File planktonConfig = new File()
        {
            sourceFilename = "plankton_config_src.json",
            destinationFilename = "/Plugins/Android/res/raw/plankton_config.json",
            placeholderSeparator = ",\n\t\t"
        };

        public File values = new File()
        {
            sourceFilename = "values_src.xml",
            destinationFilename = "/Plugins/Android/res/values/values.xml",
        };

        public File launcherManifest = new File()
        {
            sourceFilename = "LauncherManifest_src.xml",
            destinationFilename = "/Plugins/Android/LauncherManifest.xml",
        };

        public File launcherTemplate = new File()
        {
#if UNITY_2019
            sourceFilename = "launcherTemplate_19.gradle",
#elif UNITY_2020 || UNITY_2021
            sourceFilename = "launcherTemplate_20_21.gradle",
#elif UNITY_2022
            sourceFilename = "launcherTemplate_22.gradle",
#endif
            destinationFilename = "/Plugins/Android/launcherTemplate.gradle",
            placeholderSeparator = ",\n\t\t\t",
            placeholderFinalFunc = items => $"manifestPlaceholders=[\n\t\t\t{items}\n\t\t]"
        };

        public File baseProjectTemplate = new File()
        {
#if UNITY_2019 || UNITY_2020 || UNITY_2021
            sourceFilename = "baseProjectTemplate_19_20_21.gradle",
#elif UNITY_2022
            sourceFilename = "baseProjectTemplate_22.gradle",
#endif
            destinationFilename = "/Plugins/Android/baseProjectTemplate.gradle",
        };

        public File gradleTemplate = new File()
        {
#if UNITY_2019
            sourceFilename = "gradleTemplate_19.properties",
#elif UNITY_2020 || UNITY_2021
            sourceFilename = "gradleTemplate_20_21.properties",
#elif UNITY_2022
            sourceFilename = "gradleTemplate_22.properties",
#endif
            destinationFilename = "/Plugins/Android/gradleTemplate.properties",
        };

        public File settingsTemplate = new File()
        {
            sourceFilename = "settingsTemplate_22.gradle",
            destinationFilename = "/Plugins/Android/settingsTemplate.gradle",
        };

        public File androidManifest = new File()
        {
            sourceFilename = "AndroidManifest_19_20_21.xml",
            destinationFilename = "/Plugins/Android/AndroidManifest.xml",
        };

        public List<string> repositories = new List<string>();
        public List<string> dependencies = new List<string>();
        public List<string> defenitions = new List<string>();

        public PlatformFiles_Android()
        {
#if UNITY_2019 || UNITY_2020 || UNITY_2021
            files = new List<File>() { planktonConfig, values, launcherManifest, launcherTemplate, baseProjectTemplate, gradleTemplate, androidManifest };
#elif UNITY_2022
            files = new List<File>() { planktonConfig, values, launcherManifest, launcherTemplate, gradleTemplate, settingsTemplate };
#endif
            LoadFiles(BuildTarget.Android);
        }

#if UNITY_ANDROID
        public override void SaveFiles(bool generateAar)
        {
            UpdatePlaceholder(baseProjectTemplate, "[REPOSITORIES]", repositories, "\n\t\t");
            UpdatePlaceholder(settingsTemplate, "[REPOSITORIES]", repositories, "\n\t\t");
            UpdatePlaceholder(launcherTemplate, "[DEPENDENCIES]", dependencies, "\n\t");
            UpdatePlaceholder(launcherTemplate, "[DEFENITIONS]", defenitions, "\n\t\t");
            base.SaveFiles(generateAar);

            if (generateAar)
                ConvertResToArr();
        }

        public void ConvertResToArr()
        {
            var resFolder = Application.dataPath + "/Plugins/Android/res/";
            var manifest = "<?xml version=\"1.0\" encoding=\"utf-8\"?>\n\n<manifest package=\"com.funtory.resources\"> </manifest>";

            var tempFolder = Application.dataPath + "/Plugins/Android/temp/";
            Directory.CreateDirectory(tempFolder + "/res");

            DirectoryInfo source = new DirectoryInfo(resFolder);
            DirectoryInfo target = new DirectoryInfo(tempFolder + "/res");
            CopyWork(source, target);
            System.IO.File.WriteAllText(tempFolder + "AndroidManifest.xml", manifest);
            var fileList = new List<string>(Directory.GetFiles(tempFolder, "*.*", SearchOption.AllDirectories));
            fileList.RemoveAll(x => x.EndsWith(".meta"));
            for (int i = 0; i < fileList.Count; i++)
                fileList[i] = $"\"{fileList[i].Replace(tempFolder, string.Empty)}\"";
            var files = string.Join(" ", fileList);
            var argument = $"-cvf \"{tempFolder}../resources.aar\" {files}";
            Run("jar", argument, tempFolder);
            Directory.Delete(tempFolder, true);
            Directory.Delete(resFolder, true);
        }

        private static void CopyWork(DirectoryInfo source, DirectoryInfo target)
        {
            foreach (DirectoryInfo dir in source.GetDirectories())
                CopyWork(dir, target.CreateSubdirectory(dir.Name));

            foreach (FileInfo file in source.GetFiles())
                file.CopyTo(Path.Combine(target.FullName, file.Name));
        }


        //////////////////////////////////////////////////////
        /// STATIC MEMBERS
        //////////////////////////////////////////////////////
        static private void Run(string toolName, string arguments, string workingDirectory = null)
        {
            var toolPath = GetToolPath(toolName);
            var process = new System.Diagnostics.Process();
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.Arguments = arguments;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.FileName = toolPath;
            process.StartInfo.WorkingDirectory = workingDirectory ?? Environment.CurrentDirectory;
            var started = process.Start();
            if (!started)
            {
                Debug.LogErrorFormat("Failed to start {0}", process);
                return;
            }
            process.WaitForExit();
        }

        static private string GetToolPath(string toolName)
        {
            return Path.Combine(UnityEditor.Android.AndroidExternalToolsSettings.jdkRootPath, Path.Combine("bin", toolName));
        }
#endif
    }
}