using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace ConfigurationUtils
{
    /// <summary>
    /// Set of access methods to the configuration values
    /// </summary>
    public class ApplicationConfiguration
    {
        /// <summary>
        /// Checks to see if config exist at path provided
        /// </summary>
        /// <param name="configPath">Path to configuration folder</param>
        /// <param name="configName">Optional filename.  Default is the servicename</param>
        /// <returns></returns>
        public static bool DoesConfigExist(string configPath, [Optional, DefaultParameterValue("")] string configName)
        {
            GeneralUtils.MethodUtils.NotNullOrEmpty(configPath, "configPath");
            string path = Path.Combine(configPath, ConfigName(configName));
            return File.Exists(path);
        }

        /// <summary>
        /// Build the config which will include the machinename and will be in the format of hostmachine.ProcessName.ini
        /// ProcessName.ini can be overwritten with the configName parameter
        /// </summary>
        /// <param name="configName">overrides the ProcessName.ini with the configName parameter</param>
        /// <returns></returns>
        public static string ConfigName([Optional, DefaultParameterValue("")] string configName)
        {
            string filename = (!string.IsNullOrWhiteSpace(configName)) ? configName
                : (System.Diagnostics.Process.GetCurrentProcess().ProcessName + ".ini");
            return System.Environment.MachineName + "." + filename;
        }

        public Dictionary<string, string> Setting { get { return setting; } }

        public string this[Enum field] { get { return ValueSafe(Key(field)); } }

        public string this[string field] { get { return ValueSafe(field); } }

        public int Int(Enum field, [Optional, DefaultParameterValue(0)] int defaultValue)
        {
            GeneralUtils.MethodUtils.NotNull(field, "Enumeration value cannot be null");
            return Int(Key(field), defaultValue);
        }

        public int Int(string key, [Optional, DefaultParameterValue(0)] int defaultValue)
        {
            GeneralUtils.MethodUtils.NotNull(key, "Key value cannot be null");
            return GetIntValue(ValueSafe(key), defaultValue);
        }

        public bool Bool(Enum field, [Optional, DefaultParameterValue(false)] bool defaultValue)
        {
            GeneralUtils.MethodUtils.NotNull(field, "field value cannot be null");
            return Bool(Key(field), defaultValue);
        }

        public bool Bool(string key, [Optional, DefaultParameterValue(false)] bool defaultValue)
        {
            GeneralUtils.MethodUtils.NotNull(key, "Key value cannot be null");
            return GetBoolValue(ValueSafe(key), defaultValue);
        }

        public string Source { get { return source; } }

        public string ServiceName { get { return serviceName; } }

        public string UserName { get { return userName; } }

        public string ConfigurationFolder { get { return configurationFolder; } }


        public string ConfigFilename()
        {
            // properties are stored hostmachine.applicationname.ini
            return configurationFileInfo.FullName;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            foreach (string key in setting.Keys)
            {
                // all folder configuration keys must exit
                if (key.ToLower().Contains("password"))
                {
                    continue;
                }
                // all folder configuration keys must exit
                sb.Append(key + "=");
                sb.Append(this[key] + Environment.NewLine);
            }

            return sb.ToString();
        }

        /// <summary>
        /// Gets the value from the config, will never throw an execption or return null;
        /// </summary>
        /// <param name="key"></param>
        /// <returns>string, or empty string</returns>
        public string ValueSafe(string key, [Optional, DefaultParameterValue("")] string defaultValue)
        {
            GeneralUtils.MethodUtils.NotNull(key, "Key value cannot be null");
            string value = "";
            try
            {
                value = Setting[key];
            }
            catch (Exception) { }
            return (string.IsNullOrEmpty(value)) ? defaultValue : value;
        }

        private static bool GetBoolValue(string boolValue, bool defaultValue)
        {
            bool value = defaultValue;
            if (!string.IsNullOrEmpty(boolValue))
            {
                bool parsed;
                if (Boolean.TryParse(boolValue, out parsed))
                {
                    value = parsed;
                }
            }
            return value;
        }

        private static int GetIntValue(string intValue, int defaultValue)
        {
            int result = defaultValue;
            if (!string.IsNullOrEmpty(intValue))
                result = Int32.Parse(intValue);

            return result;
        }

        private string Key(Enum field)
        {
            return field.GetType().Name + "." + field.ToString();
        }

        private static void WindowsEventError(string serviceName, params object[] message)
        {
            try
            {
                // post to Windows event log
                using (EventLog eventLog = new EventLog("Application"))
                {
                    eventLog.Source = serviceName;
                    eventLog.WriteEvent(new EventInstance(1, 0, EventLogEntryType.Error), message);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }
        }


        internal static ApplicationConfiguration CreateConfiguration(string serviceName,
            string configurationFolder, string configurationFilename,
            List<ConfigurationField> configurationFields)
        {
            //----------------------------------------------------------------
            // parameter checking
            GeneralUtils.MethodUtils.NotNull(configurationFolder, "configurationFolder");
            GeneralUtils.MethodUtils.NotNull(serviceName, "serviceName");


            string configurationFile = Path.Combine(configurationFolder, ConfigName(configurationFilename));
            if (!File.Exists(configurationFile))
            {
                string message = "Cannot initilize configuration due to missing configuration file.  "
                    + "The expected configuration file for the machine is " + configurationFile + " does not exist";
                WindowsEventError(serviceName,message);
                throw new ArgumentException(message, "configuration file");
            }

            FileInfo configurationFileInfo = new FileInfo(configurationFile);
            if (!configurationFileInfo.Exists)
            {
                throw new ArgumentException("Configuration cannot be initilized because "
                    + configurationFileInfo.FullName + " does not exist", "configuration");
            }

            //----------------------------------------------------------------
            // read the configuration file
            string error = null;
            List<string> errors = new List<string>();

            Dictionary<string, string> configDictionary = new Dictionary<string, string>();
            IEnumerable<string> lines = File.ReadLines(configurationFileInfo.FullName);
            foreach (string line in lines)
            {
                if (line != null)
                {
                    string toParse = line;
                    if (toParse.Contains(";"))
                    {
                        string[] comment = toParse.Trim().Split(';');
                        if (comment != null && comment.Length > 1)
                        {
                            toParse = comment[0];
                        }
                    }

                    string[] str = toParse.Split('=');
                    if (str != null && str.Length == 2)
                    {
                        string key = str[0].Trim();
                        if (!configDictionary.ContainsKey(key))
                        {
                            configDictionary.Add(str[0].Trim(), str[1].Trim());
                            continue;
                        }
                        error = string.Format("Duplicate keys={0}.  Please verify that only 1 line has a key={0}", key);
                        System.Diagnostics.Trace.WriteLine(error);
                        errors.Add(error);
                    }
                }
            }

            //----------------------------------------------------------------
            // Verify configuration values are correct
            foreach (ConfigurationField field in configurationFields)
            {
                string value = "";
                try
                {
                    value = configDictionary[field.ToString()];
                }
                catch (Exception) { }
                value = string.IsNullOrEmpty(value) ? "" : value;

                if (string.IsNullOrWhiteSpace(value))
                {
                    if (Condition.Required.Equals(field.condition))
                    {
                        error = string.Format("Missing configuration value for key={0}", field.fieldName);
                        errors.Add(error);
                        System.Diagnostics.Trace.WriteLine(error);
                    }
                    continue;
                }

                if (ParamType.Email.Equals(field.type))
                {
                    if (!GeneralUtils.SmtpUtil.ValidateAddress(value))
                    {
                        error = string.Format("Email format is invalid for configuration key={0}, value={1}",
                            field.ToString(), value);
                        System.Diagnostics.Trace.WriteLine(error);
                        errors.Add(error);
                        continue;
                    }
                }
                else if (ParamType.File.Equals(field.type))
                {
                    if (!configDictionary.ContainsKey(field.fieldName))
                    {
                        error = string.Format("Missing configuration value for key={0}", field.ToString());
                        System.Diagnostics.Trace.WriteLine(error);
                        continue;
                    }
                    if (!File.Exists(value))
                    {
                        error = string.Format("File does not exist.  Invalid configuration value for key={0}, value={1}",
                            field.ToString(), value);
                        System.Diagnostics.Trace.WriteLine(error);
                        errors.Add(error);
                        continue;
                    }
                }
                else if (ParamType.Folder.Equals(field.type))
                {
                    if (!configDictionary.ContainsKey(field.fieldName))
                    {
                        error = string.Format("Missing configuration value for key={0}", field.ToString());
                        System.Diagnostics.Trace.WriteLine(error);
                        continue;
                    }

                    if (!Directory.Exists(value))
                    {
                        error = string.Format("Dirctory does not exist.  Invalid configuration value for key={0}, value={1}",
                            field.ToString(), value);
                        System.Diagnostics.Trace.WriteLine(error);
                        errors.Add(error);
                        continue;
                    }

                    if (! GeneralUtils.DirectoryUtils.CheckReadPermissionOnDir(value))
                    {
                        error = string.Format("Unable to read directory.  Configuration value for key={0}, value={1}",
                            field.ToString(), value);
                        System.Diagnostics.Trace.WriteLine(error);
                        errors.Add(error);
                    }

                    if (! GeneralUtils.DirectoryUtils.CheckWritePermissionOnDir(value))
                    {
                        error = string.Format("Unable to write directory.  Configuration value for key={0}, value={1}",
                            field.ToString(), value);
                        System.Diagnostics.Trace.WriteLine(error);
                        errors.Add(error);
                    }
                }
                else if (ParamType.Boolean.Equals(field.type))
                {

                    if (!GeneralUtils.StringUtils.IsBool(value))
                    {
                        error = string.Format("Expected a boolean. key={0}, value={1}", field.ToString(), value);
                        System.Diagnostics.Trace.WriteLine(error);
                        errors.Add(error);
                    }
                }
                else if (ParamType.Number.Equals(field.type))
                {
                    int intValue;
                    if (!int.TryParse(value, out intValue))
                    {
                        error = string.Format("Expected a number. key={0}, value={1}", field.ToString(), value);
                        System.Diagnostics.Trace.WriteLine(error);
                        errors.Add(error);
                    }
                }
            }

            if (errors.Count > 0)
            {
                string errorResults = String.Join(("," + Environment.NewLine), errors.ToArray());
                string errorMessage =
                    serviceName + " configuration contains invalid configuration value(s). " + Environment.NewLine + errorResults;

                throw new ArgumentException(errorMessage);
            }

            return new ApplicationConfiguration(serviceName,
                configurationFolder, configurationFileInfo, configDictionary);
        }


        private ApplicationConfiguration(String serviceName, String configurationFolder,
            FileInfo file, Dictionary<string, string> dictionary)
        {
            this.serviceName = serviceName;
            this.configurationFolder = configurationFolder;
            configurationFileInfo = file;
            setting = dictionary;
            userName = System.Security.Principal.WindowsIdentity.GetCurrent().Name;

            System.Net.IPHostEntry host = System.Net.Dns.GetHostEntry(Environment.MachineName);
            source = (host != null) ? host.HostName.ToUpper() : Environment.MachineName;
        }

        private ApplicationConfiguration()
        {
        }

        private readonly Dictionary<string, string> setting;

        private readonly FileInfo configurationFileInfo;

        private readonly string userName;

        private readonly string source;

        private string serviceName;

        private string configurationFolder;        
    }
}
