using StructureMap;
using System.Configuration;

namespace Temple.UI.Console
{
    internal class InstanceScanner : Registry
    {
        public InstanceScanner()
        {
            var configFile = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var settings = configFile.AppSettings.Settings;
            var repositoryPluginAssembly = settings["RepositoryPluginAssembly"]?.Value;

            Scan(_ =>
            {
                _.WithDefaultConventions();
                _.AssembliesFromApplicationBaseDirectory(d => d.FullName.StartsWith("Craft.Logging"));
                _.AssembliesFromApplicationBaseDirectory(d => d.FullName.StartsWith("Temple.Domain"));
                _.AssembliesFromApplicationBaseDirectory(d => d.FullName.StartsWith("Temple.IO"));
                _.Assembly(repositoryPluginAssembly);
                _.LookForRegistries();
            });
        }
    }
}
