using System.Windows;

namespace Delegates
{
    public partial class TransferForm : Window
    {
        internal TransferForm(bool transferToYourself)
        {
            InitializeComponent();
            TransferFormVM transferFormVM = new TransferFormVM(transferToYourself);
            DataContext = transferFormVM;
            WindowsService.CurrentTransferFormVM = transferFormVM;
        }
    }
}
