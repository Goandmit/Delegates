namespace Delegates
{
    internal static class TransferInfo
    {        
        public static ClientListItem Recipient { get; private set; }
        public static Account RecipientAccount { get; private set; }

        public static ClientListItem Sender { get; private set; }
        public static Account SenderAccount { get; private set; }

        public static void SetSender(ClientListItem sender)
        {
            Sender = sender;
        }

        public static void SetSenderAccount(Account account)
        {
            SenderAccount = account;
        }

        public static void SetRecipient(ClientListItem recipient)
        {
            Recipient = recipient;
        }

        public static void SetRecipientAccount(Account account)
        {
            RecipientAccount = account;
        }

        public static void ResetTransferInfo()
        {
            Sender = new ClientListItem();
            SenderAccount = new NonDepositAccount();

            Recipient = new ClientListItem();
            RecipientAccount = new NonDepositAccount();
        }
    }
}
