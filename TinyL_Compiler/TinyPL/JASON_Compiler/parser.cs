using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.AxHost;

namespace TinyL_Compiler
{
    public class Node
    {
        public List<Node> Children = new List<Node>();

        public string Name;
        public Node(string N)
        {
            this.Name = N;
        }
    }
    public class Parser
    {
        int InputPointer = 0;
        List<Token> TokenStream;
        public Node root;

        public Node StartParsing(List<Token> TokenStream)
        {
            this.InputPointer = 0;
            this.TokenStream = TokenStream;
            root = new Node("Program");
            root.Children.Add(Program());
            return root;
        }
        Node Program()
        {
            Node program = new Node("Program");
            if (InputPointer + 1 < TokenStream.Count && TokenStream[InputPointer + 1]._type == TOKEN_ENUM.TOKEN_MAIN)
            {
                program.Children.Add(Main_Function());
            }
            else
            {
                program.Children.Add(FunctionState());
                program.Children.Add(Main_Function());
            }
            MessageBox.Show("Success");
            return program;
        }

        Node Main_Function()
        {
            Node _main = new Node("Main_Function");

            _main.Children.Add(Data_Type());
            _main.Children.Add(match(TOKEN_ENUM.TOKEN_MAIN));
            _main.Children.Add(match(TOKEN_ENUM.TOKEN_LPAREN));
            _main.Children.Add(match(TOKEN_ENUM.TOKEN_RPAREN));

            _main.Children.Add(Function_body());

            return _main;
        }

        Node Statements()
        {
            Node state = new Node("statements");
            state.Children.Add(Statement());
            state.Children.Add(State());

            return state;
        }
        Node Return_state()
        {
            Node Return = new Node("Return Statement");
             Return.Children.Add(match(TOKEN_ENUM.TOKEN_RETURN));
            Return.Children.Add(Expression());
            Return.Children.Add(match(TOKEN_ENUM.TOKEN_SEMICOLON));
            return Return;
        }

