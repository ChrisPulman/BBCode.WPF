// Copyright (c) Chris Pulman. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace CP.BBCode.WPF.Annotations
{
    /// <summary>
    /// Indicates that the marked symbol is used implicitly (e.g. via reflection, in external
    /// library), so this symbol will not be marked as unused (as well as by other usage inspections).
    /// </summary>
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = true)]
    internal sealed class UsedImplicitlyAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UsedImplicitlyAttribute"/> class.
        /// </summary>
        public UsedImplicitlyAttribute()
          : this(ImplicitUseKindFlags.Default, ImplicitUseTargetFlags.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UsedImplicitlyAttribute" /> class.
        /// </summary>
        /// <param name="useKindFlags">The use kind flags.</param>
        public UsedImplicitlyAttribute(ImplicitUseKindFlags useKindFlags)
          : this(useKindFlags, ImplicitUseTargetFlags.Default)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UsedImplicitlyAttribute" /> class.
        /// </summary>
        /// <param name="targetFlags">The target flags.</param>
        public UsedImplicitlyAttribute(ImplicitUseTargetFlags targetFlags)
          : this(ImplicitUseKindFlags.Default, targetFlags)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UsedImplicitlyAttribute" /> class.
        /// </summary>
        /// <param name="useKindFlags">The use kind flags.</param>
        /// <param name="targetFlags">The target flags.</param>
        public UsedImplicitlyAttribute(
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
        public ImplicitUseKindFlags UseKindFlags { get; }

        /// <summary>
        /// Gets the target flags.
        /// </summary>
        /// <value>
        /// The target flags.
        /// </value>
        public ImplicitUseTargetFlags TargetFlags { get; }
    }
}
