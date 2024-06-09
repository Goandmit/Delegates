using System.Windows;

namespace Delegates
{
    public partial class CreatingAccountForm : Window
    {
        internal CreatingAccountForm(string clientId, string orgForm, bool VIP)
        {
            InitializeComponent();
            DataContext = new CreatingAccountFormVM(clientId, orgForm, VIP);
        }        
    }
}
