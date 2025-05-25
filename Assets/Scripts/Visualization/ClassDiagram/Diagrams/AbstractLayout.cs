using System;
using System.Collections.Generic;
using Codice.Client.Commands;

namespace Visualization.ClassDiagram.Diagrams
{
    public abstract class AbstractLayout
    {
        public AbstractLayout changeLayout(List<Diagram> diagramList, float offset)
        {
            preprocesing(diagramList);
            loopDiagrams(diagramList, offset);
            return changeType();
        }

        public abstract AbstractLayout changeType();

        public abstract void loopDiagrams(List<Diagram> diagramList, float offset);

        public abstract void preprocesing(List<Diagram> diagramList);
    }
}