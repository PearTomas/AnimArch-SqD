using System.IO;
using OALProgramControl;
using UnityEngine;
using Visualization.ClassDiagram;
using Visualization.ClassDiagram.Diagrams;

namespace AnimArch.Visualization.Diagrams
{
    public class SequenceDiagram : Diagram
    {
        const string OUTPUT_FORMAT_PNG = "png";
        const string OUTPUT_FILE_DIR = "/Resources/SequenceDiagrams/";
        
        string sqDiagramName = null;
        
        private VisitorCommandToPlantUML visitor;
        private string FileNameOfPlantUMLText;

        private void Awake()
        {
            DiagramPool.Instance.SequenceDiagram = this;
            ResetDiagram();
            visitor = new VisitorCommandToPlantUML();
        }

        public void ResetDiagram()
        {
            if (graph != null)
            {
                Destroy(graph.gameObject);
                graph = null;
            }
        }

        private void LoadDiagram(string sequenceDiaramPngPath)
        {
            Debug.Log("[PLANTUML] Loading diagram from path: " + sequenceDiaramPngPath);
            DiagramPool.Instance.SequenceDiagram.gameObject.GetComponent<SpriteChanger>().SetSprite(sequenceDiaramPngPath);
            DiagramPool.Instance.SequenceDiagram.gameObject.transform.rotation = Quaternion.Euler(0, 180, 0);
            DiagramPool.Instance.SequenceDiagram.gameObject.transform.localScale *= 5f;
        }

        private void StartPlantUMLCreation(string initialClassName)
        {
            visitor.classNames.Push(initialClassName);
            visitor.AddPlantUmlHeader();
            visitor.AddTransparentBackground();
            visitor.SetArrowColor("white");
        }

        public void ToPlantUMLCommand(EXECommand CurrentCommand)
        {
            CurrentCommand.Accept(visitor);
        }
        
        private string CreatePlantUmlPng()
        {
            PlantUmlExecutor plantUmlExecutor = new PlantUmlExecutor();
            return plantUmlExecutor.Execute(visitor.GetCommandString(), 
                Application.dataPath + OUTPUT_FILE_DIR,
                sqDiagramName,
                OUTPUT_FORMAT_PNG);
        }
        
        public void CreatePlantUMLFile()
        {
            // end plant uml
            visitor.AddPlantUmlFutter();
            // get plant uml content
            CreatePlantUmlPng();
        }
        
        public void SetDiagramName(string name)
        {
            sqDiagramName = name;
        }
        
        public void init(string startClassName, string generateJoinedFileNameForSeqD)
        {
            SetDiagramName(generateJoinedFileNameForSeqD);
            ResetDiagram();

            sqDiagramName = PlantUmlExecutor.GenerateHash(generateJoinedFileNameForSeqD);
            
            string sequenceDiaramPngPath = Application.dataPath + OUTPUT_FILE_DIR + sqDiagramName + ".png";
            
            if (File.Exists(sequenceDiaramPngPath))
            {
                LoadDiagram(sequenceDiaramPngPath);
            }
            else
            {
                StartPlantUMLCreation(startClassName);
            }
        }
    }
}