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
using Oyez.Core.Publishing;
using Xunit.Sdk;

namespace Oyez.Adapters.Tests.Retries;

public sealed class ChannelUnderTest : IXunitSerializable
{
    private static readonly ChannelUnderTest X_CHANNEL = new(
        "x",
        ChannelId.X,
        () => PostRequests.Thread(ChannelId.X, 280),
        """{"data":{"id":"1","text":"Brighter 10.1 is out."}}""");

    private static readonly ChannelUnderTest WRITE_AS_CHANNEL = new(
        "writeas",
        ChannelId.WriteAs,
        () => WriteAs.WriteAsTestData.Article(),
        """{"code":201,"data":{"id":"abc","slug":"brighter-10-1"}}""");

    private string _name = "";

    public ChannelUnderTest()
    {
    }

    private ChannelUnderTest(string name, ChannelId channel, Func<PostRequest> request, string acceptedBody)
    {
        _name = name;
        Channel = channel;
        Request = request;
        AcceptedBody = acceptedBody;
    }

    public static TheoryData<ChannelUnderTest> ALL => [X_CHANNEL, WRITE_AS_CHANNEL];

    public ChannelId Channel { get; private set; }

    public Func<PostRequest> Request { get; private set; } = () => throw new InvalidOperationException();

    public string AcceptedBody { get; private set; } = "";

    public void Deserialize(IXunitSerializationInfo info)
    {
        var known = ALL.Select(row => row.Data).Single(channel => channel._name == info.GetValue<string>("name")!);
        _name = known._name;
        Channel = known.Channel;
        Request = known.Request;
        AcceptedBody = known.AcceptedBody;
    }

    public void Serialize(IXunitSerializationInfo info) => info.AddValue("name", _name);

    public override string ToString() => _name;
}
