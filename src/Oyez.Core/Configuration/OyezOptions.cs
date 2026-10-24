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

namespace Oyez.Core.Configuration;

/// <summary>
/// The settings that shape how Oyez prepares and publishes announcements.
/// </summary>
public sealed class OyezOptions
{
    /// <summary>
    /// Gets the settings for the X channel.
    /// </summary>
    /// <value>Enabled, with a limit of 280.</value>
    public ChannelOptions X { get; } = new() { Limit = 280 };

    /// <summary>
    /// Gets the settings for the Mastodon channel.
    /// </summary>
    /// <value>Enabled, with a limit of 2,263.</value>
    public ChannelOptions Mastodon { get; } = new() { Limit = 2263 };

    /// <summary>
    /// Gets the settings for the LinkedIn channel.
    /// </summary>
    /// <value>Enabled, with a limit of 3,000.</value>
    public LinkedInOptions LinkedIn { get; } = new() { Limit = 3000 };

    /// <summary>
    /// Gets the settings for the Write.as channel.
    /// </summary>
    /// <value>Enabled, with no limit.</value>
    public ChannelOptions WriteAs { get; } = new();
}
