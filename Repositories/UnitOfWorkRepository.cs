using Higertech.Interfaces;

namespace Higertech.Repositories
{
  public class UnitOfWorkRepository : IUnitOfWorkRepository
  {
    public UnitOfWorkRepository(
      IUserRepository userRepository,
      IProductRepository productRepository,
      IArticleRepository articleRepository,
      IProjectRepository projectRepository,
      IMainRepository mainRepository
    )
    {
      User = userRepository;
      Product = productRepository;
      Article = articleRepository;
      Project = projectRepository;
      Main = mainRepository;
    }

    public IUserRepository User { get; }
    public IProductRepository Product { get; }
    public IArticleRepository Article { get; }
    public IProjectRepository Project { get; }
    public IMainRepository Main { get; }
  }
}
