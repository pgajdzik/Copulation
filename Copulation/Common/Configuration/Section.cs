using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Xml;
using System.Collections;

namespace Copulation.Common.Configuration
{
    public class Section 
    {
        private string _name;

        private IDictionary<string, object> items = new Dictionary<string, object>();

        protected object this[string uniqueKey]
        {
            get 
            {
                object value = null;
                return this.items.TryGetValue(uniqueKey, out value)
                    ? value
                    : null;
            }
            
            set 
            {
                if (value == null)
                    return;

                this.items[uniqueKey] = value; 
            }
        }

        internal string Name
        {
            get { return this._name ?? GetType().Name; }
            set { this._name = value; }
        }

        public T GetSection<T>(string uniqueName) where T : Section 
        {
            if (this.items.ContainsKey(uniqueName))
                return (T)this.items[uniqueName];

            T section = Activator.CreateInstance<T>();

            section.Name = uniqueName;
            this.items.Add(uniqueName, section);

            return section;
        }

        public SectionCollection<T> GetSectionCollection<T>(string uniqueName) where T : Section
        {
            return GetSection<SectionCollection<T>>(uniqueName);
        }

        public Section Defaults()
        {
            var cfgProperties = GetConfigProperties(GetType());

            foreach (var property in cfgProperties)
            {
                if (property.PropertyType.IsSubclassOf(typeof(Section)))
                {
                    property.GetValue(this, null);
                }
                else if (property.PropertyType.IsArray)
                {
                    property.SetValue(this, Activator.CreateInstance(property.PropertyType, 0), null);
                }
                else
                {
                    property.SetValue(this, GetDefaultValue(property), null);
                }
            }

            foreach (var section in this.items.Where(i => i.Value is Section))
            {
                Section nestedSection = (Section)section.Value;

                nestedSection.Defaults();
            }

            return this;
        }

        public void Update()
        {
            var cfgProperties = GetConfigProperties(GetType())
                .Where(cp => !cp.PropertyType.IsSubclassOf(typeof(Section)));

            foreach (var property in cfgProperties)
            {
                if (property.PropertyType.IsArray)
                {
                    this[property.Name] = property.GetValue(this, null);
                    
                    if (property.PropertyType.GetElementType().IsSubclassOf(typeof(Section)))
                    {
                        foreach (Section s in (IEnumerable)this[property.Name])
                        {

                            s.Update();
                        }
                    }
                }
                else
                {
                    this[property.Name] = property.GetValue(this, null) ?? GetDefaultValue(property);
                }
            }

            foreach (var section in this.items.Where(i => i.Value is Section))
            {
                ((Section)section.Value).Update();
            }
        }

        public virtual void UpdateFrom(XmlReader reader)
        {
            var cfgProperties = GetConfigProperties(GetType());

            while (reader.Read())
            {
                reader.MoveToElement();

                var name = reader.Name;
                var property = cfgProperties
                    .FirstOrDefault(cp => string.Compare(cp.Name, name) == 0);

                if (property == null)
                    continue;

                Read(reader, property);

            }
        }

        private Section GetSection(string uniqueName, Type typeOfSection)
        {
            if (this.items.ContainsKey(uniqueName))
                return (Section)this.items[uniqueName];

            Section section = (Section)Activator.CreateInstance(typeOfSection);

            section.Name = uniqueName;
            this.items.Add(uniqueName, section);

            return section;
        }

        private void Read(XmlReader reader, PropertyInfo property)
        {
            if (IsSection(property))
            {
                GetInstanceOfSection(property).UpdateFrom(reader.ReadSubtree());
            }
            else if (IsArray(property))
            {
                property.SetValue(this, ReadArray(property, reader.ReadSubtree()), null);
            }
            else
            {
                object value = null;

                try
                {
                    value = GetValueAs(property, reader.ReadElementString());
                }
                catch
                {
                    value = GetDefaultValue(property);
                }

                property.SetValue(this, value, null);
            }
        }

        private static bool IsArray(PropertyInfo property)
        {
            return property.PropertyType.IsArray;
        }

        private static bool IsSection(PropertyInfo property)
        {
            return property.PropertyType.IsSubclassOf(typeof(Section));
        }

        private Section GetInstanceOfSection(PropertyInfo property)
        {
            return ((Section)property.GetValue(this, null));
        }

        private object ReadArray(PropertyInfo property, XmlReader reader)
        {
            var items = new List<object>();
            var isSectionItem = false;
            var itemName = "Item";

            if (property.PropertyType.GetElementType().IsSubclassOf(typeof(Section)))
            {
                isSectionItem = true;
                itemName = property.PropertyType.GetElementType().Name;
            }    

