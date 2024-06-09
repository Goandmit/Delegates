using System;
using System.Collections.Generic;

namespace Delegates
{
    internal class PhysicalPerson : Client, IStringHandler<PhysicalPerson>
    {
        public string Surname { get; set; }
        public string Patronymic { get; set; }
        public string PassportSeries { get; set; }
        public string PassportNumber { get; set; }        

        /// <summary>
        /// Карточка клиента
        /// </summary>
        /// <param name="Id">Идентификатор</param>
        /// <param name="OrgForm">Организационно-правовая форма</param>   
        /// <param name="Surname">Фамилия</param>
        /// <param name="Name">Имя</param>
        /// <param name="Patronymic">Отчество</param>
        /// <param name="INN">ИНН</param>         
        /// <param name="PhoneNumber">Номер телефона</param>
        /// <param name="PassportSeries">Серия паспорта</param> 
        /// <param name="PassportNumber">Номер паспорта</param>
        /// <param name="VIP">Статус</param>
        public PhysicalPerson(string Id, string OrgForm, string Surname, string Name, string Patronymic,
            string INN, string PhoneNumber, string PassportSeries, string PassportNumber, bool VIP)            
        {
            id = Id;
            orgForm = OrgForm;
            this.Surname = Surname;
            this.Name = Name;
            this.Patronymic = Patronymic;
            this.INN = INN;
            this.PhoneNumber = PhoneNumber;
            this.PassportSeries = PassportSeries;
            this.PassportNumber = PassportNumber;
            this.VIP = VIP;
            requisites = $"{this.INN}";
            fullName = $"{this.Surname} {this.Name}";            

            if (this.Patronymic.Length != 0)
            {
                fullName = $"{fullName} {this.Patronymic}";
            }

            if (OrgForm == "Индивидуальный предприниматель")
            {
                fullName = $"ИП {fullName}";
            }
        }

        public PhysicalPerson() { }

        public string ConvertToString(PhysicalPerson client)
        {
           string clientInString = $"{client.Id}#" +
                $"{client.OrgForm}#" +
                $"{client.Requisites}#" +
                $"{client.FullName}#" +
                $"{client.Surname}#" +
                $"{client.Name}#" +
                $"{client.Patronymic}#" +
                $"{client.INN}#" +
                $"{client.PhoneNumber}#" +
                $"{client.PassportSeries}#" +
                $"{client.PassportNumber}#" +
                $"{client.VIP}";                

            return clientInString;
        }

        public PhysicalPerson GetFromStringSplited(string[] stringSplited)
        {
            PhysicalPerson client = new PhysicalPerson(stringSplited[0], stringSplited[1],
                stringSplited[4], stringSplited[5], stringSplited[6], stringSplited[7],
                stringSplited[8], stringSplited[9], stringSplited[10], Convert.ToBoolean(stringSplited[11]));

            return client;
        }

        protected override void GenerateRussianNames()
        {
            RussianNames = new Dictionary<int, string>
            {
                { 0, "Идентификатор" },
                { 1, "Организационно-правовая форма" },
                { 4, "Фамилия" },
                { 5, "Имя" },
                { 6, "Отчество" },
                { 7, "ИНН" },
                { 8, "Номер телефона" },
                { 9, "Серия паспорта" },
                { 10, "Номер паспорта" },
                { 11, "VIP-статус" }
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
