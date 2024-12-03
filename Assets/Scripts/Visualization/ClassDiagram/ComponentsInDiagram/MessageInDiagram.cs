using OALProgramControl;
using UnityEngine;

namespace Visualization.ClassDiagram.ComponentsInDiagram
{
    public class MessageInDiagram
    {
        public string MessageText;
        public GameObject VisualObjectArrow;
        public GameObject ActivationBlockSource;
        public GameObject ActivationBlockDestination;
        public EntityInDiagram SourceEntity;
        public EntityInDiagram DestinationEntity;
        public MessageInDiagram() 
        {}
    }
}
