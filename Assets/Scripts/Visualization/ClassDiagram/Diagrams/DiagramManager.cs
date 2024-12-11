using System;
using System.Collections.Generic;
using AnimArch.Visualization.Diagrams;
using UnityEngine;

namespace Visualization.ClassDiagram.Diagrams
{
    public class DiagramManager : Singleton<DiagramManager>
    {
        [SerializeField] private float Offset;

        [SerializeField] public ClassDiagram classDiagram;
        [SerializeField] public ObjectDiagram objectDiagram;
        [SerializeField] public ActivityDiagram activityDiagram;

        private List<Diagram> diagramList;

        private void Awake()
        {
            diagramList = new List<Diagram>()
            {
                classDiagram,
                activityDiagram,
                objectDiagram
            };
        }

        public void ChangeToQueue()
        {
            Debug.Log("DLC" + diagramList.Count);
            for (var i = 0; i < diagramList.Count; i++)
            {
                var diagram = diagramList[i];
                if (diagram)
                {
                    if (diagram.graph)
                    {
                        diagram.graph.transform.position = new Vector3(0, 0, Offset * i);
                    }
                }
            }
        }

        public void ChangeLayout(Boolean IsGridLayout)
        {
            if (IsGridLayout)
            {
                ChangeToQueue();
            }
            else
            {
                ChangeToGrid();
            }
        }

        private void ChangeToGrid()
        {
            var xOffset = 1800;
            var yOffset = 1800;
            var cnt = 0;
            for (var j = 1; j >= 0; j--)
            {
                for (var i = 1; i >= 0; i--)
                {
                    if (diagramList.Count > cnt)
                    {
                        var diagram = diagramList[cnt];
                        cnt++;
                        if (diagram)
                        {
                            if (diagram.graph)
                            {
                                diagram.graph.transform.position = new Vector3(i * xOffset, j * yOffset, 0);
                            }
                        }
                    }
                }
            }
        }
        
    }
}