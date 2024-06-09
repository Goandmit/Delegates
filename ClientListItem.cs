namespace Delegates
{
    internal class ClientListItem
    {       
        public string Id { get; set; }
        public string OrgForm { get; set; }
        public string Requisites { get; set; }
        public string FullName { get; set; }

        /// <summary>
        /// Элемент списка клиентов
        /// </summary>
        /// <param name="Id">Идентификатор клиента</param>
        /// <param name="OrgForm">Организационно-правовая форма клиента</param>
        /// <param name="Requisites">Реквизиты клиента</param>
        /// <param name="FullName">Полное наименование клиента</param>              
        public ClientListItem(string Id, string OrgForm, string Requisites, string FullName)
        {
            this.Id = Id;
            this.OrgForm = OrgForm;
            this.Requisites = Requisites;
            this.FullName = FullName;
        }

        public ClientListItem() { }        

        public string GetFilePath()
        {
            string filePath = Repository.ClientsFilePath;
            return filePath;
        }

        public ClientListItem GetFromStringSplited(string[] stringSplited)
        {
            ClientListItem clientListItem = new ClientListItem(stringSplited[0], stringSplited[1],
                stringSplited[2], stringSplited[3]);               

            return clientListItem;
        }        
    }
}
