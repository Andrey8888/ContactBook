using System;
using System.Collections.Generic;
using System.Text;

namespace HelloApp
{
    public interface IFileService
    {
        List<Contact> Open(string filename);
        void Save(string filename, List<Contact> contactsList);
    }
}
