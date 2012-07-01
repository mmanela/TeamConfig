using System.Collections.Generic;
using System.Dynamic;

namespace TeamConfig.Razor
{
    public class DynamicProperties : DynamicObject
    {
        readonly Dictionary<string,object> bag = new Dictionary<string, object>();

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            var key = binder.Name;
            if (bag.ContainsKey(key))
            {
                result = bag[key];
                return true;
            }

            throw new TemplateFileException(string.Format("No configuration variable exists for '{0}'", key));
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            bag[binder.Name] = value;
            return true;
        }

        public void Add(string name, object value)
        {
            bag[name] = value;
        }
    }
}