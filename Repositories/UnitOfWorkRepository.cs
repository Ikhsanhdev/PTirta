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
      IArticleRepository articleRepository,
      IProjectRepository projectRepository
    )
    {
      User = userRepository;
      Article = articleRepository;
      Project = projectRepository;
    }

    public IUserRepository User { get; }
    public IArticleRepository Article { get; }
    public IProjectRepository Project { get; }
  }
}
