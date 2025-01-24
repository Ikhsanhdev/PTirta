using Higertech.Interfaces;

namespace Higertech.Repositories
{
  public class UnitOfWorkRepository : IUnitOfWorkRepository
  {
    public UnitOfWorkRepository(
      IUserRepository userRepository
    )
    {
      User = userRepository;
    }

    public IUserRepository User { get; }
  }
}
