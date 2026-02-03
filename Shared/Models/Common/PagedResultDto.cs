using System.Collections.Generic;

namespace Shared.Models.Dto
{
    public class PagedResultDto<T>
    {
        public List<T> Items { get; set; } = new();

        public int Page { get; set; }
        public int PageSize { get; set; }

        public int TotalCount { get; set; }
    }
}
