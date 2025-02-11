using Higertech.Interfaces;

namespace Higertech.Repositories
{
  public class UnitOfWorkRepository : IUnitOfWorkRepository
  {
    public UnitOfWorkRepository(
      IUserRepository userRepository,
      IProductRepository productRepository
    )
    {
      User = userRepository;
      Product = productRepository;
    }

    public IUserRepository User { get; }
    public IProductRepository Product { get;}
  }
}
