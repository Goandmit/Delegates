using System;

namespace Delegates
{
    internal class DepositAccount : Account, IStringHandler<DepositAccount>
    {
        public bool OwnerVIP { get; set; }
        public decimal MinBalance { get; protected set; }
        public decimal AccrualInCurrentMonth { get; set; }
        public int InterestRate { get; set; }
        public DateTime DateOfLastAccrual { get; set; }
        public DateTime DateOfNextAccrual { get; set; }
        public bool OverwritingIsRequired { get; set; }        

        /// <summary>
        /// Депозитный счет (накопительный счет)
        /// </summary>
        /// <param name="Id">Идентификатор счета</param>
        /// <param name="AccountType">Тип счета</param>
        /// <param name="AccountNumber">Номер счета</param>
        /// <param name="OpeningDate">Дата открытия счета</param>
        /// <param name="OwnerId">Идентификатор владельца счета</param>
        /// <param name="OwnerOrgForm">Организационно-правовая форма владельца счета</param>
        /// <param name="Balance">Баланс счета</param>
        /// <param name="OwnerVIP">Статус владельца счета</param>
        /// <param name="MinBalance">Минимальный баланс счета за расчетный месяц</param>
        /// <param name="InterestRate">Процентная ставка</param>
        /// <param name="DateOfLastAccrual">Дата прошлого начисления процентов на остаток</param>
        /// <param name="DateOfNextAccrual">Дата следующего начисления процентов на остаток</param>        
        public DepositAccount(string Id, string AccountType, string AccountNumber,
            DateTime OpeningDate, string OwnerId, string OwnerOrgForm, decimal Balance,
            bool OwnerVIP, decimal MinBalance,
            int InterestRate, DateTime DateOfLastAccrual, DateTime DateOfNextAccrual)
        {
            id = Id;
            accountType = AccountType;
            accountNumber = AccountNumber;
            openingDate = OpeningDate;
            ownerId = OwnerId;
            ownerOrgForm = OwnerOrgForm;
            this.Balance = Balance;
            this.OwnerVIP = OwnerVIP;
            this.MinBalance = MinBalance;
            this.InterestRate = InterestRate;
            this.DateOfLastAccrual = DateOfLastAccrual;
            this.DateOfNextAccrual = DateOfNextAccrual;

            InterestAccrual();
        }

        public DepositAccount(string AccountType, string OwnerId, string OwnerOrgForm,
              decimal Balance, bool OwnerVIP) :
             this(String.Empty, AccountType, String.Empty, DateTime.Today, OwnerId, OwnerOrgForm,
                   Balance, OwnerVIP, 0, 0, DateTime.Today, DateTime.Today)
        {
            id = Convert.ToString(Repository.AssignId(GetIdFilePath()));
            accountNumber = AssignAccountNumber(id, OwnerOrgForm);
            MinBalance = Balance;
            InterestRate = AssignInterestRate(OwnerOrgForm, OwnerVIP);
            DateOfNextAccrual = GetDateOfNextAccrual(DateOfLastAccrual);

            InterestAccrual();
        }

        public DepositAccount() { }

        private int AssignInterestRate(string ownerOrgForm, bool ownerVIP)
        {
            int interestRate;

            if (ownerOrgForm == "Физическое лицо")
            {
                interestRate = 13;

                if (ownerVIP == true)
                {
                    interestRate = 15;
                }
            }
            else
            {
                interestRate = 10;

                if (ownerVIP == true)
                {
                    interestRate = 12;
                }
            }

            return interestRate;
        }

        private DateTime GetDateOfNextAccrual(DateTime date)
        {
            if (date.Day == 01)
            {
                date = date.AddDays(1);
            }

            while (date.Day != 01)
            {
                date = date.AddDays(1);
            }

            date = date.AddDays(-1);

            return date;
        }

        private int GetPastDays(DateTime startDate, DateTime endDate)
        {
            int pastDays = 0;

            while (startDate != endDate)
            {
                startDate = startDate.AddDays(1);
                pastDays++;
            }

            return pastDays;
        }

        private void InterestAccrual()
        {
            int daysInYear = new DateTime(DateOfNextAccrual.Year, 12, 31).DayOfYear;

            decimal accrualForDay = MinBalance * InterestRate / 100 / daysInYear;

            int pastDays = GetPastDays(DateOfLastAccrual, DateOfNextAccrual);

            AccrualInCurrentMonth = Math.Round(accrualForDay * pastDays, 2);

            if (DateOfNextAccrual < DateTime.Today)
            {
                Balance += AccrualInCurrentMonth;

                MinBalance = Balance;

                DateOfLastAccrual = DateOfNextAccrual;

                DateOfNextAccrual = GetDateOfNextAccrual(DateOfNextAccrual);

                OverwritingIsRequired = true;

                InterestAccrual();
            }
            else
            {
                OverwritingIsRequired = false;
            }
        }

        public string ConvertToString(DepositAccount account)
        {
            string accountInString = $"{account.Id}#" +
                $"{account.AccountType}#" +
                $"{account.AccountNumber}#" +
                $"{account.OpeningDate}#" +
                $"{account.OwnerId}#" +
                $"{account.OwnerOrgForm}#" +
                $"{account.Balance}#" +
                $"{account.OwnerVIP}#" +
                $"{account.MinBalance}#" +
                $"{account.InterestRate}#" +
                $"{account.DateOfLastAccrual}#" +
                $"{account.DateOfNextAccrual}#";

            return accountInString;
        }

        public DepositAccount GetFromStringSplited(string[] streamStringSplited)
        {
            DepositAccount account = new DepositAccount(streamStringSplited[0], streamStringSplited[1],
                streamStringSplited[2], Convert.ToDateTime(streamStringSplited[3]), streamStringSplited[4],
                streamStringSplited[5], Convert.ToDecimal(streamStringSplited[6]),
                Convert.ToBoolean(streamStringSplited[7]), Convert.ToDecimal(streamStringSplited[8]),
                Convert.ToInt32(streamStringSplited[9]), Convert.ToDateTime(streamStringSplited[10]),
                Convert.ToDateTime(streamStringSplited[11]));

            return account;
        }

        public override void Withdraw(decimal amount)
        {
            base.Withdraw(amount);

            MinBalance = Balance;
        }
    }
}
