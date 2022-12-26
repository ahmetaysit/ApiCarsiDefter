using Library.CustomAttributes;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Core
{
    public static class DependencyHandler
    {
        public static IServiceCollection AddBusinessServices(this IServiceCollection services)
        {
            #region IBaseDal
            var type = typeof(IBaseDAL);
            var types = Assembly.Load("Interfaces").GetTypes()
                .Where(p => type.IsAssignableFrom(p) && p.IsInterface && p != type);

            foreach (var item in types)
            {
                var typeClass = Assembly.Load("DataLayer").GetTypes()
                .Where(p => item.IsAssignableFrom(p) && !p.IsInterface && p != type).FirstOrDefault();
                if (typeClass != null)
                {
                    services.Add(new ServiceDescriptor(item, typeClass, ServiceLifetime.Transient));
                }

            }
            #endregion

            #region IBaseBS
            type = typeof(IBaseBS);
            types = Assembly.Load("Interfaces").GetTypes()
                .Where(p => type.IsAssignableFrom(p) && p.IsInterface && p != type);

            foreach (var item in types)
            {
                var typeClass = Assembly.Load("BusinessServices").GetTypes()
                .Where(p => item.IsAssignableFrom(p) && !p.IsInterface && p != type).FirstOrDefault();
                if (type != null && typeClass != null)
                {
                    services.Add(new ServiceDescriptor(item, typeClass, ServiceLifetime.Transient));
                }

            }
            #endregion

            //#region CustomBaseBS Register

            //type = typeof(ICustomBaseBS);
            //types = Assembly.Load("Interfaces").GetTypes()
            //    .Where(p => type.IsAssignableFrom(p) && p.IsInterface && p != type);

            //foreach (var item in types)
            //{
            //    var typeClass = Assembly.Load("BusinessServices").GetTypes()
            //    .Where(p => item.IsAssignableFrom(p) && !p.IsInterface && p != type).ToList();
            //    foreach (var customBaseBs in typeClass)
            //    {
            //        var transactionNameAttribute = customBaseBs.GetCustomAttribute<TransactionName>();
            //        services.Add(new ServiceDescriptor(item, typeClass, ServiceLifetime.Transient));
            //        _container.Register(Component.For(item).ImplementedBy(customBaseBs).Named(transactionNameAttribute.Name).LifestyleTransient());
            //    }
            //}

            //#endregion

            return services;
        }
    }
}