        Node Write()
        {
            Node w = new Node("Write Statement");
            if (TokenStream[InputPointer + 1]._type == TOKEN_ENUM.TOKEN_IDENTIFIER)
            {
                w.Children.Add(match(TOKEN_ENUM.TOKEN_WRITE));
                w.Children.Add(match(TOKEN_ENUM.TOKEN_IDENTIFIER));
                w.Children.Add(match(TOKEN_ENUM.TOKEN_SEMICOLON));
            }
            else
            {
                w.Children.Add(match(TOKEN_ENUM.TOKEN_WRITE));
                w.Children.Add(match(TOKEN_ENUM.TOKEN_ENDL));
                w.Children.Add(match(TOKEN_ENUM.TOKEN_SEMICOLON));
            }
            return w;
        }
        Node Read()
        {
            Node r = new Node("Read Statement");
            r.Children.Add(match(TOKEN_ENUM.TOKEN_READ));
            r.Children.Add(match(TOKEN_ENUM.TOKEN_IDENTIFIER));
            r.Children.Add(match(TOKEN_ENUM.TOKEN_SEMICOLON));
            return r;
        }
        Node Assignement()
        {
            Node assign = new Node("Assignment Statement");
            assign.Children.Add(match(TOKEN_ENUM.TOKEN_IDENTIFIER));
            assign.Children.Add(match(TOKEN_ENUM.TOKEN_BINDING));
            assign.Children.Add(Expression());
            assign.Children.Add(match(TOKEN_ENUM.TOKEN_SEMICOLON));
            return assign;
        }
        Node Identifiers()
        {
            Node i = new Node("identifiers");
            if (TokenStream[InputPointer]._type == TOKEN_ENUM.TOKEN_COMMA)
            {
                i.Children.Add(match(TOKEN_ENUM.TOKEN_COMMA));
                i.Children.Add(match(TOKEN_ENUM.TOKEN_IDENTIFIER));
                i.Children.Add(Identifiers());
            }
            else
                return null;
            return i;
        }
        Node Declaration_statement()
        {
            Node Dec = new Node("Declaration Statement");
            Dec.Children.Add(Data_Type());
            if (InputPointer+1 < TokenStream.Count && TokenStream[InputPointer + 1]._type != TOKEN_ENUM.TOKEN_BINDING)
            {
                Dec.Children.Add(match(TOKEN_ENUM.TOKEN_IDENTIFIER));
                Dec.Children.Add(Identifiers());
                Dec.Children.Add(match(TOKEN_ENUM.TOKEN_SEMICOLON));
            }
            else
            {
                Dec.Children.Add(Assignement());
            }
            return Dec;
        }
        Node Repeat()
        {
            Node rep = new Node("Repeat");
            rep.Children.Add(match(TOKEN_ENUM.TOKEN_REPEAT));
            rep.Children.Add(Expression());
            rep.Children.Add(match(TOKEN_ENUM.TOKEN_UNTIL));
            rep.Children.Add(ConditionState());
            return rep;
        }
        Node IF_state()
        {
            Node If = new Node("IF Statement");
            If.Children.Add(match(TOKEN_ENUM.TOKEN_IF));
            If.Children.Add(ConditionState()); //x>5 && y>7
            If.Children.Add(match(TOKEN_ENUM.TOKEN_THEN));
            If.Children.Add(Statements());
            if (TokenStream[InputPointer]._type == TOKEN_ENUM.TOKEN_ElSEIF)
            {
                If.Children.Add(ELSEIF_State());

            }
            else if (TokenStream[InputPointer]._type == TOKEN_ENUM.TOKEN_ELSE)
            {
                If.Children.Add(ELSE_State());

            }
            else
            {
                If.Children.Add(match(TOKEN_ENUM.TOKEN_END));
            }
            return If;
        }
        Node Statement()
        {
            Node state = new Node("Statement");

            //Read Statement
            if (TokenStream[InputPointer]._type == TOKEN_ENUM.TOKEN_READ)
            {
                state.Children.Add(Read());

            }
            //Write statement
            else if (TokenStream[InputPointer]._type == TOKEN_ENUM.TOKEN_WRITE)
            {
                state.Children.Add(Write());
            }
        
            //Assinment statement
            else if (TokenStream[InputPointer]._type == TOKEN_ENUM.TOKEN_IDENTIFIER)
            {
                state.Children.Add(Assignement());
            }

            //Return Statement
            //Declaration statement
            else if (TokenStream[InputPointer]._type == TOKEN_ENUM.TOKEN_STRING ||
                TokenStream[InputPointer]._type == TOKEN_ENUM.TOKEN_INT||
                TokenStream[InputPointer]._type == TOKEN_ENUM.TOKEN_FLOAT) 
            {
                state.Children.Add(Declaration_statement());

            }
            //Repeat statement
            else if (TokenStream[InputPointer]._type == TOKEN_ENUM.TOKEN_REPEAT)
            {
                state.Children.Add(Repeat());
                // return state;
            }
            else if (TokenStream[InputPointer]._type == TOKEN_ENUM.TOKEN_IF)
            {
                state.Children.Add(IF_state());
                // return state;
            }           
            else
            {
                return null;
            }
            return state;
        }
     
