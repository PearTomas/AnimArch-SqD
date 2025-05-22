using System.IO;
using OALProgramControl;
using UnityEngine;
using Visualization;
using Visualization.ClassDiagram;
using Visualization.ClassDiagram.Diagrams;

namespace AnimArch.Visualization.Diagrams
{

    public class PlantUmlGeneratingDiagram : SequenceDiagramBase
    {
        private VisitorCommandToPlantUML visitor = new VisitorCommandToPlantUML();
        private readonly string _startClassName;
        private readonly string _fileKeyHash;
        private IPlantUmlGeneratorAdapter _umlAdapter;

        public PlantUmlGeneratingDiagram(string startClassName, string fileKeyHash)
        {
            _startClassName = startClassName;
            _fileKeyHash = fileKeyHash;
            _umlAdapter = new PlantUmlAdapter();
        }

        private void StartPlantUMLCreation()
        {
            visitor.classNames.Push(_startClassName);
            visitor.AddPlantUmlHeader();
            visitor.AddTransparentBackground();
            visitor.SetArrowColor("white");
        }

        public override void Init()
        {
            StartPlantUMLCreation();
        }

        public override void ToPlantUMLCommand(EXECommand command)
        {
            command.Accept(visitor);
        }

        public override void CreatePlantUMLFile()
        {
            visitor.AddPlantUmlFutter();
            _umlAdapter.GenerateDiagram(visitor.GetCommandString(),
                            Application.dataPath + "/Resources/SequenceDiagrams/",
                            _fileKeyHash,
                            "png");
        }

    }
}