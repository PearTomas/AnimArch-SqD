﻿using System;

namespace AnimationControl
{
    public class EXECommandQueryDelete : EXECommand
    {
        private String VariableName { get; }

        public EXECommandQueryDelete(String VariableName)
        {
            this.VariableName = VariableName;
        }

        new public bool Execute(CDClassPool ExecutionSpace, CDRelationshipPool RelationshipSpace, EXEScope Scope)
        {
            bool Result = false;
            EXEReferencingVariable Variable = Scope.FindReferencingVariableByName(this.VariableName);
            if (Variable != null)
            {
                bool DestructionSuccess = ExecutionSpace.DestroyInstance(Variable.ClassName, Variable.ReferencedInstanceId);
                if(DestructionSuccess)
                {
                    Result = Scope.UnsetReferencingVariables(Variable.ClassName, Variable.ReferencedInstanceId);
                }
            }

            return Result;
        }
    }
}