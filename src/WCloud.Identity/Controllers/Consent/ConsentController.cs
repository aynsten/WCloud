using System;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using Lib.helper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace IdentityServer4.Quickstart.UI
{
    /// <summary>
    /// This controller processes the consent UI
    /// </summary>
    [Authorize]
    public class ConsentController : Controller
    {
        private readonly IIdentityServerInteractionService _interaction;
        private readonly IClientStore _clientStore;
        private readonly IResourceStore _resourceStore;
        private readonly ILogger _logger;

        public ConsentController(
            IIdentityServerInteractionService interaction,
            IClientStore clientStore,
            IResourceStore resourceStore,
            ILogger<ConsentController> logger)
        {
            _interaction = interaction;
            _clientStore = clientStore;
            _resourceStore = resourceStore;
            _logger = logger;
        }

        /// <summary>
        /// Shows the consent screen
        /// </summary>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Index(string returnUrl)
        {
            var vm = await BuildViewModelAsync(returnUrl);
            if (vm != null)
            {
                return View("Index", vm);
            }

            return Content("Error");
        }

        /// <summary>
        /// Handles the consent screen postback
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConsentAction(ConsentInputModel model)
        {
            // validate return url is still valid
            var request = await _interaction.GetAuthorizationContextAsync(model.ReturnUrl);
            if (request == null)
                return Content("Error");

            // user clicked 'no' - send back the standard 'access_denied' response
            if (model.Button == "no")
            {
                await _interaction.GrantConsentAsync(request, new ConsentResponse()
                {
                    Error = AuthorizationError.AccessDenied
                });
                return Redirect(model.ReturnUrl);
            }
            else if (model.Button == "yes")
            {
                if (ValidateHelper.IsNotEmpty(model.ScopesConsented))
                {
                    var scopes = model.ScopesConsented;

                    var res = new ConsentResponse
                    {
                        RememberConsent = model.RememberConsent,
                        ScopesValuesConsented = scopes.ToArray()
                    };
                    await _interaction.GrantConsentAsync(request, res);
                    return Redirect(model.ReturnUrl);
                }
                else
                {
                    return Content("����ѡ��һ��scope");
                }
            }
            else
            {
                throw new ArgumentException("button���ʹ���");
            }
        }

        async Task<ConsentViewModel> BuildViewModelAsync(string returnUrl)
        {
            var request = await _interaction.GetAuthorizationContextAsync(returnUrl);
            if (request != null && request.Client != null)
            {
                var client = await _clientStore.FindEnabledClientByIdAsync(request.Client.ClientId);
                if (client != null)
                {
                    var resources = await _resourceStore.FindEnabledResourcesByScopeAsync(request.ValidatedResources.RawScopeValues);
                    if (resources != null && (resources.IdentityResources.Any() || resources.ApiResources.Any()))
                    {
                        return await CreateConsentViewModel(returnUrl, client, resources);
                    }
                    else
                    {
                        _logger.LogError("No scopes matching: {0}", string.Join(",", request.ValidatedResources.RawScopeValues));
                    }
                }
                else
                {
                    _logger.LogError("Invalid client id: {0}", request.Client.ClientId);
                }
            }
            else
            {
                _logger.LogError("No consent request matching request: {0}", returnUrl);
            }

            return null;
        }

        async Task<ConsentViewModel> CreateConsentViewModel(
           string returnUrl,
           Client client,
           Resources resources)
        {
            var vm = new ConsentViewModel
            {
                RememberConsent = true,
                ScopesConsented = Enumerable.Empty<string>(),

                ReturnUrl = returnUrl,

                ClientName = client.ClientName ?? client.ClientId,
                ClientUrl = client.ClientUri,
                ClientLogoUrl = client.LogoUri,
                AllowRememberConsent = client.AllowRememberConsent
            };

            vm.IdentityScopes = resources.IdentityResources.Select(x => CreateScopeViewModel(x)).ToArray();
            var scopes = await this._resourceStore.FindApiScopesByNameAsync(resources.ApiResources.SelectMany(x => x.Scopes));
            vm.ResourceScopes = scopes.Select(x => CreateScopeViewModel(x)).ToArray();
            if (resources.OfflineAccess)
            {
                vm.ResourceScopes = vm.ResourceScopes.Union(new[] { GetOfflineAccessScope() });
            }

            return vm;
        }

        ScopeViewModel CreateScopeViewModel(IdentityResource identity)
        {
            return new ScopeViewModel
            {
                Name = identity.Name,
                DisplayName = identity.DisplayName,
                Description = identity.Description,
                Emphasize = identity.Emphasize,
                Required = identity.Required,
            };
        }

        ScopeViewModel CreateScopeViewModel(ApiScope scope)
        {
            return new ScopeViewModel
            {
                Name = scope.Name,
                DisplayName = scope.DisplayName,
                Description = scope.Description,
                Emphasize = scope.Emphasize,
                Required = scope.Required,
            };
        }

        ScopeViewModel GetOfflineAccessScope()
        {
            return new ScopeViewModel
            {
                Name = IdentityServer4.IdentityServerConstants.StandardScopes.OfflineAccess,
                DisplayName = "Offline Access",
                Description = "����ˢ��token",
                Emphasize = true,
            };
        }
    }
}