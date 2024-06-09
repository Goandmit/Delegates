using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Delegates
{
    internal static class Repository
    {
        private static Worker currentUser = null;
        public static Worker CurrentUser => currentUser;
        public static ClientListItem CurrentClient { get; private set; }
        public static Account CurrentAccount { get; private set; }

        public static string ClientsFilePath { get { return "Clients.txt"; } }
        public static string ClientsIdFilePath { get { return "ClientsId.txt"; } }       
        public static string AccountsIdFilePath { get { return "AccountsId.txt"; } }

        public static ClientService<JuridicalPerson> ClientServiceJP
        { get { return new ClientService<JuridicalPerson>(); } }
        public static ClientService<PhysicalPerson> ClientServicePP
        { get { return new ClientService<PhysicalPerson>(); } }

        public static AccountService<DepositAccount> AccountServiceDA
        { get { return new AccountService<DepositAccount>(); } }
        public static AccountService<NonDepositAccount> AccountServiceNDA
        { get { return new AccountService<NonDepositAccount>(); } }            

        public static void SetCurrentUser(Worker worker)
        {
            if (currentUser == null)
            {
                currentUser = worker;
            }
        }

        public static void SetCurrentClient(Client client)
        {
            CurrentClient = new ClientListItem(client.Id, client.OrgForm, client.Requisites, client.FullName);
        }

        public static void ResetCurrentClient()
        {
            CurrentClient = new ClientListItem();
        }

        public static void SetCurrentAccount(Account account)
        {
            CurrentAccount = account;
        }

        public static void ResetCurrentAccount()
        {
            CurrentAccount = new NonDepositAccount();
        }

        public static string GenerateAccountsFilePath(string ownerId)
        {
            string filePath = $"Accounts{ownerId}.txt";
            return filePath;
        }

        public static string GenerateChangeLogFilePath(string clientId)
        {
            string filePath = $"ChangeLog{clientId}.txt";
            return filePath;
        }        

        public static bool CheckBeforeReading(string filePath)
        {
            bool status = false;

            if (File.Exists(filePath) && new FileInfo(filePath).Length > 6)
            {
                status = true;
            }
            return status;
        }

        public static int AssignId(string filePath)
        {
            int id;
            string streamString;

            if (File.Exists(filePath) && new FileInfo(filePath).Length > 6)
            {
                using (StreamReader streamReader =
                    new StreamReader(filePath, Encoding.Unicode))
                {
                    streamString = $"{streamReader.ReadLine()}";
                }

                id = Convert.ToInt32(streamString) + 1;
                streamString = id.ToString();
            }
            else
            {
                streamString = "1";
                id = Convert.ToInt32(streamString);
            }

            using (StreamWriter streamWriter =
                new StreamWriter(filePath, false, Encoding.Unicode))
            {
                streamWriter.WriteLine(streamString);
            }

            return id;
        }

        public static string AssignOrReadId(string possibleId, string filePath)
        {
            string id;

            if (possibleId != null && possibleId.Length > 0)
            {
                id = possibleId;
            }
            else
            {
                id = (AssignId(filePath)).ToString();
            }

            return id;
        }
        
        public static void WriteToFile(string record, string filePath)
        {
            using (StreamWriter streamWriter =
                new StreamWriter(filePath, true, Encoding.Unicode))
            {
                streamWriter.WriteLine(record);
            }
        }

        public static void OverwriteOrWriteToFile(List<string> records, string filePath, bool notOverwrite)
        {
            using (StreamWriter streamWriter =
            new StreamWriter(filePath, notOverwrite, Encoding.Unicode))
            {
                foreach (string record in records)
                {
                    streamWriter.WriteLine(record);
                }
            }
        }       

        public static string ProcessingForWriteToFile(string[] record)
        {
            string processedRecord = String.Empty;

            foreach (string str in record)
            {
                processedRecord += $"{str}#";
            }

            processedRecord = processedRecord.Remove(processedRecord.Length - 1);

            return processedRecord;
        }

        public static string GetOnlyConvertibleIntoDecimal(string userInput)
        {
            Regex regex = new Regex("[^0-9,.]");

            if (regex.IsMatch(userInput))
            {
                userInput = userInput.Remove(userInput.Length - 1);
            }

            if (userInput.Contains('.'))
            {
                userInput = userInput.Replace('.', ',');
            }

            if (userInput.Contains(','))
            {
                int decimalPlaces = userInput.IndexOf(',') + 3;

                if (userInput.Length > decimalPlaces)
                {
                    userInput = userInput.Remove(userInput.Length - 1);
                }
            }

            return userInput;
        }

        public static void OwerwriteOrWriteClient(Client client)
        {
            if (client is JuridicalPerson jp)
            {
                ClientServiceJP.OwerwriteOrWriteClient(jp);
            }

            if (client is PhysicalPerson pp)
            {
                ClientServicePP.OwerwriteOrWriteClient(pp);
            }            
        }        

        public static void DeleteClient(string clientId)
        {
            bool status = CheckBeforeReading(GenerateAccountsFilePath(clientId));

            if (status == true)
            {
                WindowsService.CallMessageBox("Клиент, имеющий незакрытые счета не может быть удален");
            }
            else
            {
                ClientServiceJP.DeleteClient(clientId);
            }                                
        }

        public static void OpenAccount(Account account)
        {
            if (account is DepositAccount depAcc)
            {
                AccountServiceDA.OpenAccount(depAcc);
            }

            if (account is NonDepositAccount nonDepAcc)
            {
                AccountServiceNDA.OpenAccount(nonDepAcc);
            }
        }

        public static void CloseAccount(Account account)
        {
            if (account is DepositAccount depAcc)
            {
                AccountServiceDA.CloseAccount(depAcc);
            }

            if (account is NonDepositAccount nonDepAcc)
            {
                AccountServiceNDA.CloseAccount(nonDepAcc);
            }           
        }

        public static void OwerwriteAccount(Account account)
        {
            if (account is DepositAccount depAcc)
            {
                AccountServiceDA.OwerwriteAccount(depAcc);
            }

            if (account is NonDepositAccount nonDepAcc)
            {
                AccountServiceNDA.OwerwriteAccount(nonDepAcc);
            }
        }

        public static void DepositAccount(Account account, decimal amount)
        {
            if (account is DepositAccount depAcc)
            {
                AccountServiceDA.DepositAccount(depAcc, amount);
            }

            if (account is NonDepositAccount nonDepAcc)
            {
                AccountServiceNDA.DepositAccount(nonDepAcc, amount);
            }
        }   
        
        public static void WriteToChangeLog(string clientId, List<string> info)
        {
            info.Add($"Дата и время: {DateTime.Now}");
            info.Add($"Сотрудник: {CurrentUser.Position}");
            info.Add(String.Empty);            

            OverwriteOrWriteToFile(info, GenerateChangeLogFilePath(clientId), true);            
        }
                 
        public static ObservableCollection<Worker> GetWorkers()
        {
            ObservableCollection<Worker> workers = new ObservableCollection<Worker>();

            Consultant consultant = new Consultant();
            workers.Add(consultant);

            Manager manager = new Manager();
            workers.Add(manager);

            return workers;
        }

        public static List<ClientListItem> GetClientListItems()
        {
            List<ClientListItem> items = new List<ClientListItem> ();
            ClientListItem item = new ClientListItem();

            bool status = CheckBeforeReading(item.GetFilePath());

            if (status == true)
            {
                using (StreamReader streamReader =
                new StreamReader(item.GetFilePath(), Encoding.Unicode))
                {
                    while (!streamReader.EndOfStream)
                    {
                        string streamString = $"{streamReader.ReadLine()}";
                        string[] streamStringSplited = streamString.Split('#');

                        item = item.GetFromStringSplited(streamStringSplited);
                        items.Add(item);
                    }
                }
            }

            return items;
        }

        public static List<Account> GetAccounts(string ownerId)
        {
            List<Account> accounts = new List<Account>();

            bool status = CheckBeforeReading(GenerateAccountsFilePath(ownerId));

            if (status == true)
            {
                using (StreamReader streamReader =
                new StreamReader(GenerateAccountsFilePath(ownerId), Encoding.Unicode))
                {
                    while (!streamReader.EndOfStream)
                    {
                        string streamString = $"{streamReader.ReadLine()}";
                        string[] streamStringSplited = streamString.Split('#');

                        if (streamStringSplited[1] == "Депозитный")
                        {
                            DepositAccount depAcc = new DepositAccount();
                            depAcc = depAcc.GetFromStringSplited(streamStringSplited);
                            accounts.Add(depAcc);
                        }

                        if (streamStringSplited[1] == "Недепозитный")
                        {
                            NonDepositAccount nonDepAcc = new NonDepositAccount();
                            nonDepAcc = nonDepAcc.GetFromStringSplited(streamStringSplited);
                            accounts.Add(nonDepAcc);
                        }
                    }
                }
            }

            return accounts;
        }

        public static List<string> GetChangeLog(string clientId)
        {
            List<string> records = new List<string>();

            bool status = CheckBeforeReading(GenerateChangeLogFilePath(clientId));

            if (status == true)
            {
                using (StreamReader streamReader =
                new StreamReader(GenerateChangeLogFilePath(clientId), Encoding.Unicode))
                {
                    while (!streamReader.EndOfStream)
                    {
                        string streamString = $"{streamReader.ReadLine()}";

                        if (CurrentUser is Consultant)
                        {
                            if (streamString.Contains("Серия паспорта"))
                            {
                                streamString = "Серия паспорта";
                            }

                            if (streamString.Contains("Номер паспорта"))
                            {
                                streamString = "Номер паспорта";
                            }
                        }

                        records.Add(streamString);
                    }
                }
            }
            
            return records;
        }        
    }
}
