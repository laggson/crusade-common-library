namespace Laggson.Common.Notifications
{
    public class MessageItem
    {
        public string Header { get; }
        public string Message1 { get; }
        public string Message2 { get; }
        public string ImagePath { get; set; }

        public MessageItem(string header, string message1)
        {
            Header = header.Trim();
            Message1 = message1.Trim();
            Message2 = "";
        }
        public MessageItem(string header, string message1, string message2)
        {
            Header = header.Trim();
            Message1 = message1.Trim();
            Message2 = message2.Trim();
        }

        public MessageItem(string header, string message1, string message2, string imagePath)
        {
            Header = header.Trim();
            Message1 = message1.Trim();
            Message2 = message2.Trim();
            ImagePath = imagePath?.Trim();
        }
    }
}