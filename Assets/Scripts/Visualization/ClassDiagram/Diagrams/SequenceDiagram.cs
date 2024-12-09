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

        // list of entities (contains lifeline)
        public List<EntityInDiagram> Entities { get; private set; }
        public SDMessagePool SDMessagePool;
        private float InitialPositionY;
        private float OffsetX;
        private float OffsetY;
        private float DistanceFromMiddleY;

        private void Awake()
        {
            SDMessagePool = new SDMessagePool();
            SDMessagePool.sequenceDiagram = this;
            InitialPositionY = 300;
            OffsetY = 200;
            OffsetX = 200;
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
    
            graph.transform.position = new Vector3(0, 0, 3*offsetZ);
        }


        private Graph CreateGraph()
        {
            var go = Instantiate(DiagramPool.Instance.graphPrefab);
            graph = go.GetComponent<Graph>();
            graph.nodePrefab = DiagramPool.Instance.sequenceEntityPrefab;
            return graph;
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

            SDMessagePool.LayoutMessagesWithActivationBlocks();
        }

        public void Generate()
        {
            for (int i = 0; i < Entities.Count; i++)
            {
                GenerateObject(Entities[i]);
            }

            SDMessagePool.GenerateMessagesWithActivationBlocks();
        }

        private void GenerateObject(EntityInDiagram Entity)
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

            SDMessagePool.addMessage(Entities[8], Entities[9], "getName()");
        }

        public void GenerateMessage(MessageInDiagram messageInDiagram)
        {
            // activationblockSource
            graph.nodePrefab = messageInDiagram.ActivationBlockSource;
            var node = graph.AddNode();
            messageInDiagram.ActivationBlockSource = node;
            node.SetActive(true);

            // activationblockDestination
            graph.nodePrefab = messageInDiagram.ActivationBlockDestination;
            node = graph.AddNode();
            messageInDiagram.ActivationBlockDestination = node;
            node.SetActive(true);

            // arrowMessage (set text)
            //graph.nodePrefab = messageInDiagram.Arrow;

            
            // var messageText = node.transform.Find("Message");
            // messageText.GetComponent<TextMeshProUGUI>().text = messageInDiagram.MessageText;
        }

        public void generateArrow(MessageInDiagram messageInDiagram){
            var edge = graph.AddEdge(messageInDiagram.ActivationBlockSource, messageInDiagram.ActivationBlockDestination,  messageInDiagram.Arrow);
            
            // var uEdge = edge.GetComponent<UEdge>();
            // uEdge.Points = new Vector2[]
            // {
            //     messageInDiagram.ActivationBlockSource.transform.position,
            //     messageInDiagram.ActivationBlockDestination.transform.position
            // };
            // graph.nodePrefab = messageInDiagram.Arrow;
            // node = graph.AddNode();
            // messageInDiagram.Arrow = node;
            // node.SetActive(true);
        }
    }
}