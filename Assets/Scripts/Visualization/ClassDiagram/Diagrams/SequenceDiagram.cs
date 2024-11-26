using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OALProgramControl;
using TMPro;
using UMSAGL.Scripts;
using UnityEngine;
using Visualization;
using Visualization.Animation;
using Visualization.ClassDiagram;
using Animation = Visualization.Animation.Animation;
using Visualization.ClassDiagram.ComponentsInDiagram;
using Visualization.ClassDiagram.Diagrams;


namespace AnimArch.Visualization.Diagrams
{
    public class SequenceDiagram : Diagram
    {
        public Graph graph;

        // list of entities (contains lifeline)
        public List<EntityInDiagram> Entities { get; private set; }


        private float initialActivityPositionY;
        private float initialActivityPositionZ;
        private float activityOffsetX = 70;

        private void Awake()
        {
            initialActivityPositionY = 100;
            initialActivityPositionZ = 100;
            DiagramPool.Instance.SequenceDiagram = this;
            Entities = new List<EntityInDiagram>();
            string[] names = {"ahoj", "Tomi", "!"};
            for (int i = 0; i < 3; i++)
            {
            EntityInDiagram entityInDiagram = new EntityInDiagram
            {
                VisualObject = DiagramPool.Instance.sequenceEntityPrefab,
                LifeLine = DiagramPool.Instance.sequenceLinePrefab,
                EntityName = names[i]
            };
            Entities.Add(entityInDiagram);
            }
            
            ResetDiagram();
            
            
        }

        public void ResetDiagram()
        {
            if (graph != null)
            {
                Destroy(graph.gameObject);
                graph = null;
            }
        }

        public void LoadDiagram()
        {
            CreateGraph();
            Generate();
            ManualLayout();

            graph.transform.position = new Vector3(0, 0, 3*offsetZ);
        }


        private Graph CreateGraph()
        {
            var go = Instantiate(DiagramPool.Instance.graphPrefab);
            graph = go.GetComponent<Graph>();
            graph.nodePrefab = DiagramPool.Instance.sequenceEntityPrefab;
            
            return graph;
        }
        
        public void ManualLayout()
        {
            int i = 0;
            foreach (EntityInDiagram entityInDiagram in Entities)
            {
                entityInDiagram.VisualObject.transform.SetPositionAndRotation(
                    new Vector3(i * activityOffsetX, initialActivityPositionY, initialActivityPositionZ), 
                    Quaternion.identity);
                // Debug.LogErrorFormat("Repositioning activity {0}", i); //TODOa remove
                i++;
            }
        }

        public void Generate()
        {
            Debug.LogError("generate");
            //Render classes
            for (int i = 0; i < Entities.Count; i++)
            {
                Debug.Log(Entities[i].EntityName);
                GenerateObject(Entities[i]);
                graph.Layout();
            }
        }

        public float distanceBetweenNodes = 50.0f; // Distance between nodes in the sequence
        public float lineYOffset = 0.5f; // Offset for lines between entities

        private void GenerateObject(EntityInDiagram Entity)
        {
            //Setting up
            graph.nodePrefab = DiagramPool.Instance.sequenceEntityPrefab;
            var node = graph.AddNode();
            Entity.VisualObject = node;
            node.SetActive(true);
            node.name = Entity.EntityName;
            
            var header = node.transform.Find("Header/Entity");
            header.GetComponent<TextMeshProUGUI>().text = node.name;
            node.transform.position = new Vector3(0, 0, 0);

            node = graph.AddNode();
            Entity.VisualObject = node;
            node.SetActive(true);
            node.name = Entity.EntityName;
            header = node.transform.Find("Header/Entity");
            header.GetComponent<TextMeshProUGUI>().text = node.name;
            node.transform.position = new Vector3(0, distanceBetweenNodes, 0);
     
            graph.nodePrefab = DiagramPool.Instance.sequenceLinePrefab;
            node = graph.AddNode();
            Entity.VisualObject = node;
            node.transform.position = new Vector3(0, distanceBetweenNodes - lineYOffset, 0);
        

            // var background = node.transform.Find("Background");
        }

    //     private void AddObject(ObjectInDiagram Object)
    //     {
    //         if (ObjectNamesInDiagram.ContainsKey(Object.Instance.UniqueID))
    //         {
    //             string message = string.Format("Tried to add an already existing object to object diagram with id:'{0}'.", Object.Instance.UniqueID);
    //             throw new Exception(message);
    //         }

    //         if (Object.VariableName == null)
    //         {
    //             string className = Object.Instance.OwningClass.Name;
    //             string objectNamePrefix
    //                 = className.Substring(0, 1).ToLower() + className.Substring(1);
    //             string newObjectNameName = CreateObjectName(objectNamePrefix, Object.Instance.UniqueID);
    //             Object.VariableName = newObjectNameName;
    //         }
    //         else if (AllObjectNames().Contains(Object.VariableName))
    //         {
    //             string objectNamePrefix = Object.VariableName;
    //             string newObjectNameName = CreateObjectName(objectNamePrefix, Object.Instance.UniqueID);
    //             Object.VariableName = newObjectNameName;
    //         }

    //         ObjectNamesInDiagram.Add(Object.Instance.UniqueID, Object.VariableName);
    //     }

