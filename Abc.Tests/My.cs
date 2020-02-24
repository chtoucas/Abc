// See LICENSE.txt in the project root for license information.

using System;

internal static class Returns<TSource>
{
    public static readonly Func<TSource, bool> False = _ => false;

    public static readonly Func<TSource, bool> True = _ => true;
}

internal static class My
{
    public const string NullString = null;
    public const string? NullNullString = null;
}
