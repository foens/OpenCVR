using System;

namespace OpenCVR.Update.Email
{
    internal class CvrEmail
    {
        public string Id { get; set; }
        public DateTime DateTimeReceived { get; set; }
        public string Text { get; set; }

        public CvrEmail(string id, DateTime dateTimeReceived, string text)
        {
            Id = id;
            DateTimeReceived = dateTimeReceived;
            Text = text;
        }
    }
}