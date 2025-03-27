using TirtaRK.Repositories;

namespace TirtaRK.Interfaces
{
  public interface IUnitOfWorkRepository
  {
    IUserRepository User { get; }
    IProductRepository Product { get; }
    IArticleRepository Article { get; }
    IServiceRepository Service { get; }
    IGalleryRepository Gallery { get; }
    IWorkRepository Work { get; }
    IProjectRepository Project { get; }
    IMainRepository Main { get; }
    IActivitiesRepository Activities { get; }
    
  }
}
