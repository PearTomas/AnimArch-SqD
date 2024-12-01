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
                objectDiagram,
                activityDiagram
            };
        }

        public void Reposition()
        {
            for (var i = 0; i < diagramList.Count; i++)
            {
                var diagram = diagramList[i];
                if (diagram)
                {
                    if (diagram.graph)
                    {
                        Debug.Log("DiagramManager: diagram graph not Null");
                        diagram.graph.transform.position = new Vector3(0, 0, Offset * i);
                        
                    }
                }
            }
        }

        public static void StaticReposition()
        {
            Instance.Reposition();
        }
        
    }
}