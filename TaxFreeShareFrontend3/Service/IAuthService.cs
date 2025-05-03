using TaxFreeShareFrontend3.Models;

namespace TaxFreeShareFrontend3.Services;

public interface IAuthService
{
    Task<LoginResponse> LoginAsync(LoginModel model);

}