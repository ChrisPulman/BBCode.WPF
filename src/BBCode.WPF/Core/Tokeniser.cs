// Copyright (c) Chris Pulman. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace CP.BBCode.WPF.Core
{
    /// <summary>
    /// Provides basic LEXER functionality.
    /// </summary>
    internal abstract class Tokeniser
    {
        /// <summary>
        /// Defines the end-of-file token type.
        /// </summary>
        public const int TokenEnd = int.MaxValue;

        /// <summary>
        /// The buffer.
        /// </summary>
        private readonly CharBuffer _buffer;

        /// <summary>
        /// The states.
        /// </summary>
        private readonly Stack<int> _states;

        /// <summary>
        /// Initializes a new instance of the <see cref="Tokeniser"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        protected Tokeniser(string value)
        {
            _buffer = new CharBuffer(value);
            _states = new Stack<int>();
        }

        /// <summary>
        /// Gets the get mark.
        /// </summary>
        /// <value>The get mark.</value>
        protected string GetMark => _buffer.GetMark;

        /// <summary>
        /// Gets the default state of the LEXER.
        /// </summary>
        /// <value>The state of the default.</value>
        protected abstract int DefaultState { get; }

        /// <summary>
        /// Gets the current state of the LEXER.
        /// </summary>
        /// <value>The state.</value>
        protected int State => _states.Count > 0 ? _states.Peek() : DefaultState;

        /// <summary>
        /// Gets the next token.
        /// </summary>
        /// <returns>A token.</returns>
        public abstract Token NextToken();

        /// <summary>
        /// Consumes the next character.
        /// </summary>
        protected void Consume() => _buffer.Consume();

        /// <summary>
        /// Determines whether the current character is in given range.
        /// </summary>
        /// <param name="first">The first.</param>
        /// <param name="last">The last.</param>
        /// <returns><c>true</c> if the current character is in given range; otherwise, <c>false</c>.</returns>
        protected bool IsInRange(char first, char last)
        {
            var la = LookAhead(1);
            return la >= first && la <= last;
        }

        /// <summary>
        /// Determines whether the current character is in given range.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns><c>true</c> if the current character is in given range; otherwise, <c>false</c>.</returns>
        protected bool IsInRange(char[] value)
        {
            if (value == null)
            {
                return false;
            }

            var la = LookAhead(1);
            return value.Any(t => la == t);
        }

        /// <summary>
        /// Performs a look-ahead.
        /// </summary>
        /// <param name="count">The number of characters to look ahead.</param>
        /// <returns>A char.</returns>
        protected char LookAhead(int count) => _buffer.LA(count);

        /// <summary>
        /// Marks the current position.
        /// </summary>
        protected void Mark() => _buffer.Mark();

        /// <summary>
        /// Matches the specified character.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <exception cref="ParseException">A ParseException.</exception>
        protected void Match(char value)
        {
            if (LookAhead(1) == value)
            {
                Consume();
            }
            else
            {
                throw new ParseException("Character mismatch");
            }
        }

        /// <summary>
        /// Matches the specified character.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="minOccurs">The min occurs.</param>
        /// <param name="maxOccurs">The max occurs.</param>
        protected void Match(char value, int minOccurs, int maxOccurs)
        {
            var i = 0;
            while (LookAhead(1) == value)
            {
                Consume();
                i++;
            }

            ValidateOccurence(i, minOccurs, maxOccurs);
        }

        /// <summary>
        /// Matches the specified string.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <exception cref="ArgumentNullException"><paramref name="value"/>"/&gt; is <c>null</c>.</exception>
        /// <exception cref="ParseException">A ParseException.</exception>
        protected void Match(string value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            foreach (var t in value)
            {
                if (LookAhead(1) == t)
                {
                    Consume();
                }
                else
                {
                    throw new ParseException("String mismatch");
                }
            }
        }

        /// <summary>
        /// Matches the range.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <exception cref="ParseException">A ParseException.</exception>
        protected void MatchRange(char[] value)
        {
            if (IsInRange(value))
            {
                Consume();
            }
            else
            {
                throw new ParseException("Character mismatch");
            }
        }

        /// <summary>
        /// Matches the range.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="minOccurs">The min occurs.</param>
        /// <param name="maxOccurs">The max occurs.</param>
        protected void MatchRange(char[] value, int minOccurs, int maxOccurs)
        {
            var i = 0;
            while (IsInRange(value))
            {
                Consume();
                i++;
            }

            ValidateOccurence(i, minOccurs, maxOccurs);
        }

        /// <summary>
        /// Matches the range.
        /// </summary>
        /// <param name="first">The first.</param>
        /// <param name="last">The last.</param>
        /// <exception cref="ParseException">A ParseException.</exception>
        protected void MatchRange(char first, char last)
        {
            if (IsInRange(first, last))
            {
                Consume();
            }
            else
            {
                throw new ParseException("Character mismatch");
            }
        }

        /// <summary>
        /// Matches the range.
        /// </summary>
        /// <param name="first">The first.</param>
        /// <param name="last">The last.</param>
        /// <param name="minOccurs">The min occurs.</param>
        /// <param name="maxOccurs">The max occurs.</param>
        protected void MatchRange(char first, char last, int minOccurs, int maxOccurs)
        {
            var i = 0;
            while (IsInRange(first, last))
            {
                Consume();
                i++;
            }

            ValidateOccurence(i, minOccurs, maxOccurs);
        }

        /// <summary>
        /// Pops the state.
        /// </summary>
        /// <returns>A integer.</returns>
        protected int PopState() => _states.Pop();

        /// <summary>
        /// Pushes a new state on the stack.
        /// </summary>
        /// <param name="state">The state.</param>
        protected void PushState(int state) => _states.Push(state);

        /// <summary>
        /// Validates the occurrence.
        /// </summary>
        /// <param name="count">The count.</param>
        /// <param name="minOccurs">The minimum occurs.</param>
        /// <param name="maxOccurs">The maximum occurs.</param>
        /// <exception cref="ParseException">Invalid number of characters.</exception>
        private static void ValidateOccurence(int count, int minOccurs, int maxOccurs)
        {
            if (count < minOccurs || count > maxOccurs)
            {
                throw new ParseException("Invalid number of characters");
            }
        }
    }
}
