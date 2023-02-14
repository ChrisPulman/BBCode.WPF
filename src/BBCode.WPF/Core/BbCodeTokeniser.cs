// Copyright (c) Chris Pulman. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace CP.BBCode.WPF.Core
{
    /// <summary>
    /// The BBCode lexer.
    /// </summary>
    internal class BbCodeTokeniser
        : Tokeniser
    {
        /// <summary>
        /// Normal state.
        /// </summary>
        public const int StateNormal = 0;

        /// <summary>
        /// Tag state.
        /// </summary>
        public const int StateTag = 1;

        /// <summary>
        /// The token attribute.
        /// </summary>
        public const int TokenAttribute = 2;

        /// <summary>
        /// End tag.
        /// </summary>
        public const int TokenEndTag = 1;

        /// <summary>
        /// Line break.
        /// </summary>
        public const int TokenLineBreak = 4;

        /// <summary>
        /// Image.
        /// </summary>
        public const int TokenImage = 5;

        /// <summary>
        /// Image.
        /// </summary>
        public const int TokenLink = 6;

        /// <summary>
        /// Start tag.
        /// </summary>
        public const int TokenStartTag = 0;

        /// <summary>
        /// The token text.
        /// </summary>
        public const int TokenText = 3;

        /// <summary>
        /// The newline chars.
        /// </summary>
        private static readonly char[] NewlineChars = { '\r', '\n' };

        /// <summary>
        /// The quote chars.
        /// </summary>
        private static readonly char[] QuoteChars = { '\'', '"' };

        /// <summary>
        /// The whitespace chars.
        /// </summary>
        private static readonly char[] WhitespaceChars = { ' ', '\t' };

        /// <summary>
        /// Initializes a new instance of the <see cref="BbCodeTokeniser"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        public BbCodeTokeniser(string value)
            : base(value)
        {
        }

        /// <summary>
        /// Gets the default state of the lexer.
        /// </summary>
        /// <value>The state of the default.</value>
        protected override int DefaultState => StateNormal;

        /// <summary>
        /// Gets the next token.
        /// </summary>
        /// <returns>A Token.</returns>
        /// <exception cref="ParseException">A Parse Exception.</exception>
        public override Token NextToken()
        {
            if (LookAhead(1) == char.MaxValue)
            {
                return Token.End;
            }

            if (State == StateNormal)
            {
                if (LookAhead(1) == '[')
                {
                    if (LookAhead(2) == '/')
                    {
                        return CloseTag();
                    }

                    var token = OpenTag();
                    PushState(StateTag);
                    return token;
                }

                return IsInRange(NewlineChars) ? Newline() : Text();
            }

            if (State == StateTag)
            {
                if (LookAhead(1) == ']')
                {
                    Consume();
                    PopState();
                    return NextToken();
                }

                return Attribute();
            }

            throw new ParseException("Invalid state");
        }

        /// <summary>
        /// Attributes this instance.
        /// </summary>
        /// <returns>A Token.</returns>
        private Token Attribute()
        {
            Match('=');
            while (IsInRange(WhitespaceChars))
            {
                Consume();
            }

            Token token;

            if (IsInRange(QuoteChars))
            {
                Consume();
                Mark();
                while (!IsInRange(QuoteChars))
                {
                    Consume();
                }

                token = new Token(GetMark, TokenAttribute);
                Consume();
            }
            else
            {
                Mark();
                while (!IsInRange(WhitespaceChars) && LookAhead(1) != ']' && LookAhead(1) != char.MaxValue)
                {
                    Consume();
                }

                token = new Token(GetMark, TokenAttribute);
            }

            while (IsInRange(WhitespaceChars))
            {
                Consume();
            }

            return token;
        }

        /// <summary>
        /// Closes the tag.
        /// </summary>
        /// <returns>A Token.</returns>
        private Token CloseTag()
        {
            Match('[');
            Match('/');

            Mark();
            while (IsTagNameChar())
            {
                Consume();
            }

            var token = new Token(GetMark, TokenEndTag);
            Match(']');

            return token;
        }

        /// <summary>
        /// Determines whether [is tag name character].
        /// </summary>
        /// <returns>A boolean.</returns>
        private bool IsTagNameChar() => IsInRange('A', 'Z') || IsInRange('a', 'z') || IsInRange(new[] { '*' });

        /// <summary>
        /// Newlines this instance.
        /// </summary>
        /// <returns>A Token.</returns>
        private Token Newline()
        {
            Match('\r', 0, 1);
            Match('\n');

            return new Token(string.Empty, TokenLineBreak);
        }

        /// <summary>
        /// Opens the tag.
        /// </summary>
        /// <returns>A Token.</returns>
        private Token OpenTag()
        {
            Match('[');
            Mark();
            while (IsTagNameChar())
            {
                Consume();
            }

            return GetMark switch
            {
                "url" => new Token(GetMark, TokenLink),
                "img" => new Token(GetMark, TokenImage),
                "br" => new Token(GetMark, TokenLineBreak),
                _ => new Token(GetMark, TokenStartTag),
            };
        }

        /// <summary>
        /// Texts this instance.
        /// </summary>
        /// <returns>A Token.</returns>
        private Token Text()
        {
            Mark();
            while (LookAhead(1) != '[' && LookAhead(1) != char.MaxValue && !IsInRange(NewlineChars))
            {
                Consume();
            }

            return new Token(GetMark, TokenText);
        }
    }
}
