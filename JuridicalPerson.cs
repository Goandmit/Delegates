using System;
using System.Collections.Generic;

namespace Delegates
{
    internal class JuridicalPerson : Client, IStringHandler<JuridicalPerson>
    {
        public string KPP { get; set; }        

        /// <summary>
        /// Карточка клиента
        /// </summary>
        /// <param name="Id">Идентификатор</param>
        /// <param name="OrgForm">Организационно-правовая форма</param>        
        /// <param name="Name">Наименование</param>        
        /// <param name="INN">ИНН</param> 
        /// <param name="KPP">КПП</param>
        /// <param name="PhoneNumber">Номер телефона</param>        
        /// <param name="VIP">VIP-статус</param>
        public JuridicalPerson(string Id, string OrgForm, string Name, string INN, string KPP,
             string PhoneNumber, bool VIP)
        {
            id = Id;
            orgForm = OrgForm;
            this.Name = Name;
            this.INN = INN;
            this.KPP = KPP;
            this.PhoneNumber = PhoneNumber;
            this.VIP = VIP;
            requisites = $"{this.INN}/{this.KPP}";
            fullName = $"{this.Name}";
        }

        public JuridicalPerson() { }

        public string ConvertToString(JuridicalPerson client)
        {
            string clientInString = $"{client.Id}#" +
                $"{client.OrgForm}#" +
                $"{client.Requisites}#" +
                $"{client.FullName}#" +
                $"{client.Name}#" +
                $"{client.INN}#" +
                $"{client.KPP}#" +
                $"{client.PhoneNumber}#" +                
                $"{client.VIP}";

            return clientInString;
        }

        public JuridicalPerson GetFromStringSplited(string[] stringSplited)
        {
            JuridicalPerson client = new JuridicalPerson(stringSplited[0], stringSplited[1],
                stringSplited[4], stringSplited[5], stringSplited[6], stringSplited[7],
                Convert.ToBoolean(stringSplited[8]));

            return client;
        }

        protected override void GenerateRussianNames()
        {
            RussianNames = new Dictionary<int, string>
            {
                { 0, "Идентификатор" },
                { 1, "Организационно-правовая форма" },
                { 4, "Наименование" },
                { 5, "ИНН" },
                { 6, "КПП" },
                { 7, "Номер телефона" },
                { 8, "VIP-статус" }
            };
        }

        public override string GetRussianName(int fieldNumber)
        {
            GenerateRussianNames();

            string name = base.GetRussianName(fieldNumber);            

            return name;
        }
    }
}
