using System.Collections.Generic;

namespace Shared.Models.Dto
{
    public class ApiErrorDto
    {
        public string Code { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;

        public Dictionary<string, string[]>? Details { get; set; }
    }
}
