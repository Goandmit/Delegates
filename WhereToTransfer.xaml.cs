using System.Windows;

namespace Delegates
{
    public partial class WhereToTransfer : Window
    {
        internal WhereToTransfer()
        {
            InitializeComponent();
            DataContext = new WhereToTransferVM();
        }
    }
}
