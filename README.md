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

#### Construct a _maybe_
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

#### Check whether a _maybe_ is empty or not
```csharp
var some = Maybe.SomeOrNone("value");
var none = Maybe.None<string>()

bool isNone = some.IsNone             // == false
bool isNone = none.IsNone             // == true

// We can also check whether it contains a specific value or not.
bool b = some.Contains("value")       // == true
bool b = some.Contains("other")       // == false

// Contains() always returns false for an empty maybe.
bool b = none.Contains("value")       // == false
bool b = none.Contains("other")       // == false
```

#### Map and filter the enclosed value (if any)
```csharp
var some = Maybe.Some(4);

Maybe<int> q = some.Where(x >= 0).Select(x => Math.Sqrt(x));    // == Maybe(2)
// Or using the Query Expression Pattern.
Maybe<int> q = from x in some where x >= 0 select Math.Sqrt(x); // == Maybe(2)
```
If, instead, we start with an empty _maybe_ or `Maybe.Some(-4)`, the result is
an empty _maybe_, in both cases.

#### Safely extract the enclosed value
`Maybe<T>` is a strict option type, we don't get direct access to the enclosed
value.
```csharp
Maybe<string> maybe = Maybe.SomeOrNone("...");

// First the truely unsafe way of doing things, not recommended but at least
// it is clear from the method's name that the result might be null.
// WARNING: value may be null here!
string value = maybe.ValueOrDefault();

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
A word of caution, the methods may only be considered safe when targetting
.NET Core 3.0 or above, that is your project file should include something like
this:
```xml
<TargetFrameworks>netcoreapp3.0;netstandard2.0</TargetFrameworks>
<Nullable>enable</Nullable>
```
C# 8.0 will then complain if one tries to write `maybe.ValueOrElse(null)`.

#### Pattern matching: extract and map the enclosed value (if any)
```csharp
Maybe<int> q = from x in maybe where x >= 0 select Math.Sqrt(x);

string message = q.Switch(
    caseSome: x  => $"Square root = {x}."),
    caseNone: () => "The input was strictly negative.");

string message = q.Switch(
    caseSome: x => $"Square root = {x}."),
    caseNone: "The input was strictly negative.");
```

#### Side-effects: do something with the enclosed value (if any)
```csharp
Maybe<int> q = from x in maybe where x >= 0 select Math.Sqrt(x);

q.Do(
    onSome: x  => Console.WriteLine($"Square root = {x}."),
    onNone: () => Console.WriteLine("The input was strictly negative."));

q.OnSome(x => Console.WriteLine($"Square root = {x}.");

if (q.IsNone) { Console.WriteLine("The input was strictly negative."); }
```

#### Binding
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

#### Query Expression Pattern
We already saw `select` and `where`, but there is more.
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

### Guidelines

Usage.
- CONSIDER using this type when `T` is a value type or an _immutable_ reference
  type.
- AVOID using this type when `T` is a _mutable_ reference type.
- DO NOT use a _maybe_ with nullable types, eg `Maybe<int?>`.

General recommendations.
- DO NOT use `Maybe<T>` in public APIs.
- DO use _maybe_'s when processing multiple nullable objects.
- CONSIDER using _maybe_'s to validate then transform data you don't control.

May-Parse pattern.
- DO use this pattern instead of the Try-Parse pattern for reference types.
- DO use the prefix _May_ for methods implementing this pattern.

Developer Notes
---------------

- Methods that return something should have the attribure `Pure`. It is not
  mandatory but it clearly states that the result should not be ignored.
- Use nullable attributes whenever necessary.
- Seal classes unless they are designed with extensibility in mind.

### TODOs

- Target .NET Standard 2.1 too to be able to distribute a version with nullable
  annotations?
- Tests & code coverage.
- XML comments with complete examples.
- NuGet package.
- Perf tool.