        Node FunctionStates()
        {
            Node funcStates = new Node("Function Statements");
            if (InputPointer + 1 < TokenStream.Count && TokenStream[InputPointer + 1]._type == TOKEN_ENUM.TOKEN_MAIN)
            {
                return null;
            }
            funcStates.Children.Add(FunctionState());
            funcStates.Children.Add(FunctionStates());
            return funcStates;

        }
        Node FunctionState()
        {
            Node functionState = new Node("Function Statement");
    
                functionState.Children.Add(Function_declaration());
                functionState.Children.Add(Function_body());
                   
            return functionState;
        }
        Node Return_State()
        {
            Node returnstate = new Node("Return Statement");
            returnstate.Children.Add(match(TOKEN_ENUM.TOKEN_RETURN));
            returnstate.Children.Add(Expression());
            returnstate.Children.Add(match(TOKEN_ENUM.TOKEN_SEMICOLON));
            return returnstate;
        }
        Node Function_body()
        {
            Node _func_body = new Node("Function Body");
            _func_body.Children.Add(match(TOKEN_ENUM.TOKEN_LBRACE));
            _func_body.Children.Add(Statements());
            _func_body.Children.Add(Return_State());
            _func_body.Children.Add(match(TOKEN_ENUM.TOKEN_RBRACE));
            return _func_body;
        }

        Node Function_name()
        {
            Node _func_name = null;
            if(TokenStream[InputPointer]._type==TOKEN_ENUM.TOKEN_IDENTIFIER)
            {
                _func_name = new Node("Function Name");
                _func_name.Children.Add(match(TOKEN_ENUM.TOKEN_IDENTIFIER));
            }
            return _func_name;
        }
        Node ArgList()
        {
            Node Arglist = new Node("Argument List");
            Arglist.Children.Add(Data_Type());
            Arglist.Children.Add(match(TOKEN_ENUM.TOKEN_IDENTIFIER));
            Arglist.Children.Add(Args());
            return Arglist;
        }
        Node Args()
        {
            Node arg = new Node("Argument");
     
            if(InputPointer < TokenStream.Count && TokenStream[InputPointer]._type== TOKEN_ENUM.TOKEN_COMMA)
                {
                arg.Children.Add(match(TOKEN_ENUM.TOKEN_COMMA));
                arg.Children.Add(Data_Type());
                arg.Children.Add(match(TOKEN_ENUM.TOKEN_IDENTIFIER));
                arg.Children.Add(Args());
            }else
            {
                return null;
            }
            return arg;
        }
        Node Function_declaration()
        {
            Node _func_decl = new Node("Function declaration");
            _func_decl.Children.Add(Data_Type());
            _func_decl.Children.Add(match(TOKEN_ENUM.TOKEN_IDENTIFIER));
            _func_decl.Children.Add(match(TOKEN_ENUM.TOKEN_LPAREN));
            if (InputPointer < TokenStream.Count && TokenStream[InputPointer]._type== TOKEN_ENUM.TOKEN_RPAREN)
            {
                _func_decl.Children.Add(match(TOKEN_ENUM.TOKEN_RPAREN));

            }
            else
            {
                _func_decl.Children.Add(ArgList());
                _func_decl.Children.Add(match(TOKEN_ENUM.TOKEN_RPAREN));
            }

            return _func_decl;

        }

      
        Node State() //statements | null
        {
            Node stat = new Node("state");
            if (InputPointer < TokenStream.Count &&(TokenStream[InputPointer]._type == TOKEN_ENUM.TOKEN_IDENTIFIER ||
                TokenStream[InputPointer]._type == TOKEN_ENUM.TOKEN_INT||
                TokenStream[InputPointer]._type == TOKEN_ENUM.TOKEN_FLOAT||
                TokenStream[InputPointer]._type == TOKEN_ENUM.TOKEN_STRING||
                TokenStream[InputPointer]._type == TOKEN_ENUM.TOKEN_READ||
                TokenStream[InputPointer]._type == TOKEN_ENUM.TOKEN_WRITE||
                TokenStream[InputPointer]._type == TOKEN_ENUM.TOKEN_REPEAT))
            {
           
                stat.Children.Add(Statements());
                return stat;
            }
            else
            {
                return null;
            }
        }

