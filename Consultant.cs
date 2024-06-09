namespace Delegates
{
    internal class Consultant : Worker
    {
        public Consultant()
        {
            Position = "Консультант";
        }

        public override ClientFormsVM GetClientFormsVM(Client client)
        {
            ClientFormsVM clientFormsVM = base.GetClientFormsVM(client);

            if (client is PhysicalPerson pp)
            {
                clientFormsVM.PassportSeries = HideString(pp.PassportSeries);
                clientFormsVM.PassportNumber = HideString(pp.PassportNumber);
            }            

            return clientFormsVM;
        }
    }
}
