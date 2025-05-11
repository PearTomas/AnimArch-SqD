using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Debug = UnityEngine.Debug;

namespace AnimArch.Visualization.Diagrams
{
    public class PlantUmlExecutor
    {
        private const string TEMP_DIRECTORY_PATH = "./Temp/PlantUML/";
        private const string JAR_SEARCH_DIRECTORY = "./Assets/JavaTools/PlantUML/";
        private readonly string jarPath;
        private bool disposed;

        public PlantUmlExecutor(string plantUmlJarPath = null)
        {
            Directory.CreateDirectory(TEMP_DIRECTORY_PATH);

            if (string.IsNullOrEmpty(plantUmlJarPath))
            {
                jarPath = FindLatestPlantUmlJar(JAR_SEARCH_DIRECTORY);
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
                    $"And place it in: {JAR_SEARCH_DIRECTORY}"
                );
            }
        }

        ~PlantUmlExecutor()
        {
            Dispose();
        }

        public string Execute(string plantUmlContent,string pngPath, string pngFileName, string outputFormat)
        {
            // save the PlantUML content to a temporary file
            var pumlPath = Path.Combine(TEMP_DIRECTORY_PATH, $"{pngFileName}.puml");
            File.WriteAllText(pumlPath, plantUmlContent);

            // generate the sqd diagram
            var outPath = Path.Combine(pngPath, $"{pngFileName}.{outputFormat}");
            RunPlantUml(pumlPath, outPath, $"-t{outputFormat}");

            // delete the puml temporary file
            //File.Delete(pumlPath);
            return outPath;
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
                    string output = process.StandardOutput.ReadToEnd();
                    string error = process.StandardError.ReadToEnd();

                    process.WaitForExit();

                    // Display the output and error (if any)
                    Console.WriteLine(output);
                    if (!string.IsNullOrEmpty(error))
                    {
                        Console.WriteLine("Error: " + error);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
            }
        }

        public static string GenerateHash(string input)
        {
            using var sha = SHA1.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(input));
            return BitConverter.ToString(bytes).Replace("-", "").ToLowerInvariant();
        }

        public void Dispose()
        {
            if (disposed) return;
            if (Directory.Exists(TEMP_DIRECTORY_PATH))
                //Directory.Delete(TEMP_DIRECTORY_PATH, true);
            disposed = true;
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

            return files[0]; // Most recently updated
        }

        
    }
}