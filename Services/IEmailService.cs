using MailKit;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Stories.Services
{
    public interface IEmailService
    {
        public Task SendEmailAsync(string email, string subject, string body);
    }

    public class EmailService : IEmailService
    {
        private readonly AppSettings _appSettings;
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IOptions<AppSettings> smtpSettings, IWebHostEnvironment env, ILogger<EmailService> logger)
        {
            _appSettings = smtpSettings.Value;
            _env = env;
            _logger = logger;
        }

        public async Task SendEmailAsync(string email, string subject, string body)
        {
            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("Stories", "stories@lutonet.com"));
                message.To.Add(new MailboxAddress("", email));
                message.Subject = subject;

                message.Body = new TextPart("html")
                {
                    Text = body
                };

                using (var client = new SmtpClient(new ProtocolLogger("SmtpLog.log")))
                {
                    await client.ConnectAsync("lutonet.com", 587, SecureSocketOptions.Auto);
                    await client.AuthenticateAsync("stories@lutonet.com", "mOtherb0ard");
                    try
                    {
                        await client.SendAsync(message);
                    }
                    catch (SmtpCommandException ex)
                    {
                        Console.WriteLine("Error sending message: {0}", ex.Message);
                        Console.WriteLine("\tStatusCode: {0}", ex.StatusCode);
                        _logger.LogError("Sending mail to " + email + "failed with error: " + ex.StatusCode, ex);
                        switch (ex.ErrorCode)
                        {
                            case SmtpErrorCode.RecipientNotAccepted:
                                Console.WriteLine("\tRecipient not accepted: {0}", ex.Mailbox);
                                break;

                            case SmtpErrorCode.SenderNotAccepted:
                                Console.WriteLine("\tSender not accepted: {0}", ex.Mailbox);
                                break;

                            case SmtpErrorCode.MessageNotAccepted:
                                Console.WriteLine("\tMessage not accepted.");
                                break;
                        }
                    }
                    catch (SmtpProtocolException ex)
                    {
                        Console.WriteLine("Protocol error while sending message: {0}", ex.Message);
                        _logger.LogError("Error sending email due to protocol error", ex);
                    }

                    Console.WriteLine("Email To: " + email + " sent Successfully");
                    _logger.LogInformation("Email To: " + email + " sent Successfully");
                    await client.DisconnectAsync(true);
                }
            }
            catch (Exception ex)
            {
                Console.Write(ex.ToString());
                throw new InvalidOperationException(ex.Message);
            }
        }
    }

    public static class MessageDeliveryStatusCheck
    {
        public static void ProcessDeliveryStatusNotification(MimeMessage message)
        {
            var report = message.Body as MultipartReport;

            if (report == null || report.ReportType == null || !report.ReportType.Equals("delivery-status", StringComparison.OrdinalIgnoreCase))
            {
                Console.Write(report.Count);
                return;
            }

            // process the report
            foreach (var mds in report.OfType<MessageDeliveryStatus>())
            {
                // process the status groups - each status group represents a different recipient

                // The first status group contains information about the message
                var envelopeId = mds.StatusGroups[0]["Original-Envelope-Id"];

                // all of the other status groups contain per-recipient information
                for (int i = 1; i < mds.StatusGroups.Count; i++)
                {
                    var recipient = mds.StatusGroups[i]["Original-Recipient"];
                    var action = mds.StatusGroups[i]["Action"];

                    if (recipient == null)
                        recipient = mds.StatusGroups[i]["Final-Recipient"];

                    // the recipient string should be in the form: "rfc822;user@domain.com"
                    var index = recipient.IndexOf(';');
                    var address = recipient.Substring(index + 1);

                    switch (action)
                    {
                        case "failed":
                            Console.WriteLine("Delivery of message {0} failed for {1}", envelopeId, address);
                            break;

                        case "delayed":
                            Console.WriteLine("Delivery of message {0} has been delayed for {1}", envelopeId, address);
                            break;

                        case "delivered":
                            Console.WriteLine("Delivery of message {0} has been delivered to {1}", envelopeId, address);
                            break;

                        case "relayed":
                            Console.WriteLine("Delivery of message {0} has been relayed for {1}", envelopeId, address);
                            break;

                        case "expanded":
                            Console.WriteLine("Delivery of message {0} has been delivered to {1} and relayed to the the expanded recipients", envelopeId, address);
                            break;
                    }
                    Console.WriteLine("Script over");
                }
            }
        }
    }
}