    //     private void AddVisualPartOfObject(ObjectInDiagram Object)
    //     {
    //         Objects.Add(Object);
    //         GenerateObject(Object);
    //         graph.Layout();
    //     }

    //     private IEnumerable<string> AllObjectNames()
    //     {
    //         return ObjectNamesInDiagram.Values;
    //     }
    //     private string CreateObjectName(string variableNamePrefix, long objectId)
    //     {
    //         string variableName = null;
    //         for (int i = 1; i < int.MaxValue; i++)
    //         {
    //             variableName = string.Format("{0}_{1}", variableNamePrefix, i.ToString());
    //             if (!AllObjectNames().Contains(variableName))
    //             {
    //                 break;
    //             }
    //         }

    //         if (variableName == null)
    //         {
    //             string message = string.Format("Failed to create name for object with id:'0'.", objectId);
    //             throw new Exception(message);
    //         }

    //         return variableName;
    //     }

    //     public void ShowObject(ObjectInDiagram Object)
    //     {
    //         Object.VisualObject.SetActive(true);
    //         graph.Layout();
    //     }

    //     public ObjectInDiagram AddObjectInDiagram(string variableName, CDClassInstance instance, bool visible = true)
    //     {
    //         ObjectInDiagram objectInDiagram = new ObjectInDiagram
    //         {
    //             Class = DiagramPool.Instance.ClassDiagram.FindClassByName(instance.OwningClass.Name),
    //             Instance = instance,
    //             VisualObject = null,
    //             VariableName = variableName
    //         };

    //         AddObject(objectInDiagram);

    //         if (visible)
    //         {
    //             AddVisualPartOfObject(objectInDiagram);
    //         }

    //         return objectInDiagram;
    //     }

    //     public void AddRelation(CDClassInstance classInstanceStart, CDClassInstance classInstanceEnd, string relationType)
    //     {
    //         if (classInstanceStart == classInstanceEnd || classInstanceStart.UniqueID == classInstanceEnd.UniqueID)
    //         {
    //             return;
    //         }

    //         if (!ObjectExists(classInstanceStart.UniqueID) || !ObjectExists(classInstanceEnd.UniqueID))
    //         {
    //             return;
    //         }

    //         ObjectRelation relation = new ObjectRelation(graph, classInstanceStart.UniqueID,
    //             classInstanceEnd.UniqueID, relationType, "R" + Relations.Count);
    //         if (!ContainsObjectRelation(relation))
    //         {
    //             Relations.Add(relation);
    //             relation.Generate();
    //         }
    //     }

    //     public ObjectInDiagram FindByID(long instanceID)
    //     {
    //         foreach (var objectInDiagram in Objects)
    //         {
    //             if (objectInDiagram.Instance.UniqueID == instanceID)
    //             {
    //                 return objectInDiagram;
    //             }
    //         }

    //         return null;
    //     }
    //     public bool ObjectExists(long instanceID)
    //     {
    //         return FindByID(instanceID) != null;
    //     }
    //     public string GetObjectName(long instanceID)
    //     {
    //         if (!ObjectNamesInDiagram.ContainsKey(instanceID))
    //         {
    //             string message = string.Format("Tried to get name of object with non-existent id:'{0}'", instanceID);
    //             throw new Exception(message);
    //         }

    //         return ObjectNamesInDiagram[instanceID];
    //     }

    //     public bool UpdateAttributeValues(CDClassInstance classInstance)
    //     {
    //         ObjectInDiagram objectInDiagram = FindByID(classInstance.UniqueID);
    //         if (objectInDiagram == null)
    //         {
    //             return false;
    //         }

    //         var background = objectInDiagram.VisualObject.transform.Find("Background");
    //         var attributes = background.Find("Attributes");
    //         attributes.GetComponent<TextMeshProUGUI>().text = CreateAttributeValueText(classInstance);

    //         return true;
    //     }

    //     private string CreateAttributeValueText(CDClassInstance instance)
    //     {
    //         IEnumerable<string> values
    //             = instance.State.Keys
    //                 .Select
    //                 (
    //                     key =>
    //                     {
    //                         instance.State[key].Accept(visitor);
    //                         return visitor.Result;
    //                     }
    //                 );
    //         IEnumerable<string> attributeValueTexts
    //             = instance.State.Keys.Select(key => string.Format("{0} = {1}", key, CreateValueText(instance.State[key])));
    //         string attributeValueText
    //             = string.Join("\n", attributeValueTexts);

    //         return attributeValueText;
    //     }
    //     private string CreateValueText(EXEValueBase value)
    //     {
    //         value.Accept(visitor);
    //         return visitor.Result;
    //     }

    //     private bool ContainsObjectRelation(ObjectRelation objectRelation)
    //     {
    //         foreach (var relation in Relations)
    //         {
    //             if (relation.Equals(objectRelation))
    //             {
    //                 return true;
    //             }
    //         }

    //         return false;
    //     }

    //     public ObjectRelation FindRelation(long callerInstanceId, long calledInstanceId)
    //     {
    //         foreach (var objectRelation in Relations)
    //         {
    //             if ((objectRelation.startUniqueId == callerInstanceId &&
    //                  objectRelation.endUniqueId == calledInstanceId) ||
    //                 objectRelation.startUniqueId == calledInstanceId && objectRelation.endUniqueId == callerInstanceId)
    //             {
    //                 return objectRelation;
    //             }
    //         }

    //         return null;
    //     }
    }
}