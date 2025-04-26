using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hachodromo.Shared.Responses
{
    public class Response
    {
        public bool IsSuccess { get; set; }
        public string? Message { get; set; }
        public object? Result { get; set; }
        public int StatusCode { get; set; } = 200;
        public List<string>? Errors { get; set; } = new List<string>();
    }
}
