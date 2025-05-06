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
        [SerializeField] public SequenceDiagram sequenceDiagram;

        private List<Diagram> diagramList;
        private AbstractLayout layoutState = new QueueLayout();

        private void Awake()
        {
            diagramList = new List<Diagram>()
            {
                classDiagram,
                activityDiagram,
                objectDiagram,
                sequenceDiagram
            };
        }

        public void ChangeLayout()
        {
            layoutState = layoutState.changeLayout(diagramList, Offset);
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