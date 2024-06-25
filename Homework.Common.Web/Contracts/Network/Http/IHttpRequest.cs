using Homework.Common.Dto;
using Homework.Common.Dto.OAuth;
using Homework.Common.Web.Network.Http;

namespace Homework.Common.Web.Contracts.Network.Http
{
    public interface IHttpRequest
    {
        public Task<HttpRequestBuilder> MakeOAuthHandshakeAsync(OAuthRequestDto request, CancellationToken cancellationToken = default);

        public Task<TResponse> HttpPostAsync<TRequest, TResponse, TData>(string requestUri, string jwt, string jwtRefreshToken, string language, TRequest data, CancellationToken cancellationToken = default)
            where TRequest : class
            where TResponse : BaseClientResult<TData>, new();

        public Task<TResponse> HttpGetAsync<TResponse, TData>(string requestUri, string jwt, string jwtRefreshToken, string? language = default, CancellationToken cancellationToken = default)
            where TResponse : BaseClientResult<TData>, new();

        public Task<TResponse> HttpPutAsync<TRequest, TResponse, TData>(string requestUri, string jwt, string jwtRefreshToken, string language, TRequest data, CancellationToken cancellationToken = default)
            where TRequest : class
            where TResponse : BaseClientResult<TData>, new();


        public Task<TResponse> HttpDeleteAsync<TResponse, TData>(string requestUri, string jwt, string jwtRefreshToken, string language, CancellationToken cancellationToken = default)
            where TResponse : BaseClientResult<TData>, new();

    }
}
