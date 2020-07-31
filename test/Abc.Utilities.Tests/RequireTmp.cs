// SPDX-License-Identifier: BSD-3-Clause
// Copyright (c) 2019 Narvalo.Org. All rights reserved.

namespace Abc
{
    using System;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Diagnostics.Contracts;

    using Abc.Utilities;

    // TODO: check with net452 without NRT's.
    internal partial class RequireTmp
    {
        public static string Test_Require1(object value)
        {
            Require(value, nameof(value));
            string? r = value.ToString();
            return r!;
        }
        public static string Test_Require2(object? value)
        {
            Require(value, nameof(value));
            // TODO: damnit?
            string? r = value!.ToString();
            return r!;
        }

        public static string Test_Require_ALT1(object value)
        {
            Require_ALT(value, nameof(value));
            string? r = value.ToString();
            return r!;
        }
        public static string Test_Require_ALT2(object? value)
        {
            Require_ALT(value, nameof(value));
            // TODO: damnit?
            string? r = value!.ToString();
            return r!;
        }

        public static string Test_Guard1(object value)
        {
            object x = Guard(value, nameof(value));
            string? r = x.ToString();
            return r!;
        }
        public static string Test_Guard2(object? value)
        {
            object x = Guard(value, nameof(value));
            string? r = x.ToString();
            return r!;
        }

        public static string Test_Guard_ALT1(object value)
        {
            object x = Guard_ALT(value, nameof(value));
            string? r = x.ToString();
            return r!;
        }
        public static string Test_Guard_ALT2(object? value)
        {
            object? x = Guard_ALT(value, nameof(value));
            string? r = x.ToString();
            return r!;
        }

        // TODO: ValidatedNotNull?
        [DebuggerStepThrough]
        public static void Require<T>([ValidatedNotNull] T? value, string paramName)
            where T : class
        {
            if (value is null)
            {
                throw new ArgumentNullException(paramName);
            }
        }
        [DebuggerStepThrough]
        public static void Require_ALT([ValidatedNotNull] object? value, string paramName)
        {
            if (value is null)
            {
                throw new ArgumentNullException(paramName);
            }
        }

        //[Pure]
        [DebuggerStepThrough]
        public static T Guard<T>(T? value, string paramName)
            where T : class
            => value ?? throw new ArgumentNullException(paramName);

        // the other one is better.
        //[Pure]
        [DebuggerStepThrough]
        [return: NotNull]
        public static T Guard_ALT<T>(T value, string paramName)
            where T : class?
            => value ?? throw new ArgumentNullException(paramName);
    }
}
