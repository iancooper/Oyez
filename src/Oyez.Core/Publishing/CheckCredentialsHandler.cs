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

using Oyez.Core.Channels;
using Paramore.Darker;

namespace Oyez.Core.Publishing;

internal sealed class CheckCredentialsHandler(
    IEnumerable<IAmAChannelPublisher> publishers,
    IEnumerable<IAmAChannelProfile> profiles,
    TimeProvider clock) : QueryHandlerAsync<CheckCredentials, CredentialsReport>
{
    private const int WARNING_DAYS = 14;

    public override async Task<CredentialsReport> ExecuteAsync(CheckCredentials query, CancellationToken cancellationToken = default)
    {
        var checks = new List<ChannelCredentials>();
        foreach (var publisher in EnabledPublishers())
        {
            var status = await publisher.VerifyAsync(cancellationToken);
            checks.Add(new ChannelCredentials(publisher.Channel, status, DaysLeft(status)));
        }

        return new CredentialsReport(checks);
    }

    private IEnumerable<IAmAChannelPublisher> EnabledPublishers() =>
        ChannelId.All.SelectMany(channel => publishers.Where(publisher => publisher.Channel == channel && IsEnabled(channel)));

    private bool IsEnabled(ChannelId channel) =>
        profiles.Any(profile => profile.Id == channel && profile.IsEnabled);

    private int? DaysLeft(CredentialStatus status) =>
        status.ExpiresAt is { } expiresAt && WholeDaysUntil(expiresAt) is var days && days <= WARNING_DAYS ? days : null;

    private int WholeDaysUntil(DateTimeOffset expiresAt) =>
        (int)Math.Floor((expiresAt - clock.GetUtcNow()).TotalDays);
}
