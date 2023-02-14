// Copyright (c) Chris Pulman. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace CP.BBCode.WPF.Annotations
{
    /// <summary>
    /// Should be used on attributes and causes ReSharper to not mark symbols marked with such
    /// attributes as unused (as well as by other usage inspections).
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    internal sealed class MeansImplicitUseAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MeansImplicitUseAttribute"/> class.
        /// </summary>
        public MeansImplicitUseAttribute()
          : this(ImplicitUseKindFlags.Default, ImplicitUseTargetFlags.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MeansImplicitUseAttribute" /> class.
        /// </summary>
        /// <param name="useKindFlags">The use kind flags.</param>
        public MeansImplicitUseAttribute(ImplicitUseKindFlags useKindFlags)
          : this(useKindFlags, ImplicitUseTargetFlags.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MeansImplicitUseAttribute" /> class.
        /// </summary>
        /// <param name="targetFlags">The target flags.</param>
        public MeansImplicitUseAttribute(ImplicitUseTargetFlags targetFlags)
          : this(ImplicitUseKindFlags.Default, targetFlags)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MeansImplicitUseAttribute" /> class.
        /// </summary>
        /// <param name="useKindFlags">The use kind flags.</param>
        /// <param name="targetFlags">The target flags.</param>
        public MeansImplicitUseAttribute(
          ImplicitUseKindFlags useKindFlags, ImplicitUseTargetFlags targetFlags)
        {
            UseKindFlags = useKindFlags;
            TargetFlags = targetFlags;
        }

        /// <summary>
        /// Gets the use kind flags.
        /// </summary>
        /// <value>
        /// The use kind flags.
        /// </value>
        [UsedImplicitly]
        public ImplicitUseKindFlags UseKindFlags { get; }

        /// <summary>
        /// Gets the target flags.
        /// </summary>
        /// <value>
        /// The target flags.
        /// </value>
        [UsedImplicitly]
        public ImplicitUseTargetFlags TargetFlags { get; }
    }
}
