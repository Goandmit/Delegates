using System;

namespace Delegates
{
    internal abstract class Account
    {
        private protected string id;
        public string Id { get { return this.id; } }

        private protected string accountType;
        public string AccountType { get { return this.accountType; } }

        private protected string accountNumber;
        public string AccountNumber { get { return this.accountNumber; } }

        private protected DateTime openingDate;
        public DateTime OpeningDate { get { return this.openingDate; } }

        private protected string ownerId;
        public string OwnerId { get { return this.ownerId; } }

        private protected string ownerOrgForm;
        public string OwnerOrgForm { get { return this.ownerOrgForm; } }
       
        public decimal Balance { get; protected set; }       

        public virtual string GetFilePath(string ownerId)
        {
            string filePath = Repository.GenerateAccountsFilePath(ownerId);
            return filePath;
        }

        public virtual string GetIdFilePath()
        {
            string filePath = Repository.AccountsIdFilePath;
            return filePath;
        }

        private protected string AssignAccountNumber(string id, string ownerOrgForm)
        {
            string accountNumber;

            if (ownerOrgForm == "Юридическое лицо")
            {
                accountNumber = "407";
            }
            else
            {
                accountNumber = "408";
            }

            accountNumber += "0281045555";

            if ((id.Length + accountNumber.Length) < 20)
            {
                do
                {
                    accountNumber += "0";
                }
                while ((id.Length + accountNumber.Length) <= 20);
            }

            accountNumber += $"{id}";

            return accountNumber;
        }

        public virtual void Deposit(decimal amount)
        {
            Balance += amount;
        }

        public virtual void Withdraw(decimal amount)
        {
            if (Balance >= amount)
            {
                Balance -= amount;
            }
            else
            {
                WindowsService.CallMessageBox("Недостаточно средств");
            }
        }
    }
}
