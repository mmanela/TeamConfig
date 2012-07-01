using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Management.Automation;
using System.Management.Automation.Provider;
using System.Linq;

namespace TeamConfig.PowerShellConfig
{
    [CmdletProvider("TeamConfig", ProviderCapabilities.ShouldProcess)]
    public class TeamConfigProvider : ContainerCmdletProvider, IContentCmdletProvider
    {
        public const string ProviderName = "TeamConfig";
        public static Dictionary<string, object> ConfigValues { get; private set; }

        static TeamConfigProvider()
        {
            ConfigValues = new Dictionary<string, object>();
        }

        protected override Collection<PSDriveInfo> InitializeDefaultDrives()
        {
            return new Collection<PSDriveInfo>()
                           {
                               new PSDriveInfo("Config", this.ProviderInfo, string.Empty, string.Empty, (PSCredential) null)
                           };

        }

        protected override bool ConvertPath(string path, string filter, ref string updatedPath, ref string updatedFilter)
        {
            return base.ConvertPath(path, filter, ref updatedPath, ref updatedFilter);
        }

        protected override void GetChildItems(string path, bool recurse)
        {
            base.GetChildItems(path, recurse);
        }

        protected override void GetChildNames(string path, ReturnContainers returnContainers)
        {
            base.GetChildNames(path, returnContainers);
        }

        protected override void NewItem(string path, string itemTypeName, object newItemValue)
        {
            base.NewItem(path, itemTypeName, newItemValue);
        }

        protected override object NewItemDynamicParameters(string path, string itemTypeName, object newItemValue)
        {
            return base.NewItemDynamicParameters(path, itemTypeName, newItemValue);
        }


        protected override void InvokeDefaultAction(string path)
        {
        }

        protected override object InvokeDefaultActionDynamicParameters(string path)
        {
            return base.InvokeDefaultActionDynamicParameters(path);
        }

        protected override object ItemExistsDynamicParameters(string path)
        {
            return ConfigValues.ContainsKey(path);
        }

        protected override bool ItemExists(string path)
        {
            return true;
        }

        protected override void RemoveItem(string path, bool recurse)
        {
            ConfigValues.Remove(path);
        }

        protected override void ClearItem(string path)
        {
            ConfigValues.Remove(path);
        }

        protected override void SetItem(string path, object value)
        {
            ConfigValues[path] = value;
        }

        protected override object SetItemDynamicParameters(string path, object value)
        {
            ConfigValues[path] = value;
            return value;
        }

        protected override object GetItemDynamicParameters(string path)
        {
            var item = Get(path);
            WriteItemObject(item, path, false);
            return item;
        }

        protected override object GetChildItemsDynamicParameters(string path, bool recurse)
        {
            return base.GetChildItemsDynamicParameters(path, recurse);
        }

        protected override object GetChildNamesDynamicParameters(string path)
        {
            return base.GetChildNamesDynamicParameters(path);
        }

        protected override void GetItem(string name)
        {
            WriteItemObject(Get(name), name, false);
        }

        protected override bool IsValidPath(string path)
        {
            return true;
        }


        private object Get(string name)
        {
            return ConfigValues.ContainsKey(name) ? ConfigValues[name] : null;
        }

        public IContentReader GetContentReader(string path)
        {
            return new TeamConfigContentManager(path, ConfigValues);
        }

        public object GetContentReaderDynamicParameters(string path)
        {
            throw new NotImplementedException();
        }

        public IContentWriter GetContentWriter(string path)
        {
            return new TeamConfigContentManager(path, ConfigValues);
        }

        public object GetContentWriterDynamicParameters(string path)
        {
            throw new NotImplementedException();
        }

        public void ClearContent(string path)
        {
            ConfigValues[path] = null;
        }

        public object ClearContentDynamicParameters(string path)
        {
            throw new NotImplementedException();
        }


        public class TeamConfigContentManager : IContentReader, IContentWriter
        {
            private readonly string path;
            private readonly IDictionary<string, object> mapping;

            public TeamConfigContentManager(string path, IDictionary<string,object> mapping)
            {
                this.path = path;
                this.mapping = mapping;
            }

            public void Dispose()
            {
            }

            public IList Read(long readCount)
            {
                return new[] {Get(path)};
            }

            public IList Write(IList content)
            {
                ConvertToGenericTypes(content);

                if (content.Count == 1)
                {
                    mapping[path] = content[0];
                }
                else
                {
                    mapping[path] = content;
                }

                return content;
            }

            private static void ConvertToGenericTypes(IList content)
            {
                for (int i = 0; i < content.Count; i++)
                {
                    var item = content[i];

                    var hashTable = item as Hashtable;
                    if (hashTable != null)
                    {
                        content[i] = HashtableToDictionary(hashTable);
                    }
                }
            }

            void IContentWriter.Seek(long offset, SeekOrigin origin)
            {
            }

            void IContentWriter.Close()
            {
            }

            void IContentReader.Seek(long offset, SeekOrigin origin)
            {
            }

            void IContentReader.Close()
            {
            }

            private object Get(string name)
            {
                return mapping.ContainsKey(name) ? mapping[name] : null;
            }

            public static Dictionary<string, object> HashtableToDictionary(Hashtable table)
            {
                return table
                  .Cast<DictionaryEntry>()
                  .ToDictionary(kvp => (string)kvp.Key, kvp => kvp.Value);
            }
        }
    }
}