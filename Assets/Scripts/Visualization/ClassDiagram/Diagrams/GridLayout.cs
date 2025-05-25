using System;
using System.Collections.Generic;
using UnityEngine;

namespace Visualization.ClassDiagram.Diagrams
{
    public class GridLayout : AbstractLayout
    {
        private static AbstractLayout otherLayout = new QueueLayout();
        private float rows;
        private float cols;

        public override void preprocesing(List<Diagram> diagramList)
        {
            rows = (int)Math.Ceiling(Math.Sqrt(diagramList.Count));
            cols = (int)Math.Ceiling((double)diagramList.Count / rows);
        }

        public override void loopDiagrams(List<Diagram> diagramList, float offset)
        {
            var diagramIndex = 0;
            
            for (var j = 0; j < rows; j++)
            {
                for (var i = 0; i < cols; i++)
                {
                    if (diagramIndex >= diagramList.Count)
                        return;
                    var diagram = diagramList[diagramIndex++];
                    if (!diagram || !diagram.graph)
                        return;
                    
                    diagram.graph.transform.position = new Vector3(i * offset, j * offset, 0);
                }
            }
        }

        public override AbstractLayout changeType()
        {
            return otherLayout;
        }
    }
}