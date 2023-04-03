using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureFunction.Portfolio
{
    public class SmtpSetting
    {
        public string? SmtpServer { get; set; }
        public string? SmtpServerPort { get; set; }
        public string? SmtpServerSSL { get; set; }
        public string? SmtpServerUserName { get; set; }
        public string? SmtpServerPassword { get; set; }
        public string? SmtpEmailFrom { get; set; }
        public string? SmtpEmailTo { get; set; }
    }
}
