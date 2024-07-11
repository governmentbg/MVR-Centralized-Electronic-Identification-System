using Microsoft.Extensions.Logging;

namespace eID.RO.Service.Extensions;

public class ObtainKeycloakTokenHandler : DelegatingHandler
{
    private readonly ILogger<ObtainKeycloakTokenHandler> _logger;
    private readonly IKeycloakCaller _keycloakCaller;

    public ObtainKeycloakTokenHandler(
        ILogger<ObtainKeycloakTokenHandler> logger,
        IKeycloakCaller keycloakCaller)
    {
        _logger = logger;
        _keycloakCaller = keycloakCaller;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var keycloakToken = await _keycloakCaller.GetTokenAsync();
        if (string.IsNullOrWhiteSpace(keycloakToken))
        {
            _logger.LogWarning("Unable to obtain Keycloak token");
            throw new InvalidOperationException("Unable to obtain Keycloak token");
        }

        request.Headers.Add("Authorization", $"Bearer {keycloakToken}");

        return await base.SendAsync(request, cancellationToken);
    }
}
