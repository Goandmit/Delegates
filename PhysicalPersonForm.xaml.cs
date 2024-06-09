using System.Windows;

namespace Delegates
{
    public partial class PhysicalPersonForm : Window
    {
        internal PhysicalPersonForm(PhysicalPerson client)
        {
            InitializeComponent();
            DataContext = Repository.CurrentUser.GetClientFormsVM(client);
        }

        internal PhysicalPersonForm(string orgForm)
        {
            InitializeComponent();
            ClientFormsVM clientFormsVM = new ClientFormsVM(orgForm);
            WindowsService.CurrentClientFormsVM = clientFormsVM;
            DataContext = clientFormsVM;
        }
    }
}
