using System;
using MailKit.Net.Imap;
using MailKit;
using MimeKit;
using System.Threading.Tasks;

namespace emailReader
{
    public class GmailReader
    {
        private ImapClient client;
        bool authOk;

        public GmailReader()
        {
                 client = new ImapClient();
        }
        public async Task<bool> ConectarAsync(string email, string password)
        {
            try
            {
                await client.ConnectAsync("imap.gmail.com", 993, true);
                await client.AuthenticateAsync(email, password);
                authOk = true;
                Console.WriteLine("auth ok!");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error de autenticación: " + ex.Message);
                return false;
            }
        }
        public async Task<string> LeerUltimoEmail()
        {


            if (!authOk)
            {
                Console.WriteLine("no auth");
                return "";
            }

            try
            {
                

                var inbox = client.Inbox;
                await inbox.OpenAsync(FolderAccess.ReadOnly);

                if (inbox.Count == 0)
                {
                    Console.WriteLine("La bandeja de entrada está vacía.");
                    return "";
                }

                var message = await inbox.GetMessageAsync(inbox.Count - 1);

                return message.HtmlBody;

                await client.DisconnectAsync(true);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al leer el correo: " + ex.Message);
                return "";
            }
        }
    }
}
