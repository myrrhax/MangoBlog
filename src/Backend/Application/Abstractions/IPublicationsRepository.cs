using Domain.Entities;
using Domain.Utils;

namespace Application.Abstractions;
public interface IPublicationsRepository
{
    Task<Result> AddPublication(Publication publication);
}
