// Copyright (c) Chris Pulman. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using CP.BBCode.WPF.Annotations;

namespace CP.BBCode.WPF.Core
{
    /// <summary>
    /// Represents the BBCode parser supporting a basic set of BBCode tags.
    /// </summary>
    internal class BbCodeParser
        : Parser<Span>
    {
        private const string TagBold = "b";
        private const string TagBreak = "br";
        private const string TagColor = "color";
        private const string TagFont = "font";
        private const string TagImage = "img";
        private const string TagItalic = "i";
        private const string TagSize = "size";
        private const string TagStrikeThrough = "s";
        private const string TagUnderline = "u";
        private const string TagUrl = "url";
        private readonly BBCodeBlock _source;

        /// <summary>
        /// Initializes a new instance of the <see cref="BbCodeParser"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="source">The source.</param>
        /// <exception cref="System.ArgumentNullException">source Argument Null Exception.</exception>
        public BbCodeParser(string value, BBCodeBlock source)
            : base(new BbCodeTokeniser(value)) => _source = source ?? throw new ArgumentNullException(nameof(source));

        /// <summary>
        /// Parses the text and returns a Span containing the parsed result.
        /// </summary>
        /// <returns>A span.</returns>
        public override Span Parse()
        {
            var span = new Span();

            Parse(span);

            return span;
        }

        /// <summary>
        /// Parses the specified span.
        /// </summary>
        /// <param name="span">The span.</param>
        /// <exception cref="ParseException">Parse Exception.</exception>
        private void Parse(Span span)
        {
            var context = new ParseContext(span);

            while (true)
            {
                var token = LA(1);
                Consume();

                if (token.TokenType == BbCodeTokeniser.TokenStartTag)
                {
                    ParseTag(token.Value, true, context);
                }
                else if (token.TokenType == BbCodeTokeniser.TokenEndTag)
                {
                    ParseTag(token.Value, false, context);
                }
                else if (token.TokenType == BbCodeTokeniser.TokenText)
                {
                    var run = context.CreateRun(token.Value);
                    span.Inlines.Add(run);
                }
                else if (token.TokenType == BbCodeTokeniser.TokenLink)
                {
                    ParseTag(token.Value, true, context);
                    if (context.NavigateUri != null)
                    {
                        var tokenText = LA(1);
                        var handled = false;
                        if (tokenText.TokenType == BbCodeTokeniser.TokenText)
                        {
                            Consume();
                            if (!string.IsNullOrEmpty(tokenText.Value))
                            {
                                var linkWithText = new Hyperlink(context.CreateRun(tokenText.Value))
                                {
                                    NavigateUri = new Uri(context.NavigateUri!),
                                };
                                span.Inlines.Add(linkWithText);
                                handled = true;
                            }
                        }

                        if (!handled)
                        {
                            var linkWithText = new Hyperlink(context.CreateRun(context.NavigateUri!))
                            {
                                NavigateUri = new Uri(context.NavigateUri!)
                            };
                            span.Inlines.Add(linkWithText);
                        }
                    }
                    else
                    {
                        var run = context.CreateRun(token.Value);
                        span.Inlines.Add(run);
                    }
                }
                else if (token.TokenType == BbCodeTokeniser.TokenImage)
                {
                    ParseTag(token.Value, true, context);
                    if (context.Image != null)
                    {
                        var sp = new StackPanel();
                        sp.Children.Add(context.Image);
                        var tokenText = LA(1);
                        if (tokenText.TokenType == BbCodeTokeniser.TokenText)
                        {
                            Consume();
                            if (!string.IsNullOrEmpty(tokenText.Value))
                            {
                                sp.Children.Add(new TextBlock(context.CreateRun(tokenText.Value)) { TextAlignment = TextAlignment.Center });
                            }
                        }

                        sp.Orientation = Orientation.Vertical;
                        span.Inlines.Add(sp);
                    }
                    else
                    {
                        var run = context.CreateRun(token.Value);
                        span.Inlines.Add(run);
                    }
                }
                else if (token.TokenType == BbCodeTokeniser.TokenLineBreak)
                {
                    span.Inlines.Add(new LineBreak());
                }
                else if (token.TokenType == BbCodeTokeniser.TokenAttribute)
                {
                    throw new ParseException("Unexpected Token");
                }
                else if (token.TokenType == Tokeniser.TokenEnd)
                {
                    break;
                }
                else
                {
                    throw new ParseException("Unknown Token Type");
                }
            }
        }

        /// <summary>
        /// Parses the tag.
        /// </summary>
        /// <param name="tag">The tag.</param>
        /// <param name="start">if set to <c>true</c> [start].</param>
        /// <param name="context">The context.</param>
        private void ParseTag(string tag, bool start, ParseContext context)
        {
            switch (tag)
            {
                case TagBold:
                    context.FontWeight = null;
                    if (start)
                    {
                        context.FontWeight = FontWeights.Bold;
                    }

                    break;

                case TagColor:
                    if (start)
                    {
                        var token = LA(1);
                        if (token.TokenType == BbCodeTokeniser.TokenAttribute)
                        {
                            var convertFromString = ColorConverter.ConvertFromString(token.Value);
                            if (convertFromString != null)
                            {
                                var color = (Color)convertFromString;
                                context.Foreground = new SolidColorBrush(color);
                            }

                            Consume();
                        }
                    }
                    else
                    {
                        context.Foreground = Brushes.Transparent;
                    }

                    break;

                case TagFont:
                    if (start)
                    {
                        var token = LA(1);
                        if (token.TokenType == BbCodeTokeniser.TokenAttribute)
                        {
                            context.FontFamily = new FontFamily(token.Value);
                            Consume();
                        }
                    }
                    else
                    {
                        context.FontFamily = null!;
                    }

                    break;

                case TagItalic:
                    context.FontStyle = start ? FontStyles.Italic : null;

                    break;

                case TagStrikeThrough:
                    context.TextDecorations = start ? TextDecorations.Strikethrough : null!;
                    break;

                case TagSize:
                    if (start)
                    {
                        var token = LA(1);
                        if (token.TokenType == BbCodeTokeniser.TokenAttribute)
                        {
                            context.FontSize = Convert.ToDouble(token.Value);
                            Consume();
                        }
                    }
                    else
                    {
                        context.FontSize = null;
                    }

                    break;

                case TagUnderline:
                    context.TextDecorations = start ? TextDecorations.Underline : null!;
                    break;

                case TagUrl:
                    if (start)
                    {
                        var token = LA(1);
                        if (token.TokenType == BbCodeTokeniser.TokenAttribute)
                        {
                            context.NavigateUri = token.Value;
                            Consume();
                        }
                    }
                    else
                    {
                        context.NavigateUri = null!;
                    }

                    break;

                case TagImage:
                    if (start)
                    {
                        var token = LA(1);
                        if (token.TokenType == BbCodeTokeniser.TokenAttribute)
                        {
                            var attributes = token.Value.Split(',').ToList();
                            var image = new Image
                            {
                                Source = new BitmapImage(new Uri(attributes[0], UriKind.RelativeOrAbsolute))
                            };

                            var height = attributes.Find(x => x.Contains("height="));
                            if (height != null)
                            {
                                image.Height = double.Parse(height.Split('=')[1]);
                            }

                            var width = attributes.Find(x => x.Contains("width="));
                            if (width != null)
                            {
                                image.Width = double.Parse(width.Split('=')[1]);
                            }

                            context.Image = image;
                            Consume();
                        }
                    }
                    else
                    {
                        context.Image = null!;
                    }

                    break;
            }
        }

        /// <summary>
        /// Parse Context.
        /// </summary>
        private class ParseContext
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="ParseContext"/> class.
            /// </summary>
            /// <param name="parent">The parent.</param>
            public ParseContext(Span parent) => Parent = parent;

            /// <summary>
            /// Sets the font family.
            /// </summary>
            /// <value>
            /// The font family.
            /// </value>
            public FontFamily? FontFamily { private get; set; }

            /// <summary>
            /// Sets the size of the font.
            /// </summary>
            /// <value>The size of the font.</value>
            public double? FontSize { private get; set; }

            /// <summary>
            /// Sets the font style.
            /// </summary>
            /// <value>The font style.</value>
            public FontStyle? FontStyle { private get; set; }

            /// <summary>
            /// Sets the font weight.
            /// </summary>
            /// <value>The font weight.</value>
            public FontWeight? FontWeight { private get; set; }

            /// <summary>
            /// Sets the foreground.
            /// </summary>
            /// <value>The foreground.</value>
            public Brush? Foreground { private get; set; }

            /// <summary>
            /// Gets or sets the navigate URI.
            /// </summary>
            /// <value>The navigate URI.</value>
            public string? NavigateUri { get; set; }

            /// <summary>
            /// Sets the text decorations.
            /// </summary>
            /// <value>The text decorations.</value>
            public TextDecorationCollection? TextDecorations { private get; set; }

            /// <summary>
            /// Gets or sets the image.
            /// </summary>
            /// <value>
            /// The image.
            /// </value>
            public Image? Image { get; internal set; }

            /// <summary>
            /// Gets the parent.
            /// </summary>
            /// <value>The parent.</value>
            private Span Parent
            {
                [UsedImplicitly]
                get;
            }

            /// <summary>
            /// Creates a run reflecting the current context settings.
            /// </summary>
            /// <param name="text">The text.</param>
            /// <returns>A run.</returns>
            public Run CreateRun(string text)
            {
                var run = new Run() { Text = text };
                if (FontSize.HasValue)
                {
                    run.FontSize = FontSize.Value;
                }

                if (FontFamily != null)
                {
                    run.FontFamily = FontFamily;
                }

                if (FontWeight.HasValue)
                {
                    run.FontWeight = FontWeight.Value;
                }

                if (FontStyle.HasValue)
                {
                    run.FontStyle = FontStyle.Value;
                }

                if (Foreground != null)
                {
                    run.Foreground = Foreground;
                }

                run.TextDecorations = TextDecorations;

                return run;
            }
        }
    }
}
