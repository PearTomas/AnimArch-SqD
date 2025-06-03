using System.IO;
using System;
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
using Animation = Visualization.Animation.Animation;
using Visualization.ClassDiagram.ComponentsInDiagram;
using Visualization.ClassDiagram.Diagrams;
using OALProgramControl;

namespace AnimArch.Visualization.Diagrams
{
    public class SequenceDiagram : SequenceDiagramBase
    {
        private const string OUTPUT_FORMAT = "png";
        private const string OUTPUT_DIR = "/Resources/SequenceDiagrams/";

        private string diagramNameHash;
        private string startClassName;

        public override void Init()
        {
            ResetDiagram();
            var go = Instantiate(DiagramPool.Instance.graphPrefab);
            graph = go.GetComponent<Graph>();
            graph.nodePrefab = DiagramPool.Instance.SequenceDiagramGO;
            LoadGeneratedDiagram();
            graph.Layout();

        }

        private void ResetDiagram()
        {
            if (graph != null)
            {
                Destroy(graph.gameObject);
                graph = null;
            }
        }

        public void LoadGeneratedDiagram()
        {
            var node = graph.AddNode();
            node.SetActive(true);
            var png = node.transform.Find("PNG");
            var spriteChanger = png.GetComponent<SpriteChanger>();


            string fullPath = Application.dataPath + OUTPUT_DIR + diagramNameHash + ".png";

            if (!File.Exists(fullPath))
            {
                Debug.LogError("[RealSequenceDiagram] PNG not found when trying to load: " + fullPath);
                return;
            }
            spriteChanger?.SetSprite(fullPath);

            // Debug.Log("[RealSequenceDiagram] Loading PNG from path: " + fullPath);
            // var spriteChanger = GetComponent<SpriteChanger>();
            // spriteChanger?.SetSprite(fullPath);

            // transform.rotation = Quaternion.Euler(0, 180, 0);
            // transform.localScale *= 5f;
        }

        public void SetParameters(string startClassName, string fileKeyHash)
        {
            this.startClassName = startClassName;
            this.diagramNameHash = fileKeyHash;
        }

        public override void ToPlantUMLCommand(EXECommand command)
        {
        }

        public override void CreatePlantUMLFile()
        {
        }
    }
}
