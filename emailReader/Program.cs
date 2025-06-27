using System.Net;
using System.Net;
using System.IO;

namespace emailReader
{

    internal class Program
    {
        static GmailReader gmailReader;

        private static async Task Main(string[] args)
        {

            if (args.Count() != 3)
            {
                Console.WriteLine("cantidad de argumentos invalidos");
                Console.WriteLine("puerto email pass");
                Console.WriteLine("si el pass tiene espacios tiene que ir entre comillas dobles");
                return;
            }

            int httpPort = Convert.ToInt32(args[0]);
            string emailUser = args[1];
            string emailPass = args[2];

            string path = "messages";

            Console.WriteLine("iniciacion gmail reader");
            Console.WriteLine("iniciando auth en email ");

            gmailReader = new GmailReader();
            bool conectado = await gmailReader.ConectarAsync(args[1], args[2]);

            var builder = WebApplication.CreateBuilder(args);
            builder.WebHost
            .UseSetting(WebHostDefaults.SuppressStatusMessagesKey, "True")
            .ConfigureKestrel((context, serverOptions) =>
            {
                serverOptions.Listen(IPAddress.Any, httpPort);
                //serverOptions.Listen(IPAddress.Any, httpsPort, listenOptions =>
                //{
                //    listenOptions.UseHttps();//nada de certificados.... por ahora-
                //});
            });

            var app = builder.Build();

            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapGet("messages", ReadMessage);
                endpoints.MapGet("/messages/{file}", ReadMessage);
                endpoints.MapDelete("messages", DelMessages);


            });
            Console.WriteLine("ready on line! on port> " + httpPort);
            app.Run();
        }

        private static async Task ReadMessage(HttpContext context)
        {
            var file = context.Request.RouteValues["file"]?.ToString(); // null si es solo /messages

            if (!string.IsNullOrEmpty(file))
            {

                string htmEmail = await gmailReader.LeerUltimoEmail();
                context.Response.ContentType = "text/html";
                await context.Response.WriteAsync(htmEmail);
            }
            else
            {

                string dbresponse = "todo ok, ahora voy a leer tu email de prueba";
                var response = new { message = dbresponse };
                context.Response.ContentType = "application/json";
                var jsonResponse = System.Text.Json.JsonSerializer.Serialize(response);
                await context.Response.WriteAsync(jsonResponse);
            }

        }
        private static async Task DelMessages(HttpContext context)
        {
            context.Response.StatusCode = StatusCodes.Status204NoContent;
            await Task.CompletedTask;
        }

    }

}