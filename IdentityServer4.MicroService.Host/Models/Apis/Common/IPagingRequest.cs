using System;

namespace IdentityServer4.MicroService.Models.Apis.Common
{
   public interface IPagingRequest
    {
        bool? asc { get; set; }

        string orderby { get; set; }

        int? skip { get; set; }

        int? take { get; set; }

        DateTime? startTime { get; set; }

        DateTime? endTime { get; set; }
    }
}
