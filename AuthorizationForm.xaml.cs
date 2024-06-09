using System.Windows;

namespace Delegates
{    
    public partial class AuthorizationForm : Window
    {
        public AuthorizationForm()
        {
            InitializeComponent();
            DataContext = new AuthorizationFormVM();
        }
    }
}
