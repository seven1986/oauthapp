namespace IdentityServer4.MicroService.Models.Apis.Common
{
    public interface IApiResult<T>
    {
        int code { get; set; }

        string codeName { get; set; }

        string message { get; set; }

        T data { get; set; }
    }
}
