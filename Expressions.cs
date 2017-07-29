using System.Collections.Generic;
using System.Text;

namespace Calculator
{
    public interface IExpression
    {
        string ToString();
    }

    public class AssignExpression : IExpression
    {
        private string name;
        private IExpression right;

        public AssignExpression(string name, IExpression right)
        {
            this.name = name;
            this.right = right;
        }

        public override string ToString()
        {
            return "(" + name + " = " + right.ToString() + ")";
        }
    }

    public class CallExpression : IExpression
    {
        private IExpression function;
        private List<IExpression> args;

        public CallExpression(IExpression function, List<IExpression> args)
        {
            this.function = function;
            this.args = args;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append(function.ToString());
            sb.Append("(");
            for (int i = 0; i < args.Count; i++)
            {
                sb.Append(args[i].ToString());
                if (i < (args.Count - 1))
                {
                    sb.Append(", ");
                }
            }
            sb.Append(")");
            return sb.ToString();
        }
    }

    public class ConditionalExpression : IExpression
    {
        private IExpression condition;
        private IExpression thenArm;
        private IExpression elseArm;

        public ConditionalExpression(IExpression condition, IExpression thenArm, IExpression elseArm)
        {
            this.condition = condition;
            this.thenArm = thenArm;
            this.elseArm = elseArm;
        }

        public override string ToString()
        {
            return "(" + condition.ToString() + " ? " + thenArm.ToString() + " : " + elseArm.ToString() + ")";
        }
    }

    public class NameExpression : IExpression
    {
        public NameExpression(string name)
        {
            Name = name;
        }

        public string Name { get; }

        public override string ToString()
        {
            return Name.ToString();
        }
    }

    public class OperatorExpression : IExpression
    {
        private IExpression left;
        private TokenType  op;
        private IExpression right;

        public OperatorExpression(IExpression left, TokenType op, IExpression right)
        {
            this.left = left;
            this.op = op;
            this.right = right;
        }

        public override string ToString()
        {
            return "(" + left.ToString() + " " + op.Punctuator() + " " + right.ToString() + ")";
        }
    }

    public class PostfixExpression : IExpression
    {
        private IExpression left;
        private TokenType op;

        public PostfixExpression(IExpression left, TokenType op)
        {
            this.left = left;
            this.op = op;
        }

        public override string ToString()
        {
            return "(" + left.ToString() + op.Punctuator() + ")";
        }
    }

    public class PrefixExpression : IExpression
    {
        private TokenType  op;
        private IExpression right;

        public PrefixExpression(TokenType op, IExpression right)
        {
            this.op = op;
            this.right = right;
        }

        public override string ToString()
        {
            return "(" + op.Punctuator() + right.ToString() + ")";
        }
    }
}
