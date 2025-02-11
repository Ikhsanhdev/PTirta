using Higertech.Repositories;

namespace Higertech.Interfaces
{
  public interface IUnitOfWorkRepository
  {
    IUserRepository User { get; }
    IProductRepository Product { get; }
  }
}
