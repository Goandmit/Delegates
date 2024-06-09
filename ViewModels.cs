using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Delegates
{
    internal class AuthorizationFormVM
    {
        public ObservableCollection<Worker> Workers { get; set; }
        public Worker SelectedWorker { get; set; }

        public AuthorizationFormVM()
        {
            Workers = Repository.GetWorkers();

            WindowsService.DefineWorkingAreaSize();
        }

        private RelayCommand oKCommand;
        public RelayCommand OKCommand
        {
            get
            {
                return oKCommand ??
                  (oKCommand = new RelayCommand(obj =>
                  {
                      if (SelectedWorker is Worker worker)
                      {
                          WindowsService.CallClientList(worker);
                      }
                  }));
            }
        }
    }

    internal class ClientListVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

        public bool ButtonsIsEnabled { get; set; }

        private ObservableCollection<ClientListItem> items;
        public ObservableCollection<ClientListItem> Items
        {
            get { return items; }
            set
            {
                items = value;
                OnPropertyChanged($"{nameof(Items)}");
            }
        }

        public ClientListItem SelectedItem { get; set; }

        public ClientListVM()
        {
            Items = new ObservableCollection<ClientListItem>();
            FillClientList();
            WindowsService.ClientListClosed();
        }

        private void FillClientList()
        {
            List<ClientListItem> items = Repository.GetClientListItems();

            foreach (ClientListItem item in items)
            {
                Items.Add(item);
            }
        }

        public void RefreshClientList()
        {
            Items.Clear();
            FillClientList();
        }

        private RelayCommand createCommand;
        public RelayCommand CreateCommand
        {
            get
            {
                return createCommand ??
                  (createCommand = new RelayCommand(obj =>
                  {
                      WindowsService.CallCreatingClientForm();
                  }));
            }
        }

        private RelayCommand deleteCommand;
        public RelayCommand DeleteCommand
        {
            get
            {
                return deleteCommand ??
                  (deleteCommand = new RelayCommand(obj =>
                  {
                      if (!string.IsNullOrEmpty($"{SelectedItem}"))
                      {
                          Repository.DeleteClient(SelectedItem.Id);
                          RefreshClientList();
                      }                      
                  }));
            }
        }

        private RelayCommand openCommand;
        public RelayCommand OpenCommand
        {
            get
            {
                return openCommand ??
                  (openCommand = new RelayCommand(obj =>
                  {
                      if (!string.IsNullOrEmpty($"{SelectedItem}"))
                      {
                          WindowsService.CallClientForm(SelectedItem.Id, SelectedItem.OrgForm);                          
                      }
                  }));
            }
        }
    }

    internal class CreatingClientFormVM
    {
        public bool JP { get; set; }
        public bool PP { get; set; }
        public bool IB { get; set; }

        public string JuridicalPerson { get { return "Юридическое лицо"; } }
        public string PhysicalPerson { get { return "Физическое лицо"; } }
        public string IndividualBusinessman { get { return "Индивидуальный предприниматель"; } }        

        private RelayCommand oKCommand;
        public RelayCommand OKCommand
        {
            get
            {
                return oKCommand ??
                  (oKCommand = new RelayCommand(obj =>
                  {
                      if (JP == true) { WindowsService.CallNewClientForm(JuridicalPerson); }

                      if (PP == true) { WindowsService.CallNewClientForm(PhysicalPerson); }

                      if (IB == true) { WindowsService.CallNewClientForm(IndividualBusinessman); }

                      if (JP == true || PP == true || IB == true)
                      {
                          WindowsService.CloseCreatingClientForm();
                      }

                  }));
            }
        }
    }

    internal class ClientFormsVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

        private string id;
        public string Id
        {
            get { return id; }
            set
            {
                id = value;
                OnPropertyChanged($"{nameof(Id)}");
            }
        }

        private string fullName;
        public string FullName
        {
            get { return fullName; }
            set
            {
                fullName = value;
                OnPropertyChanged($"{nameof(FullName)}");
            }
        }

        public string OrgForm { get; set; }
        public string Surname { get; set; }
        public string Name { get; set; }
        public string Patronymic { get; set; }
        public string INN { get; set; }
        public string KPP { get; set; }
        public string PhoneNumber { get; set; }
        public string PassportSeries { get; set; }
        public string PassportNumber { get; set; }
        public bool VIP { get; set; }
        public bool FieldsIsEnabled { get; set; }

        private ObservableCollection<Account> accounts;
        public ObservableCollection<Account> Accounts
        {
            get { return accounts; }
            set
            {
                accounts = value;
                OnPropertyChanged($"{nameof(Accounts)}");
            }
        }

        public Account SelectedAccount { get; set; }        

        public ClientFormsVM(Client client)
        {
            Accounts = new ObservableCollection<Account>();           

            if (!string.IsNullOrEmpty($"{client}"))
            {
                Id = client.Id;
                OrgForm = client.OrgForm;
                Name = client.Name;
                INN = client.INN;
                PhoneNumber = client.PhoneNumber;
                VIP = client.VIP;
                FullName = client.FullName;

                if (client is JuridicalPerson jp)
                {
                    KPP = jp.KPP;
                }

                if (client is PhysicalPerson pp)
                {
                    Surname = pp.Surname;
                    Patronymic = pp.Patronymic;
                    PassportSeries = pp.PassportSeries;
                    PassportNumber = pp.PassportNumber;
                }

                FillAccounts(client.Id);
                Repository.SetCurrentClient(client);
            }
        }

        public ClientFormsVM(string orgForm)
        {
            OrgForm = orgForm;
            FieldsIsEnabled = true;
        }

        private void FillAccounts(string clientId)
        {
            List<Account> accounts = Repository.GetAccounts(clientId);

            foreach (Account account in accounts)
            {
                if (account is DepositAccount depAcc)
                {
                    if (depAcc.OverwritingIsRequired == true)
                    {
                        Repository.OwerwriteAccount(depAcc);
                    }
                }

                Accounts.Add(account);
            }
        }

        public void RefreshAccounts()
        {
            Accounts = new ObservableCollection<Account>();
            FillAccounts(Id);            
        }

        private void EliminateNull()
        {
            Surname = (Surname == null) ? String.Empty : Surname.Trim();
            Name = (Name == null) ? String.Empty : Name.Trim();
            Patronymic = (Patronymic == null) ? String.Empty : Patronymic.Trim();
            INN = (INN == null) ? String.Empty : INN.Trim();
            KPP = (KPP == null) ? String.Empty : KPP.Trim();
            PhoneNumber = (PhoneNumber == null) ? String.Empty : PhoneNumber.Trim();
            PassportSeries = (PassportSeries == null) ? String.Empty : PassportSeries.Trim();
            PassportNumber = (PassportNumber == null) ? String.Empty : PassportNumber.Trim();
        }

        private bool CheckRequiredFields()
        {
            bool status = true;

            EliminateNull();

            if (OrgForm == "Физическое лицо")
            {
                if (Surname.Length == 0 || Name.Length == 0 || PhoneNumber.Length == 0 ||
                PassportSeries.Length == 0 || PassportNumber.Length == 0)
                {
                    WindowsService.CallMessageBox("Пустыми могут быть только поля \"Отчество\" и \"ИНН\"");
                    status = false;
                }
            }

            if (OrgForm == "Индивидуальный предприниматель")
            {
                if (Surname.Length == 0 || Name.Length == 0 || INN.Length == 0 || PhoneNumber.Length == 0 ||
                PassportSeries.Length == 0 || PassportNumber.Length == 0)
                {
                    WindowsService.CallMessageBox("Пустым может быть только поле \"Отчество\"");
                    status = false;
                }
            }

            if (OrgForm == "Юридическое лицо")
            {
                if (Name.Length == 0 || INN.Length == 0 || KPP.Length == 0 || PhoneNumber.Length == 0)
                {
                    WindowsService.CallMessageBox("Все поля должны быть заполнены");
                    status = false;
                }
            }

            return status;
        }

        private Client GetClientFromForm()
        {
            Client client = new JuridicalPerson();

            if (OrgForm == "Юридическое лицо")
            {
                client = new JuridicalPerson(Repository.AssignOrReadId(Id, client.GetIdFilePath()),
                    OrgForm, Name, INN, KPP, PhoneNumber, VIP);
            }
            else
            {
                client = new PhysicalPerson(Repository.AssignOrReadId(Id, client.GetIdFilePath()),
                    OrgForm, Surname, Name, Patronymic, INN, PhoneNumber, PassportSeries, PassportNumber, VIP);
            }

            Id = client.Id;

            return client;
        }

        private void OwerwriteOrWriteClient()
        {
            Client client = GetClientFromForm();
            Repository.OwerwriteOrWriteClient(client);

            Repository.SetCurrentClient(client);           
        }

        private RelayCommand changeLogCommand;
        public RelayCommand ChangeLogCommand
        {
            get
            {
                return changeLogCommand ??
                  (changeLogCommand = new RelayCommand(obj =>
                  {
                      if (!string.IsNullOrEmpty($"{Id}"))
                      {
                          WindowsService.CallChangeLog(Id);
                      }
                  }));
            }
        }

        private RelayCommand writeCommand;
        public RelayCommand WriteCommand
        {
            get
            {
                return writeCommand ??
                  (writeCommand = new RelayCommand(obj =>
                  {
                      bool status = CheckRequiredFields();

                      if (status == true)
                      {
                          OwerwriteOrWriteClient();
                      }
                  }));
            }
        }

        private RelayCommand oKCommand;
        public RelayCommand OKCommand
        {
            get
            {
                return oKCommand ??
                  (oKCommand = new RelayCommand(obj =>
                  {
                      bool status = CheckRequiredFields();

                      if (status == true)
                      {
                          OwerwriteOrWriteClient();

                          WindowsService.CloseClientForm();
                      }

                  }));
            }
        }

        private RelayCommand createCommand;
        public RelayCommand CreateCommand
        {
            get
            {
                return createCommand ??
                  (createCommand = new RelayCommand(obj =>
                  {
                      if (!string.IsNullOrEmpty(Id))
                      {
                          WindowsService.CallCreatingAccountForm(Id, OrgForm, VIP);
                      }

                  }));
            }
        }

        private RelayCommand deleteCommand;
        public RelayCommand DeleteCommand
        {
            get
            {
                return deleteCommand ??
                  (deleteCommand = new RelayCommand(obj =>
                  {
                      if (!string.IsNullOrEmpty(Id))
                      {
                          if (!string.IsNullOrEmpty($"{SelectedAccount}"))
                          {
                              Repository.CloseAccount(SelectedAccount);

                              RefreshAccounts();
                          }
                      }
                  }));
            }
        }

        private RelayCommand openCommand;
        public RelayCommand OpenCommand
        {
            get
            {
                return openCommand ??
                  (openCommand = new RelayCommand(obj =>
                  {
                      if (!string.IsNullOrEmpty($"{SelectedAccount}"))
                      {
                          WindowsService.CallAccountForm(SelectedAccount);                          
                      }
                  }));
            }
        }
    }

    internal class CreatingAccountFormVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

        public ObservableCollection<AccountType> AccountTypes { get; set; }
        public AccountType SelectedType { get; set; }
        public string OrgForm { get; set; }
        public string ClientId { get; set; }
        public bool VIP { get; set; }

        public string AccountId { get; set; }

        private string balance;
        public string Balance
        {
            get { return balance; }
            set
            {
                balance = Repository.GetOnlyConvertibleIntoDecimal(value);
                OnPropertyChanged($"{nameof(Balance)}");
            }
        }

        public class AccountType
        {
            public string Type { get; set; }
            public AccountType(string Type) { this.Type = Type; }
        }

        public CreatingAccountFormVM(string clientId, string orgForm, bool VIP)
        {
            ClientId = clientId;
            OrgForm = orgForm;
            this.VIP = VIP;

            AccountTypes = new ObservableCollection<AccountType>();

            AccountType type = new AccountType("Депозитный");
            AccountTypes.Add(type);

            type = new AccountType("Недепозитный");
            AccountTypes.Add(type);
        }

        private Account GetAccountFromForm()
        {
            Account account;

            if (SelectedType.Type == "Депозитный")
            {
                account = new DepositAccount(SelectedType.Type, ClientId, OrgForm, Convert.ToDecimal(Balance), VIP);
            }
            else
            {
                account = new NonDepositAccount(SelectedType.Type, ClientId, OrgForm, Convert.ToDecimal(Balance));
            }

            return account;
        }

        private RelayCommand oKCommand;
        public RelayCommand OKCommand
        {
            get
            {
                return oKCommand ??
                  (oKCommand = new RelayCommand(obj =>
                  {
                      if (SelectedType is AccountType)
                      {
                          if (Balance == null) { Balance = "0,00"; }

                          if (decimal.TryParse(Balance, out var result))
                          {
                              if (!Balance.Contains(',')) { Balance += ",00"; }

                              Account account = GetAccountFromForm();
                              Repository.OpenAccount(account);

                              WindowsService.CloseCreatingAccountForm();
                              WindowsService.CloseAccountForm();
                              WindowsService.CurrentClientFormsVM.RefreshAccounts();
                          }
                      }
                  }));
            }
        }
    }

    internal class ReplenishAccountFormVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
               
        public string AccountNumber { get; set; }

        private string balance;
        public string Balance
        {
            get { return balance; }
            set
            {
                balance = Repository.GetOnlyConvertibleIntoDecimal(value);
                OnPropertyChanged($"{nameof(Balance)}");
            }
        }

        public ReplenishAccountFormVM()
        {
            TransferInfo.SetSenderAccount(Repository.CurrentAccount);
            AccountNumber = TransferInfo.SenderAccount.AccountNumber;            
        }

        private RelayCommand oKCommand;
        public RelayCommand OKCommand
        {
            get
            {
                return oKCommand ??
                  (oKCommand = new RelayCommand(obj =>
                  {
                      if (decimal.TryParse(Balance, out var result))
                      {
                          if (result > 0)
                          {
                              Repository.DepositAccount(TransferInfo.SenderAccount, result);

                              TransferInfo.ResetTransferInfo();
                              WindowsService.CloseReplenishAccountForm();
                              WindowsService.CloseAccountForm();
                              WindowsService.CurrentClientFormsVM.RefreshAccounts();
                          }                          
                      }
                  }));
            }
        }
    }

    internal class AccountFormsVM
    {     
        public string AccountType { get; set; }
        public string AccountNumber { get; set; }
        public string OpeningDate { get; set; }
        public string Balance { get; set; }
        public string InterestRate { get; set; }
        public string DateOfNextAccrual { get; set; }
        public string AccrualInCurrentMonth { get; set; }

        public AccountFormsVM(Account account)
        {
            AccountType = account.AccountType;
            AccountNumber = account.AccountNumber;
            OpeningDate = (account.OpeningDate).ToShortDateString();
            Balance = account.Balance.ToString();

            if (account is DepositAccount depositAccount)
            {
                InterestRate = depositAccount.InterestRate.ToString();
                DateOfNextAccrual = (depositAccount.DateOfNextAccrual).ToShortDateString();
                AccrualInCurrentMonth = depositAccount.AccrualInCurrentMonth.ToString();
            }

            Repository.SetCurrentAccount(account);            
        }       
        
        private RelayCommand transferCommand;
        public RelayCommand TransferCommand
        {
            get
            {
                return transferCommand ??
                  (transferCommand = new RelayCommand(obj =>
                  {
                      WindowsService.CallTransferForm(false);
                  }));
            }
        }

        private RelayCommand transferToYourselfCommand;
        public RelayCommand TransferToYourselfCommand
        {
            get
            {
                return transferToYourselfCommand ??
                  (transferToYourselfCommand = new RelayCommand(obj =>
                  {
                      WindowsService.CallTransferForm(true);
                  }));
            }
        }

        private RelayCommand depositCommand;
        public RelayCommand DepositCommand
        {
            get
            {
                return depositCommand ??
                  (depositCommand = new RelayCommand(obj =>
                  {
                      WindowsService.CallReplenishAccountForm();
                  }));
            }
        }
    }

    internal class TransferFormVM : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

        private string whom;
        public string Whom
        {
            get { return whom; }
            set
            {
                whom = value;
                OnPropertyChanged($"{nameof(Whom)}");
            }
        }

        private string where;
        public string Where
        {
            get { return where; }
            set
            {
                where = value;
                OnPropertyChanged($"{nameof(Where)}");
            }
        }

        private string amount;
        public string Amount
        {
            get { return amount; }
            set
            {
                amount = Repository.GetOnlyConvertibleIntoDecimal(value);
                OnPropertyChanged($"{nameof(Amount)}");
            }
        }

        public bool WhomIsEnabled { get; set; } 
        
        public TransferFormVM(bool transferToYourself)
        {
            TransferInfo.SetSender(Repository.CurrentClient);
            TransferInfo.SetSenderAccount(Repository.CurrentAccount);

            if (transferToYourself == true)
            {                
                TransferInfo.SetRecipient(Repository.CurrentClient);
                Whom = TransferInfo.Recipient.FullName;
                WhomIsEnabled = false;
            }
            else
            {
                WhomIsEnabled = true;
            }
        }

        public void WhomToTransfer_Closed(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty($"{TransferInfo.Recipient}"))
            {                
                Whom = TransferInfo.Recipient.FullName;
                Where = String.Empty;
            }
        }

        public void WhereToTransfer_Closed(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty($"{TransferInfo.RecipientAccount}"))
            {
                Where = TransferInfo.RecipientAccount.AccountNumber;
            }
        }

        private RelayCommand whomCommand;
        public RelayCommand WhomCommand
        {
            get
            {
                return whomCommand ??
                  (whomCommand = new RelayCommand(obj =>
                  {
                      WindowsService.CallWhomToTransfer();                         
                  }));
            }
        }

        private RelayCommand whereCommand;
        public RelayCommand WhereCommand
        {
            get
            {
                return whereCommand ??
                  (whereCommand = new RelayCommand(obj =>
                  {
                      if (!string.IsNullOrEmpty($"{TransferInfo.Recipient}"))
                      {
                          WindowsService.CallWhereToTransfer();                          
                      }
                  }));
            }
        }

        private RelayCommand transferCommand;
        public RelayCommand TransferCommand
        {
            get
            {
                return transferCommand ??
                  (transferCommand = new RelayCommand(obj =>
                  {
                      if (!string.IsNullOrEmpty($"{Whom}") &&
                          !string.IsNullOrEmpty($"{Where}") &&
                          !string.IsNullOrEmpty($"{Amount}"))
                      {
                          if (decimal.TryParse(Amount, out var result))
                          {
                              if (result > 0)
                              {
                                  TransferService<Account> ts = new TransferService<Account>();
                                  ts.Transfer(TransferInfo.SenderAccount, TransferInfo.RecipientAccount, result);

                                  WindowsService.CloseTransferForm();
                              }                              
                          }
                      }
                  }));
            }
        }
    }

    internal class WhomToTransferVM
    {
        public ObservableCollection<ClientListItem> Items { get; set; }
        public ClientListItem SelectedItem { get; set; }

        public WhomToTransferVM()
        {
            Items = new ObservableCollection<ClientListItem>();
            List<ClientListItem> items = Repository.GetClientListItems();

            foreach (ClientListItem item in items)
            {
                if (item.Id == TransferInfo.Sender.Id)
                {                    
                    continue;
                }

                Items.Add(item);
            }
        }

        private RelayCommand selectionCommand;
        public RelayCommand SelectionCommand
        {
            get
            {
                return selectionCommand ??
                  (selectionCommand = new RelayCommand(obj =>
                  {
                      if (!string.IsNullOrEmpty($"{SelectedItem}"))
                      {
                          TransferInfo.SetRecipient(SelectedItem);
                          WindowsService.CloseWhomToTransfer();
                      }

                  }));
            }
        }
    }

    internal class WhereToTransferVM
    {
        public ObservableCollection<Account> Accounts { get; set; }
        public Account SelectedAccount { get; set; }

        public WhereToTransferVM()
        {
            Accounts = new ObservableCollection<Account>();
            List<Account> accounts = Repository.GetAccounts(TransferInfo.Recipient.Id);

            foreach (Account account in accounts)
            {
                if (TransferInfo.Recipient.Id == TransferInfo.Sender.Id)
                {
                    if (account.Id == TransferInfo.SenderAccount.Id)
                    {
                        continue;
                    }
                }

                Accounts.Add(account);
            }       
        }

        private RelayCommand selectionCommand;
        public RelayCommand SelectionCommand
        {
            get
            {
                return selectionCommand ??
                  (selectionCommand = new RelayCommand(obj =>
                  {
                      if (!string.IsNullOrEmpty($"{SelectedAccount}"))
                      {
                          TransferInfo.SetRecipientAccount(SelectedAccount);
                          WindowsService.CloseWhereToTransfer();
                      }
                  }));
            }
        }
    }

    internal class ChangeLogVM
    {
        public ObservableCollection<string> Records { get; set; }
        public ChangeLogVM(string clientId)
        {
            Records = new ObservableCollection<string>();

            List<string> records = Repository.GetChangeLog(clientId);

            foreach (string record in records)
            {
                Records.Add(record);
            }
        }
    }
}
