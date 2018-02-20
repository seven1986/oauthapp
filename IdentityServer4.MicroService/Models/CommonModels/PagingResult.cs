using IdentityServer4.MicroService.Codes;
using System.Collections.Generic;

namespace IdentityServer4.MicroService.Models.CommonModels
{
    public class PagingResult<T> where T : class
    {
        public int take { get; set; }

        public int skip { get; set; }

        public int code { get; set; } = (int)BasicControllerEnums.Status200OK;

        public int total { get; set; }

        public string error_msg { get; set; }

        public IList<T> data { get; set; } = new List<T>();

        public PagingResult(IList<T> _data, int _total, int _skip, int _take)
        {
            data = _data;
            skip = _skip;
            take = _take;
            total = _total;
            code = (int)BasicControllerEnums.Status200OK;
        }

        public PagingResult() { }
    }
}
