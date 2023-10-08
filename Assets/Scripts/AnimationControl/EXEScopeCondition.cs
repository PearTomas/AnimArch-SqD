﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OALProgramControl
{
    public class EXEScopeCondition : EXEScope
    {
        public EXEASTNode Condition { get; set; }
        private List<EXEScopeCondition> ElifScopes;
        public EXEScope ElseScope { get; set; }

        public EXEScopeCondition(EXEASTNode Condition) : base()
        {
            this.Condition = Condition;
            this.ElifScopes = null;
            this.ElseScope = null;
        }
        public EXEScopeCondition(EXEScope SuperScope, EXECommand[] Commands, EXEASTNode Condition) : base(SuperScope, Commands)
        {
            this.Condition = Condition;
            this.ElifScopes = null;
            this.ElseScope = null;
        }
        public EXEScopeCondition(EXEScope SuperScope, EXECommand[] Commands, EXEASTNode Condition, EXEScope ElseScope) : base(SuperScope, Commands)
        {
            this.Condition = Condition;
            this.ElifScopes = null;
            this.ElseScope = ElseScope;
        }
        public EXEScopeCondition(EXEScope SuperScope, EXECommand[] Commands, EXEASTNode Condition, EXEScopeCondition[] ElifScopes) : base(SuperScope, Commands)
        {
            this.Condition = Condition;
            this.ElifScopes = ElifScopes.ToList();
            this.ElseScope = null;
        }
        public EXEScopeCondition(EXEScope SuperScope, EXECommand[] Commands, EXEASTNode Condition, EXEScopeCondition[] ElifScopes, EXEScope ElseScope) : base(SuperScope, Commands)
        {
            this.Condition = Condition;
            this.ElifScopes = ElifScopes.ToList();
            this.ElseScope = ElseScope;
        }

        public override void SetSuperScope(EXEScope SuperScope)
        {
            base.SetSuperScope(SuperScope);

            if (this.ElifScopes != null)
            {
                foreach (EXEScope ElifScope in this.ElifScopes)
                {
                    ElifScope.SetSuperScope(this.GetSuperScope());
                }
            }

            if (this.ElseScope != null)
            {
                this.ElseScope.SetSuperScope(this.GetSuperScope());
            }
        }

        public void AddElifScope(EXEScopeCondition ElifScope)
        {
            if (this.ElifScopes == null)
            {
                this.ElifScopes = new List<EXEScopeCondition>();
            }

            this.ElifScopes.Add(ElifScope);
            ElifScope.SetSuperScope(this.GetSuperScope());
        }

        protected override EXEExecutionResult Execute(OALProgram OALProgram)
        {
            Boolean AScopeWasExecuted = false;

            if (this.Condition == null)
            {
                return Error("XEC1144", ErrorMessage.ConditionInIfStatementNotSet());
            }

            String ConditionResult = this.Condition.Evaluate(SuperScope, OALProgram.ExecutionSpace);

            if (ConditionResult == null)
            {
                return Error("XEC1145", ErrorMessage.FailedExpressionEvaluation(this.Condition, this.SuperScope));
            }
            if (!EXETypes.BooleanTypeName.Equals(EXETypes.DetermineVariableType("", ConditionResult)))
            {
                return Error("XEC1146", ErrorMessage.InvalidValueForType(ConditionResult, EXETypes.BooleanTypeName));
            }
            Boolean IfConditionResult = EXETypes.BooleanTrue.Equals(ConditionResult) ? true : false;

            if (IfConditionResult)
            {
                AddCommandsToStack(this.Commands);
                AScopeWasExecuted = true;
            }

            if (AScopeWasExecuted)
            {
                return Success();
            }

            EXEExecutionResult elifScopeResult = null;

            if (this.ElifScopes != null)
            {
                foreach (EXEScopeCondition CurrentElif in this.ElifScopes)
                {
                    if (CurrentElif.Condition == null)
                    {
                        return Error("XEC1147", ErrorMessage.ConditionInIfStatementNotSet());
                    }

                    ConditionResult = CurrentElif.Condition.Evaluate(SuperScope, OALProgram.ExecutionSpace);


                    if (ConditionResult == null)
                    {
                        return Error("XEC1148", ErrorMessage.FailedExpressionEvaluation(CurrentElif.Condition, this.SuperScope));
                    }
                    if (!EXETypes.BooleanTypeName.Equals(EXETypes.DetermineVariableType("", ConditionResult)))
                    {
                        return Error("XEC1149", ErrorMessage.InvalidValueForType(ConditionResult, EXETypes.BooleanTypeName));
                    }
                    IfConditionResult = EXETypes.BooleanTrue.Equals(ConditionResult) ? true : false;
                    
                    if (IfConditionResult)
                    {
                        elifScopeResult = CurrentElif.PerformExecution(OALProgram);
                        AScopeWasExecuted = true;
                        break;
                    }
                }
            }

            if (AScopeWasExecuted)
            {
                return elifScopeResult;
            }

            if (this.ElseScope != null)
            {
                return this.ElseScope.PerformExecution(OALProgram);
            }

            return Success();
        }
        public override String ToCode(String Indent = "")
        {
            return FormatCode(Indent, false);
        }

        public override string ToFormattedCode(string Indent = "")
        {
            return FormatCode(Indent, IsActive);
        }

        private string FormatCode(String Indent, bool Highlight)
        {
            String Result = HighlightCodeIf(Highlight, Indent + "if (" + this.Condition.ToCode() + ")\n");
            foreach (EXECommand Command in this.Commands)
            {
                Result += Command.ToFormattedCode(Indent + "\t");
            }
            if (this.ElifScopes != null)
            {
                foreach (EXEScopeCondition Elif in this.ElifScopes)
                {
                    Result += HighlightCodeIf(Highlight, Indent + "elif (" + Elif.Condition.ToCode() + ")\n");
                    foreach (EXECommand Command in Elif.Commands)
                    {
                        Result += Command.ToFormattedCode(Indent + "\t");
                    }
                }
            }
            if (this.ElseScope != null)
            {
                Result += HighlightCodeIf(Highlight, Indent + "else\n");
                foreach (EXECommand Command in this.ElseScope.Commands)
                {
                    Result += Command.ToFormattedCode(Indent + "\t");
                }
            }
            Result += HighlightCodeIf(Highlight, Indent + "end if;\n");
            return Result;
        }

        protected override EXEScope CreateDuplicateScope()
        {
            return new EXEScopeCondition(Condition)
            {
                ElifScopes = ElifScopes == null ? null : ElifScopes.Select(x => (EXEScopeCondition)x.CreateClone()).ToList(),
                ElseScope = ElseScope == null ? null : (EXEScope)ElseScope.CreateClone()
            };
        }
    }
}