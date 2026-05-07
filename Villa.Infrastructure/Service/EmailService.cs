using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Villla.Infrastructure.Service
{
    public class EmailService
    {
        public void SendEmail()
        {
            var email = Environment.GetEnvironmentVariable("EMAIL");
            var password = Environment.GetEnvironmentVariable("EMAIL_PASSWORD");
            var host = Environment.GetEnvironmentVariable("EMAIL_HOST");
            var port = Environment.GetEnvironmentVariable("EMAIL_PORT");
            var ssl = Environment.GetEnvironmentVariable("EMAIL_SSL");

        }
    }
}
