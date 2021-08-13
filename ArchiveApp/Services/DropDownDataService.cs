using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using ArchiveApp.Resources.Components;

namespace ArchiveApp.Services
{
    public class DropDownDataService
    {
        private readonly XmlFileService fileService;

        private Root root;

        private Dictionary<string, RootUnitsItem[]> units;

        public bool HasChanges { get; private set; }

        public DropDownDataService(XmlFileService fileService)
        {
            this.fileService = fileService;
        }

        public void ReloadData()
        {            
            root = fileService.Deserialize<Root>();
            units = root.Units.ToDictionary(x => x.Name, y => y.Item);
        }

        public RootUnitsItem[] GetUnits(string name)
        {
            if(units != null && units.ContainsKey(name))
            {
                return units[name];
            }
            throw new ArgumentException();
        }

        private Dictionary<string, List<RootUnitsItem>> newItems = new Dictionary<string, List<RootUnitsItem>>();

        internal void AddItem(string name, string nationalityText)
        {

            if (units.ContainsKey(name))
            {
                byte key = (byte)(units[name].Max(x => x.Key) + 1);
                var item = new RootUnitsItem() { Key = key, Value = nationalityText };

                if (!newItems.ContainsKey(name))
                {
                    newItems.Add(name, new List<RootUnitsItem>());
                }
                newItems[name].Add(item);
                HasChanges = true;
            }
        }

        internal void SaveChanges()
        {

            foreach(var toAdd in newItems)
            {
                var oldArr = units[toAdd.Key];

                var newArr = new RootUnitsItem[oldArr.Length + toAdd.Value.Count];

                oldArr.CopyTo(newArr, 0);
                int i = oldArr.Length;

                foreach(var newItem in toAdd.Value)
                {
                    newArr[i] = newItem;
                    i++;
                }
                
                units[toAdd.Key] = newArr;

                for (int j = 0; j < root.Units.Length; j++)
                {
                    if(root.Units[j].Name == toAdd.Key)
                    {
                        root.Units[j].Item = newArr;
                    }
                }
            }
            newItems.Clear();
            HasChanges = false;

            fileService.Serialize(root);

            //using (StreamWriter stream = new StreamWriter(XmlFileService.FILE_NAME_XML, false))
            //{

            //    XDocument doc = XDocument.Load(stream);

            //    foreach (var unit in doc.Root.Elements("Units"))
            //    {
            //        string name = unit.Attribute("name").Value;

            //        if (newItems.ContainsKey(name))
            //        {
            //            var toAddCollection = newItems[name];
            //            foreach (var toAdd in toAddCollection)
            //            {
            //                unit.Add(new XElement("Item", new XAttribute("key", toAdd.Key), new XAttribute("value", toAdd.Value)));
            //            }
            //        }
            //    }



            //}
            //ReloadData();
        }
    }
}
