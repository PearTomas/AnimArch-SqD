using AnimArch.Visualization.Diagrams;
using UnityEngine;

namespace Visualization.Animation
{
    public class DiagramManager : Singleton<DiagramManager>
    {
        [SerializeField] private float offset;

        public ClassDiagram.Diagrams.ClassDiagram classDiagram;
        public ObjectDiagram objectDiagram;
        public ActivityDiagram activityDiagram;

        public void reposition()
        {

        }
   
    }
}