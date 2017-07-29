using System;
using System.Collections;
using System.Collections.Generic;

namespace Calculator
{
    public static class Extensions
    {
        public static char Punctuator(this TokenType type)
        {
            switch (type)
            {
                case TokenType.LEFT_PAREN: return '(';
                case TokenType.RIGHT_PAREN: return ')';
                case TokenType.COMMA: return ',';
                case TokenType.ASSIGN: return '=';
                case TokenType.PLUS: return '+';
                case TokenType.MINUS: return '-';
                case TokenType.ASTERISK: return '*';
                case TokenType.SLASH: return '/';
                case TokenType.CARET: return '^';
                case TokenType.TILDE: return '~';
                case TokenType.BANG: return '!';
                case TokenType.QUESTION: return '?';
                case TokenType.COLON: return ':';
                default: return ' ';
            }
        }
    }

    public enum TokenType
    {
        LEFT_PAREN,
        RIGHT_PAREN,
        COMMA,
        ASSIGN,
        PLUS,
        MINUS,
        ASTERISK,
        SLASH,
        CARET,
        TILDE,
        BANG,
        QUESTION,
        COLON,
        NAME,
        EOF
    }

    public class Token
    {
        public Token(TokenType type, string text)
        {
            Type = type;
            Text = text;
        }

        public TokenType Type { get; }
        public string Text { get; }

        public override string ToString() { return Text; }
    }

    public class Lexer : IEnumerable<Token>
    {

        private Dictionary<char, TokenType> punctuators = new Dictionary<char, TokenType>();
        private string text;
        private int index = 0;

        public Lexer(String text)
        {
            index = 0;
            this.text = text;

            // Register all of the TokenTypes that are explicit punctuators.
            foreach (var type in Enum.GetValues(typeof(TokenType)))
            {
                char punctuator = ((TokenType)type).Punctuator();
                if (punctuator != ' ')
                {
                    punctuators[punctuator] = (TokenType)type;
                }
            }
        }
        
        IEnumerator<Token> IEnumerable<Token>.GetEnumerator()
        {
            while (index < text.Length)
            {
                char c = text[index++];

                if (punctuators.ContainsKey(c))
                {
                    // Handle punctuation.
                    yield return new Token(punctuators[c], c.ToString());
                }
                else if (char.IsLetter(c))
                {
                    // Handle names.
                    int start = index - 1;
                    while (index < text.Length)
                    {
                        if (!char.IsLetter(text[index])) break;
                        index++;
                    }

                    String name = text.Substring(start, index - start);
                    yield return new Token(TokenType.NAME, name);
                }
                else
                {
                    // Ignore all other characters (whitespace, etc.)
                }
            }

            // Once we've reached the end of the string, just return EOF tokens. We'll
            // just keeping returning them as many times as we're asked so that the
            // parser's lookahead doesn't have to worry about running out of tokens.
            while (true)
            {
                yield return new Token(TokenType.EOF, "");
            }
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}
