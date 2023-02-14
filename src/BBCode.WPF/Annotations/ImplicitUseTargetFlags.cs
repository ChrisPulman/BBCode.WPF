// Copyright (c) Chris Pulman. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace CP.BBCode.WPF.Annotations
{
    /// <summary>
    /// Specify what is considered used implicitly when marked with <see
    /// cref="MeansImplicitUseAttribute"/> or <see cref="UsedImplicitlyAttribute"/>.
    /// </summary>
    [Flags]
    internal enum ImplicitUseTargetFlags
    {
        /// <summary>
        /// The none.
        /// </summary>
        None = 0,

        /// <summary>
        /// The default.
        /// </summary>
        Default = Itself,

        /// <summary>
        /// The itself.
        /// </summary>
#pragma warning disable CA1069 // Enums values should not be duplicated
        Itself = 1,
#pragma warning restore CA1069 // Enums values should not be duplicated

        /// <summary>
        /// Members of entity marked with attribute are considered used.
        /// </summary>
        Members = 2,

        /// <summary>
        /// Entity marked with attribute and all its members considered used.
        /// </summary>
        WithMembers = Itself | Members,
    }
}
