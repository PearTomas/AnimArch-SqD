using OALProgramControl;
using UnityEngine;

namespace Visualization.ClassDiagram.ComponentsInDiagram
{
    public class MessageInDiagram
    {
        public string MessageText;
        public GameObject Message;
        public GameObject Arrow;
        public GameObject ActivationBlockSource;
        public GameObject ActivationBlockDestination;
        public EntityInDiagram SourceEntity;
        public EntityInDiagram DestinationEntity;
        public MessageInDiagram() 
        {}
    }
}
