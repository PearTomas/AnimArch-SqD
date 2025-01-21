using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AnimArch.Visualization.Diagrams;
using Assets.Scripts.AnimationControl.OAL;
using OALProgramControl;
using TMPro;
using UMSAGL.Scripts;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI.Extensions;
using Visualisation.Animation;
using Visualization.ClassDiagram;
using Visualization.ClassDiagram.ClassComponents;
using Visualization.ClassDiagram.ComponentsInDiagram;
using Visualization.ClassDiagram.Diagrams;
using Visualization.ClassDiagram.Relations;
using Visualization.UI;
using AnimArch.Extensions;
using UnityEngine.AI;

namespace OALProgramControl
{
    public class SeqDMessagePool
    {
        public SequenceDiagram sequenceDiagram;
        public Graph graph;
        public List<MessageInDiagram> Messages { get; private set; }
        public SeqDMessagePool() 
        {
            Messages = new List<MessageInDiagram>();
        }

        public void addMessage(EntityInDiagram EntitySource, EntityInDiagram EntityDestination, string Message)
        {
            MessageInDiagram messageInDiagram = new MessageInDiagram
            {
                MessageText = Message,
                Message = DiagramPool.Instance.sequenceMessage,
                Arrow = DiagramPool.Instance.associationFullPrefab,
                ActivationBlockSource = DiagramPool.Instance.sequenceActivationBlock,
                ActivationBlockDestination = DiagramPool.Instance.sequenceActivationBlock,
                SourceEntity = EntitySource,
                DestinationEntity = EntityDestination
            };
            Messages.Add(messageInDiagram);

        }

        public void GenerateMessagesWithActivationBlocksAndArrows()
        {
            for (int i = 0; i < Messages.Count; i++)
            {
                GenerateMessage(Messages[i]);
            } 
        }

        public void GenerateMessage(MessageInDiagram messageInDiagram)
        {
            graph = sequenceDiagram.graph;

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

            generateArrow(messageInDiagram);
        }

        public void generateArrow(MessageInDiagram messageInDiagram)
        {
            var edge = graph.AddEdgeSeq(messageInDiagram.ActivationBlockSource, messageInDiagram.ActivationBlockDestination,  messageInDiagram.Arrow);
            
            var uEdge = edge.GetComponent<UEdge>();
            uEdge.Points = new Vector2[]
            {
                messageInDiagram.ActivationBlockSource.transform.position,
                messageInDiagram.ActivationBlockDestination.transform.position
            };
            // graph.nodePrefab = messageInDiagram.Arrow;
            // node = graph.AddNode();
            // messageInDiagram.Arrow = node;
            // node.SetActive(true);
        }

        public void LayoutMessagesWithActivationBlocks()
        {
            foreach (MessageInDiagram messageInDiagram in Messages) {
                
                float X1 = messageInDiagram.SourceEntity.LifeLine.transform.position.x;
                float Y = messageInDiagram.SourceEntity.VisualObjectHeader.transform.position.y;
                messageInDiagram.ActivationBlockSource.transform.SetPositionAndRotation(
                    new Vector3(X1,Y-150, 0), 
                    Quaternion.identity
                );

                float X2 = messageInDiagram.DestinationEntity.LifeLine.transform.position.x;
                Y = messageInDiagram.DestinationEntity.VisualObjectHeader.transform.position.y;
                messageInDiagram.ActivationBlockDestination.transform.SetPositionAndRotation(
                    new Vector3(X2, Y-150, 0), 
                    Quaternion.identity
                );

                // messageInDiagram.Arrow.transform.SetPositionAndRotation(
                //     new Vector3((X1+X2)/2, Y-150, 0), 
                //     Quaternion.identity
                // );
            }
        }
    }
}
