using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Class to help for deferred execution tests: it throw an exception
/// if GetEnumerator is called.
/// </summary>
internal sealed class ThrowingEnumerable<T> : IEnumerable<T>
{
    public IEnumerator<T> GetEnumerator() => throw new InvalidOperationException();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
