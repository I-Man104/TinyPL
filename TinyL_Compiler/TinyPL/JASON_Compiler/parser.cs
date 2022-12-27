using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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
            program.Children.Add(Statements());

            program.Children.Add(Main_Function());


            MessageBox.Show("Success");
            return program;
        }

        Node Main_Function()
        {
            Node _main = new Node("Main_Function");

            _main.Children.Add(Data_Type());
            _main.Children.Add(match(TOKEN_ENUM.TOKEN_MAIN));


            return _main;
        }

        Node Statements()
        {
            Node state = new Node("statements");
            state.Children.Add(Statement());
            state.Children.Add(State());

            return state;
        }
        Node Statement()
        {
            Node state = new Node("Statement");
            if (TokenStream[InputPointer]._type == TOKEN_ENUM.TOKEN_READ)
            {
                state.Children.Add(match(TOKEN_ENUM.TOKEN_READ));
                state.Children.Add(match(TOKEN_ENUM.TOKEN_IDENTIFIER));
                state.Children.Add(match(TOKEN_ENUM.TOKEN_SEMICOLON));
                //return state;

            }
            else if (TokenStream[InputPointer]._type == TOKEN_ENUM.TOKEN_WRITE)
            {
                state.Children.Add(match(TOKEN_ENUM.TOKEN_WRITE));
                if (Expression() != null)
                {
                    state.Children.Add(Expression());

                }
                if (TokenStream[InputPointer]._type == TOKEN_ENUM.TOKEN_ENDL)
                {
                    state.Children.Add(match(TOKEN_ENUM.TOKEN_ENDL));

                }

                state.Children.Add(match(TOKEN_ENUM.TOKEN_SEMICOLON));
                // return state;

            }
            else if (TokenStream[InputPointer]._type == TOKEN_ENUM.TOKEN_IDENTIFIER)
            {

                state.Children.Add(match(TOKEN_ENUM.TOKEN_IDENTIFIER));
                state.Children.Add(match(TOKEN_ENUM.TOKEN_BINDING));
                state.Children.Add(Expression());
                //state.Children.Add(match(TOKEN_ENUM.TOKEN_SEMICOLON));
                // return state;

            }
            else if (TokenStream[InputPointer]._type == TOKEN_ENUM.TOKEN_RETURN)
            {

                state.Children.Add(match(TOKEN_ENUM.TOKEN_RETURN));
                state.Children.Add(Expression());
                //state.Children.Add(match(TOKEN_ENUM.TOKEN_SEMICOLON));
                //return state;

            }
            else if (Data_Type() != null) //Declaration statement
            {
                if (TokenStream[InputPointer]._type == TOKEN_ENUM.TOKEN_STRING)
                {
                    state.Children.Add(match(TOKEN_ENUM.TOKEN_STRING));
                }
                else if (TokenStream[InputPointer]._type == TOKEN_ENUM.TOKEN_INT)
                {
                    state.Children.Add(match(TOKEN_ENUM.TOKEN_INT));
                }
                else if (TokenStream[InputPointer]._type == TOKEN_ENUM.TOKEN_FLOAT)
                {
                    state.Children.Add(match(TOKEN_ENUM.TOKEN_FLOAT));
                }

                state.Children.Add(match(TOKEN_ENUM.TOKEN_IDENTIFIER));
                if (TokenStream[InputPointer]._type == TOKEN_ENUM.TOKEN_SEMICOLON) return state;
                state.Children.Add(match(TOKEN_ENUM.TOKEN_BINDING));
                state.Children.Add(Expression());
                // return state;

            }
            else if (TokenStream[InputPointer]._type == TOKEN_ENUM.TOKEN_REPEAT)
            {
                state.Children.Add(match(TOKEN_ENUM.TOKEN_REPEAT));
                state.Children.Add(Statements());
                state.Children.Add(match(TOKEN_ENUM.TOKEN_UNTIL));
                state.Children.Add(ConditionState());
                // return state;
            }
            else if (Condition() != null)
            {
                state.Children.Add(ConditionState());
            }
            else if (TokenStream[InputPointer]._type == TOKEN_ENUM.TOKEN_IF)
            {
                state.Children.Add(match(TOKEN_ENUM.TOKEN_IF));
                state.Children.Add(ConditionState());
                state.Children.Add(match(TOKEN_ENUM.TOKEN_THEN));
                state.Children.Add(Statements());
                if (TokenStream[InputPointer]._type == TOKEN_ENUM.TOKEN_ElSEIF)
                {
                    state.Children.Add(ELSEIF_State());

                }
                else if (TokenStream[InputPointer]._type == TOKEN_ENUM.TOKEN_ELSE)
                {
                    state.Children.Add(ELSE_State());

                }
                else
                {
                    state.Children.Add(match(TOKEN_ENUM.TOKEN_END));
                }
                // return state;
            }

            else
            {
                return null;
            }

            return state;

        }
        // Implement your logic here

        //function call 
        //function state
        //function declaration
        //function body
        //parameter/s

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
            if (TokenStream[InputPointer]._type == TOKEN_ENUM.TOKEN_ElSEIF)
            {
                _elseIfState.Children.Add(ELSEIF_State());

            }
            else if (TokenStream[InputPointer]._type == TOKEN_ENUM.TOKEN_ELSE)
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
            if (ConditionOP() != null)
            {
                condState.Children.Add(ConditionOP());
                condState.Children.Add(Condition());
            }
            return condState;
        }
        Node Condition()
        {
            Node cond = null;
            if (TokenStream[InputPointer]._type == TOKEN_ENUM.TOKEN_IDENTIFIER)
            {
                cond = new Node("condition");
                cond.Children.Add(match(TOKEN_ENUM.TOKEN_IDENTIFIER));
                cond.Children.Add(ConditionOP());
                cond.Children.Add(Term());
            }
            return cond;

        }
        Node ConditionOP()
        {
            Node condOp = null;
            if (TokenStream[InputPointer]._type == TOKEN_ENUM.TOKEN_LESS_THAN)
            {
                condOp = new Node("Less Than");
                condOp.Children.Add(match(TOKEN_ENUM.TOKEN_LESS_THAN));
            }
            else if (TokenStream[InputPointer]._type == TOKEN_ENUM.TOKEN_GREATER_THAN)
            {
                condOp = new Node("Greater Than");
                condOp.Children.Add(match(TOKEN_ENUM.TOKEN_GREATER_THAN));
            }
            else if (TokenStream[InputPointer]._type == TOKEN_ENUM.TOKEN_EQUALITY_OPERATOR)
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
        Node Term()
        {
            Node term = new Node("Term");
            term.Children.Add(Factor());
            term.Children.Add(Ter());
            return term;
        }
        Node Ter()
        {
            Node ter = new Node("Ter");
            if (TokenStream[InputPointer]._type == TOKEN_ENUM.TOKEN_MULTIPLY_OPERATOR ||
                TokenStream[InputPointer]._type == TOKEN_ENUM.TOKEN_DIVIDE_OPERATOR)
            {
                ter.Children.Add(ArithetmeticOp());
                ter.Children.Add(Factor());
                ter.Children.Add(Ter());
                return ter;
            }
            else
                return null;
        }
        Node Factor()
        {
            Node Fact = new Node("Factor");
            if (TokenStream[InputPointer]._type == TOKEN_ENUM.TOKEN_IDENTIFIER)
            {
                Fact.Children.Add(match(TOKEN_ENUM.TOKEN_IDENTIFIER));
            }
            else if (TokenStream[InputPointer]._type == TOKEN_ENUM.TOKEN_INT)
            {
                Fact.Children.Add(match(TOKEN_ENUM.TOKEN_INT));
            }
            else
            {
                Fact.Children.Add(match(TOKEN_ENUM.TOKEN_FLOAT));
            }
            return Fact;

        }
        Node Data_Type()
        {
            Node dataType = null;
            if (TokenStream[InputPointer]._type == TOKEN_ENUM.TOKEN_STRING)
            {
                dataType = new Node("Data Type String");
                dataType.Children.Add(match(TOKEN_ENUM.TOKEN_STRING));
            }
            else if (TokenStream[InputPointer]._type == TOKEN_ENUM.TOKEN_INT)
            {
                dataType = new Node("Data Type Integer");
                dataType.Children.Add(match(TOKEN_ENUM.TOKEN_INT));
            }
            else
            {
                dataType = new Node("Data Type Float");
                dataType.Children.Add(match(TOKEN_ENUM.TOKEN_FLOAT));
            }
            return dataType;

        }

        Node State()
        {
            Node stat = new Node("state");
            if (TokenStream[InputPointer]._type == TOKEN_ENUM.TOKEN_SEMICOLON)
            {
                stat.Children.Add(match(TOKEN_ENUM.TOKEN_SEMICOLON));
                stat.Children.Add(Statements());
                stat.Children.Add(State());
                return stat;
            }
            else
            {
                return null;
            }
        }

        Node Expression()
        {
            Node expression = null;

            if (TokenStream[InputPointer]._type == TOKEN_ENUM.TOKEN_STRING)
            {
                expression = new Node("Expression");
                expression.Children.Add(match(TOKEN_ENUM.TOKEN_STRING));
            }
            else if (TokenStream[InputPointer]._type == TOKEN_ENUM.TOKEN_INT)
            {
                expression = new Node("Expression");
                expression.Children.Add(Term());
            }
            else
            {
                expression = new Node("Expression");
                expression.Children.Add(Equation());
            }
            return expression;
        }
        Node Equation()
        {
            //term || term + - * / term
            Node equation = new Node("Equation");
            if (TokenStream[InputPointer]._type == TOKEN_ENUM.TOKEN_INT)
            {
                equation.Children.Add(Term());
            }
            else
            {
                equation.Children.Add(Equat());
            }
            equation.Children.Add(Term());
            return equation;
        }

        Node Equat()

        {
            Node equat = new Node("Equat");

            if (TokenStream[InputPointer]._type == TOKEN_ENUM.TOKEN_MINUS_OPERATOR ||
                TokenStream[InputPointer]._type == TOKEN_ENUM.TOKEN_PLUS_OPERATOR ||
                TokenStream[InputPointer]._type == TOKEN_ENUM.TOKEN_MULTIPLY_OPERATOR ||
                TokenStream[InputPointer]._type == TOKEN_ENUM.TOKEN_DIVIDE_OPERATOR)
            {
                equat.Children.Add(ArithetmeticOp());
                equat.Children.Add(Term());
            }
            else
                return null;

            return equat;
        }
        Node Exp()
        {
            Node exp = new Node("exp");
            if (TokenStream[InputPointer]._type == TOKEN_ENUM.TOKEN_PLUS_OPERATOR || TokenStream[InputPointer]._type == TOKEN_ENUM.TOKEN_MINUS_OPERATOR)
            {
                exp.Children.Add(ArithetmeticOp());
                exp.Children.Add(Term());
                exp.Children.Add(Exp());
                return exp;
            }
            else
            {
                return null;
            }

        }
        Node ArithetmeticOp()
        {

            if (TokenStream[InputPointer]._type == TOKEN_ENUM.TOKEN_PLUS_OPERATOR)
            {
                Node add = new Node("AddOp");
                add.Children.Add(match(TOKEN_ENUM.TOKEN_PLUS_OPERATOR));
                return add;
            }
            else if (TokenStream[InputPointer]._type == TOKEN_ENUM.TOKEN_MINUS_OPERATOR)
            {
                Node minus = new Node("MinusOp");
                minus.Children.Add(match(TOKEN_ENUM.TOKEN_MINUS_OPERATOR));
                return minus;
            }

            else if (TokenStream[InputPointer]._type == TOKEN_ENUM.TOKEN_MULTIPLY_OPERATOR)
            {
                Node mult = new Node("MultOp");
                mult.Children.Add(match(TOKEN_ENUM.TOKEN_MULTIPLY_OPERATOR));
                return mult;
            }
            else
            {
                Node div = new Node("DivideOp");
                div.Children.Add(match(TOKEN_ENUM.TOKEN_DIVIDE_OPERATOR));
                return div;
            }
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