using System;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using Eg.FluentMappings.Mappings;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate.Bytecode;
using NHibernate.Cfg;

namespace Queries
{
    public class ConfigurationBuilder
    {
        const string SerializedCfg = "configuration.bin";

        public Configuration Configure()
        {
            Configuration cfg = LoadConfigurationFromFile();
            if (cfg == null)
            {
                cfg = Fluently.Configure()
                                    .Database(MsSqlConfiguration.MsSql2008.ConnectionString(
                                        connstr => connstr.FromConnectionStringWithKey("db"))
                                                                .AdoNetBatchSize(100))
                                    .ProxyFactoryFactory<DefaultProxyFactoryFactory>()
                                    .Mappings(mappings => mappings.FluentMappings.AddFromAssemblyOf<ProductMapping>())
                                    .BuildConfiguration();
                SaveConfigurationToFile(cfg);
            }
            return cfg;
        }

        private Configuration LoadConfigurationFromFile()
        {
            if (!IsConfigurationFileValid())
                return null;
            try
            {
                using (var file = File.Open(SerializedCfg, FileMode.Open))
                {
                    var bf = new BinaryFormatter();
                    return bf.Deserialize(file) as Configuration;
                }
            }
            catch (Exception)
            {
                // Something went wrong
                // Just build a new one
                return null;
            }
        }

        private bool IsConfigurationFileValid()
        {
            // If we don't have a cached config, 
            // force a new one to be built
            if (!File.Exists(SerializedCfg))
                return false;
            var configInfo = new FileInfo(SerializedCfg);
            var asm = Assembly.GetExecutingAssembly();
            if (asm.Location == null)
                return false;
            // If the assembly is newer, 
            // the serialized config is stale
            var asmInfo = new FileInfo(asm.Location);
            if (asmInfo.LastWriteTime > configInfo.LastWriteTime)
                return false;
            // If the app.config is newer, 
            // the serialized config is stale
            var appDomain = AppDomain.CurrentDomain;
            var appConfigPath = appDomain.SetupInformation.ConfigurationFile;
            var appConfigInfo = new FileInfo(appConfigPath);
            if (appConfigInfo.LastWriteTime > configInfo.LastWriteTime)
                return false;
            // It's still fresh
            return true;
        }

        private void SaveConfigurationToFile(Configuration cfg)
        {
            using (var file = File.Open(SerializedCfg, FileMode.Create))
            {
                var bf = new BinaryFormatter();
                bf.Serialize(file, cfg);
            }
        }
    }
}
