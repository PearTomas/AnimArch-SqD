using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Debug = UnityEngine.Debug;
using UnityEngine;
using Assets.Scripts.Util.IO;

namespace AnimArch.Visualization.Diagrams
{
    public class PlantUml
    {
        private const string TEMP_DIRECTORY_PATH = "./Temp/PlantUML/";
        private const string JAR_SEARCH_DIRECTORY = "/JavaTools/PlantUML/";
        private readonly string jarPath;
        private bool disposed;
        private TempDirectory tempDirectory;

        public PlantUml(string plantUmlJarPath = null)
        {
            if (string.IsNullOrEmpty(plantUmlJarPath))
            {
                jarPath = FindLatestPlantUmlJar(Application.dataPath + JAR_SEARCH_DIRECTORY);
                Debug.Log("[PLANTUML] Java Jar Path found: " + jarPath);
            }
            else
            {
                jarPath = plantUmlJarPath;
            }

            if (string.IsNullOrEmpty(jarPath) || !File.Exists(jarPath))
            {
                Debug.LogError(
                    $"[PLANTUML] PlantUML .jar not found.\n" +
                    $"Download from: https://github.com/plantuml/plantuml/releases\n" +
                    $"and place it in to: {Application.dataPath + JAR_SEARCH_DIRECTORY}"
                );
            }
        }
        ~PlantUml()
        {
        }
        
        public void GenerateDiagramFromPuml(string plantUmlContent,string pngPath, string pngFileName, string outputFormat)
        {
            // create a temporary directory
            new TempDirectory(TEMP_DIRECTORY_PATH);
            // save the PlantUML content to a temporary file
            var pumlPath = Path.Combine(TEMP_DIRECTORY_PATH, $"{pngFileName}.puml");
            File.WriteAllText(pumlPath, plantUmlContent);
            
            RunPlantUml(pumlPath, pngPath, $"-t{outputFormat}");
        }

        private void RunPlantUml(string inputFilePath, string outputFilePath, string outputFormat)
        {
            // Initialize the process start info
            ProcessStartInfo processStartInfo = new ProcessStartInfo
            {
                FileName = "java",
                Arguments = $"-jar \"{jarPath}\" {outputFormat} \"{inputFilePath}\" -o \"{outputFilePath}\"",
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            try
            {
                // Start the process
                using (Process process = new Process { StartInfo = processStartInfo })
                {
                    process.Start();

                    // Read the standard output and error
                    string error = process.StandardError.ReadToEnd();

                    process.WaitForExit();

                    // Display the output and error (if any)
                    if (!string.IsNullOrEmpty(error))
                    {
                        Debug.LogError("[PLANTUML] error when generating SqD:" + error);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError("[PLANTUML] An error occurred: " + ex.Message);
            }
        }

        public static string GenerateHash(string input)
        {
            using var sha = SHA1.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(input));
            return BitConverter.ToString(bytes).Replace("-", "").ToLowerInvariant();
        }
        
        private string FindLatestPlantUmlJar(string directory)
        {
            if (!Directory.Exists(directory))
            {
                Debug.LogError($"[PLANTUML] Directory not found: {directory}");
                return null;
            }

            var files = Directory.GetFiles(directory, "*.jar", SearchOption.TopDirectoryOnly)
                .Where(path => Path.GetFileName(path).ToLower().Contains("plantuml"))
                .OrderByDescending(File.GetLastWriteTime)
                .ToList();

            if (files.Count == 0)
            {
                Debug.LogError("[PLANTUML] No plantuml-related .jar files found in: " + directory);
                return null;
            }

            return files[0];
        }
    }
}