using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Json;

namespace HelloApp
{
    public class JsonFileService : IFileService
    {
        public List<Contact> Open(string filename)
        {
            List<Contact> contacts = new List<Contact>();
            DataContractJsonSerializer jsonFormatter =
                new DataContractJsonSerializer(typeof(List<Contact>));
            using (FileStream fs = new FileStream(filename, FileMode.OpenOrCreate))
            {
                contacts = jsonFormatter.ReadObject(fs) as List<Contact>;
            }

            return contacts;
        }

        public void Save(string filename, List<Contact> contactsList)
        {
            DataContractJsonSerializer jsonFormatter =
                new DataContractJsonSerializer(typeof(List<Contact>));
            using (FileStream fs = new FileStream(filename, FileMode.Create))
            {
                jsonFormatter.WriteObject(fs, contactsList);
            }
        }
    }
}
