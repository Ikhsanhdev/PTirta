using TirtaRK.Interfaces;

namespace TirtaRK.Services
{
  public class UnitOfWorkService : IUnitOfWorkService
  {
    public UnitOfWorkService(
      IAuthService authService,
      ImageUploadService imageUploadService
    )
    {
      Auths = authService;
      ImageUploads = imageUploadService;
    }

    public IAuthService Auths { get; }
    public ImageUploadService ImageUploads { get; }
  }
}
