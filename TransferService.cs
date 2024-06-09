using System.Collections.Generic;

namespace Delegates
{
    internal class TransferService<T>
        where T : Account
    {
        private delegate void ClientNotifications(string message);
        private event ClientNotifications notify;
        private string operation;

        private List<string> GenerateOperationInfo(T senderAccount, T recipientAccount, decimal amount)
        {
            List<string> info = new List<string>();

            if (senderAccount.OwnerId == recipientAccount.OwnerId)
            {
                operation = "Перевод между счетами клиента";

                info.Add($"{operation}");
                info.Add($"Счет-отправитель: {senderAccount.AccountNumber}");
                info.Add($"Счет-получатель: {recipientAccount.AccountNumber}");
            }
            else
            {
                operation = "Перевод клиенту банка";

                info.Add($"{operation}");
                info.Add($"Отправитель: {TransferInfo.Sender.FullName}");
                info.Add($"Счет отправителя: {senderAccount.AccountNumber}");
                info.Add($"Получатель: {TransferInfo.Recipient.FullName}");
                info.Add($"Счет получателя: {recipientAccount.AccountNumber}");                                
            }

            info.Add($"Сумма: {amount} руб.");

            return info;
        }

        private void WriteToChangeLog(T senderAccount, T recipientAccount, decimal amount)
        {
            List<string> info = GenerateOperationInfo(senderAccount, recipientAccount, amount);

            Repository.WriteToChangeLog(senderAccount.OwnerId, info);

            if (senderAccount.OwnerId != recipientAccount.OwnerId)
            {
                Repository.OverwriteOrWriteToFile
                    (info, Repository.GenerateChangeLogFilePath(recipientAccount.OwnerId), true);
            }

            notify += WindowsService.CallNotification;
            notify?.Invoke(operation);
        }

        public void Transfer(T senderAccount, T recipientAccount, decimal amount)
        {
            if (senderAccount.Balance >= amount)
            {
                senderAccount.Withdraw(amount);
                recipientAccount.Deposit(amount);

                Repository.OwerwriteAccount(senderAccount);
                Repository.OwerwriteAccount(recipientAccount);

                WriteToChangeLog(senderAccount, recipientAccount, amount);
            }
            else
            {
                WindowsService.CallMessageBox("Недостаточно средств");
            }

            TransferInfo.ResetTransferInfo();
        }
    }
}
