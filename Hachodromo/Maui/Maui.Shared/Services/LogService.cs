using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maui.Shared.Services
{
    public class LogService : ILogService
    {
        private string _estado = "";

        public string Estado => _estado;

        public void Info(string message)
        {
            _estado = $"[{DateTime.Now:HH:mm:ss}] {message}";
            Console.WriteLine(_estado);           // Para Depurar en Visual Studio
            Debug.WriteLine(_estado);            // Por si estás usando Debug.WriteLine
        }
    }
}
