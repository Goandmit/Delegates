using System.Windows;

namespace Delegates
{
    public partial class NonDepositAccountForm : Window
    {
        internal NonDepositAccountForm(Account account)
        {
            InitializeComponent();
            DataContext = new AccountFormsVM(account);
        }
    }
}
