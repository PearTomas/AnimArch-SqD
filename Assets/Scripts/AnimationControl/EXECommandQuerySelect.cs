﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OALProgramControl
{
    public class EXECommandQuerySelect : EXECommand
    {
        public const String CardinalityAny = "any";
        public const String CardinalityMany = "many";

        public String Cardinality { get; set; }
        public String ClassName { get; set; }
        public String VariableName { get; set; }
        public String AttributeName { get; set; }
        public EXEASTNode WhereCondition { get; set; }

        public EXECommandQuerySelect(String Cardinality, String ClassName, String VariableName, String AttributeName)
        {
            this.Cardinality = Cardinality;
            this.ClassName = ClassName;
            this.VariableName = VariableName;
            this.AttributeName = AttributeName;
            this.WhereCondition = null;
        }
        public EXECommandQuerySelect(String Cardinality, String ClassName, String VariableName, String AttributeName, EXEASTNode WhereCondition)
        {
            this.Cardinality = Cardinality;
            this.ClassName = ClassName;
            this.VariableName = VariableName;
            this.AttributeName = AttributeName;
            this.WhereCondition = WhereCondition;
        }

        // SetUloh2
        protected override EXEExecutionResult Execute(OALProgram OALProgram)
        {
            //Select instances of given class that match the criteria and assign them to variable with given name
            // ClassName tells us which class we are interested in
            // Cardinality tells us whether we want one random instance (matching the criteria) or all of them
            // "Many" - we create variable EXEReferencingSetVariable, "Any" - we create variable EXEReferencingVariable
            // Variable name tells us how to name the newly created referencing variable
            // Where condition tells us which instances to select from all instances of the class (just do EXEASTNode.Evaluate and return true if the result "true" and false for "false")
            // When making unit tests, do not use the "where" causule yet, because its evaluation is not yet implemented

            CDClass Class = OALProgram.ExecutionSpace.getClassByName(this.ClassName);
            if (Class == null)
            {
                return Error("XEC1083", ErrorMessage.ClassNotFound(this.ClassName, OALProgram));
            }

            CDClassInstance ClassInstance = null; // This is important if we have AttributeName

            if (this.AttributeName == null)
            {
                // We need to check, if the variable already exists, it must be of corresponding type
                if (SuperScope.VariableNameExists(this.VariableName))
                {
                    EXEReferencingVariable refVariable = SuperScope.FindReferencingVariableByName(this.VariableName);

                    if
                    (
                        EXECommandQuerySelect.CardinalityAny.Equals(this.Cardinality)
                        && this.ClassName != refVariable.ClassName
                    )
                    {
                        return Error("XEC1084", ErrorMessage.InvalidAssignment("", this.ClassName, this.VariableName, refVariable.ClassName));
                    }

                    EXEReferencingSetVariable setVariable = SuperScope.FindSetReferencingVariableByName(this.VariableName);

                    if
                    (
                        EXECommandQuerySelect.CardinalityMany.Equals(this.Cardinality)
                        &&
                        this.ClassName != setVariable.ClassName
                    )
                    {
                        return Error("XEC1085", ErrorMessage.InvalidAssignment("", this.ClassName, this.VariableName, refVariable.ClassName));
                    }
                }
            }
            else
            {
                // If we have AttributeName, VariableName must be reference (not set variable)
                EXEReferencingVariable Variable = SuperScope.FindReferencingVariableByName(this.VariableName);
                if (Variable == null)
                {
                    return Error("XEC1086", ErrorMessage.VariableNotFound(this.VariableName, this.SuperScope));
                }

                CDClass VariableClass = OALProgram.ExecutionSpace.getClassByName(Variable.ClassName);
                if (VariableClass == null)
                {
                    return Error("XEC1087", ErrorMessage.ClassNotFound(Variable.ClassName, OALProgram));
                }

                CDAttribute Attribute = VariableClass.GetAttributeByName(this.AttributeName);
                if (Attribute == null)
                {
                    return Error("XEC1088", ErrorMessage.AttributeNotFoundOnClass(this.AttributeName, VariableClass));
                }

                // We need to check the corresponding type of attribute
                if ("[]".Equals(Attribute.Type.Substring(Attribute.Type.Length - 2, 2)))
                {
                    if (!Attribute.Type.Equals(this.ClassName + "[]"))
                    {
                        return Error("XEC1089", ErrorMessage.AssignNewListToVariableHoldingListOfAnotherType(this.VariableName + "." + this.AttributeName, Attribute.Type.Replace("[]", ""), this.ClassName));
                    }

                    if (EXECommandQuerySelect.CardinalityAny.Equals(this.Cardinality))
                    {
                        return Error("XEC1090", ErrorMessage.SelectingAnyIntoAnArray());
                    }
                }
                else
                {
                    if (!Attribute.Type.Equals(this.ClassName))
                    {
                        return Error("XEC1091", ErrorMessage.InvalidAssignment("irrelevant", this.ClassName, this.VariableName + "." + this.AttributeName, Attribute.Type));
                    }

                    if (EXECommandQuerySelect.CardinalityMany.Equals(this.Cardinality))
                    {
                        return Error("XEC1092", ErrorMessage.SelectingManyIntoAReference());
                    }
                }

                // Get instance representing VariableName, important in assignment section at end of this Execute method
                ClassInstance = VariableClass.GetInstanceByID(Variable.ReferencedInstanceId);
                if (ClassInstance == null)
                {
                    return Error("XEC1093", ErrorMessage.InstanceNotFound(Variable.ReferencedInstanceId, VariableClass));
                }
            }

            // Evaluate relationship selection. If it fails, execution fails too
            List<long> SelectedIds = Class.GetAllInstanceIDs();
            if (SelectedIds == null)
            {
                return Error("XEC1094", ErrorMessage.FailedRetrievingAllClassInstanceIds(Class));
            }

            // Now let's evaluate the condition
            if (this.WhereCondition != null && SelectedIds.Any())
            {
                String TempSelectedVarName = "selected";

                //Console.WriteLine("creating selected var");
                EXEReferencingVariable SelectedVar = new EXEReferencingVariable(TempSelectedVarName, this.ClassName, -1);
                EXEExecutionResult addVariableResult = SuperScope.AddVariable(SelectedVar);
                addVariableResult.OwningCommand = this;
                if (!addVariableResult.IsSuccess)
                {
                    return addVariableResult;
                }

               // Console.WriteLine("created selected var");
                List<long> ResultIds = new List<long>();
                foreach (long Id in SelectedIds)
                {
                    //Console.WriteLine("id check iteration start");
                    SelectedVar.ReferencedInstanceId = Id;
                    String ConditionResult = this.WhereCondition.Evaluate(SuperScope, OALProgram.ExecutionSpace);

                    //Console.WriteLine("cond evaluated");
                    //Console.WriteLine(Id + " : " + ConditionResult == null ? "null" : ConditionResult);

                    if (!EXETypes.IsValidValue(ConditionResult, EXETypes.BooleanTypeName))
                    {
                        SuperScope.DestroyReferencingVariable(TempSelectedVarName);
                        return Error("XEC1095", ErrorMessage.InvalidValueForType(string.Format("Condition in WHERE clause evaluated to '{0}'", ConditionResult), EXETypes.BooleanTypeName));
                    }

                    if (EXETypes.BooleanTrue.Equals(ConditionResult))
                    {
                        ResultIds.Add(Id);
                    }
                }

                SelectedIds = ResultIds;
                SuperScope.DestroyReferencingVariable(TempSelectedVarName);
            
            }

            // Now we have ids of selected instances. Let's assign them to a variable
            if (EXECommandQuerySelect.CardinalityMany.Equals(this.Cardinality))
            {
                if (this.AttributeName == null)
                {
                    EXEReferencingSetVariable Variable = SuperScope.FindSetReferencingVariableByName(this.VariableName);
                    if (Variable == null)
                    {
                        Variable = new EXEReferencingSetVariable(this.VariableName, this.ClassName);
                        EXEExecutionResult addVariableResult = SuperScope.AddVariable(Variable);
                        addVariableResult.OwningCommand = this;
                        if (!addVariableResult.IsSuccess)
                        {
                            return addVariableResult;
                        }
                    }

                    Variable.ClearVariables();

                    foreach (long Id in SelectedIds)
                    {
                        Variable.AddReferencingVariable(new EXEReferencingVariable("", Variable.ClassName, Id));
                    }
                }
                else
                {
                    String Result = String.Join(",", SelectedIds);

                    EXEExecutionResult setAttributeResult = ClassInstance.SetAttribute(this.AttributeName, Result);
                    setAttributeResult.OwningCommand = this;
                    if (!setAttributeResult.IsSuccess)
                    {
                        return setAttributeResult;
                    }
                }
            }
            else if (EXECommandQuerySelect.CardinalityAny.Equals(this.Cardinality))
            {
                long ResultId = SelectedIds.Any() ? SelectedIds[0] : -1;

                if (this.AttributeName == null)
                {
                    EXEReferencingVariable Variable = SuperScope.FindReferencingVariableByName(this.VariableName);

                    if (Variable == null)
                    {
                        //Console.WriteLine("Final 'any' id is " + ResultId);
                        Variable = new EXEReferencingVariable(this.VariableName, this.ClassName, ResultId);
                        EXEExecutionResult addVariableResult = SuperScope.AddVariable(Variable);
                        addVariableResult.OwningCommand = this;
                        if(!addVariableResult.IsSuccess)
                        {
                            return addVariableResult;
                        }
                    }
                    else
                    {
                        Variable.ReferencedInstanceId = ResultId;
                    }
                }
                else
                {
                    EXEExecutionResult setAttributeResult = ClassInstance.SetAttribute(this.AttributeName, ResultId.ToString());
                    setAttributeResult.OwningCommand = this;
                    if (!setAttributeResult.IsSuccess)
                    {
                        return setAttributeResult;
                    }
                }
            }
            else
            {
                return Error("XEC1096", ErrorMessage.UnknownSelectCardinality(this.Cardinality));
            }

            return Success();
        }
        public override string ToCodeSimple()
        {
            return "select " + this.Cardinality + " " + (this.AttributeName == null ? this.VariableName : (this.VariableName + "." + this.AttributeName))
                + " from instances of " + this.ClassName + (this.WhereCondition == null ? "" : (" where ") + this.WhereCondition.ToCode());
        }

        public override EXECommand CreateClone()
        {
            return new EXECommandQuerySelect(Cardinality, ClassName, VariableName, AttributeName, WhereCondition);
        }
    }
}