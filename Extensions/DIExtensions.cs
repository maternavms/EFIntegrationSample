using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TestEfMultipleSqlVersions.Attributes;

namespace TestEfMultipleSqlVersions.Extensions
{
    public static class DIExtensions
    {
        public static ModelBuilder UseVersionSpecificModel(this ModelBuilder builder, Version sqlVersion)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies().ByReferencesODataCore();
            foreach (var assembly in assemblies)
            {
                builder.ApplyConfigurationsFromAssembly(assembly);
            }

            builder.ApplyVersionSpecificConfigurations(sqlVersion);

            return builder;
        }

        //by now all entity sets should be registered, so we can start trimming the model properties that are not valid for the version.
        public static ModelBuilder ApplyVersionSpecificConfigurations(this ModelBuilder builder, Version version)
        {
            var entityTypes = builder.Model.GetEntityTypes();

            foreach (var entityType in entityTypes)
            {
                var supportedAttribute = entityType.ClrType.GetCustomAttribute<SupportedAttribute>();
                if (supportedAttribute is not null && !supportedAttribute.IsSupported(version))
                {
                    builder.Ignore(entityType.ClrType);
                    continue;
                }
                entityType.IgnoreUnsupportedProperties(version);
            }
            return builder;
        }

        public static void IgnoreUnsupportedProperties(this IMutableTypeBase type, Version version)
        {
            var properties = type.GetProperties().ToList();
            foreach (var property in properties)
            {
                var supportedAttribute = property.PropertyInfo?.GetCustomAttribute<SupportedAttribute>();
                if (supportedAttribute is not null && !supportedAttribute.IsSupported(version))
                {
                    type.RemoveProperty(property);
                }
            }
        }
    }
}
