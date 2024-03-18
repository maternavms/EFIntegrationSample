using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestEfMultipleSqlVersions.ArcContext
{
    public class ArcDataContextProvider : IArcDataContextProvider
    {
        private readonly IServiceProvider serviceProvider;

        public ArcDataContextProvider(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }
        public IArcAgentDbContext GetContext(string instanceName)
        {
            return serviceProvider
                .GetRequiredService<IArcAgentDbContext>()
                .Instance(instanceName);
        }
    }
}
