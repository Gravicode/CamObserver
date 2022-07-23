using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using CamObserver.App.Data;
using CamObserver.Models;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CamObserver.App.Helpers
{
    public class AuthStateProvider : AuthenticationStateProvider
    {
        private readonly ISyncMemoryStorageService _localStorage;
        public AuthStateProvider(ISyncMemoryStorageService localStorage)
        {
            _localStorage = localStorage;
        }
        public async override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var auth = await _localStorage.GetItem<AuthenticatedUserModel>(AppConstants.Authentication);
            var state = new AuthenticationState(new ClaimsPrincipal());
            if (auth is not null)
            {
                state = new AuthenticationState(new ClaimsPrincipal(
                    new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, auth.Username) }
                    , auth.TokenType)));
            }

            NotifyAuthenticationStateChanged(Task.FromResult(state));
            return state;
        }
    }
}
