# Abécédaire

Features
--------

- A strict Option type.
- Utilities to write code in the ROP style (Railway Oriented Programming).

Quick start with `Maybe<T>`
---------------------------

An Option type can help preventing null reference exceptions, but that's not the
point, it really forces us to think about the outcome of a computation. We shall
see a few real-world use cases below (web, UI, logging, to be written...).

Constructing a _maybe_.
```csharp
// Maybe of a value type.
Maybe<int> q = Maybe.Some(1);
Maybe<int> q = Maybe<int>.None; // The empty maybe.

// Maybe of a nullable value type.
Maybe<int> q = Maybe.SomeOrNone((int?)1);
Maybe<int> q = Maybe.SomeOrNone((int?)null);        // == Maybe<int>.None

// Maybe of a reference type (use NRT if available).
Maybe<string> q = Maybe.SomeOrNone("value");
Maybe<string> q = Maybe.SomeOrNone((string?)null);  // == Maybe<string>.None
```

Map and filter the enclosed value if any.
```csharp
var some = Maybe.Some(4);

Maybe<int> q = some.Where(x >= 0).Select(x => Math.Sqrt(x));
// Or using the Query Expression Pattern.
Maybe<int> q = from x in some where x >= 0 select Math.Sqrt(x);
```
In both cases, the result is a _maybe_ of 2.
If instead we start with an empty _maybe_ or `Maybe.Some(-4)`, the result is
an empty _maybe_ in both cases.

Safely extract the enclosed value. `Maybe<T>` is a strict option type, we don't
get a direct access to the enclosed value.
A word of caution, the methods may only be considered safe when targetting
.NET Core 3.0 or above. For instance, C# 8.0 will complain if one tries to
write `maybe.ValueOrElse(null)`.
```csharp
Maybe<string> maybe = Maybe.SomeOrNone("...");

// First the truely unsafe way of doing things, not recommended but at least
// it is clear from the method's name that the result might be null.
// WARNING: value may be null here!
string value = maybe.ValueOrDefault();

string value = maybe.ValueOrElse("other");
// We may delay the creation of the replacement value. Here the example is a bit
// contrive, we should imagine a situation where the replacement value is
// retrieved from an external source.
string value = maybe.ValueOrElse(() => "other");

// If maybe is empty, throw InvalidOperationException.
string value = maybe.ValueOrThrow();
// Throw a custom exception.
string value = maybe.ValueOrThrow(new NotSupportedException("..."));
```

Pattern matching. Extract and map the enclosed value if any.
```csharp
Maybe<int> q = from x in maybe where x >= 0 select Math.Sqrt(x);

string message = q.Switch(
    caseSome: x  => $"Square root = {x}."),
    caseNone: () => "The input was strictly negative.");

string message = q.Switch(
    caseSome: x => $"Square root = {x}."),
    caseNone: "The input was strictly negative.");
```

Side-effects. Do something with the enclosed value if any.
```csharp
Maybe<int> q = from x in maybe where x >= 0 select Math.Sqrt(x);

q.Do(
    onSome: x  => Console.WriteLine($"Square root = {x}."),
    onNone: () => Console.WriteLine("The input was strictly negative."));

q.OnSome(x => Console.WriteLine($"Square root = {x}.");

if (q.IsNone) { Console.WriteLine("The input was strictly negative."); }
```

`Bind()` vs `Select()`.
Let's say that we wish to map a _maybe_ using the method `May.ParseInt32` which
parses a string and returns a `Maybe<int>`.
```csharp
Maybe<string> maybe = Maybe.Some("12345");

// DO write.
var q = maybe.Bind(May.ParseInt32);     // <-- Maybe<int>

// DO NOT write.
var q = maybe.Select(May.ParseInt32)    // <-- Maybe<Maybe<int>>
    // We MUST then flatten the "double" maybe.
    .Flatten();                         // <-- Maybe<int>
```

Join.
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

And much more, see the XML comments for samples:
- LINQ and collection extensions; see `Abc.Linq` and `Abc.Extensions`.
- Parsing helpers; see `May`.
- XML & SQL helpers; see `Abc.Extensions`.

### Guidelines

Usage:
- CONSIDER using this type when `T` is a value type or an immutable reference type.
- AVOID using this type when `T` is a mutable reference type.
- DO NOT create a _maybe_ value for nullable types, eg `Maybe<int?>`.

General recommendations:
- DO NOT use `Maybe<T>` in a public APIs.
- DO use _maybe_'s when processing multiple nullable values.
- CONSIDER using _maybe_'s to validate and transform data you don't control.

May-Parse pattern:
- DO use this pattern instead of the Try-Parse pattern for reference types.
- DO use the prefix _May_ for methods implementing this pattern.

Developer Notes
---------------

- Methods that return something should have the attribure `Pure`. It is not
  mandatory but it clearly states that the result should not be ignored.
- Use nullable attributes whenever necessary.

### TODOs

- .NET Core 3.0 instead of 3.1.
- Tests & code coverage.
- XML comments with complete examples.
- NuGet package.
- Perf tool.
