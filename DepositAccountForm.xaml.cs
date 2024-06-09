using System.Windows;

namespace Delegates
{    
    public partial class DepositAccountForm : Window
    {
        internal DepositAccountForm(Account account)
        {
            InitializeComponent();
            DataContext = new AccountFormsVM(account);
        }
    }
}
