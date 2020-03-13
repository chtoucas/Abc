# Abécédaire

- [Quick start with `Maybe<T>`](#quick-start-with-maybet)
- [Guidelines](#guidelines)
- [Developer notes](#developer-notes)

Features:
- An option type for C#.
- Utilities to write code in the ROP style (Railway Oriented Programming).

Quick start with `Maybe<T>`
---------------------------

Summary:
- [Construct a _maybe_](#construct-a-maybe)
- [Check whether a _maybe_ is empty or not](#check-whether-a-maybe-is-empty-or-not)
- [Map and filter the enclosed value (if any)](#map-and-filter-the-enclosed-value-if-any)
- [Safely extract the enclosed value](#safely-extract-the-enclosed-value)
- [Pattern matching: extract and map the enclosed value (if any)](#pattern-matching-extract-and-map-the-enclosed-value-if-any)
- [Side effects: do something with the enclosed value (if any)](#side-effects-do-something-with-the-enclosed-value-if-any)
- [Binding](#binding)
- [Query Expression Pattern](#query-expression-pattern)
- [More Features](#more-features)
- [Samples](#samples)

An option type or maybe type (a better fit for what we use it for), is like
a box containing a value or no value at all.

It can help preventing null reference exceptions, but that's not the point, it
really forces us to think about the outcome of a computation. Hopefully, we
shall see a few compelling use cases for a maybe type.

An option type is a very simple sum type `Maybe<T> = Some<T> | None` (exclusive
or, but of course this is not possible in C#), whereas a nullable value type,
like `int?`, is just an integer type to which the language adds a special
(sentinel) value, the `null` value. What's the difference? You can't ignore the
fact that an option is either something or nothing, you are forced to handle
both cases.

`Maybe<T>` also differs from a nullable in that it applies to both value types
and reference types; NRTs do not count since they are not actual .NET types but
annotations that the compiler can take advantage of. There are many other small
differences. For instance, one can nest _maybe_'s (`Maybe<Maybe<T>>`) whereas
one can't create an `int??`; `Nullable<Nullable<T>>` is not valid in C#.

### Construct a _maybe_

```csharp
// Maybe of a value type.
Maybe<int> q = Maybe.Some(1);
Maybe<int> q = Maybe.None<int>();                   // The empty maybe of type int.

// Maybe of a nullable value type.
Maybe<int> q = Maybe.SomeOrNone((int?)1);
Maybe<int> q = Maybe.SomeOrNone((int?)null);        // The empty maybe of type int.

// Maybe of a reference type.
Maybe<string> q = Maybe.SomeOrNone("value");
Maybe<string> q = Maybe.SomeOrNone((string?)null);  // The empty maybe of type string.
```
If NRTs (Nullable Reference Type) are available, `Maybe.SomeOrNone()` with a
nullable string does not create a `Maybe<string?>` but it (correctly) returns
a `Maybe<string>`. When working with unconstrained generic type, you cannot use
`Maybe.Some[OrNone]()` or `Maybe.None<T>()`. Hopefully, `Maybe.Of()` and
`Maybe<T>.None` come to the rescue, they were specifically created to handle
this kind of situation; otherwise there is no reason to use `Maybe.Of()` ---
for `Maybe<T>.None` it is more a matter of taste.

### Check whether a _maybe_ is empty or not

```csharp
var some = Maybe.SomeOrNone("value");
var none = Maybe.None<string>()

bool isNone = some.IsNone             // == false
bool isNone = none.IsNone             // == true

// We can also check whether it contains a specific value or not.
bool b = some.Contains("value")       // == true
bool b = some.Contains("other")       // == false

// Of course, Contains() always returns false if the maybe is empty.
bool b = none.Contains("value")       // == false
bool b = none.Contains("other")       // == false
```

### Map and filter the enclosed value (if any)

```csharp
var some = Maybe.Some(4);

// NB: next line is better written with Select(Math.Sqrt).
Maybe<int> q = some.Where(x >= 0).Select(x => Math.Sqrt(x));    // == Maybe(2)

// Or using the query expression syntax.
Maybe<int> q = from x in some where x >= 0 select Math.Sqrt(x); // == Maybe(2)
```
If, instead, we start with an empty _maybe_ or `Maybe.Some(-4)`, the result is
an empty _maybe_, in both cases.

### Safely extract the enclosed value

`Maybe<T>` is a strict option type, we don't get direct access to the enclosed
value.
```csharp
Maybe<string> maybe = Maybe.SomeOrNone("...");

// First the truely unsafe way of doing things, not recommended but at least
// it is clear from the method's name that the result might be null.
// WARNING: value may be null here!
string value = maybe.ValueOrDefault();

// When the maybe is empty returns the specified replacement value.
string value = maybe.ValueOrElse("other");
// We may delay the creation of the replacement value. Here the example is a bit
// contrive, we should imagine a situation where the creation of the replacement
// value is an expensive operation, for instance when retrieved from a remote
// source.
string value = maybe.ValueOrElse(() => "other");

// If maybe is empty, throw InvalidOperationException.
string value = maybe.ValueOrThrow();
// Throw a custom exception.
string value = maybe.ValueOrThrow(new NotSupportedException("..."));
```
A word of **caution**, the methods may only be considered safe when targetting
.NET Core 3.0 or above, that is your project file should include something like
this:
```xml
<TargetFrameworks>netcoreapp3.0;netstandard2.0</TargetFrameworks>
<Nullable>enable</Nullable>
```
C# 8.0 will then complain if one tries to write `maybe.ValueOrElse(null)`.

### Pattern matching: extract and map the enclosed value (if any)

```csharp
Maybe<int> q = from x in maybe where x >= 0 select Math.Sqrt(x);

string message = q.Switch(
    caseSome: x  => $"Square root = {x}."),
    caseNone: () => "The input was strictly negative.");

// But, since caseNone is constant, it is better written:
string message = q.Switch(
    caseSome: x => $"Square root = {x}."),
    caseNone: "The input was strictly negative.");
```

### Side effects: do something with the enclosed value (if any)

```csharp
Maybe<int> q = from x in maybe where x >= 0 select Math.Sqrt(x);

q.Do(
    onSome: x  => Console.WriteLine($"Square root = {x}."),
    onNone: () => Console.WriteLine("The input was strictly negative."));

q.OnSome(x => Console.WriteLine($"Square root = {x}.");

if (q.IsNone) { Console.WriteLine("The input was strictly negative."); }
```

### Binding

`Bind()` and `Select()` look very similar, but `Bind()` is for situations where
the "selector" maps a value to a _maybe_ not to another value; the selector is
then said to be a binder.
Let's say that we wish to map a _maybe_ using the method `May.ParseInt32` which
parses a string and returns a `Maybe<int>`, it's not a mapping operation but
rather a binding operation.
```csharp
Maybe<string> maybe = Maybe.Some("12345");

// DO write.
var q = maybe.Bind(May.ParseInt32);     // <-- Maybe<int>

// DO NOT write.
var q = maybe.Select(May.ParseInt32)    // <-- Maybe<Maybe<int>>
    // To get back a Maybe<int>, we MUST then flatten the "double" maybe.
    .Flatten();                         // <-- Maybe<int>
```

### Query Expression Pattern

We already saw `Select` and `Where`, but there is more.
```csharp
var q = from i in maybe1
        from j in maybe2
        select i + j

var q = from i in maybe
        from j in Maybe.Some(2 * i)
        select i + j

// Pythagorean triple? q is a Maybe<(int, int, int)>.
var q = from x in maybe1
        from y in maybe2
        from z in maybe3
        where x ^ 2 + y ^ 2 == z ^ 2
        select (x, y, z)
```

### More Features

See the XML comments for samples.
- LINQ and collection extensions; see `Abc.Linq` and `Abc.Extensions`.
- Parsing helpers; see `May`.
- XML & SQL data type helpers; see `Abc.Extensions`.

### Samples

#### Processing multiple nullable objects together
`MayGetSingle()` is an extension that returns something only if the key exists and
there is a unique value associated to it.
```csharp
NameValueCollection nvc;

var q = from a in nvc.MayGetSingle("a")
        from b in nvc.MayGetSingle("b")
        from c in nvc.MayGetSingle("c").Bind(May.ParseInt32)
        let x = nvc.MayGetSingle("x")
        where c > 10
        select new {
            A = a,
            B = b,
            X = x.ValueOrElse("Y")
        }
```
In the above query, the result is empty when one of the keys `a`, `b` or `c`
does not exist or is multi-valued, or the single value of `c` is not the string
representation of an integer > 10. Morevover, the result is NOT empty even if
the key `x` does not exist or is multi-valued, in which case we pick a default
value.

Guidelines
----------

Your mantra should be "**_maybe do not abuse the maybe_**".

### Usage

The `Maybe<T>` type is a value type. Even if it is a natural choice, it worried
me and I hesitated for a while. The addition of value tuples to .NET convinced
me that the benefits will outweight the drawbacks.
- **CONSIDER using this type when `T` is a value type or an _immutable_ reference
  type.**
- **AVOID using this type when `T` is a _mutable_ reference type.**
- **DO NOT use a _maybe_ with nullable types, eg `Maybe<int?>`.**

The intended usage is when `T` is a value type, a string, a (read-only?) record,
or a function. For other reference types, it should be fine as long as T is an
immutable reference type.

One can _indirectly_ create a maybe for a nullable type, but all static
factory methods do not permit it. If you end up having to manipulate for say a
`Maybe<int?>`, there is a method `Squash()` to convert it to a `Maybe<int>`.

NB: `Result<T>` (not yet sure I will keep it) is a reference type and may serve
as a replacement for `Maybe<T>`.

### General recommendations

First and foremost,
- **DO apply all guidelines for `ValueTuple<>`** and, if they contradict what I say
  here, follow your own wisdom.

- **DO NOT use `Maybe<T>` as a parameter in public APIs.**

In general, I would even not recommend to use it in a general purpose library.
Of course, this does not mean that you should not use this type at all, otherwise
I would not have written this library.

- **AVOID using a _maybe_ if the object is expected to be long-lived.**

- **AVOID using a _maybe_ in a performance-critical path.**

### May-Parse pattern

- **DO use this pattern instead of the Try-Parse pattern for reference types.**
- **DO use the prefix _May_ for methods implementing this pattern.**

Developer Notes
---------------

### Guidelines

- Methods that return something should have the attribure `Pure`. It is not
  mandatory but it clearly states that the result should not be ignored.
  It might be superfluous these days, but "ça ne mange pas de pain".
- Add nullable annotations whenever necessary.
- Seal classes unless they are designed with extensibility in mind.

### TODOs

- Target .NET Standard 2.1 too to be able to distribute a version with nullable
  annotations?
- Tests & code coverage.
- XML comments with complete examples.
- NuGet package.
- Perf tool.
