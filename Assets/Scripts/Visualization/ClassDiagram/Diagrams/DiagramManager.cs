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
        
        private void ChangeToGrid()
        {
            var xOffset = 1800;
            var yOffset = 1800;
            var diagramIndex = 0;
            var rows = (int)Math.Ceiling(Math.Sqrt(diagramList.Count));
            var cols = (int)Math.Ceiling((double)diagramList.Count / rows);

            for (var j = 0; j < rows; j++)
            {
                for (var i = 0; i < cols; i++)
                {
                    if (diagramIndex >= diagramList.Count)
                        return;
                    var diagram = diagramList[diagramIndex++];
                    if (!diagram || !diagram.graph)
                        return;
                    
                    // set grid position for diagrams in the list.
                    diagram.graph.transform.position = new Vector3(i * xOffset, j * yOffset, 0);
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
            PinCamToDiagramLayout();
        }

        private void PinCamToDiagramLayout()
        {
            var camera = Camera.main;
            if (camera == null) return;

            var bounds = new Bounds(Vector3.zero, Vector3.zero);
            foreach (var diagram in diagramList)
            {
                if (!diagram || !diagram.graph)
                    return;
                
                bounds.Encapsulate(diagram.graph.transform.position);
                
            }
            
            // TODO: eatch call of this method the camera is moved to back of couple of px, with cause that eventually
            // it will be moved to far from the diagram (it is no longer visible)
            const float cameraPadding = 1f;
            camera.transform.position = 
                new Vector3(
                    bounds.center.x, bounds.center.y, camera.transform.position.z) - 
                    camera.transform.forward * (Mathf.Max(bounds.size.x, bounds.size.y) / cameraPadding
                );
            camera.orthographicSize = Mathf.Max(bounds.size.x, bounds.size.y) / 2;
        }
    }
}