        Node ELSE_State()
        {
            Node _elseState = new Node("Else Statement");
            _elseState.Children.Add(match(TOKEN_ENUM.TOKEN_ELSE));
            _elseState.Children.Add(Statements());
            _elseState.Children.Add(match(TOKEN_ENUM.TOKEN_END));
            return _elseState;
        }
        Node ELSEIF_State()
        {
            Node _elseIfState = new Node("Else If Statement");
            _elseIfState.Children.Add(match(TOKEN_ENUM.TOKEN_ElSEIF));
            _elseIfState.Children.Add(ConditionState());
            _elseIfState.Children.Add(match(TOKEN_ENUM.TOKEN_THEN));
            _elseIfState.Children.Add(Statements());
            if (InputPointer < TokenStream.Count && TokenStream[InputPointer]._type == TOKEN_ENUM.TOKEN_ElSEIF)
            {
                _elseIfState.Children.Add(ELSEIF_State());

            }
            else if (InputPointer < TokenStream.Count && TokenStream[InputPointer]._type == TOKEN_ENUM.TOKEN_ELSE)
            {
                _elseIfState.Children.Add(ELSE_State());

            }
            else
            {
                _elseIfState.Children.Add(match(TOKEN_ENUM.TOKEN_END));
            }
            return _elseIfState;
        }

        Node ConditionState()
        {
            Node condState = new Node("Condition Statement");
            condState.Children.Add(Condition());
            Node ConditionOpNode = ConditionOP();
            if (ConditionOpNode != null)
            {
                condState.Children.Add(ConditionOpNode);
                condState.Children.Add(Condition());
            }
            return condState;
        }
        Node BooleanOp()
        {
            Node b = null;
            if(InputPointer < TokenStream.Count && TokenStream[InputPointer]._type == TOKEN_ENUM.TOKEN_ANDING)
            {
                b = new Node("Anding");
                b.Children.Add(match(TOKEN_ENUM.TOKEN_ANDING));
            }else
            {
                b = new Node("Oring");
                b.Children.Add(match(TOKEN_ENUM.TOKEN_ORING));
            }
            return b;
        }
        Node Conditions()
        {
            Node cond = new Node("Conditions");
            if (InputPointer < TokenStream.Count && (TokenStream[InputPointer]._type == TOKEN_ENUM.TOKEN_ANDING ||
                TokenStream[InputPointer]._type == TOKEN_ENUM.TOKEN_ORING))
            {
                cond.Children.Add(BooleanOp());
                cond.Children.Add(Condition());
                cond.Children.Add(Conditions());
            }else
            {
                return null;
            }
            return cond;

        }
        Node Condition()
        {
            Node  cond = new Node("condition");
                cond.Children.Add(Term());
                cond.Children.Add(ConditionOP());
                cond.Children.Add(Term());
            
            return cond;

        }
        Node ConditionOP()
        {
            Node condOp = null;
            if (InputPointer < TokenStream.Count && TokenStream[InputPointer]._type == TOKEN_ENUM.TOKEN_LESS_THAN)
            {
                condOp = new Node("Less Than");
                condOp.Children.Add(match(TOKEN_ENUM.TOKEN_LESS_THAN));
            }
            else if (InputPointer < TokenStream.Count && TokenStream[InputPointer]._type == TOKEN_ENUM.TOKEN_GREATER_THAN)
            {
                condOp = new Node("Greater Than");
                condOp.Children.Add(match(TOKEN_ENUM.TOKEN_GREATER_THAN));
            }
            else if (InputPointer < TokenStream.Count && TokenStream[InputPointer]._type == TOKEN_ENUM.TOKEN_EQUALITY_OPERATOR)
            {
                condOp = new Node("Equality");
                condOp.Children.Add(match(TOKEN_ENUM.TOKEN_EQUALITY_OPERATOR));
            }
            else
            {
                condOp = new Node("not Equal");
                condOp.Children.Add(match(TOKEN_ENUM.TOKEN_NOT_EQUAL));
            }
            return condOp;
        }
        Node Parameters()
        {
            Node par = new Node("Parameters");
            if(InputPointer < TokenStream.Count && TokenStream[InputPointer]._type == TOKEN_ENUM.TOKEN_COMMA)
            {
                par.Children.Add(match(TOKEN_ENUM.TOKEN_COMMA));
                par.Children.Add(Parameters());

            }else
            {
                return null;
            }
            return par;
        }
        Node Function_call()
        {
            Node FC = new Node("Function Call");
            FC.Children.Add(match(TOKEN_ENUM.TOKEN_IDENTIFIER));
            FC.Children.Add(match(TOKEN_ENUM.TOKEN_LPAREN));
            if (InputPointer < TokenStream.Count && TokenStream[InputPointer]._type ==TOKEN_ENUM.TOKEN_RPAREN)
            {
                FC.Children.Add(match(TOKEN_ENUM.TOKEN_RPAREN));
            }
            else
            {
                FC.Children.Add(Term());
                FC.Children.Add(Parameters());
                FC.Children.Add(match(TOKEN_ENUM.TOKEN_RPAREN));
            }
            return FC;
        }
        Node Term()
        {
            Node term = new Node("Term");
            if (InputPointer < TokenStream.Count && TokenStream[InputPointer]._type == TOKEN_ENUM.TOKEN_INT )
            {
                term.Children.Add(match(TOKEN_ENUM.TOKEN_INT));
            }else if(InputPointer < TokenStream.Count && TokenStream[InputPointer]._type == TOKEN_ENUM.TOKEN_FLOAT )
            {
                term.Children.Add(match(TOKEN_ENUM.TOKEN_FLOAT));
            }else if(InputPointer < TokenStream.Count && TokenStream[InputPointer+1]._type == TOKEN_ENUM.TOKEN_LPAREN)
            {
                term.Children.Add(Function_call());

            }
            else 
            {
                term.Children.Add(match(TOKEN_ENUM.TOKEN_IDENTIFIER));
            }
            return term;
        }
     
