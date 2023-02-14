// Copyright (c) Chris Pulman. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Navigation;
using CP.BBCode.WPF.Core;

namespace CP.BBCode.WPF
{
    /// <summary>
    /// A lightweight control for displaying small amounts of rich formatted BBCode content.
    /// </summary>
    [ContentProperty("BBCode")]
    public class BBCodeBlock
        : TextBlock, ICommandSource
    {
        /// <summary>
        /// Identifies the BBCode dependency property.
        /// </summary>
        public static readonly DependencyProperty BBCodeProperty = DependencyProperty.Register("BBCode", typeof(string), typeof(BBCodeBlock), new PropertyMetadata(OnBbCodeChanged));

        /// <summary>
        /// The hyperlink command property.
        /// </summary>
        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register("Command", typeof(ICommand), typeof(BBCodeBlock));

        /// <summary>
        /// The command target property.
        /// </summary>
        public static readonly DependencyProperty CommandTargetProperty = DependencyProperty.Register("CommandTarget", typeof(IInputElement), typeof(BBCodeBlock));

        /// <summary>
        /// The command parameter property.
        /// </summary>
        public static readonly DependencyProperty CommandParameterProperty = DependencyProperty.Register("CommandParameter", typeof(object), typeof(BBCodeBlock));

        private bool _dirty;

        /// <summary>
        /// Initializes a new instance of the <see cref="BBCodeBlock"/> class.
        /// </summary>
        public BBCodeBlock()
        {
            // ensures the implicit BBCodeBlock style is used
            DefaultStyleKey = typeof(BBCodeBlock);

            AddHandler(FrameworkContentElement.LoadedEvent, new RoutedEventHandler(OnLoaded));
            AddHandler(Hyperlink.RequestNavigateEvent, new RequestNavigateEventHandler(OnRequestNavigate));
        }

        /// <summary>
        /// Gets or sets the BB code.
        /// </summary>
        /// <value>The BB code.</value>
        public string BBCode
        {
            get => (string)GetValue(BBCodeProperty);

            set => SetValue(BBCodeProperty, value);
        }

        /// <summary>
        /// Gets or sets the hyperlink command.
        /// </summary>
        /// <value>
        /// The hyperlink command.
        /// </value>
        [Bindable(true)]
        [Category("Action")]
        [Localizability(LocalizationCategory.NeverLocalize)]
        public ICommand Command
        {
            get => (ICommand)GetValue(CommandProperty);

            set => SetValue(CommandProperty, value);
        }

        /// <summary>
        /// Gets or sets the command parameter.
        /// </summary>
        /// <value>
        /// The command parameter.
        /// </value>
        [Bindable(true)]
        [Category("Action")]
        [Localizability(LocalizationCategory.NeverLocalize)]
        public object CommandParameter
        {
            get => GetValue(CommandParameterProperty);

            set => SetValue(CommandParameterProperty, value);
        }

        /// <summary>
        /// Gets or sets the object that the command is being executed on.
        /// </summary>
        [Bindable(true)]
        [Category("Action")]
        public IInputElement CommandTarget
        {
            get => (IInputElement)GetValue(CommandTargetProperty);

            set => SetValue(CommandTargetProperty, value);
        }

        /// <summary>
        /// Called when [bb code changed].
        /// </summary>
        /// <param name="o">The o.</param>
        /// <param name="e">
        /// The <see cref="DependencyPropertyChangedEventArgs"/> instance containing the event data.
        /// </param>
        private static void OnBbCodeChanged(DependencyObject o, DependencyPropertyChangedEventArgs e) => ((BBCodeBlock)o).UpdateDirty();

        /// <summary>
        /// Called when [loaded].
        /// </summary>
        /// <param name="o">The o.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void OnLoaded(object o, EventArgs e) => Update();

        /// <summary>
        /// Called when [request navigate].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">
        /// The <see cref="RequestNavigateEventArgs"/> instance containing the event data.
        /// </param>
        private void OnRequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            var p = e.Uri.OriginalString;
            if (p.StartsWith("cmd:"))
            {
                CommandParameter = p.Split(':')[1];
                if (Command?.CanExecute(CommandParameter) == true)
                {
                    Command.Execute(CommandParameter);
                }
            }
            else
            {
                try
                {
                    var sInfo = new System.Diagnostics.ProcessStartInfo(p)
                    {
                        UseShellExecute = true,
                    };
                    System.Diagnostics.Process.Start(sInfo);
                }
                catch
                {
                }
            }
        }

        /// <summary>
        /// Updates this instance.
        /// </summary>
        private void Update()
        {
            if (!IsLoaded || !_dirty)
            {
                return;
            }

            var bbcode = BBCode;

            Inlines.Clear();

            if (!string.IsNullOrWhiteSpace(bbcode))
            {
                Inline inline;
                try
                {
                    var parser = new BbCodeParser(bbcode, this);

                    inline = parser.Parse();
                }
                catch (Exception)
                {
                    // parsing failed, display BBCode value as-is
                    inline = new Run { Text = bbcode };
                }

                Inlines.Add(inline);
            }

            _dirty = false;
        }

        /// <summary>
        /// Updates the dirty.
        /// </summary>
        private void UpdateDirty()
        {
            _dirty = true;
            Update();
        }
    }
}
