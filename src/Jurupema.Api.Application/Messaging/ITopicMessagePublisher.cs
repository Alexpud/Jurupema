namespace Jurupema.Api.Application.Messaging;

public interface ITopicMessagePublisher
{
    Task PublishAsync<T>(
        string topicName,
        string messageName,
        T message,
        CancellationToken cancellationToken = default);
}
