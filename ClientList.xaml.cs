using System.Windows;

namespace Delegates
{    
    public partial class ClientList : Window
    {
        internal ClientList()
        {
            InitializeComponent();
            DataContext = Repository.CurrentUser.GetClientListVM();
        }
    }
}
