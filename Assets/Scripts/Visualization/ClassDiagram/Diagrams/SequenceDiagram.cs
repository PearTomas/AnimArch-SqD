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
    public class SequenceDiagram : Diagram
    {
        public Graph graph;
        public List<EntityInDiagram> Entities { get; private set; }
        public SeqDMessagePool SeqDMessagePool;
        private float InitialPositionY = 300;
        private float OffsetX = 200;
        private float OffsetY = 200;
        private float OffsetZ = 800;

        private void Awake()
        {
            SeqDMessagePool = new SeqDMessagePool();
            SeqDMessagePool.sequenceDiagram = this;
            DiagramPool.Instance.SequenceDiagram = this;
            Entities = new List<EntityInDiagram>();
            ResetDiagram();
        }

        public void ResetDiagram()
        {
            if (graph != null)
            {
                Destroy(graph.gameObject);
                graph = null;
            }
        }

        public void LoadDiagram()
        {
            CreateGraph();
            Generate();
            ManualLayout();
        }

        private Graph CreateGraph()
        {
            var go = Instantiate(DiagramPool.Instance.graphPrefab);
            graph = go.GetComponent<Graph>();
            graph.nodePrefab = DiagramPool.Instance.sequenceEntityPrefab;
            return graph;
        }

        public void Generate()
        {
            for (int i = 0; i < Entities.Count; i++)
            {
                GenerateEntity(Entities[i]);
            }

            SeqDMessagePool.GenerateMessagesWithActivationBlocksAndArrows();
        }

        private void GenerateEntity(EntityInDiagram Entity)
        {
            //Lifeline
            graph.nodePrefab = DiagramPool.Instance.sequenceLinePrefab;
            var node = graph.AddNode();
            Entity.LifeLine = node;
            node.SetActive(true);

            //Header
            graph.nodePrefab = DiagramPool.Instance.sequenceEntityPrefab;
            node = graph.AddNode();
            Entity.VisualObjectHeader = node;
            node.SetActive(true);
            node.name = Entity.EntityName;

            EntityTextSetter textSetter = node.GetComponent<EntityTextSetter>();
            textSetter.setText(node.name);

            //Footer
            node = graph.AddNode();
            Entity.VisualObjectFooter = node;
            node.SetActive(true);
            node.name = Entity.EntityName;
            
            textSetter = node.GetComponent<EntityTextSetter>();
            textSetter.setText(node.name);
        }

        public void ManualLayout()
        {
            int i = 0;
            foreach (EntityInDiagram entityInDiagram in Entities)
            {
                entityInDiagram.LifeLine.transform.SetPositionAndRotation(
                    new Vector3(i * OffsetX, InitialPositionY - OffsetY, 0), 
                    Quaternion.identity);

                entityInDiagram.VisualObjectHeader.transform.SetPositionAndRotation(
                    new Vector3(i * OffsetX, InitialPositionY, 0), 
                    Quaternion.identity);

                entityInDiagram.VisualObjectFooter.transform.SetPositionAndRotation(
                    new Vector3(i * OffsetX, InitialPositionY - 2 * OffsetY, 0), 
                    Quaternion.identity);
                i++;
            }

            SeqDMessagePool.LayoutMessagesWithActivationBlocks();
            graph.transform.position = new Vector3(0, 0, 3*OffsetZ);
        }

        public void AddEntities(List<CDClass> Classes)
        {
            Entities = new List<EntityInDiagram>();
            foreach(CDClass entity in Classes)
            {
                EntityInDiagram entityInDiagram = new EntityInDiagram
                {
                    VisualObjectHeader = DiagramPool.Instance.sequenceEntityPrefab,
                    VisualObjectFooter = DiagramPool.Instance.sequenceEntityPrefab,
                    LifeLine = DiagramPool.Instance.sequenceLinePrefab,
                    EntityName = entity.Name
                };
            Entities.Add(entityInDiagram);
            }

            // example of message
            SeqDMessagePool.addMessage(Entities[8], Entities[9], "getName()");
        }
    }
}