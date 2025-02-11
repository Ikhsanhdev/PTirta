using Higertech.Repositories;

namespace Higertech.Interfaces
{
  public interface IUnitOfWorkRepository
  {
    IUserRepository User { get; }
    IArticleRepository Article { get; }
    IProjectRepository Project { get; }
  }
}