        Node Factor()
        {
            Node Fact = new Node("Factor");
           if(InputPointer < TokenStream.Count && TokenStream[InputPointer]._type == TOKEN_ENUM.TOKEN_LPAREN)
            {
                Fact.Children.Add(match(TOKEN_ENUM.TOKEN_LPAREN));
                Fact.Children.Add(Equation());
                Fact.Children.Add(match(TOKEN_ENUM.TOKEN_RPAREN));
            }
            else
            {
                Fact.Children.Add(Term());
            }
            return Fact;

        }
        Node Data_Type()
        {
            Node dataType = null;
            if (InputPointer < TokenStream.Count && TokenStream[InputPointer]._type == TOKEN_ENUM.TOKEN_STRING)
            {
                dataType = new Node("Data Type String");
                dataType.Children.Add(match(TOKEN_ENUM.TOKEN_STRING));
            }
            else if (InputPointer < TokenStream.Count && TokenStream[InputPointer]._type == TOKEN_ENUM.TOKEN_INT)
            {
                dataType = new Node("Data Type Integer");
                dataType.Children.Add(match(TOKEN_ENUM.TOKEN_INT));
            }
            else if (InputPointer < TokenStream.Count && TokenStream[InputPointer]._type == TOKEN_ENUM.TOKEN_FLOAT)
            {
                dataType = new Node("Data Type Float");
                dataType.Children.Add(match(TOKEN_ENUM.TOKEN_FLOAT));
            }
            
            return dataType;

        }

       

