public class PlantUmlExecutor
{
    private const string TEMP_DIRECTORY_PATH = "./Temp/PlantUML";
    private readonly string jarPath;
    private bool disposed;

    public PlantUmlExecutor(string plantUmlJarPath = "./Assets/lib/plantuml-1.2025.2.jar")
    {
        jarPath = plantUmlJarPath;
        Directory.CreateDirectory(TEMP_DIRECTORY_PATH);
    }
    
    public ~PlantUmlExecutor()
    {
        Dispose();
    }

    public string Execute(string plantUmlContent, string outputFormat)
    {
        // save the PlantUML content to a temporary file
        var hash = GenerateHash(plantUmlContent);
        var pumlPath = Path.Combine(TEMP_DIRECTORY_PATH, $"{hash}.puml");
        File.WriteAllText(pumlPath, plantUmlContent);

        // generate the sqd diagram
        var outPath = Path.Combine(TEMP_DIRECTORY_PATH, $"{hash}.{outputFormat}");
        RunPlantUml(pumlPath, outPath, $"-t{outputFormat}");

        // delete the puml temporary file
        File.Delete(pumlPath);
        return outPath;
    }
    
    private void RunPlantUml(string inputFilePath, string outputFilePath, string outputFormat)
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

    private static string GenerateHash(string input)
    {
        using var sha = SHA256.Create();
        var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(input));
        return BitConverter.ToString(bytes).Replace("-", "").ToLowerInvariant();
    }

    public void Dispose()
    {
        if (disposed) return;
        if (Directory.Exists(TEMP_DIRECTORY_PATH))
            Directory.Delete(TEMP_DIRECTORY_PATH, true);
        disposed = true;
    }
}