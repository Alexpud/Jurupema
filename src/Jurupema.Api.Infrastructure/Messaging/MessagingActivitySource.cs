using System.Diagnostics;

namespace Jurupema.Api.Infrastructure.Messaging;

internal static class MessagingActivitySource
{
    internal const string Name = "Jurupema.Api.Messaging";

    internal static readonly ActivitySource Instance = new(Name);
}
