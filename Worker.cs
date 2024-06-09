using System;

namespace Delegates
{
    internal abstract class Worker
    {
        public string Position { get; protected set; }

        protected string HideString(string normalString)
        {
            string hiddenString = String.Empty;

            for (int i = 0; i < normalString.Length; i++)
            {
                if (normalString[i] != ' ')
                {
                    hiddenString += "*";
                }
                else
                {
                    hiddenString += " ";
                }
            }

            return hiddenString;
        }

        public virtual ClientListVM GetClientListVM()
        {
            ClientListVM clientListVM = new ClientListVM
            {
                ButtonsIsEnabled = false                
            };

            WindowsService.CurrentClientListVM = clientListVM;

            return clientListVM;
        }

        public virtual ClientFormsVM GetClientFormsVM(Client client)
        {
            ClientFormsVM clientFormsVM = new ClientFormsVM(client)
            {
                FieldsIsEnabled = false
            };

            WindowsService.CurrentClientFormsVM = clientFormsVM;

            return clientFormsVM;
        }
    }
}
