using System.Windows;

namespace Delegates
{
    public partial class WhomToTransfer : Window
    {
        internal WhomToTransfer()
        {
            InitializeComponent();
            DataContext = new WhomToTransferVM();
        }
    }
}
