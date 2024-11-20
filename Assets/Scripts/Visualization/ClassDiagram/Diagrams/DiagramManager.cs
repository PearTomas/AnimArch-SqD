using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AnimArch.Visualization.Diagrams;
using Assets.Scripts.AnimationControl.OAL;
using OALProgramControl;
using TMPro;
using UMSAGL.Scripts;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI.Extensions;
using Visualisation.Animation;
using Visualization.ClassDiagram;
using Visualization.ClassDiagram.ClassComponents;
using Visualization.ClassDiagram.ComponentsInDiagram;
using Visualization.ClassDiagram.Diagrams;
using Visualization.ClassDiagram.Relations;
using Visualization.UI;
using AnimArch.Extensions;
using UnityEngine.AI;

namespace Visualization.Animation
{
    public class DiagramManager : Singleton<DiagramManager>
    {
        public float classOffsetZ = 0;
        public float objectOffsetZ = 800;
        public float activityOffsetZ = 1600;

        public ClassDiagram.Diagrams.ClassDiagram classDiagram { get; private set; }
        public ObjectDiagram objectDiagram { get; private set; }
        public ActivityDiagram activityDiagram { get; private set; }

        private void Awake()
        {
            classDiagram = GameObject.Find("ClassDiagram").GetComponent<ClassDiagram.Diagrams.ClassDiagram>();
            objectDiagram = GameObject.Find("ObjectDiagram").GetComponent<ObjectDiagram>();
            activityDiagram = GameObject.Find("ActivityDiagram").GetComponent<ActivityDiagram>();

            classDiagram.setZOffset(classOffsetZ);
            objectDiagram.setZOffset(objectOffsetZ);
            activityDiagram.setZOffset(activityOffsetZ);
        }
   
    }
}