// Copyright (c) Chris Pulman. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace CP.BBCode.WPF.Core
{
    /// <summary>
    /// Provides basic parse functionality.
    /// </summary>
    /// <typeparam name="TResult">The type of the result.</typeparam>
    internal abstract class Parser<TResult>
    {
        /// <summary>
        /// The buffer.
        /// </summary>
        private readonly TokenBuffer _buffer;

        /// <summary>
        /// Initializes a new instance of the <see cref="Parser{TResult}"/> class.
        /// </summary>
        /// <param name="lexer">The lexer.</param>
        /// <exception cref="ArgumentNullException">lexer Argument Null Exception.</exception>
        protected Parser(Tokeniser lexer)
        {
            if (lexer == null)
            {
                throw new ArgumentNullException(nameof(lexer));
            }

            _buffer = new TokenBuffer(lexer);
        }

        /// <summary>
        /// Parses the text and returns an object of type TResult.
        /// </summary>
        /// <returns>A T.</returns>
        public abstract TResult Parse();

        /// <summary>
        /// Consumes the next token.
        /// </summary>
        protected void Consume() => _buffer.Consume();

        /// <summary>
        /// Determines whether the current token is in specified range.
        /// </summary>
        /// <param name="tokenTypes">The token types.</param>
        /// <returns><c>true</c> if current token is in specified range; otherwise, <c>false</c>.</returns>
        protected bool IsInRange(params int[] tokenTypes)
        {
            if (tokenTypes == null)
            {
                return false;
            }

            var token = LA(1);
            return tokenTypes.Any(t => token.TokenType == t);
        }

        /// <summary>
        /// Performs a token look-ahead.
        /// </summary>
        /// <param name="count">The count.</param>
        /// <returns>A token.</returns>
        protected Token LA(int count) => _buffer.LA(count);

        /// <summary>
        /// Matches the specified token type.
        /// </summary>
        /// <param name="tokenType">Type of the token.</param>
        /// <exception cref="ParseException">Token mismatch.</exception>
        protected void Match(int tokenType)
        {
            if (LA(1).TokenType == tokenType)
            {
                Consume();
            }
            else
            {
                throw new ParseException("Token mismatch");
            }
        }

        /// <summary>
        /// Does not matches the specified token type.
        /// </summary>
        /// <param name="tokenType">Type of the token.</param>
        /// <exception cref="ParseException">Token mismatch.</exception>
        protected void MatchNot(int tokenType)
        {
            if (LA(1).TokenType != tokenType)
            {
                Consume();
            }
            else
            {
                throw new ParseException("Token mismatch");
            }
        }

        /// <summary>
        /// Matches the range.
        /// </summary>
        /// <param name="tokenTypes">The token types.</param>
        /// <param name="minOccurs">The min occurs.</param>
        /// <param name="maxOccurs">The max occurs.</param>
        /// <exception cref="ParseException">Invalid number of tokens.</exception>
        protected void MatchRange(int[] tokenTypes, int minOccurs, int maxOccurs)
        {
            var i = 0;
            while (IsInRange(tokenTypes))
            {
                Consume();
                i++;
            }

            if (i < minOccurs || i > maxOccurs)
            {
                throw new ParseException("Invalid number of tokens");
            }
        }
    }
}
