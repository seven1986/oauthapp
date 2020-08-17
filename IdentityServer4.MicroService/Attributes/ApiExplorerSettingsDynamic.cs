using Microsoft.AspNetCore.Mvc;
using System;
using static IdentityServer4.MicroService.IdentityServer4MicroServiceOptions;

namespace IdentityServer4.MicroService
{
    public class ApiExplorerSettingsDynamic : ApiExplorerSettingsAttribute
    {
        public ApiExplorerSettingsDynamic(string name)
        {
            if (APIDocuments.Count > 0 && APIDocuments.Contains(APIDocumentEnums.ALL))
            {
                IgnoreApi = true;
            }

            else if (!string.IsNullOrWhiteSpace(name))
            {
                var APIDocumentEnum = Enum.Parse<APIDocumentEnums>(name);

                if (APIDocuments.Contains(APIDocumentEnum))
                {
                    IgnoreApi = true;
                }
            }

            else{
                IgnoreApi = false;
            }
        }
    }
}
