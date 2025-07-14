using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace PersonalFinanceTracker_EnterpriseEdition.Application.Helpers;

public static class HttpContextHelper
{
    public static IHttpContextAccessor Accessor { get; set; }
    public static HttpContext HttpContext => Accessor?.HttpContext;
    public static IHeaderDictionary ResponseHeaders => HttpContext?.Response?.Headers;
    public static Guid UserId => Guid.TryParse(HttpContext?.User?.FindFirst("Id")?.Value, out _tempUserId) ? _tempUserId : _tempUserId;
    public static string UserRole => HttpContext?.User?.FindFirst("http://schemas.microsoft.com/ws/2008/06/identity/claims/role")?.Value;
    
    private static Guid _tempUserId;
}