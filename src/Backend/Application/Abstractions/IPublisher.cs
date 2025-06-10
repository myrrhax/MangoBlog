namespace Application.Abstractions;

public interface IPublisher
{
    void PublishMessage(string id, string message);
}
