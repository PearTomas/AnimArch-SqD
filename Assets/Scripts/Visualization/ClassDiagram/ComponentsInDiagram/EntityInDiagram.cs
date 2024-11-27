using OALProgramControl;
using UnityEngine;

namespace Visualization.ClassDiagram.ComponentsInDiagram
{
    public class EntityInDiagram
    {
        public string EntityName;
        public ClassInDiagram Class;
        public CDClassInstance Instance;
        public GameObject VisualObjectHeader;
        public GameObject VisualObjectFooter;
        public GameObject LifeLine;

        public EntityInDiagram() 
        {}
    }
}
