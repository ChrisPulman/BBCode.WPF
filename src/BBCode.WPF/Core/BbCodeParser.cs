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
        private const string TagEmail = "email";
        private const string TagQuote = "quote";
        private const string TagCode = "code";
        private const string TagList = "list";
        private const string TagListItem = "*"; // [*]
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
                    // Handle list item before generic tag parsing.
                    if (token.Value == TagListItem && context.ListMode)
                    {
                        // New bullet: line break if not first.
                        if (context.ListItemCount > 0)
                        {
                            span.Inlines.Add(new LineBreak());
                        }

                        context.ListItemCount++;

                        // Bullet symbol.
                        span.Inlines.Add(context.CreateRun("• "));
                        continue;
                    }

                    ParseTag(token.Value, true, context, span);
                }
                else if (token.TokenType == BbCodeTokeniser.TokenEndTag)
                {
                    ParseTag(token.Value, false, context, span);
                }
                else if (token.TokenType == BbCodeTokeniser.TokenText)
                {
                    var run = context.CreateRun(token.Value);
                    span.Inlines.Add(run);
                }
                else if (token.TokenType == BbCodeTokeniser.TokenLink)
                {
                    var linkTagName = token.Value; // url or email
                    ParseTag(token.Value, true, context, span);

                    // If attribute present context.NavigateUri is already set.
                    // If not, attempt to use next text token as both address and display text when followed by closing tag.
                    if (context.NavigateUri == null)
                    {
                        var possibleText = LA(1);
                        var possibleEnd = LA(2);
                        if (possibleText.TokenType == BbCodeTokeniser.TokenText &&
                            possibleEnd.TokenType == BbCodeTokeniser.TokenEndTag && possibleEnd.Value == linkTagName)
                        {
                            Consume(); // consume text
                            var raw = possibleText.Value;
                            string target;
                            if (linkTagName == TagEmail)
                            {
                                target = "mailto:" + raw;
                            }
                            else
                            {
                                target = raw;
                            }

                            var hyperlink = new Hyperlink(context.CreateRun(raw)) { NavigateUri = new Uri(target, UriKind.RelativeOrAbsolute) };
                            span.Inlines.Add(hyperlink);

                            // Leave end tag to normal processing which will reset NavigateUri anyway.
                            continue;
                        }
                    }

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
                                    NavigateUri = new Uri(context.NavigateUri!, UriKind.RelativeOrAbsolute),
                                };
                                span.Inlines.Add(linkWithText);
                                handled = true;
                            }
                        }

                        if (!handled)
                        {
                            var linkWithText = new Hyperlink(context.CreateRun(context.NavigateUri!))
                            {
                                NavigateUri = new Uri(context.NavigateUri!, UriKind.RelativeOrAbsolute)
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
                    ParseTag(token.Value, true, context, span);
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
        /// <param name="root">Root span for inline insertions (used for non-style tags like list).</param>
        private void ParseTag(string tag, bool start, ParseContext context, Span root)
        {
            switch (tag)
            {
                case TagBold:
                    context.FontWeight = start ? FontWeights.Bold : null;
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
                        context.Foreground = null; // reset
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
                        context.FontFamily = null;
                    }

                    break;

                case TagItalic:
                    context.FontStyle = start ? FontStyles.Italic : null;
                    break;

                case TagStrikeThrough:
                    context.TextDecorations = start ? TextDecorations.Strikethrough : null;
                    break;

                case TagSize:
                    if (start)
                    {
                        var token = LA(1);
                        if (token.TokenType == BbCodeTokeniser.TokenAttribute)
                        {
                            if (double.TryParse(token.Value, out var size))
                            {
                                context.FontSize = size;
                            }

                            Consume();
                        }
                    }
                    else
                    {
                        context.FontSize = null;
                    }

                    break;

                case TagUnderline:
                    context.TextDecorations = start ? TextDecorations.Underline : null;
                    break;

                case TagUrl:
                case TagEmail:
                    if (start)
                    {
                        var token = LA(1);
                        if (token.TokenType == BbCodeTokeniser.TokenAttribute)
                        {
                            var addr = token.Value;
                            if (tag == TagEmail && !addr.StartsWith("mailto:", StringComparison.OrdinalIgnoreCase))
                            {
                                addr = "mailto:" + addr;
                            }

                            context.NavigateUri = addr;
                            Consume();
                        }
                        else
                        {
                            context.NavigateUri = null; // will attempt auto-detect
                        }
                    }
                    else
                    {
                        context.NavigateUri = null;
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
                                if (double.TryParse(height.Split('=')[1], out var h))
                                {
                                    image.Height = h;
                                }
                            }

                            var width = attributes.Find(x => x.Contains("width="));
                            if (width != null)
                            {
                                if (double.TryParse(width.Split('=')[1], out var w))
                                {
                                    image.Width = w;
                                }
                            }

                            context.Image = image;
                            Consume();
                        }
                    }
                    else
                    {
                        context.Image = null;
                    }

                    break;

                case TagQuote:
                    if (start)
                    {
                        var token = LA(1);
                        if (token.TokenType == BbCodeTokeniser.TokenAttribute)
                        {
                            // Prepend author line.
                            Consume();
                            root.Inlines.Add(new LineBreak());
                            root.Inlines.Add(new Run(token.Value + " wrote:") { FontWeight = FontWeights.Bold });
                            root.Inlines.Add(new LineBreak());
                        }

                        context.FontStyle = FontStyles.Italic;
                        context.Foreground = new SolidColorBrush(Colors.Gray);
                    }
                    else
                    {
                        context.FontStyle = null;
                        context.Foreground = null;
                        root.Inlines.Add(new LineBreak());
                    }

                    break;

                case TagCode:
                    if (start)
                    {
                        context.FontFamily = new FontFamily("Consolas");
                        context.Background = new SolidColorBrush(Color.FromRgb(240, 240, 240));
                    }
                    else
                    {
                        context.FontFamily = null;
                        context.Background = null;
                        root.Inlines.Add(new LineBreak());
                    }

                    break;

                case TagList:
                    if (start)
                    {
                        context.ListMode = true;
                        context.ListItemCount = 0;
                        root.Inlines.Add(new LineBreak());
                    }
                    else
                    {
                        context.ListMode = false;
                        root.Inlines.Add(new LineBreak());
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

            public bool ListMode { get; set; }

            public int ListItemCount { get; set; }

            /// <summary>
            /// Gets or sets the font family.
            /// </summary>
            public FontFamily? FontFamily { get; set; }

            /// <summary>
            /// Gets or sets the size of the font.
            /// </summary>
            public double? FontSize { get; set; }

            /// <summary>
            /// Gets or sets the font style.
            /// </summary>
            public FontStyle? FontStyle { get; set; }

            /// <summary>
            /// Gets or sets the font weight.
            /// </summary>
            public FontWeight? FontWeight { get; set; }

            /// <summary>
            /// Gets or sets the foreground brush.
            /// </summary>
            public Brush? Foreground { get; set; }

            /// <summary>
            /// Gets or sets the navigate URI.
            /// </summary>
            public string? NavigateUri { get; set; }

            /// <summary>
            /// Gets or sets the text decorations.
            /// </summary>
            public TextDecorationCollection? TextDecorations { get; set; }

            /// <summary>
            /// Gets or sets the image.
            /// </summary>
            public Image? Image { get; internal set; }

            /// <summary>
            /// Gets or sets the background brush (e.g. for code blocks).
            /// </summary>
            public Brush? Background { get; set; }

            /// <summary>
            /// Gets the parent.
            /// </summary>
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

                if (Background != null)
                {
                    run.Background = Background;
                }

                run.TextDecorations = TextDecorations;

                return run;
            }
        }
    }
}
