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

            var bounds = new Bounds();
            bool hasInitBounds = false;
            foreach (var diagram in diagramList)
            {
                if (!diagram || !diagram.graph)
                    return;

                if (!hasInitBounds)
                {
                    bounds = new Bounds(diagram.graph.transform.position, Vector3.zero);
                    hasInitBounds = true;
                }
                else
                {
                    bounds.Encapsulate(diagram.graph.transform.position);
                }

                bounds.Encapsulate(diagram.graph.transform.position);

            }

            // Center camera on X/Y but keep original Z position
            camera.transform.position = new Vector3(
                bounds.center.x,
                bounds.center.y,
                camera.transform.position.z // Maintain original Z position
            );

            // Calculate required zoom level based on diagram bounds
            float aspectRatio = camera.aspect;
            float diagramWidth = bounds.size.x;
            float diagramHeight = bounds.size.y;

            // Calculate required size based on both width and height
            float sizeBasedOnWidth = diagramWidth / (2 * aspectRatio);
            float sizeBasedOnHeight = diagramHeight / 2;

            // Use whichever requires more zoom-out
            camera.orthographicSize = Mathf.Max(sizeBasedOnWidth, sizeBasedOnHeight);

            const float paddingFactor = 1.1f;
            camera.orthographicSize *= paddingFactor;
        }
    }
}