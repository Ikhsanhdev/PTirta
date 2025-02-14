using Higertech.Services;

namespace Higertech.Interfaces
{
  public interface IUnitOfWorkService
  {
    IAuthService Auths { get; }
    ImageUploadService ImageUploads { get; }
  }
}
