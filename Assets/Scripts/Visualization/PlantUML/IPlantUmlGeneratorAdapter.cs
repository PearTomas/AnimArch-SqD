using System.IO;
using OALProgramControl;
using UnityEngine;
using Visualization.ClassDiagram;
using Visualization.ClassDiagram.Diagrams;

namespace AnimArch.Visualization.Diagrams
{
    public interface IPlantUmlGeneratorAdapter
    {
        void GenerateDiagram(string plantUmlContent, string outputPath, string fileName, string outputFormat);
    }
}
