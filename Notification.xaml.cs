using System.Windows;

namespace Delegates
{
    public partial class Notification : Window
    {
        internal Notification(string text)
        {
            InitializeComponent();

            Message.Text = text;
            Left = WindowsService.WorkingAreaWidth - Width;
            Top = WindowsService.WorkingAreaHeight - Height;
        }       
    }
}
