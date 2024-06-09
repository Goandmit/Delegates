using System.Windows;

namespace Delegates
{
    public partial class ReplenishAccountForm : Window
    {
        internal ReplenishAccountForm()
        {
            InitializeComponent();
            DataContext = new ReplenishAccountFormVM();
        }
    }
}
