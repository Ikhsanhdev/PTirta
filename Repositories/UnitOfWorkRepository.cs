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
      IMainRepository mainRepository,
      IActivitiesRepository activitiesRepository,
      IServiceRepository serviceRepository,
      IGalleryRepository galleryRepository,
      IWorkRepository workRepository
    )
    {
      User = userRepository;
      Product = productRepository;
      Article = articleRepository;
      Project = projectRepository;
      Main = mainRepository;
      Activities = activitiesRepository;
      Service = serviceRepository;
      Gallery = galleryRepository;
      Work = workRepository;
    }

    public IUserRepository User { get; }
    public IProductRepository Product { get; }
    public IArticleRepository Article { get; }
    public IProjectRepository Project { get; }
    public IMainRepository Main { get; }
    public IActivitiesRepository Activities { get; }
    public IServiceRepository Service { get; }
    public IGalleryRepository Gallery { get; }
    public IWorkRepository Work { get; }
  }
}
