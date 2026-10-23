namespace Oyez.Core.Announcements;

/// <summary>
/// Identifies an announcement. It is the name of the announcement's folder under <c>announcements/</c>.
/// </summary>
/// <param name="Value">The folder name, such as <c>2026-11-03-brighter-10-3-0</c>.</param>
public sealed record AnnouncementId(string Value)
{
    /// <summary>Returns the folder name.</summary>
    /// <returns>The <see cref="string"/> value of the ID.</returns>
    public override string ToString() => Value;
}
