using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web.Hosting;

namespace GF.UCenter.Common
{
    public static class CompositionContainerFactory
    {
        private const CompositionOptions ContainerOptions = CompositionOptions.IsThreadSafe | CompositionOptions.DisableSilentRejection;

        private static readonly AggregateCatalog Catalog = CreateAggregateCatalog();

        public static CompositionContainer Create(params ExportProvider[] overrideProviders)
        {
            CompositionContainer container = null;
            if (overrideProviders != null && overrideProviders.Length > 0)
            {
                container = new CompositionContainer(Catalog, ContainerOptions, overrideProviders);
            }
            else
            {
                container = new CompositionContainer(Catalog, ContainerOptions);
            }

            container.ComposeExportedValue<ExportProvider>(container);
            return container;
        }

        private static ApplicationCatalog CreateCatalog()
        {
            using (var assemblyCatalog = new AssemblyCatalog(Assembly.GetExecutingAssembly()))
            using (var container = new CompositionContainer(assemblyCatalog, ContainerOptions))
            {
                return container.GetExportedValue<ApplicationCatalog>();
            }
        }

        private static AggregateCatalog CreateAggregateCatalog()
        {
            var aggregateCatalog = new AggregateCatalog();
            foreach (var file in GetDirectoryAssemblies())
            {
                try
                {
                    var catalog = new AssemblyCatalog(file);
                    catalog.DisposeOnException(() =>
                    {
                        var assembly = catalog.Assembly;
                        if (catalog.Parts.Count() > 0)
                        {
                            aggregateCatalog.Catalogs.Add(catalog);
                        }
                    });
                }
                catch (Exception)
                {

                }
            }

            return aggregateCatalog;
        }

        private static IEnumerable<string> GetDirectoryAssemblies()
        {
            string directory;
            if (HostingEnvironment.IsHosted)
            {
                directory = HostingEnvironment.MapPath("~/bin");
            }
            else
            {
                directory = Path.GetDirectoryName(typeof(CompositionContainerFactory).Assembly.Location);
            }

            var dlls = Directory.EnumerateFiles(directory, "*.dll", SearchOption.AllDirectories);
            var exes = Directory.EnumerateFiles(directory, "*.exe", SearchOption.AllDirectories);

            return dlls.Concat(exes);
        }
    }
}
