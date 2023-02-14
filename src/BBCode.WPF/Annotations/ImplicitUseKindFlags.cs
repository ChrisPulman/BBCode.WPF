// Copyright (c) Chris Pulman. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace CP.BBCode.WPF.Annotations
{
    /// <summary>
    /// Implicit Use Kind Flags.
    /// </summary>
    [Flags]
    internal enum ImplicitUseKindFlags
    {
        /// <summary>
        /// The none.
        /// </summary>
        None = 0,

        /// <summary>
        /// The default.
        /// </summary>
        Default = Access | Assign | InstantiatedWithFixedConstructorSignature,

        /// <summary>
        /// Only entity marked with attribute considered used.
        /// </summary>
        Access = 1,

        /// <summary>
        /// Indicates implicit assignment to a member.
        /// </summary>
        Assign = 2,

        /// <summary>
        /// Indicates implicit instantiation of a type with fixed constructor signature. That means
        /// any unused constructor parameters won't be reported as such.
        /// </summary>
        InstantiatedWithFixedConstructorSignature = 4,

        /// <summary>
        /// Indicates implicit instantiation of a type.
        /// </summary>
        InstantiatedNoFixedConstructorSignature = 8,
    }
}
