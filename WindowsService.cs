using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using MessageBox = System.Windows.MessageBox;
using MessageBoxOptions = System.Windows.MessageBoxOptions;

namespace Delegates
{
    internal static class WindowsService
    {
        public static double WorkingAreaWidth { get; set; }
        public static double WorkingAreaHeight { get; set; }

        public static ClientListVM CurrentClientListVM { get; set; }
        public static ClientFormsVM CurrentClientFormsVM { get; set; }
        public static TransferFormVM CurrentTransferFormVM { get; set; }        

        private static async Task ClosingTasks(Notification window)
        {
            await Task.Delay(2500);
            window.Close();
        }

        private static async void CloseNotification(Notification window)
        {
            await ClosingTasks(window);
        }        

        private static void ClientList_Closed(object sender, EventArgs e)
        {
            foreach (Window window in App.Current.Windows)
            {
                window.Close();
            }
        }

        private static void CreatingClientForm_Closed(object sender, EventArgs e)
        {
            foreach (Window window in App.Current.Windows)
            {
                if (window is JuridicalPersonForm ||
                window is PhysicalPersonForm)
                {
                    window.Closed += ClientForm_Closed;
                }
            }
        }

        private static void ClientForm_Closed(object sender, EventArgs e)
        {
            Repository.ResetCurrentClient();

            CurrentClientListVM.RefreshClientList();

            foreach (Window window in App.Current.Windows)
            {
                if (window is ClientList)
                {
                    window.Activate();
                }
            }
        }

        private static void CreatingOrReplenishAccountForm_Closed(object sender, EventArgs e)
        {
            CurrentClientFormsVM.RefreshAccounts();
        }

        private static void TransferForm_Closed(object sender, EventArgs e)
        {
            CurrentClientFormsVM.RefreshAccounts();
            TransferInfo.ResetTransferInfo();
        }        

        public static void DefineWorkingAreaSize()
        {
            WorkingAreaWidth = SystemInformation.WorkingArea.Width;
            WorkingAreaHeight = SystemInformation.WorkingArea.Height;
        }

        public static void CallMessageBox(string text)
        {
            MessageBox.Show(text,
                        "Операция не выполнена",
                        MessageBoxButton.OK,
                        MessageBoxImage.Error,
                        MessageBoxResult.OK,
                        MessageBoxOptions.DefaultDesktopOnly);
        }

        public static void CallNotification(string text)
        {
            Notification window = new Notification(text);
            window.Show();
            CloseNotification(window);
        }

        public static void CallClientList(Worker worker)
        {
            Repository.SetCurrentUser(worker);

            ClientList clientList = new ClientList();
            clientList.Show();

            foreach (Window window in App.Current.Windows)
            {
                if (window is AuthorizationForm)
                {
                    window.Close();
                }
            }
        }

        public static void CallCreatingClientForm()
        {
            CreatingClientForm window = new CreatingClientForm();
            window.Show();

            window.Closed += CreatingClientForm_Closed;
        }

        public static void CallClientForm(string clientId, string orgForm)
        {
            Window clientForm;

            if (orgForm == "Юридическое лицо")
            {
                JuridicalPerson jp = Repository.ClientServiceJP.GetClient(clientId);
                clientForm = new JuridicalPersonForm(jp);
            }
            else
            {
                PhysicalPerson pp = Repository.ClientServicePP.GetClient(clientId);
                clientForm = new PhysicalPersonForm(pp);
            }

            clientForm.Show();

            BindToClientList(clientForm);
        }

        public static void CallNewClientForm(string orgForm)
        {
            Window clientForm = null;

            switch (orgForm)
            {
                case "Юридическое лицо":
                    clientForm = new JuridicalPersonForm(orgForm);
                    break;
                case "Физическое лицо":
                case "Индивидуальный предприниматель":
                    clientForm = new PhysicalPersonForm(orgForm);
                    break;
            }

            if (clientForm != null)
            {
                clientForm.Show();

                BindToClientList(clientForm);
            }
        }       

