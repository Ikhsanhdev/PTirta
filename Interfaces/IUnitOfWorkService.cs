using TirtaRK.Services;

namespace TirtaRK.Interfaces
{
  public interface IUnitOfWorkService
  {
    IAuthService Auths { get; }
    ImageUploadService ImageUploads { get; }
  }
}
