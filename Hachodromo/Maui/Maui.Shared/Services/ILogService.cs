using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maui.Shared.Services
{
    public interface ILogService
    {
        void Info(string message);
        string Estado { get; }
    }
}
