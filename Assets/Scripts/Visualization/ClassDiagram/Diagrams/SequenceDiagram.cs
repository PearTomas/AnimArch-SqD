using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OALProgramControl;
using TMPro;
using UMSAGL.Scripts;
using UnityEngine;
using Visualization;
using Visualization.Animation;
using Visualization.ClassDiagram;
using Visualization.ClassDiagram.Diagrams;
using OALProgramControl;
using Assets.Scripts.AnimationControl;
using AnimArch.Encryption;


namespace AnimArch.Visualization.Diagrams
{
    public class SequenceDiagram : Diagram
    {
        const string OUTPUT_FORMAT_PNG = "png";
        const string OUTPUT_FILE_DIR = "/Resources/SequenceDiagrams/";
        
        string sqDiagramName = null;
        
        private VisitorCommandToPlantUML visitor;
        private string FileNameOfPlantUMLText;

        private bool fileExists = false;

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
            //CreateGraph(); TODO Delete if not needed
            // Generate(); TODO Delete if not needed
            //ManualLayout(); TODO Delete if not needed
        }

        private Graph CreateGraph() // TODO Delete if not needed
        { 
            var go = Instantiate(DiagramPool.Instance.graphPrefab);
            graph = go.GetComponent<Graph>();
            graph.nodePrefab = DiagramPool.Instance.sequenceEntityPrefab;
            return graph;
        }

        private void SetFileExists()
        {
            string path = Application.dataPath + "/PlantUMLs/" + FileNameOfPlantUMLText + ".svg";
            if (File.Exists(path)) {
              fileExists = true;  
            }
        }

        private void StartPlantUMLCreation(string initialClassName)
        {
            //SetFileExists();
            visitor.classNames.Push(initialClassName);
            visitor.StartPlantUml();
        }

        public void ToPlantUMLCommand(EXECommand CurrentCommand)
        {
            CurrentCommand.Accept(visitor);
        }
        
        private string CreatePlantUmlPng()
        {
            PlantUmlExecutor plantUmlExecutor = new PlantUmlExecutor();
            return plantUmlExecutor.Execute(visitor.GetCommandString(), 
                OUTPUT_FILE_DIR,
                sqDiagramName,
                OUTPUT_FORMAT_PNG);
        }
        
        public void CreatePlantUMLFile()
        {
            // end plant uml
            visitor.EndPlantUml();
            // get plant uml content
            string pngPath = CreatePlantUmlPng();
            
            // get sprite from PNG
            //DiagramPool.Instance.SequenceDiagram.gameObject.GetComponent<SpriteChanger>().SetSprite("C:\\Users\\Tomas\\Documents\\diagram.png");
        }
        
        public void Generate() // TODO Delete if not needed
        {
            //  graph.nodePrefab = messageInDiagram.Arrow;
            // node = graph.AddNode();
            // messageInDiagram.Arrow = node;
            // var messageText = node.transform.Find("Message");
            // messageText.GetComponent<TextMeshProUGUI>().text = messageInDiagram.MessageText;
        }

        public void SetDiagramName(string name)
        {
            sqDiagramName = name;
        }
        
        public void ManualLayout() // TODO Delete if not needed
        {
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