using System.IO;
using OALProgramControl;
using UnityEngine;
using Visualization.ClassDiagram;
using Visualization.ClassDiagram.Diagrams;

namespace AnimArch.Visualization.Diagrams
{
    public class FactorySequenceDiagram
    {
        public FactorySequenceDiagram()
        {}

        public SequenceDiagramBase CreateSequenceDiagram(string startClassName, string fileKeyHash)
        {
            string path = Application.dataPath + "/Resources/SequenceDiagrams/" + fileKeyHash + ".png";
            if (File.Exists(path))
            {
                SequenceDiagram sequenceDiagram = GameObject.Find("SequenceDiagram").GetComponent<SequenceDiagram>();
                sequenceDiagram.SetParameters(startClassName, fileKeyHash);
                return sequenceDiagram;
            }
           
            Debug.Log("[Factory] PNG not found — PlantUML generation will be started.");
            return new PlantUmlGeneratingDiagram(startClassName, fileKeyHash);
        }

    }
}