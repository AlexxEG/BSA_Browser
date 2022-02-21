using System;
using System.Security.Principal;
using System.Text;
using System.Windows.Forms;
using Microsoft.Win32;

namespace BSA_Browser.Classes
{
    public static class FileAssociation
    {
        private const string BSABrowserKey = "BSABrowser";

        private const string BSAKey = ".bsa";
        private const string BA2Key = ".ba2";

        private const string IntegrationHereKey = BSABrowserKey + @"\shell\Extract\shell\1here";
        private const string IntegrationHereCommandKey = IntegrationHereKey + @"\command";

        private const string IntegrationDirectoryKey = BSABrowserKey + @"\shell\Extract\shell\2directory";
        private const string IntegrationDirectoryCommandKey = IntegrationDirectoryKey + @"\command";

        private const string DefaultIcon = BSABrowserKey + @"\DefaultIcon";
        private const string ShellOpenCommandKey = BSABrowserKey + @"\shell\open\command";
        private const string ShellExtractKey = BSABrowserKey + @"\shell\Extract";

        public static bool HasAdminPrivileges()
        {
            using (var identity = WindowsIdentity.GetCurrent())
            {
                var principal = new WindowsPrincipal(identity);
                return principal.IsInRole(WindowsBuiltInRole.Administrator);
            }
        }

        public static bool GetFileAssociationEnabled()
        {
            RegistryKey key;

            if ((key = Registry.ClassesRoot.OpenSubKey(BSAKey)) == null)
                return false;

            if ((string)key.GetValue(string.Empty) != BSABrowserKey)
                return false;

            if ((key = Registry.ClassesRoot.OpenSubKey(BA2Key)) == null)
                return false;

            if ((string)key.GetValue(string.Empty) != BSABrowserKey)
                return false;

            if (Registry.ClassesRoot.OpenSubKey(ShellOpenCommandKey) == null)
                return false;

            return true;
        }

        public static bool GetShellIntegrationEnabled()
        {
            if (Registry.ClassesRoot.OpenSubKey(IntegrationHereCommandKey) == null)
                return false;

            if (Registry.ClassesRoot.OpenSubKey(IntegrationDirectoryCommandKey) == null)
                return false;

            return true;
        }

        /// <summary>
        /// Toggles file association and/or shell integration based on <see cref="args"/>. Returns true if something was toggled.
        /// </summary>
        public static bool ToggleAssociationAndIntegration(string[] args)
        {
            if (args.Length == 0)
                return false;

            ParseArgs(args,
                out bool association,
                out bool associationDisable,
                out bool integration,
                out bool integrationDisable);

            return ToggleAssociationAndIntegration(association, associationDisable, integration, integrationDisable);
        }

