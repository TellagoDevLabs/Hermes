using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace ExampleDecouplingApplications
{
    class Repository
    {
        static public Repository Instance = new Repository();

        const string FileName = "data.xml";

        public List<Movement> Movements { get; private set; }
        public Account[] Accounts { get; private set; }

        private Repository()
        {
            Load();
        }

        public void Load()
        {
            Accounts = new Account[] 
            {
                new Account() {Id = 1, Name = "Jhon"},
                new Account() {Id = 2, Name = "Chris"},
                new Account() {Id = 3, Name = "Sam"}
            };
            
            Movements = new List<Movement>();


            try
            {
                if (File.Exists(FileName))
                {
                    using (var file = File.OpenText(FileName))
                    {
                        var serializer = new XmlSerializer(typeof(List<Movement>));
                        Movements = (List<Movement>) serializer.Deserialize(file);
                    }
                }
            }
            catch
            {
            }
        }

        public void Save()
        {
            using (var file = File.CreateText(FileName))
            {
                var serializer = new XmlSerializer(typeof(List<Movement>));
                serializer.Serialize(file, Movements);
            }
        }
    }
}