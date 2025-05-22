using System.IO;
using OALProgramControl;
using UnityEngine;
using Visualization.ClassDiagram;
using Visualization.ClassDiagram.Diagrams;

namespace AnimArch.Visualization.Diagrams
{
    public class PlantUmlAdapter : PlantUml, IPlantUmlGeneratorAdapter
    {
        public void GenerateDiagram(string plantUmlContent, string outputPath, string fileName, string outputFormat)
        {
            Debug.Log("[ADAPTER] Generating diagram via PlantUmlExecutorAdapter...");
            GenerateDiagramFromPuml(plantUmlContent, outputPath, fileName, outputFormat);
        }
    }
}
