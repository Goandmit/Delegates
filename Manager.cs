namespace Delegates
{
    internal class Manager : Worker
    {
        public Manager()
        {
            Position = "Менеджер";
        }

        public override ClientListVM GetClientListVM()
        {
            ClientListVM clientListVM = base.GetClientListVM();

            clientListVM.ButtonsIsEnabled = true;            

            return clientListVM;
        }

        public override ClientFormsVM GetClientFormsVM(Client client)
        {
            ClientFormsVM clientFormsVM = base.GetClientFormsVM(client);

            clientFormsVM.FieldsIsEnabled = true;

            return clientFormsVM;
        }
    }
}
