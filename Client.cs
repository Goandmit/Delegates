using System;
using System.Collections.Generic;

namespace Delegates
{
    internal abstract class Client
    {
        protected string id;
        protected string orgForm;
        protected string fullName;
        protected string requisites;

        public string Id { get { return id; } }
        public string OrgForm { get { return orgForm; } }
        public string Requisites { get { return requisites; } }
        public string FullName { get { return fullName; } }
        public string Name { get; set; }
        public string INN { get; set; }
        public string PhoneNumber { get; set; }
        public bool VIP { get; set; }

        public Dictionary<int, string> RussianNames { get; set; }        

        public virtual string GetFilePath()
        {
            string filePath = Repository.ClientsFilePath;
            return filePath;
        }

        public virtual string GetIdFilePath()
        {
            string filePath = Repository.ClientsIdFilePath;
            return filePath;
        }

        protected virtual void GenerateRussianNames()
        {
            if (RussianNames == null)
            {
                RussianNames = new Dictionary<int, string>();
            }                        
        }

        public virtual string GetRussianName(int fieldNumber)
        {
            GenerateRussianNames();

            bool status = RussianNames.TryGetValue(fieldNumber, out string name);

            if (status == false)
            {
                name = String.Empty;
            }

            return name;
        }
    }
}
