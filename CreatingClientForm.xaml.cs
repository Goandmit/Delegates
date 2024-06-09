using System.Windows;

namespace Delegates
{    
    public partial class CreatingClientForm : Window
    {
        internal CreatingClientForm()
        {
            InitializeComponent();           
            DataContext = new CreatingClientFormVM();
        }
    }
}