        Node Expression()
        {
            Node expression = null;

            if (InputPointer < TokenStream.Count && TokenStream[InputPointer]._type == TOKEN_ENUM.TOKEN_STRING)
            {
                expression = new Node("String");
                expression.Children.Add(match(TOKEN_ENUM.TOKEN_STRING));
            }
            else
            {
                expression = new Node("Equation");
                expression.Children.Add(Equation());
            }
            return expression;
        }
        Node Equation()
        {
            //term || term + - * / term
            Node equation = new Node("Equation");
            equation.Children.Add(LFactorR());
            equation.Children.Add(EqDash());
            return equation;
        }
        Node AddOp()
        {
            Node Add=new Node("Add Operator");
            if (InputPointer < TokenStream.Count && TokenStream[InputPointer]._type == TOKEN_ENUM.TOKEN_PLUS_OPERATOR)
            {
                Add.Children.Add(match(TOKEN_ENUM.TOKEN_PLUS_OPERATOR));
            }
            else
            {
                Add.Children.Add(match(TOKEN_ENUM.TOKEN_MINUS_OPERATOR));

            }
            return Add;
        }
        Node MulOp()
        {
            Node Mult = new Node("Multiply operator");
            if (InputPointer < TokenStream.Count && TokenStream[InputPointer]._type == TOKEN_ENUM.TOKEN_MULTIPLY_OPERATOR)
            {
                Mult.Children.Add(match(TOKEN_ENUM.TOKEN_MULTIPLY_OPERATOR));
            }
            else
            {
                Mult.Children.Add(match(TOKEN_ENUM.TOKEN_DIVIDE_OPERATOR));

            }
            return Mult;
        }
        Node EqDash()
        {
            Node Eq = new Node("More Of equation");
            if (InputPointer < TokenStream.Count && (TokenStream[InputPointer]._type != TOKEN_ENUM.TOKEN_PLUS_OPERATOR &&
                TokenStream[InputPointer]._type != TOKEN_ENUM.TOKEN_MINUS_OPERATOR))
                return null;

            Eq.Children.Add(AddOp());
            Eq.Children.Add(Equation());
            Eq.Children.Add(EqDash());
            return Eq;
        }
       Node LFactorR()
        {
            Node Fact = new Node("LeftPrances Factor RightPrances");
            Fact.Children.Add(Factor());
            Fact.Children.Add(LFactorRDash());
            return Fact;

        }
        Node LFactorRDash()
        {
            Node LFact = new Node("More Factor");
            if(InputPointer < TokenStream.Count && (TokenStream[InputPointer]._type != TOKEN_ENUM.TOKEN_MULTIPLY_OPERATOR &&
                TokenStream[InputPointer]._type != TOKEN_ENUM.TOKEN_DIVIDE_OPERATOR))
            {
                return null;
            }
            else
            {
                LFact.Children.Add(MulOp());
                LFact.Children.Add(LFactorR());
                LFact.Children.Add(LFactorRDash());
            }
            return LFact;
        }
        
        public Node match(TOKEN_ENUM ExpectedToken)
        {

            if (InputPointer < TokenStream.Count)
            {
                if (ExpectedToken == TokenStream[InputPointer]._type)
                {
                    InputPointer++;
                    Node newNode = new Node(ExpectedToken.ToString());

                    return newNode;

                }

                else
                {
                    
                    Errors.Error_List.Add("Parsing Error: Expected "
                        + ExpectedToken.ToString() + " and " +
                        TokenStream[InputPointer]._type.ToString() +
                        "  found\r\n");
                    InputPointer++;
                    return null;
                }
            }
            else
            {
                Errors.Error_List.Add("Parsing Error: Expected "
                        + ExpectedToken.ToString() + "\r\n");
                InputPointer++;
                return null;
            }
        }

        public static TreeNode PrintParseTree(Node root)
        {
            TreeNode tree = new TreeNode("Parse Tree");
            TreeNode treeRoot = PrintTree(root);
            if (treeRoot != null)
                tree.Nodes.Add(treeRoot);
            return tree;
        }
        static TreeNode PrintTree(Node root)
        {
            if (root == null || root.Name == null)
                return null;
            TreeNode tree = new TreeNode(root.Name);
            if (root.Children.Count == 0)
                return tree;
            foreach (Node child in root.Children)
            {
                if (child == null)
                    continue;
                tree.Nodes.Add(PrintTree(child));
            }
            return tree;
        }
    }
}