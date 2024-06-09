using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Delegates
{
    internal class AccountService<T>
        where T : Account, IStringHandler<T>, new()
    {
        private delegate void ClientNotifications(string message);
        private event ClientNotifications notify;
        private string operation;

        private List<string> GenerateOperationInfo(T account, decimal amount)
        {
            List<string> info = new List<string>
            {
                $"{operation}",
                $"Номер счета: {account.AccountNumber}"
            };

            if (operation == "Открыт счет") { info.Add($"Начальный баланс: {account.Balance} руб."); }
            if (operation == "Пополнение счета") { info.Add($"Сумма: {amount} руб."); }

            return info;
        }

        private void WriteToChangeLog(T account, decimal amount)
        {
            List<string> info = GenerateOperationInfo(account, amount);

            Repository.WriteToChangeLog(account.OwnerId, info);

            notify += WindowsService.CallNotification;
            notify?.Invoke(operation);            
        }

        private bool CheckZeroAccountBalance(decimal balance)
        {
            bool status = true;

            if (balance != 0)
            {
                WindowsService.CallMessageBox("Баланс закрываемого счета должен быть нулевым. " +
                "Переведите средства на другой счет");

                status = false;
            }

            return status;
        }

        public void OpenAccount(T account)
        {
            string accountInString = account.ConvertToString(account);
            Repository.WriteToFile(accountInString, account.GetFilePath(account.OwnerId));

            operation = "Открыт счет";
            WriteToChangeLog(account, account.Balance);            
        }

        public void CloseAccount(T account)
        {
            string filePath = account.GetFilePath(account.OwnerId);
            bool status = Repository.CheckBeforeReading(filePath);

            if (status == true)
            {
                List<string> records = new List<string>();

                using (StreamReader streamReader = new StreamReader(filePath, Encoding.Unicode))
                {
                    while (!streamReader.EndOfStream)
                    {
                        string record = $"{streamReader.ReadLine()}";
                        string[] recordSplited = record.Split('#');

                        if (recordSplited[0] == account.Id)
                        {
                            if (CheckZeroAccountBalance(account.Balance) == true)
                            {
                                status = false;
                                continue;
                            }                                
                        }

                        records.Add(record);
                    }
                }

                if (status == false)
                {                
                    operation = "Закрыт счет";
                    WriteToChangeLog(account, account.Balance);
                    Repository.OverwriteOrWriteToFile(records, filePath, false);

                    if (records.Count == 0)
                    {
                        File.Delete(filePath);
                    }                    
                }
            }
        }        

        public void OwerwriteAccount(T account)
        {
            string newRecord = account.ConvertToString(account);
            string[] newRecordSplited = newRecord.Split('#');

            List<string> records = new List<string>();

            using (StreamReader streamReader = new StreamReader
                (account.GetFilePath(account.OwnerId), Encoding.Unicode))
            {
                while (!streamReader.EndOfStream)
                {
                    string oldRecord = $"{streamReader.ReadLine()}";
                    string[] oldRecordSplited = oldRecord.Split('#');

                    if (oldRecordSplited[0] == newRecordSplited[0])
                    {
                        oldRecord = newRecord;
                    }

                    records.Add(oldRecord);
                }
            }

            Repository.OverwriteOrWriteToFile(records, account.GetFilePath(account.OwnerId), false);
        }

        public T GetAccountFromFile(string ownerId, string accountId)
        {
            T account = new T();

            bool status = Repository.CheckBeforeReading(account.GetFilePath(ownerId));

            if (status == true)
            {
                using (StreamReader streamReader = new StreamReader(account.GetFilePath(ownerId), Encoding.Unicode))
                {
                    while (!streamReader.EndOfStream)
                    {
                        string streamString = $"{streamReader.ReadLine()}";
                        string[] streamStringSplited = streamString.Split('#');

                        if (streamStringSplited[0] == accountId)
                        {
                            account = account.GetFromStringSplited(streamStringSplited);
                        }
                    }
                }
            }

            return account;
        }

        public void DepositAccount(T account, decimal amount)
        {
            account.Deposit(amount);
            OwerwriteAccount(account);

            operation = "Пополнение счета";
            WriteToChangeLog(account, amount);
        }       
    }
}
