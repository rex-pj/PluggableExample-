﻿using Microsoft.Extensions.DependencyInjection;
using Plugable.Core.Contracts;
using Plugable.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Plugable.Core.Insfrastructure
{
    public static class ServiceCollectionExtensions
    {
        public static void AddPlugins(this IServiceCollection services, IList<PluginInfo> plugins)
        {
            var mvcBuilder = services.AddMvc();
            mvcBuilder.AddRazorOptions(o =>
            {
            });

            foreach (var module in plugins)
            {
                mvcBuilder.AddApplicationPart(module.Assembly);

                var pluginStartupType = module.Assembly.GetTypes().Where(x => typeof(IPluginStartup).IsAssignableFrom(x))
                    .FirstOrDefault();

                if (pluginStartupType != null && pluginStartupType != typeof(IPluginStartup))
                {
                    var moduleInitializer = (IPluginStartup)Activator.CreateInstance(pluginStartupType);
                    moduleInitializer.ConfigureServices(services);
                }
            }
        }
    }
}