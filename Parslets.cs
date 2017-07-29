using System;
using System.Collections.Generic;

namespace Calculator
{
    public enum ParsletPrecedence : int
    {
        ASSIGNMENT = 1,
        CONDITIONAL = 2,
        SUM = 3,
        PRODUCT = 4,
        EXPONENT = 5,
        PREFIX = 6,
        POSTFIX = 7,
        CALL = 8
    }

    public interface IInfixParselet
    {
        IExpression Parse(Parser parser, IExpression left, Token token);
        ParsletPrecedence Precedence { get; }
    }

    public interface IPrefixParselet
    {
        IExpression Parse(Parser parser, Token token);
    }

    public class AssignParselet : IInfixParselet
    {
        public ParsletPrecedence Precedence { get { return ParsletPrecedence.ASSIGNMENT; } }

        public IExpression Parse(Parser parser, IExpression left, Token token)
        {
            IExpression right = parser.ParseExpression((int)ParsletPrecedence.ASSIGNMENT - 1);

            if (!(left is NameExpression)) throw new ParseException(
                "The left-hand side of an assignment must be a name.");

            return new AssignExpression(((NameExpression)left).ToString(), right);
        }
    }

    public class BinaryOperatorParselet : IInfixParselet
    {
        public ParsletPrecedence Precedence { get; }
        private bool isRight;

        public BinaryOperatorParselet(ParsletPrecedence precedence, bool isRight)
        {
            Precedence = precedence;
            this.isRight = isRight;
        }

        public IExpression Parse(Parser parser, IExpression left, Token token)
        {
            // To handle right-associative operators like "^", we allow a slightly
            // lower precedence when parsing the right-hand side. This will let a
            // parselet with the same precedence appear on the right, which will then
            // take *this* parselet's result as its left-hand argument.
            IExpression right = parser.ParseExpression(Precedence - (isRight ? 1 : 0));

            return new OperatorExpression(left, token.Type, right);
        }
    }

    public class CallParselet : IInfixParselet
    {
        public ParsletPrecedence Precedence { get { return ParsletPrecedence.CALL; } }

        public IExpression Parse(Parser parser, IExpression left, Token token)
        {
            // Parse the comma-separated arguments until we hit, ")".
            List<IExpression> args = new List<IExpression>();

            // There may be no arguments at all.
            if (!parser.Match(TokenType.RIGHT_PAREN))
            {
                do
                {
                    args.Add(parser.ParseExpression());
                } while (parser.Match(TokenType.COMMA));
                parser.Consume(TokenType.RIGHT_PAREN);
            }

            return new CallExpression(left, args);
        }
    }

    public class ConditionalParselet : IInfixParselet
    {
        public ParsletPrecedence Precedence { get { return ParsletPrecedence.CONDITIONAL; } }
        public IExpression Parse(Parser parser, IExpression left, Token token)
        {
            IExpression thenArm = parser.ParseExpression();
            parser.Consume(TokenType.COLON);
            IExpression elseArm = parser.ParseExpression(ParsletPrecedence.CONDITIONAL - 1);

            return new ConditionalExpression(left, thenArm, elseArm);
        }
    }

    public class GroupParselet : IPrefixParselet
    {
        public IExpression Parse(Parser parser, Token token)
        {
            IExpression expression = parser.ParseExpression();
            parser.Consume(TokenType.RIGHT_PAREN);
            return expression;
        }
    }

    public class NameParselet : IPrefixParselet
    {
        public IExpression Parse(Parser parser, Token token)
        {
            return new NameExpression(token.Text);
        }
    }

    public class PostfixOperatorParselet : IInfixParselet
    {
        public PostfixOperatorParselet(ParsletPrecedence precedence)
        {
            Precedence = precedence;
        }

        public ParsletPrecedence Precedence { get; }

        public IExpression Parse(Parser parser, IExpression left, Token token)
        {
            return new PostfixExpression(left, token.Type);
        }
    }

    public class PrefixOperatorParselet : IPrefixParselet
    {
        public PrefixOperatorParselet(ParsletPrecedence precedence)
        {
            Precedence = precedence;
        }

        public ParsletPrecedence Precedence { get; }
        public IExpression Parse(Parser parser, Token token)
        {
            // To handle right-associative operators like "^", we allow a slightly
            // lower precedence when parsing the right-hand side. This will let a
            // parselet with the same precedence appear on the right, which will then
            // take *this* parselet's result as its left-hand argument.
            IExpression right = parser.ParseExpression(Precedence);

            return new PrefixExpression(token.Type, right);
        }
    }
}
