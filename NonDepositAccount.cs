using System;

namespace Delegates
{
    internal class NonDepositAccount : Account, IStringHandler<NonDepositAccount>
    {
        /// <summary>
        /// Недепозитный счет (расчетный счет)
        /// </summary>
        /// <param name="Id">Идентификатор счета</param>
        /// <param name="AccountType">Тип счета</param>
        /// <param name="AccountNumber">Номер счета</param>
        /// <param name="OpeningDate">Дата открытия счета</param>
        /// <param name="OwnerId">Идентификатор владельца счета</param>
        /// <param name="OwnerOrgForm">Организационно-правовая форма владельца счета</param>
        /// <param name="Balance">Баланс счета</param>        
        public NonDepositAccount(string Id, string AccountType, string AccountNumber,
            DateTime OpeningDate, string OwnerId, string OwnerOrgForm, decimal Balance)
        {
            id = Id;
            accountType = AccountType;
            accountNumber = AccountNumber;
            openingDate = OpeningDate;
            ownerId = OwnerId;
            ownerOrgForm = OwnerOrgForm;
            this.Balance = Balance;
        }

        public NonDepositAccount(string AccountType, string OwnerId, string OwnerOrgForm, decimal Balance) :
            this(String.Empty, AccountType, String.Empty, DateTime.Today, OwnerId, OwnerOrgForm, Balance)
        {
            id = Convert.ToString(Repository.AssignId(GetIdFilePath()));
            accountNumber = AssignAccountNumber(id, OwnerOrgForm);
        }

        public NonDepositAccount() { }

        public string ConvertToString(NonDepositAccount account)
        {
            string accountInString = $"{account.Id}#" +
                $"{account.AccountType}#" +
                $"{account.AccountNumber}#" +
                $"{account.OpeningDate}#" +
                $"{account.OwnerId}#" +
                $"{account.OwnerOrgForm}#" +
                $"{account.Balance}#";

            return accountInString;
        }

        public NonDepositAccount GetFromStringSplited(string[] streamStringSplited)
        {
            NonDepositAccount account = new NonDepositAccount(streamStringSplited[0], streamStringSplited[1],
                streamStringSplited[2], Convert.ToDateTime(streamStringSplited[3]),
                streamStringSplited[4], streamStringSplited[5], Convert.ToDecimal(streamStringSplited[6]));

            return account;
        }
    }
}
