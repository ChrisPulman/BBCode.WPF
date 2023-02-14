// Copyright (c) Chris Pulman. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Globalization;

namespace CP.BBCode.WPF.Core
{
    /// <summary>
    /// Represents a single token.
    /// </summary>
    internal class Token
    {
        /// <summary>
        /// Represents the token that marks the end of the input.
        /// </summary>
        public static readonly Token End = new(string.Empty, Tokeniser.TokenEnd);

        /// <summary>
        /// Initializes a new instance of the <see cref="Token"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="tokenType">Type of the token.</param>
        public Token(string value, int tokenType)
        {
            Value = value;
            TokenType = tokenType;
        }

        /// <summary>
        /// Gets the type of the token.
        /// </summary>
        /// <value>The type.</value>
        public int TokenType { get; }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <value>The value.</value>
        public string Value { get; }

        /// <summary>
        /// Returns a <see cref="string"/> that represents the current <see cref="object"/>.
        /// </summary>
        /// <returns>A <see cref="string"/> that represents the current <see cref="object"/>.</returns>
        public override string ToString() => string.Format(CultureInfo.InvariantCulture, "{0}: {1}", TokenType, Value);
    }
}
