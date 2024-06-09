using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Delegates
{
    internal class ClientService<T>
        where T : Client, IStringHandler<T>, new()        
    {
        private delegate void ClientNotifications(string message);
        private event ClientNotifications notify;
        private string operation;
        private List<string> changedFields = new List<string>();

        private void FillChangedFields(string fieldName, string oldVersion, string newVersion)
        {
            if (fieldName.Length != 0)
            {
                changedFields.Add($"{fieldName} (было \"{oldVersion}\", стало \"{newVersion})\"");
            }
        }

        private List<string> GenerateOperationInfo()
        {
            List<string> info = new List<string> { $"{operation}" };

            if (operation != "Создан новый клиент")
            {
                if (changedFields.Count != 0)
                {
                    info.Add($"Изменены поля:");

                    foreach (string field in changedFields)
                    {
                        info.Add(field);
                    }
                }
                else
                {
                    info.Add($"Перезапись без изменения полей");
                }
            }

            return info;
        }

        private void WriteToChangeLog(string clientId)
        {
            List<string> info = GenerateOperationInfo();

            Repository.WriteToChangeLog(clientId, info);            

            notify += WindowsService.CallNotification;
            notify?.Invoke(operation);

            changedFields = new List<string>();
        }        

        private string UpdateClientRecord(string[] oldRecord, string[] newRecord, T client)
        {
            string updatedRecord;

            for (int i = 0; i < oldRecord.Length; i++)
            {
                if (oldRecord[i] != newRecord[i])
                {
                    string fieldName = client.GetRussianName(i);

                    if (Repository.CurrentUser is Consultant)
                    {
                        if (fieldName == "Номер телефона" || fieldName == "VIP-статус")
                        {
                            FillChangedFields(fieldName, oldRecord[i], newRecord[i]);
                            oldRecord[i] = newRecord[i];
                        }
                    }
                    else
                    {
                        FillChangedFields(fieldName, oldRecord[i], newRecord[i]);
                        oldRecord[i] = newRecord[i];
                    }
                }
            }

            updatedRecord = Repository.ProcessingForWriteToFile(oldRecord);

            return updatedRecord;
        }

        public void WriteClient(T client)
        {
            string clientInString = client.ConvertToString(client);
            Repository.WriteToFile(clientInString, client.GetFilePath());

            operation = "Создан новый клиент";
            WriteToChangeLog(client.Id);
        }

        public void OwerwriteClient(T client)
        {
            string newRecord = client.ConvertToString(client);
            string[] newRecordSplited = newRecord.Split('#');

            List<string> records = new List<string>();

            using (StreamReader streamReader = new StreamReader(client.GetFilePath(), Encoding.Unicode))
            {
                while (!streamReader.EndOfStream)
                {
                    string oldRecord = $"{streamReader.ReadLine()}";
                    string[] oldRecordSplited = oldRecord.Split('#');

                    if (oldRecordSplited[0] == newRecordSplited[0])
                    {
                        oldRecord = UpdateClientRecord(oldRecordSplited, newRecordSplited, client);                        
                    }

                    records.Add(oldRecord);
                }
            }

            Repository.OverwriteOrWriteToFile(records, client.GetFilePath(), false);
            operation = "Данные клиента изменены";
            WriteToChangeLog(client.Id);
        }              

        public T GetClient(string id)
        {
            T client = new T();

            bool status = Repository.CheckBeforeReading(client.GetFilePath());

            if (status == true)
            {
                using (StreamReader streamReader = new StreamReader(client.GetFilePath(), Encoding.Unicode))
                {
                    while (!streamReader.EndOfStream)
                    {
                        string streamString = $"{streamReader.ReadLine()}";
                        string[] streamStringSplited = streamString.Split('#');

                        if (streamStringSplited[0] == id)
                        {
                            client = client.GetFromStringSplited(streamStringSplited);
                        }                                                
                    }
                }
            }

            return client;
        }        

        public void OwerwriteOrWriteClient(T client)
        {
            bool status = Repository.CheckBeforeReading(client.GetFilePath());

            if (status == true)
            {
                T clientFromFile = GetClient(client.Id);

                if (!string.IsNullOrEmpty($"{clientFromFile.Id}"))
                {
                    if (client != clientFromFile)
                    {
                        OwerwriteClient(client);                        
                    }
                }
                else
                {                    
                    WriteClient(client);                    
                }
            }
            else
            {                
                WriteClient(client);                
            }           
        }

        public void DeleteClient(string clientId)
        {
            T client = new T();
            string filePath = client.GetFilePath();
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

                        if (recordSplited[0] == clientId)
                        {
                            status = false;
                            continue;
                        }

                        records.Add(record);
                    }
                }

                if (status == false)
                {
                    Repository.OverwriteOrWriteToFile(records, filePath, false);
                    File.Delete(Repository.GenerateChangeLogFilePath(clientId));                    
                    notify += WindowsService.CallNotification;
                    notify?.Invoke("Клиент удален");
                }                
            }            
        }
    }
}
