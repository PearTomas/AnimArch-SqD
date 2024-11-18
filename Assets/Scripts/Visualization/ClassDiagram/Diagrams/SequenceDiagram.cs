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


        private void Awake()
        {
            DiagramPool.Instance.SequenceDiagram = this;
            ResetDiagram();
        }

        public void ResetDiagram()
        {}

        private Graph CreateGraph()
        {
            ResetDiagram();
            var go = Instantiate(DiagramPool.Instance.graphPrefab);
            graph = go.GetComponent<Graph>();
            graph.nodePrefab = DiagramPool.Instance.objectPrefab;
            return graph;
        }

        public void ManualLayout()
        {
            // int i = 0;
            // foreach (ObjectInDiagram objectInDiagram in Objects)
            // {
            //     // objectInDiagram.VisualObject.GetComponent<RectTransform>()
            //     //     .Shift(300 * ((int) (i / 2) - 1), 200 * (i % 2), 0);
            //     objectInDiagram.VisualObject.transform.position = objectInDiagram.Class.VisualObject.transform.position;
            //     i++;
            // }
        }

    //     private void Generate()
    //     {
    //         //Render classes
    //         for (int i = 0; i < Entities.Count; i++)
    //         {
    //             Debug.Log(Entities[i].EntityName);
    //             GenerateObject(Entities[i]);
    //         }
    //     }

    //     private void GenerateObject(ObjectInDiagram Object)
    //     {
    //         //Setting up
    //         var node = graph.AddNode();
    //         node.GetComponent<Clickable>().IsObject = true;
    //         node.SetActive(false);
    //         node.name = Object.EntityName;
    //         var background = node.transform.Find("Background");


    //         //Attributes
    //         foreach (string AttributeName in Object.Instance.State.Keys)
    //         {
    //             attributes.GetComponent<TextMeshProUGUI>().text +=
    //                 AttributeName + " = " + Object.Instance.State[AttributeName].ToObjectDiagramText() + "\n";
    //         }

    //         foreach (CDMethod method in Object.Class.ClassInfo.GetMethods(true))
    //         {
    //             string arguments = "(";
    //             if (method.Parameters != null)
    //                 for (int d = 0; d < method.Parameters.Count; d++)
    //                 {
    //                     if (d < method.Parameters.Count - 1)
    //                         arguments += (method.Parameters[d] + ", ");
    //                     else arguments += (method.Parameters[d]);
    //                 }

    //             arguments += ")";

    //             methods.GetComponent<TextMeshProUGUI>().text +=
    //                 method.Name + arguments + " :" + method.ReturnType + "\n";
    //         }

    //         //Add Class to Dictionary
    //         Object.VisualObject = node;

    //         // Create Edge towards class
    //         GameObject InterGraphLine = CreateInterGraphLine(Object.Class.VisualObject, Object.VisualObject);
    //         InterGraphLine.GetComponent<InterGraphRelation>().Initialize(Object, Object.Class);
    //         DiagramPool.Instance.RelationsClassToObject.Add
    //         (
    //             InterGraphLine.GetComponent<InterGraphRelation>()
    //         );
    //         // InterGraphLine.GetComponent<InterGraphRelation>().Hide();
    //     }

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