// See LICENSE.txt in the project root for license information.

using System;
using System.Diagnostics.CodeAnalysis;

public static partial class My
{
    public const string NullString = null;

    public sealed class Mutable
    {
        public Mutable() { }

        public Mutable(string value) => Value = value;

        [DisallowNull]
        public string Value { get; set; } = String.Empty;

        public override string ToString() => Value;
    }
}
