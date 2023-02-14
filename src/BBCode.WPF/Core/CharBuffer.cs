// Copyright (c) Chris Pulman. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace CP.BBCode.WPF.Core
{
    /// <summary>
    /// Represents a character buffer.
    /// </summary>
    internal class CharBuffer
    {
        /// <summary>
        /// The value.
        /// </summary>
        private readonly string _value;

        /// <summary>
        /// The mark.
        /// </summary>
        private int _mark;

        /// <summary>
        /// The position.
        /// </summary>
        private int _position;

        /// <summary>
        /// Initializes a new instance of the <see cref="CharBuffer"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <exception cref="ArgumentNullException">value Argument Null Exception.</exception>
        public CharBuffer(string value) => _value = value ?? throw new ArgumentNullException(nameof(value));

        /// <summary>
        /// Gets the get mark.
        /// </summary>
        /// <value>The get mark.</value>
        public string GetMark => _mark < _position ? _value.Substring(_mark, _position - _mark) : string.Empty;

        /// <summary>
        /// Consumes the next character.
        /// </summary>
        public void Consume() => _position++;

        /// <summary>
        /// Performs a look-ahead.
        /// </summary>
        /// <param name="count">The number of character to look ahead.</param>
        /// <returns>A char.</returns>
        public char LA(int count)
        {
            var index = _position + count - 1;
            return index < _value.Length ? _value[index] : char.MaxValue;
        }

        /// <summary>
        /// Marks the current position.
        /// </summary>
        public void Mark() => _mark = _position;
    }
}
