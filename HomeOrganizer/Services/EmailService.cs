using SendGrid;
using SendGrid.Extensions.DependencyInjection;
using SendGrid.Helpers.Mail;

namespace HomeOrganizer.Services
{
    public class EmailService
    {
        private static IConfiguration Configuration { get; set; }

        public async Task SendEmail(string emailTo, string message)
        {
            var env = Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT") ?? "Production";
            Configuration = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json", optional: true)
                    .AddJsonFile($"appsettings.{env}.json", optional: true)
                    .Build();
            var services = ConfigureServices(new ServiceCollection()).BuildServiceProvider();
            var client = services.GetRequiredService<ISendGridClient>();
            var from = new EmailAddress(Configuration.GetValue("SendGrid:From", SettingsVar.EmailLetterConfirm), "Sushi Admin");
            var to = new EmailAddress(Configuration.GetValue("SendGrid:To", emailTo), "Dear New User");
            var msg = new SendGridMessage
            {
                From = from,
                Subject = "Sushi Email Confirmation"
            };
            string msgBody = $"<div><h2>To confirm your Email tapp the link below: </h2><br/><a href=\"{message}\"><h1>Link</h1></a></div>";

            msg.HtmlContent = msgBody;
            msg.AddTo(to);

            if (Configuration.GetValue("SendGrid:SandboxMode", false))
            {
                msg.MailSettings = new MailSettings
                {
                    SandboxMode = new SandboxMode
                    {
                        Enable = true
                    }
                };
            }
            Console.WriteLine($"Sending email with payload: \n{msg.Serialize()}");
            var response = await client.SendEmailAsync(msg).ConfigureAwait(false);

            Console.WriteLine($"Response: {response.StatusCode}");
            Console.WriteLine(response.Headers);
        }
        private static IServiceCollection ConfigureServices(IServiceCollection services)
        {
            services.AddSendGrid(options => { options.ApiKey = Environment.GetEnvironmentVariable("SENDGRID_API_KEY") ?? Configuration["SendGrid_ApiKey"]; });

            return services;
        }
    }
}