        public static void CallCreatingAccountForm(string clientId, string orgForm, bool VIP)
        {
            CreatingAccountForm window = new CreatingAccountForm(clientId, orgForm, VIP);
            window.Show();

            window.Closed += CreatingOrReplenishAccountForm_Closed;
        }

        public static void CallReplenishAccountForm()
        {
            ReplenishAccountForm window = new ReplenishAccountForm();
            window.Show();

            window.Closed += CreatingOrReplenishAccountForm_Closed;
        }

        public static void CallAccountForm(Account account)
        {
            if (account is DepositAccount depAcc)
            {
                DepositAccountForm window = new DepositAccountForm(depAcc);
                window.Show();
            }

            if (account is NonDepositAccount nonDepAcc)
            {
                NonDepositAccountForm window = new NonDepositAccountForm(nonDepAcc);
                window.Show();
            }
        }

        public static void CallTransferForm(bool transferToYourself)
        {
            TransferForm window = new TransferForm(transferToYourself);
            window.Show();

            window.Closed += TransferForm_Closed;

            CloseAccountForm();
        }

        public static void CallWhomToTransfer()
        {
            WhomToTransfer window = new WhomToTransfer();
            window.Show();

            window.Closed += CurrentTransferFormVM.WhomToTransfer_Closed;
        }        

        public static void CallWhereToTransfer()
        {
            WhereToTransfer window = new WhereToTransfer();
            window.Show();

            window.Closed += CurrentTransferFormVM.WhereToTransfer_Closed;
        }              

        public static void CallChangeLog(string clientId)
        {
            ChangeLog changeLog = new ChangeLog(clientId);
            changeLog.Show();

            foreach (Window window in App.Current.Windows)
            {
                if (window is JuridicalPersonForm || window is PhysicalPersonForm)
                {
                    changeLog.Owner = window;
                }
            }
        }
        
        public static void BindToClientList(Window clientForm)
        {
            foreach (Window window in App.Current.Windows)
            {
                if (window is ClientList)
                {
                    clientForm.Owner = window;
                }
            }

            clientForm.Closed += ClientForm_Closed;
        }

        public static void ClientListClosed()
        {
            foreach (Window window in App.Current.Windows)
            {
                if (window is ClientList)
                {
                    window.Closed += ClientList_Closed;
                }
            }
        }

        public static void CloseCreatingClientForm()
        {
            foreach (Window window in App.Current.Windows)
            {
                if (window is CreatingClientForm)
                {
                    window.Close();
                }
            }
        }

        public static void CloseClientForm()
        {
            foreach (Window window in App.Current.Windows)
            {
                if (window is JuridicalPersonForm || window is PhysicalPersonForm)
                {
                    window.Close();
                }
            }
        }

        public static void CloseCreatingAccountForm()
        {
            foreach (Window window in App.Current.Windows)
            {
                if (window is CreatingAccountForm)
                {
                    window.Close();
                }
            }
        }

        public static void CloseReplenishAccountForm()
        {
            foreach (Window window in App.Current.Windows)
            {
                if (window is ReplenishAccountForm)
                {
                    window.Close();
                }
            }
        }

        public static void CloseAccountForm()
        {
            Repository.ResetCurrentAccount();

            foreach (Window window in App.Current.Windows)
            {
                if (window is DepositAccountForm || window is NonDepositAccountForm)
                {
                    window.Close();
                }
            }
        }

        public static void CloseTransferForm()
        {
            foreach (Window window in App.Current.Windows)
            {
                if (window is TransferForm)
                {
                    window.Close();
                }
            }
        }        

        public static void CloseWhomToTransfer()
        {
            foreach (Window window in App.Current.Windows)
            {
                if (window is WhomToTransfer)
                {
                    window.Close();
                }
            }
        }

        public static void CloseWhereToTransfer()
        {
            foreach (Window window in App.Current.Windows)
            {
                if (window is WhereToTransfer)
                {
                    window.Close();
                }
            }
        }
    }
}
