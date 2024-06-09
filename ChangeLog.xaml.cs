using System.Windows;

namespace Delegates
{    
    public partial class ChangeLog : Window
    {
        internal ChangeLog(string clientId)
        {
            InitializeComponent();
            DataContext = new ChangeLogVM(clientId);
        }
    }
}
