using Jurupema.Api.Application.Messaging;

namespace Jurupema.Api.Infrastructure.Messaging;

public sealed class NoOpTopicMessagePublisher : ITopicMessagePublisher
{
    public Task PublishAsync<T>(
        string topicName,
        string messageName,
        T message,
        CancellationToken cancellationToken = default) =>
        Task.CompletedTask;
}
