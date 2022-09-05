using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using GeneralUtils;
using System.Diagnostics;

namespace ConfigurationUtils
{
    public enum Condition { Optional, Required }

    public enum ParamType { Text, Number, Boolean, Folder, File, WebService, Email }

    /// <summary>
    /// ConfigurationRulesBuilder defines the values expected in the configuration file.  This feature allows developers to provide ways of verifying configuration files by admin personnel.
    /// </summary>
    public class ConfigurationRulesBuilder
    {
        /// <summary></summary>
        /// <param name="serviceName"></param>
        /// <returns></returns>
        public static ConfigurationRulesBuilder NewBuilder(string serviceName)
        {
            MethodUtils.NotNullOrEmpty(serviceName, "ServiceName", "Must provide the ServiceName of the configuration.");
            return new ConfigurationRulesBuilder(serviceName);
        }

        public ApplicationConfiguration Build()
        {
            MethodUtils.NotNullOrEmpty(serviceName, "ServiceName", "Must provide the ServiceName of the configuration.");
            MethodUtils.NotNullOrEmpty(configurationFolder, "configurationFolder", "Must provide the folder where the configuration exist.");

            return ApplicationConfiguration.CreateConfiguration(serviceName, configurationFolder,
                configurationFile, configurationFields);
        }

        /// <summary>
        /// Location of the configuration files.  The default is the present location of the configuration file.
        /// </summary>
        public ConfigurationRulesBuilder SetConfigurationFolder(string configurationFolder)
        {
            MethodUtils.NotNullOrEmpty(configurationFolder, "configurationFolder", "Must provide the folder where the configuration exist.");
            this.configurationFolder = configurationFolder;
            return this;
        }


        /// <summary>
        /// Location of the configuration files.  The default is the present location of the configuration file.
        /// </summary>
        public ConfigurationRulesBuilder SetConfigurationName(string configurationFile)
        {
            MethodUtils.NotNullOrEmpty(configurationFile, "configurationFile", "Must provide the file where the configuration exist.");
            this.configurationFile = configurationFile;
            return this;
        }

        public ConfigurationRulesBuilder AddField(string fieldName,
            [Optional, DefaultParameterValue(Condition.Optional)] Condition param,
            [Optional, DefaultParameterValue(ParamType.Text)] ParamType type)
        {
            MethodUtils.NotNull(fieldName, "fieldName");
            configurationFields.Add(new ConfigurationField(fieldName, param, type));
            return this;
        }

        public ConfigurationRulesBuilder AddField(Enum field,
            [Optional, DefaultParameterValue(Condition.Optional)] Condition param,
            [Optional, DefaultParameterValue(ParamType.Text)] ParamType type)
        {
            string fullname = field.GetType().Name + "." + field.ToString();
            AddField(fullname, param, type);
            return this;
        }

        public ConfigurationRulesBuilder(string serviceName)
        {
            this.serviceName = serviceName;

            // default config
            Assembly assembly = System.Reflection.Assembly.GetEntryAssembly();
            if (assembly != null && assembly.Location != null)
            {
                FileInfo info = new FileInfo(assembly.Location);
                configurationFolder = Path.Combine(info.Directory.ToString(), "config");
            }
        }

        private List<ConfigurationField> configurationFields = new List<ConfigurationField>();
        private string serviceName;
        private string configurationFolder;
        private string configurationFile;
    }

    class ConfigurationField
    {
        public string fieldName;
        public Condition condition;
        public ParamType type;

        internal ConfigurationField(string fieldName, Condition condition, ParamType type)
        {
            this.fieldName = fieldName;
            this.condition = condition;
            this.type = type;
        }

        public override string ToString()
        {
            return fieldName;
        }
    }
}
