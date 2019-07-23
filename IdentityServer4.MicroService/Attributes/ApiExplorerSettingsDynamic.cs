using Microsoft.AspNetCore.Mvc;
using System;
using static IdentityServer4.MicroService.IdentityServer4MicroServiceOptions;

namespace IdentityServer4.MicroService
{
    public class ApiExplorerSettingsDynamic : ApiExplorerSettingsAttribute
    {
        public ApiExplorerSettingsDynamic(string name)
        {
            if (ISAPIDocuments.Count > 0 && ISAPIDocuments.Contains(IdentityServerAPIDocuments.ALL))
            {
                IgnoreApi = true;
            }

            else if (!string.IsNullOrWhiteSpace(name))
            {
                var APIDocumentEnum = Enum.Parse<IdentityServerAPIDocuments>(name);

                if (ISAPIDocuments.Contains(APIDocumentEnum))
                {
                    IgnoreApi = true;
                }
            }
        }
    }
}
