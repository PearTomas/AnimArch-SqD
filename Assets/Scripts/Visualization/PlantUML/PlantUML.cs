using System;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace DefaultNamespace
{
    /// <summary>
    /// Represents a PlantUML diagram generator.
    /// </summary>
    public class PlantUML
    {
        /// <summary>
        /// The output format for PNG files.
        /// </summary>
        const string OUTPUT_FORMAT_PNG = "png";
        /// <summary>
        /// The output format for SVG files.
        /// </summary>
        const string OUTPUT_FORMAT_SVG = "svg";
        /// <summary>
        /// The path to the PlantUML temporary project directory. (No windows tmp. path)
        /// </summary>
        const string TEMP_DIRECTORY_PATH = "./Temp/PlantUML";

        /// <summary>
        /// The path to the PlantUML JAR file.
        /// </summary>
        private string plantUmlJarPath;

        /// <summary>
        /// Initializes a new instance of the <see cref="PlantUML"/> class with the default JAR path.
        /// </summary>
        public PlantUML()
        {
            this.plantUmlJarPath = "./Assets/lib/plantuml-1.2025.2.jar";
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PlantUML"/> class with a specified JAR path.
        /// </summary>
        /// <param name="plantUmlJarPath">The path to the PlantUML JAR file.</param>
        public PlantUML(string plantUmlJarPath)
        {
            this.plantUmlJarPath = plantUmlJarPath;
        }
        
        ~PlantUML()
        {
            DeleteTempDirectory();
        }

        /// <summary>
        /// Generates a PNG sequence diagram from PlantUML content.
        /// </summary>
        /// <param name="plantUmlContent">The PlantUML content.</param>
        /// <returns>The path to the generated PNG file.</returns>
        public string GeneratePngSequenceDiagramFromPlantUml(string plantUmlContent)
        {
            return GenerateSequenceDiagramFromPlantUmlContent(plantUmlContent, OUTPUT_FORMAT_PNG);
        }

        /// <summary>
        /// Generates an SVG sequence diagram from PlantUML content.
        /// </summary>
        /// <param name="plantUmlContent">The PlantUML content.</param>
        /// <returns>The path to the generated SVG file.</returns>
        public string GenerateSvgSequenceDiagramFromPlantUml(string plantUmlContent)
        {
            return GenerateSequenceDiagramFromPlantUmlContent(plantUmlContent, OUTPUT_FORMAT_SVG);
        }

        /// <summary>
        /// Generates a sequence diagram from PlantUML content.
        /// </summary>
        /// <param name="plantUmlContent">The PlantUML content.</param>
        /// <param name="outputFormat">The output format (e.g., "png" or "svg").</param>
        /// <returns>The path to the generated diagram file.</returns>
        private string GenerateSequenceDiagramFromPlantUmlContent(string plantUmlContent, string outputFormat)
        {
            // Generate a hash for the file name
            string hash = GenerateHash(plantUmlContent);

            // Save the PlantUML content to a temporary file
            string pumlFilePath = SaveToTempDirectory(plantUmlContent, hash);

            // Define the path to the output diagram file
            string diagramFilePath = Path.Combine(TEMP_DIRECTORY_PATH, $"{hash}.{outputFormat}");

            // Generate the diagram
            GenerateDiagram(pumlFilePath, diagramFilePath, $"-t{outputFormat}");

            // Delete the .puml file
            if (File.Exists(pumlFilePath))
            {
                File.Delete(pumlFilePath);
            }

            return diagramFilePath;
        }

        /// <summary>
        /// Generates a diagram using the PlantUML JAR file.
        /// </summary>
        /// <param name="inputFilePath">The path to the input .puml file.</param>
        /// <param name="outputFilePath">The path to the output diagram file.</param>
        /// <param name="outputFormat">The output format (e.g., "-tpng" or "-tsvg").</param>
        private void GenerateDiagram(string inputFilePath, string outputFilePath, string outputFormat)
        {
            // Initialize the process start info
            ProcessStartInfo processStartInfo = new ProcessStartInfo
            {
                FileName = "java",
                Arguments = $"-jar \"{this.plantUmlJarPath}\" {outputFormat} \"{inputFilePath}\" -o \"{outputFilePath}\"",
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

        /// <summary>
        /// Saves the PlantUML content to a temporary directory.
        /// </summary>
        /// <param name="plantUmlContent">The PlantUML content.</param>
        /// <param name="fileName">The file name.</param>
        /// <returns>The path to the saved .puml file.</returns>
        private string SaveToTempDirectory(string plantUmlContent, string fileName)
        {
            // Ensure the temp directory exists
            Directory.CreateDirectory(TEMP_DIRECTORY_PATH);

            string filePath = Path.Combine(TEMP_DIRECTORY_PATH, $"{fileName}.puml");

            // Save the content to the file
            File.WriteAllText(filePath, plantUmlContent);

            return filePath;
        }

        /// <summary>
        /// Deletes the temporary directory.
        /// </summary>
        private void DeleteTempDirectory()
        {
            if (Directory.Exists(TEMP_DIRECTORY_PATH))
            {
                Directory.Delete(TEMP_DIRECTORY_PATH, true);
            }
        }

        /// <summary>
        /// Generates a hash for the given input string.
        /// </summary>
        /// <param name="input">The input string.</param>
        /// <returns>The generated hash.</returns>
        private string GenerateHash(string input) // TODO: shall be in diffrent class ?!
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
                StringBuilder hashStringBuilder = new StringBuilder();
                foreach (byte b in hashBytes)
                {
                    hashStringBuilder.Append(b.ToString("x2"));
                }
                return hashStringBuilder.ToString();
            }
        }
    }
}