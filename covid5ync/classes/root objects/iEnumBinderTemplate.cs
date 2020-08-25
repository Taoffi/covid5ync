using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.ComponentModel;

namespace isosoft.root
{
	public class iEnumBinderTemplate<T> where T: Enum
	{
        T               _enumObject;

        public string   Name 
        {
            get { return _enumObject == null ? "" : _enumObject.EnumDescription(); }
        }

        public T Value
        {
            get {  return _enumObject; }
            set {  _enumObject = value; }
        }

        public iEnumBinderTemplate(T obj)
        {
            _enumObject = obj;
        }

        public Dictionary<string, T> EnumList
        {
            get
            {
                Dictionary<string, T> dico = new Dictionary<string, T>();

                var     values   = Enum.GetValues(typeof(T));

                foreach(var v in values)
                {
                    string      name        = ((T)v).EnumDescription();
                    //int         itemValue   = (int) v;

                    dico.Add(name, (T) v);
                }

                return dico;
            }
        }
    }


	public static class EnumBindingHelpers
	{
        public static string EnumDescription(this Enum value)
        {
            if(value == null || !(value is Enum))
                return null;
 
            var             type        = value.GetType();
            Type            typeInfo    = type.GetTypeInfo();
            string          typeName    = Enum.GetName(type, value);
 
            if (string.IsNullOrEmpty(typeName))
                return null;
 
            var             field       = typeInfo.GetFields().FirstOrDefault(f => f.Name == typeName);
 
            if(field == null)
                return typeName;
 
            var         attrib  = field.GetCustomAttribute<DescriptionAttribute>();
            return      attrib == null ? typeName : attrib.Description;
        }
	}
}