            while (reader.Read())
            {
                reader.MoveToContent();

                if (string.Compare(reader.Name, itemName) != 0)
                    continue;

                if (isSectionItem)
                {
                    var section = GetSection(itemName + items.Count.ToString(), property.PropertyType.GetElementType());
                    var sectionReader = reader.ReadSubtree();
                    
                    section.UpdateFrom(sectionReader);
                    
                    items.Add(section); 

                }
                else
                {
                    var content = reader.ReadElementString();
                
                    items.Add(GetValueAs(property, content));
                }
            }

            var typedItems = Activator.CreateInstance(property.PropertyType, items.Count);

            Array.Copy(items.ToArray(), (Array)typedItems, items.Count);
            return typedItems;
        }

        internal protected virtual void RenderXml(XmlWriter writer)
        {
            RenderStartXml(writer);

            foreach (var item in this.items)
            {
                var section = item.Value as Section;

                if (section != null)
                {
                    section.RenderXml(writer);
                }
                else
                {
                    RenderItemXml(writer, item.Key, item.Value);
                }
            }

            RenderEndXml(writer);
        }

        private void RenderItemXml(XmlWriter writer, string name, object item)
        {
            writer.WriteStartElement(name);
            
            if (item.GetType().IsArray)
            {
                foreach (var e in (IEnumerable)item)
                {
                    if (item.GetType().GetElementType().IsSubclassOf(typeof(Section)))
                    {
                        var section = item as Section;

                        section.RenderXml(writer);
                    }
                    else
                    {
                        RenderItemXml(writer, "Item", e);
                    }
                }
            }
            else
            {
                writer.WriteString(Convert.ToString(item));
            }

            writer.WriteEndElement();
        }

        private static IEnumerable<PropertyInfo> GetConfigProperties(Type typeOfThis)
        {
            var properties = typeOfThis.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            var cfgProperties = properties
                .Where(p => p.GetCustomAttributes(typeof(ConfigPropertyAttribute), false).Length != 0);
            return cfgProperties;
        }

        private object GetDefaultValue(PropertyInfo property)
        {
            var configPropertyAttrib = (ConfigPropertyAttribute)property
                .GetCustomAttributes(typeof(ConfigPropertyAttribute), false)
                .First();

            return GetValueAs(property, configPropertyAttrib.DefaultValue);
        }

        private static object GetValueAs(PropertyInfo property, string value)
        {
            if (string.IsNullOrEmpty(value))
                return null;
            
            Type destType = property.PropertyType;
            
            if (property.PropertyType.IsArray)
                destType = property.PropertyType.GetElementType();

            return Convert.ChangeType(value, destType);
        }
        
        protected void RenderStartXml(XmlWriter writer)
        {
            writer.WriteStartElement(Name);
        }
   
        protected void RenderEndXml(XmlWriter writer)
        {
            writer.WriteEndElement();
        }


        //#region Implementation of IEnumerator
        
        //IEnumerator<KeyValuePair<string, object>> IEnumerable<KeyValuePair<string, object>>.GetEnumerator()
        //{
        //    return items.GetEnumerator();
        //}

        //IEnumerator IEnumerable.GetEnumerator()
        //{
        //    return items.GetEnumerator();
        //} 

        //#endregion
    }

    public class SectionCollection<T> : Section, IEnumerable<T> where T : Section
    {
        private IList<T> _items = new List<T>();

        public T this[int index]
        {
            get { return _items[index]; }
        }

        public T Add()
        {
            var section = GetSection<T>("Item" + _items.Count.ToString());

            section.Name = typeof(T).Name;

            _items.Add(section);

            return section;
        }

        public T FindBy( Func<T, bool> predicate)
        {
            return _items.FirstOrDefault(predicate);
        }

        public void RemoveAt(int index)
        {
            _items.RemoveAt(index);
        }

        public bool RemoveBy(Func<T, bool> predicate)
        {
            var toRemove = _items.Where(predicate);

            foreach (var item in toRemove)
            {
                _items.Remove(item);
            }

            return toRemove.Count() != 0;
        }

        public void RemoveAll()
        {
            _items.Clear();
        }

        internal protected override void RenderXml(XmlWriter writer)
        {
            RenderStartXml(writer);

            foreach (Section section in _items)
            {
                section.RenderXml(writer);
            }
            
            RenderEndXml(writer);
        }

        public override void UpdateFrom(XmlReader reader)
        {
            while (reader.Read())
            {
                reader.MoveToContent();

                if (string.Compare(reader.Name, typeof(T).Name) != 0)
                    continue;

                Add().UpdateFrom(reader.ReadSubtree());
            }
        }

        #region Implementation of IEnumerable
        
        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return _items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _items.GetEnumerator();
        } 

        #endregion
    }
}