        /// <summary>
        /// Toggles file association and/or shell integration. Returns true if something was toggled.
        /// </summary>
        public static bool ToggleAssociationAndIntegration(bool association, bool associationDisable, bool integration, bool integrationDisable)
        {
            if (association || associationDisable || integration || integrationDisable)
            {
                if (!HasAdminPrivileges())
                {
                    MessageBox.Show("Changing file association and/or shell integration requires admin privileges.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return true;
                }
            }

            if (association)
            {
                Registry.ClassesRoot.CreateSubKey(BSABrowserKey)
                    .SetValueEnd(string.Empty, "Bethesda File Archive", RegistryValueKind.String);

                Registry.ClassesRoot.CreateSubKey(DefaultIcon)
                    .SetValueEnd(string.Empty, $"\"{Application.ExecutablePath}\",0", RegistryValueKind.String);

                Registry.ClassesRoot.CreateSubKey(ShellOpenCommandKey)
                    .SetValueEnd(string.Empty, $"\"{Application.ExecutablePath}\" \"%1\"", RegistryValueKind.String);

                // .bsa
                Registry.ClassesRoot.CreateSubKey(BSAKey, true)
                    .SetValueEnd(string.Empty, BSABrowserKey, RegistryValueKind.String);

                // .ba2
                Registry.ClassesRoot.CreateSubKey(BA2Key, true)
                    .SetValueEnd(string.Empty, BSABrowserKey, RegistryValueKind.String);
            }

            if (associationDisable)
            {
                Registry.ClassesRoot.DeleteSubKeyTree(BSABrowserKey, false);
                // Delete '.bsa' and '.ba2' if it's associated to this program
                Registry.ClassesRoot.DeleteSubKeyTreeIf(BSAKey, string.Empty, x => x?.ToString() == BSABrowserKey);
                Registry.ClassesRoot.DeleteSubKeyTreeIf(BA2Key, string.Empty, x => x?.ToString() == BSABrowserKey);
            }

            if (integration)
            {
                Registry.ClassesRoot.CreateSubKey(ShellExtractKey)
                    .SetValueAnd("Icon", $"\"{Application.ExecutablePath}\",0", RegistryValueKind.String)
                    .SetValueAnd("MUIVerb", "BSA Browser", RegistryValueKind.String)
                    .SetValueEnd("subcommands", string.Empty, RegistryValueKind.String);

                Registry.ClassesRoot.CreateSubKey(IntegrationHereKey)
                    .SetValueAnd(string.Empty, "Extract here", RegistryValueKind.String)
                    .SetValueEnd("Icon", $"\"{Application.ExecutablePath}\",0", RegistryValueKind.String);

                Registry.ClassesRoot.CreateSubKey(IntegrationDirectoryKey)
                    .SetValueAnd(string.Empty, "Extract to directory", RegistryValueKind.String)
                    .SetValueEnd("Icon", $"\"{Application.ExecutablePath}\",0", RegistryValueKind.String);

                Registry.ClassesRoot.CreateSubKey(IntegrationHereCommandKey)
                    .SetValueEnd(string.Empty, $"\"{Application.ExecutablePath}\" /h /extract \"%1\"", RegistryValueKind.String);

                Registry.ClassesRoot.CreateSubKey(IntegrationDirectoryCommandKey)
                    .SetValueEnd(string.Empty, $"\"{Application.ExecutablePath}\" /d /extract \"%1\"", RegistryValueKind.String);
            }

            if (integrationDisable)
            {
                Registry.ClassesRoot.DeleteSubKeyTree(ShellExtractKey, false);
            }

            return association || associationDisable || integration || integrationDisable;
        }

        private static void ParseArgs(string[] args, out bool association, out bool associationDisable, out bool integration, out bool integrationDisable)
        {
            association = associationDisable = integration = integrationDisable = false;

            foreach (string arg in args)
            {
                switch (arg.ToLower())
                {
                    case "--associate":
                        association = true;
                        break;
                    case "--associate-disable":
                        associationDisable = true;
                        break;
                    case "--integration":
                        integration = true;
                        break;
                    case "--integration-disable":
                        integrationDisable = true;
                        break;
                }
            }
        }

        private static RegistryKey SetValueAnd(this RegistryKey key, string name, object value, RegistryValueKind valueKind)
        {
            key.SetValue(name, value, valueKind);
            return key;
        }

        private static void SetValueEnd(this RegistryKey key, string name, object value, RegistryValueKind valueKind)
        {
            key.SetValue(name, value, valueKind);
            key.Dispose();
        }

        private static bool DeleteSubKeyTreeIf(this RegistryKey key, string name, string valueName, Predicate<object> predicate, bool throwOnMissingSubKey = false)
        {
            var valueKey = key.OpenSubKey(name);

            // Doesn't exist
            if (valueKey == null)
                return false;

            object value = valueKey.GetValue(valueName);
            valueKey.Close();

            bool result = predicate(value);

            if (result)
                key.DeleteSubKeyTree(name, throwOnMissingSubKey);

            return result;
        }
    }
}
