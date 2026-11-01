#region Licence

/* The MIT License (MIT)
Copyright © 2026 Ian Cooper

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE. */

#endregion

namespace Oyez.Adapters.Tests;

public sealed class OccurrenceCounter
{
    private static readonly TimeSpan GIVE_UP_AFTER = TimeSpan.FromSeconds(10);

    private readonly object _gate = new();
    private readonly List<(int Count, TaskCompletionSource Reached)> _waiters = [];
    private int _count;

    public void Record()
    {
        lock (_gate)
        {
            _count++;
            _waiters.RemoveAll(waiter => Release(waiter));
        }
    }

    public Task ReachedAsync(int count)
    {
        lock (_gate)
        {
            if (_count >= count)
            {
                return Task.CompletedTask;
            }
            var reached = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
            _waiters.Add((count, reached));
            return reached.Task.WaitAsync(GIVE_UP_AFTER);
        }
    }

    private bool Release((int Count, TaskCompletionSource Reached) waiter) =>
        waiter.Count <= _count && waiter.Reached.TrySetResult();
}
