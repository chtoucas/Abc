# Abécédaire

Features
--------

- A strict Option type; see project Abc.Maybe.
- Utilities to write code in the ROP style (Railway Oriented Programming).
- A fast Gregorian date type?

Abc.Maybe
---------

An Option type can help to avoid null reference exceptions, but that's not the
point, it forces us to think about the outcome of a computation. We shall see
a few concrete examples below (web, UI, logging, to be written...).

Quick start with the `Maybe<T>` type.
```csharp
// Maybe of a value type.
Maybe<int> q = Maybe.Some(1);
Maybe<int> q = Maybe<int>.None;

// Maybe of a nullable value type.
Maybe<int> q = Maybe.SomeOrNone((int?)1);
Maybe<int> q = Maybe.SomeOrNone((int?)null);        // == Maybe<int>.None

// Maybe of a reference type (NRT if available).
Maybe<string> q = Maybe.SomeOrNone("value");
Maybe<string> q = Maybe.SomeOrNone((string?)null);  // == Maybe<string>.None
```

Map, filter the enclosed value.
```csharp
var some = Maybe.Some(4);
var none = Maybe<int>.None;

Maybe<int> q = some.Where(x >= 0).Select(x => Math.Sqrt(x));    
// Or using the Query Expression Pattern.
Maybe<int> q = from x in some where x >= 0 select Math.Sqrt(x);
```
In both cases, the result is Maybe(2).
If we started from `none` or `Maybe.Some(-4)` instead of `some`, the result
would be `Maybe<int>.None`.

Safely extract the enclosed value. `Maybe<T>` is a strict option type, we don't 
get a direct access to the enclosed value.
A word of caution, the methods can only be considered safe when targetting 
.NET Core 3.0 or above. For instance, the C# 8.0 will complain if one tries to 
write maybe.ValueOrElse(null).
```csharp
Maybe<string> maybe = ...

// First the truely unsafe way of doing things, not recommended but at least
// it is clear from the method's name that the result might be null.
// WARNING: value may be null here!
string value = maybe.ValueOrDefault();     

string value = maybe.ValueOrElse("Replacement string");
// We may delay the creation of the replacement value.
string value = maybe.ValueOrElse(() => "Replacement string");

// If maybe is empty, throw InvalidOperationException.
string value = maybe.ValueOrThrow();
// Throw a custom exception.
string value = maybe.ValueOrThrow(() => new Exception());    
```

Pattern matching.
```csharp
Maybe<int> q = from x in some where x >= 0 select Math.Sqrt(x);
string value = q.Switch(
    caseSome: x  => $"Square root = {x}."), 
    caseNone: () => "The input was strictly negative.");   

string value = q.Switch(
    caseSome: x => $"Square root = {x}."), 
    caseNone: "The input was strictly negative.");
```

Side-effects.
```csharp
q.Do(
    onSome: x  => Console.WriteLine($"Square root = {x}."), 
    onNone: () => Console.WriteLine("The input was strictly negative."));

q.OnSome(x => Console.WriteLine($"Square root = {x}.");

if (q.IsNone) { Console.WriteLine("The input was strictly negative."); }
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

And much more, see the XML comments for samples.

Developer Notes
---------------

- Methods that return something should have the attribure `Pure`. It is not
  mandatory but it clearly states that the result should not be ignored.
- Use nullable attributes whenever necessary.