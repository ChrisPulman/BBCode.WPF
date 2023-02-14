// Copyright (c) Chris Pulman. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace CP.BBCode.WPF.Core
{
    /// <summary>
    /// Represents a token buffer.
    /// </summary>
    internal class TokenBuffer
    {
        /// <summary>
        /// The tokens.
        /// </summary>
        private readonly List<Token> _tokens = new();

        /// <summary>
        /// The position.
        /// </summary>
        private int _position;

        /// <summary>
        /// Initializes a new instance of the <see cref="TokenBuffer"/> class.
        /// </summary>
        /// <param name="lexer">The lexer.</param>
        /// <exception cref="ArgumentNullException">lexer Argument Null Exception.</exception>
        public TokenBuffer(Tokeniser lexer)
        {
            if (lexer == null)
            {
                throw new ArgumentNullException(nameof(lexer));
            }

            Token token;
            do
            {
                token = lexer.NextToken();
                _tokens.Add(token);
            }
            while (token.TokenType != Tokeniser.TokenEnd);
        }

        /// <summary>
        /// Consumes the next token.
        /// </summary>
        public void Consume() => _position++;

        /// <summary>
        /// Performs a look-ahead.
        /// </summary>
        /// <param name="count">The number of tokens to look ahead.</param>
        /// <returns>A token.</returns>
        public Token LA(int count)
        {
            var index = _position + count - 1;
            return index < _tokens.Count ? _tokens[index] : Token.End;
        }
    }
}
