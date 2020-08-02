// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#region License
// The MIT License (MIT)
//
// Copyright (c) .NET Foundation and Contributors
//
// All rights reserved.
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
#endregion

#nullable enable

#if NETSTANDARD1_2 || NETSTANDARD1_1 || NETSTANDARD1_0 || NET452 || NET451 || NET45 // FormattableString

namespace System
{
    /// <summary>
    /// A composite format string along with the arguments to be formatted. An instance of this
    /// type may result from the use of the C# or VB language primitive "interpolated string".
    /// </summary>
    internal abstract class FormattableString : IFormattable
    {
        /// <summary>
        /// The composite format string.
        /// </summary>
        public abstract string Format { get; }

        /// <summary>
        /// Returns an object array that contains zero or more objects to format. Clients should not
        /// mutate the contents of the array.
        /// </summary>
        public abstract object?[] GetArguments();

        /// <summary>
        /// The number of arguments to be formatted.
        /// </summary>
        public abstract int ArgumentCount { get; }

        /// <summary>
        /// Returns one argument to be formatted from argument position <paramref name="index"/>.
        /// </summary>
        public abstract object? GetArgument(int index);

        /// <summary>
        /// Format to a string using the given culture.
        /// </summary>
        public abstract string ToString(IFormatProvider? formatProvider);

        string IFormattable.ToString(string? ignored, IFormatProvider? formatProvider)
        {
            return ToString(formatProvider);
        }

        /// <summary>
        /// Format the given object in the invariant culture. This static method may be
        /// imported in C# by
        /// <code>
        /// using static System.FormattableString;
        /// </code>.
        /// Within the scope
        /// of that import directive an interpolated string may be formatted in the
        /// invariant culture by writing, for example,
        /// <code>
        /// Invariant($"{{ lat = {latitude}; lon = {longitude} }}")
        /// </code>
        /// </summary>
        public static string Invariant(FormattableString formattable)
        {
            if (formattable == null)
            {
                throw new ArgumentNullException(nameof(formattable));
            }

            return formattable.ToString(Globalization.CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Format the given object in the current culture. This static method may be
        /// imported in C# by
        /// <code>
        /// using static System.FormattableString;
        /// </code>.
        /// Within the scope
        /// of that import directive an interpolated string may be formatted in the
        /// current culture by writing, for example,
        /// <code>
        /// CurrentCulture($"{{ lat = {latitude}; lon = {longitude} }}")
        /// </code>
        /// </summary>
        public static string CurrentCulture(FormattableString formattable)
        {
            if (formattable == null)
            {
                throw new ArgumentNullException(nameof(formattable));
            }

            return formattable.ToString(Globalization.CultureInfo.CurrentCulture);
        }

        public override string ToString()
        {
            return ToString(Globalization.CultureInfo.CurrentCulture);
        }
    }
}

namespace System.Runtime.CompilerServices
{
    /// <summary>
    /// A factory type used by compilers to create instances of the type <see cref="FormattableString"/>.
    /// </summary>
    internal static class FormattableStringFactory
    {
        /// <summary>
        /// Create a <see cref="FormattableString"/> from a composite format string and object
        /// array containing zero or more objects to format.
        /// </summary>
        public static FormattableString Create(string format, params object?[] arguments)
        {
            if (format == null)
            {
                throw new ArgumentNullException(nameof(format));
            }

            if (arguments == null)
            {
                throw new ArgumentNullException(nameof(arguments));
            }

            return new ConcreteFormattableString(format, arguments);
        }

        private sealed class ConcreteFormattableString : FormattableString
        {
            private readonly string _format;
            private readonly object?[] _arguments;

            internal ConcreteFormattableString(string format, object?[] arguments)
            {
                _format = format;
                _arguments = arguments;
            }

            public override string Format => _format;
            public override object?[] GetArguments() { return _arguments; }
            public override int ArgumentCount => _arguments.Length;
            public override object? GetArgument(int index) { return _arguments[index]; }
            public override string ToString(IFormatProvider? formatProvider) { return string.Format(formatProvider, _format, _arguments); }
        }
    }
}

#endif
