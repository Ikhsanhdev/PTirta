using Higertech.Interfaces;
using Higertech.Repositories;
using Higertech.Services;

namespace Higertech
{
  public static class ServiceCollectionteExtension
  {
    public static void RegisterServices(this IServiceCollection services)
    {
      #region ========== [ Register Unit Of Works ] ==========
      services.AddScoped<IUnitOfWorkRepository, UnitOfWorkRepository>();
      services.AddScoped<IUnitOfWorkService, UnitOfWorkService>();
      #endregion

      #region ========== [ Register Services ] ==========
      services.AddScoped<IAuthService, AuthService>();
      #endregion

      #region ========== [ Register Repositories ] ==========
      services.AddScoped<IUserRepository, UserRepository>();
      services.AddScoped<IArticleRepository, ArticleRepository>();
      services.AddScoped<IProductRepository, ProductRepository>();
      services.AddScoped<IProjectRepository, ProjectRepository>();
      #endregion
    }
  }
}