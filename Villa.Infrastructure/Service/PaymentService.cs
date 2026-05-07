using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Villla.Infrastructure.Service
{
    public class PaymentService
    {
        public void Pay()
        {
            var secret = Environment.GetEnvironmentVariable("STRIPE_SECRET");
            var publishable = Environment.GetEnvironmentVariable("STRIPE_PUBLISHABLE");
        }
    }
}
