using Microsoft.Extensions.Configuration;
using Homework.AspNetCoreMvc.Utils;
using Homework.Common.Dto;
using Homework.Common.Dto.OAuth;
using Homework.Common.Enums;
using Homework.Common.Web.Contracts.Network.Http;
using System.Diagnostics;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace Homework.Common.Web.Network.Http
{
    public sealed class HttpRequestBuilder : IHttpRequest
    {
        public string? JwtToken { get; private set; }
        public string? JwtRefreshToken { get; private set; }
        public bool IsValidHandshake { get { return JwtToken != null; } }
        public bool NeedPasswordReset { get; set; }
        public string? ErrorMessage { get; private set; }

        private readonly IConfiguration _configuration;

        public HttpRequestBuilder(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<HttpRequestBuilder> MakeOAuthHandshakeAsync(OAuthRequestDto request, CancellationToken cancellationToken = default)
        {
            try
            {
                using var httpClient = new HttpClient() { BaseAddress = new Uri(_configuration.GetSection(Constants.API_BASE_URL).Value!) };

                httpClient.DefaultRequestHeaders.Clear();

                if (!string.IsNullOrEmpty(JwtToken))
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(Constants.API_AUTHORIZATION_HEADER_BEARER, JwtToken);

                // Serialize the data object to JSON
                var jsonContent = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, Constants.API_MEDIA_TYPE_JSON);

                // Perform the HTTP POST request
                using var response = await httpClient.PostAsync(Constants.API_LOGIN_PATH, jsonContent, cancellationToken);

                // Deserialize the response content to the expected type (TResponse)
                var responseContent = await response.Content.ReadFromJsonAsync<BaseResult<OAuthResponseDto>>();

                if (responseContent != null && responseContent.IsValid)
                {
                    JwtToken = responseContent!.Data?.JwtToken!;
                    JwtRefreshToken = responseContent?.Data?.JwtRefreshToken!;
                    NeedPasswordReset = responseContent?.Data?.NeedPasswordReset ?? false;
                }
                if (responseContent != null && !responseContent.IsValid)
                {
                    StringBuilder errors = new StringBuilder();
                    foreach (var item in responseContent.ErrorCodes)
                    {
                        errors.AppendLine(ResourceProxy.GetResourceValue($"ErrorCode_{(int)item}"));
                    }

                    ErrorMessage = errors.ToString();
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }

            return this;
        }

        public async Task<TResponse> HttpPostAsync<TRequest, TResponse, TData>(string requestUri, string jwt, string jwtRefreshToken, string language, TRequest data, CancellationToken cancellationToken = default)
            where TRequest : class
            where TResponse : BaseClientResult<TData>, new()
        {
            try
            {
                using var httpClient = new HttpClient() { BaseAddress = new Uri(_configuration.GetSection(Constants.API_BASE_URL).Value!) };

                httpClient.DefaultRequestHeaders.Clear();

                if (!string.IsNullOrEmpty(jwt))
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(Constants.API_AUTHORIZATION_HEADER_BEARER, jwt);
                httpClient.DefaultRequestHeaders.Add("Accept-Language", language);
                var dt = JsonSerializer.Serialize(data);
                var jsonContent = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, Constants.API_MEDIA_TYPE_JSON);
                using var response = await httpClient.PostAsync(requestUri, jsonContent, cancellationToken);

                if (response.StatusCode == HttpStatusCode.Unauthorized && !string.IsNullOrWhiteSpace(jwtRefreshToken))
                {
                    // try to get new token from refresh token 
                    OAuthRefreshTokenResponseDto? responseRefreshContent = await GetNewToken(jwtRefreshToken, httpClient, cancellationToken);

                    JwtToken = responseRefreshContent!.JwtToken;

                    if (!string.IsNullOrEmpty(jwt))
                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(Constants.API_AUTHORIZATION_HEADER_BEARER, responseRefreshContent.JwtToken);

                    using var retryResponse = await httpClient.PostAsync(requestUri, jsonContent, cancellationToken);

                    retryResponse.EnsureSuccessStatusCode();

                    var result = await retryResponse.Content.ReadFromJsonAsync<BaseResult<TData>>();

                    if (result.IsValid)
                        return new TResponse() { Data = result.Data };
                    else
                        return new TResponse() { Errors = ResourceProxy.MapToDictionary(result.ErrorCodes) };
                }

                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadFromJsonAsync<BaseResult<TData>>(cancellationToken: cancellationToken);

                if (responseContent.IsValid)
                    return new TResponse() { Data = responseContent.Data };
                else
                    return new TResponse() { Errors = ResourceProxy.MapToDictionary(responseContent.ErrorCodes) };
            }
            catch (HttpRequestException httpEx)
            {
                if (httpEx.StatusCode == HttpStatusCode.Unauthorized)
                {
                    return (TResponse)new TResponse().AddError(ErrorCode.Unauthorized, httpEx.Message);
                }
                return (TResponse)new TResponse().AddError(ErrorCode.HttpRequestException, httpEx.Message);
            }
            catch (Exception ex)
            {
                return (TResponse)new TResponse().AddError(ErrorCode.Exception, ex.Message);
            }
        }

        public async Task<TResponse> HttpGetAsync<TResponse, TData>(string requestUri, string jwt, string jwtRefreshToken, string? language = default, CancellationToken cancellationToken = default)
            where TResponse : BaseClientResult<TData>, new()
        {
            try
            {
                using var httpClient = new HttpClient() { BaseAddress = new Uri(_configuration.GetSection(Constants.API_BASE_URL).Value!) };

                httpClient.DefaultRequestHeaders.Clear();

                if (!string.IsNullOrEmpty(jwt))
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(Constants.API_AUTHORIZATION_HEADER_BEARER, jwt);
                if (!string.IsNullOrWhiteSpace(language))
                    httpClient.DefaultRequestHeaders.Add("Accept-Language", language);
                using var response = await httpClient.GetAsync(requestUri, cancellationToken);

                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    // try to get new token from refresh token 
                    OAuthRefreshTokenResponseDto? responseRefreshContent = await GetNewToken(jwtRefreshToken, httpClient, cancellationToken);

                    JwtToken = responseRefreshContent.JwtToken;

                    if (!string.IsNullOrEmpty(jwt))
                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(Constants.API_AUTHORIZATION_HEADER_BEARER, responseRefreshContent.JwtToken);

                    using var retryResponse = await httpClient.GetAsync(requestUri, cancellationToken);

                    retryResponse.EnsureSuccessStatusCode();

                    return await retryResponse.Content.ReadFromJsonAsync<TResponse>();
                }

                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadFromJsonAsync<TResponse>();

                return responseContent!;
            }
            catch (Exception ex)
            {
                return (TResponse)new TResponse().AddError(ErrorCode.Exception, ex.Message);
            }
        }

        public async Task<TResponse> HttpPutAsync<TRequest, TResponse, TData>(string requestUri, string jwt, string jwtRefreshToken, string language, TRequest data, CancellationToken cancellationToken = default)
            where TRequest : class
            where TResponse : BaseClientResult<TData>, new()
        {

            try
            {
                using var httpClient = new HttpClient() { BaseAddress = new Uri(_configuration.GetSection(Constants.API_BASE_URL).Value!) };

                httpClient.DefaultRequestHeaders.Clear();

                if (!string.IsNullOrEmpty(jwt))
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(Constants.API_AUTHORIZATION_HEADER_BEARER, jwt);
                httpClient.DefaultRequestHeaders.Add("Accept-Language", language);
                var jsonContent = new StringContent(JsonSerializer.Serialize(data), Encoding.UTF8, Constants.API_MEDIA_TYPE_JSON);

                using var response = await httpClient.PutAsync(requestUri, jsonContent, cancellationToken);

                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    // try to get new token from refresh token 
                    OAuthRefreshTokenResponseDto? responseRefreshContent = await GetNewToken(jwtRefreshToken, httpClient, cancellationToken);

                    JwtToken = responseRefreshContent.JwtToken;

                    if (!string.IsNullOrEmpty(jwt))
                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(Constants.API_AUTHORIZATION_HEADER_BEARER, responseRefreshContent.JwtToken);

                    using var retryResponse = await httpClient.PutAsync(requestUri, jsonContent, cancellationToken);

                    retryResponse.EnsureSuccessStatusCode();

                    return await retryResponse.Content.ReadFromJsonAsync<TResponse>();
                }

                var responseContent = await response.Content.ReadFromJsonAsync<TResponse>();
                Debug.WriteLine(responseContent);

                return responseContent!;
            }
            catch (Exception ex)
            {
                return (TResponse)new TResponse().AddError(ErrorCode.Exception, ex.Message);
            }
        }

        public async Task<TResponse> HttpDeleteAsync<TResponse, TData>(string requestUri, string jwt, string jwtRefreshToken, string language, CancellationToken cancellationToken = default)
            where TResponse : BaseClientResult<TData>, new()
        {
            try
            {
                using var httpClient = new HttpClient() { BaseAddress = new Uri(_configuration.GetSection(Constants.API_BASE_URL).Value!) };

                httpClient.DefaultRequestHeaders.Clear();

                if (!string.IsNullOrEmpty(jwt))
                    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(Constants.API_AUTHORIZATION_HEADER_BEARER, jwt);

                using var response = await httpClient.DeleteAsync(requestUri, cancellationToken);

                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    // try to get new token from refresh token 
                    OAuthRefreshTokenResponseDto? responseRefreshContent = await GetNewToken(jwtRefreshToken, httpClient, cancellationToken);

                    JwtToken = responseRefreshContent!.JwtToken;

                    if (!string.IsNullOrEmpty(jwt))
                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(Constants.API_AUTHORIZATION_HEADER_BEARER, responseRefreshContent.JwtToken);

                    using var retryResponse = await httpClient.DeleteAsync(requestUri, cancellationToken);

                    retryResponse.EnsureSuccessStatusCode();

                    return await retryResponse.Content.ReadFromJsonAsync<TResponse>();
                }

                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadFromJsonAsync<TResponse>();

                return responseContent!;
            }
            catch (Exception ex)
            {
                return (TResponse)new TResponse().AddError(ErrorCode.Exception, ex.Message);
            }
        }

        private static async Task<OAuthRefreshTokenResponseDto?> GetNewToken(string jwtRefreshToken, HttpClient httpClient, CancellationToken cancellationToken)
        {
            var serializedObject = JsonSerializer.Serialize(new OAuthRefreshTokenRequestDto() { JwtRefreshToken =  jwtRefreshToken });
            var refreshTokenContent = new StringContent(serializedObject, Encoding.UTF8, Constants.API_MEDIA_TYPE_JSON);
            var refreshTokenResponse = await httpClient.PostAsync(Constants.API_REFRESH_TOKEN_PATH, refreshTokenContent, cancellationToken);
            var responseRefreshContent = await refreshTokenResponse.Content.ReadFromJsonAsync<BaseResult<OAuthRefreshTokenResponseDto>>();
            return responseRefreshContent.Data;
        }
    }
}
