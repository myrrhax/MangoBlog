using Domain.Entities;
using Domain.Utils;

namespace Application.Abstractions;

public interface IQueuePublisher
{
    Result Publish(Publication publication);
}
