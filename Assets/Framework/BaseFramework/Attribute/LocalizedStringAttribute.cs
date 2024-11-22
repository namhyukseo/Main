using System;
using System.Collections.Generic;
using System.Reflection;
using Framework.Singleton;

namespace Framework
{
    public class LocalizedStringAttribute : System.Attribute
    {
        public string Text { get; set; }
    }

    public class LocalizeStringTable<T> : Singleton<LocalizeStringTable<T>>
    {
        protected   Dictionary<T, string>   stringTable = null;

        protected override void OnInit()
        {
            base.OnInit();

            stringTable = new Dictionary<T, string>();
            
            Type _type = typeof(T);

            foreach(T iter in Enum.GetValues(_type))
            {
                var _attribute = _type.GetField(iter.ToString()).GetCustomAttribute<LocalizedStringAttribute>(false);
                stringTable.Add((T)iter, _attribute.Text);
            }
        }

        protected override void OnRelease()
        {
            base.OnRelease();

            stringTable.Clear();
        }

        public string ToLoclizedString(T _id)
        {
            string _loclizedString = string.Empty;
            stringTable.TryGetValue(_id, out _loclizedString);

            return _loclizedString;
        }
    }
}