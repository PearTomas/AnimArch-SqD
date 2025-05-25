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

        public virtual AbstractLayout changeType()
        {
            throw new NotImplementedException();
        }

        public virtual void loopDiagrams(List<Diagram> diagramList, float offset)
        {
            throw new NotImplementedException();
        }

        public virtual void preprocesing(List<Diagram> diagramList) {}
        
    }
}