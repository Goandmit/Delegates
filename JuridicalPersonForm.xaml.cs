using System.Windows;

namespace Delegates
{
    public partial class JuridicalPersonForm : Window
    {
        internal JuridicalPersonForm(JuridicalPerson client)
        {
            InitializeComponent();
            DataContext = Repository.CurrentUser.GetClientFormsVM(client);
        }

        internal JuridicalPersonForm(string orgForm)
        {
            InitializeComponent();            
            ClientFormsVM clientFormsVM = new ClientFormsVM(orgForm);
            WindowsService.CurrentClientFormsVM = clientFormsVM;
            DataContext = clientFormsVM;
        }
    }
}
