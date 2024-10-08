using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Qanat.Common.Util;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Qanat.API.Services
{
    public class SitkaSmtpClientService
    {
        private readonly ISendGridClient _sendGridClient;
        private readonly QanatConfiguration _qanatConfiguration;

        private readonly ILogger<SitkaSmtpClientService> _logger;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public SitkaSmtpClientService(ISendGridClient sendGridClient, IOptions<QanatConfiguration> qanatConfiguration, IWebHostEnvironment webHostEnvironment, ILogger<SitkaSmtpClientService> logger)
        {
            _sendGridClient = sendGridClient;
            _qanatConfiguration = qanatConfiguration.Value;
            _logger = logger;
            _webHostEnvironment = webHostEnvironment;
        }

        /// <summary>
        /// Sends an email including mock mode and address redirection  <see cref="QanatConfiguration.SITKA_EMAIL_REDIRECT"/>, then calls onward to <see cref="SendDirectly"/>
        /// </summary>
        /// <param name="message"></param>
        public async Task Send(MailMessage message)
        {
            var messageWithAnyAlterations = AlterMessageIfInRedirectMode(message);
            var messageAfterAlterationsAndCreatingAlternateViews = CreateAlternateViewsIfNeeded(messageWithAnyAlterations);
            await SendDirectly(messageAfterAlterationsAndCreatingAlternateViews);
        }

        private static MailMessage CreateAlternateViewsIfNeeded(MailMessage message)
        {
            if (!message.IsBodyHtml)
            {
                return message;
            }
            // Define the plain text alternate view and add to message
            const string plainTextBody = "You must use an email client that supports HTML messages";

            var plainTextView = AlternateView.CreateAlternateViewFromString(plainTextBody, null, MediaTypeNames.Text.Plain);

            message.AlternateViews.Add(plainTextView);

            // Define the html alternate view with embedded image and
            // add to message. To reference images attached as linked
            // resources from your HTML message body, use "cid:contentID"
            // in the <img> tag...
            var htmlBody = message.Body;

            var htmlView = AlternateView.CreateAlternateViewFromString(htmlBody, null, MediaTypeNames.Text.Html);
            message.AlternateViews.Add(htmlView);


            return message;
        }


        /// <summary>
        /// Sends an email message at a lower level than <see cref="Send"/>, skipping mock mode and address redirection  <see cref="QanatConfiguration.SITKA_EMAIL_REDIRECT"/>
        /// </summary>
        /// <param name="mailMessage"></param>
        public async Task SendDirectly(MailMessage mailMessage)
        {
            var defaultEmailFrom = GetDefaultEmailFrom();
            var sendGridMessage = new SendGridMessage()
            {
                From = new EmailAddress(defaultEmailFrom.Address, defaultEmailFrom.DisplayName),
                Subject = mailMessage.Subject,
                PlainTextContent = mailMessage.Body,
                HtmlContent = mailMessage.IsBodyHtml ? mailMessage.Body : null
            };
            sendGridMessage.AddTos(mailMessage.To.Select(x => new EmailAddress(x.Address, x.DisplayName)).ToList());
            if (mailMessage.CC.Any())
            {
                sendGridMessage.AddCcs(mailMessage.CC.Select(x => new EmailAddress(x.Address, x.DisplayName)).ToList());
            }

            if (mailMessage.Bcc.Any())
            {
                sendGridMessage.AddBccs(mailMessage.Bcc.Select(x => new EmailAddress(x.Address, x.DisplayName)).ToList());
            }

            if (_webHostEnvironment.IsDevelopment())
            {
                sendGridMessage.SetSandBoxMode(true);
            }
            
            var response = await _sendGridClient.SendEmailAsync(sendGridMessage);
            _logger.LogInformation($"Email sent to SendGrid, Details:\r\n{sendGridMessage.PlainTextContent}");
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError($"Error sending email sent to SendGrid, Details:\r\n{response.Body}");
            }
        }

        /// <summary>
        /// Alter message TO, CC, BCC if the setting <see cref="QanatConfiguration.SITKA_EMAIL_REDIRECT"/> is set
        /// Appends the real to the body
        /// </summary>
        /// <param name="realMailMessage"></param>
        /// <returns></returns>
        private MailMessage AlterMessageIfInRedirectMode(MailMessage realMailMessage)
        {
            var redirectEmail = _qanatConfiguration.SITKA_EMAIL_REDIRECT;
            var isInRedirectMode = !String.IsNullOrWhiteSpace(redirectEmail);

            if (!isInRedirectMode)
            {
                return realMailMessage;
            }

            ClearOriginalAddressesAndAppendToBody(realMailMessage, "To", realMailMessage.To);
            ClearOriginalAddressesAndAppendToBody(realMailMessage, "CC", realMailMessage.CC);
            ClearOriginalAddressesAndAppendToBody(realMailMessage, "BCC", realMailMessage.Bcc);

            realMailMessage.To.Add(redirectEmail);

            return realMailMessage;
        }

        private static void ClearOriginalAddressesAndAppendToBody(MailMessage realMailMessage, string addressType, ICollection<MailAddress> addresses)
        {
            var newline = realMailMessage.IsBodyHtml ? "<br />" : Environment.NewLine;
            var separator = newline + "\t";

            var toExpected = addresses.Aggregate(String.Empty, (s, mailAddress) => s + Environment.NewLine + "\t" + mailAddress.ToString());
            if (!String.IsNullOrWhiteSpace(toExpected))
            {
                var toAppend =
                    $"{newline}{separator}Actual {addressType}:{(realMailMessage.IsBodyHtml ? toExpected.HtmlEncodeWithBreaks() : toExpected)}";
                realMailMessage.Body += toAppend;

                for (var i = 0; i < realMailMessage.AlternateViews.Count; i++)
                {
                    var stream = realMailMessage.AlternateViews[i].ContentStream;
                    using (var reader = new StreamReader(stream))
                    {
                        var alternateBody = reader.ReadToEnd();
                        alternateBody += toAppend;
                        var newAlternateView = AlternateView.CreateAlternateViewFromString(alternateBody, null, realMailMessage.AlternateViews[i].ContentType.MediaType);
                        realMailMessage.AlternateViews[i].LinkedResources.ToList().ForEach(x => newAlternateView.LinkedResources.Add(x));
                        realMailMessage.AlternateViews[i] = newAlternateView;
                    }
                }


            }
            addresses.Clear();
        }

        private static string FlattenMailAddresses(IEnumerable<MailAddress> addresses)
        {
            return String.Join("; ", addresses.Select(x => x.ToString()));
        }

        public string GetDefaultEmailSignature()
        {
            string defaultEmailSignature = $@"<br /><br />
Respectfully, the {_qanatConfiguration.PlatformLongName} team
<br /><br />
***
<br /><br />
You have received this email because you are a registered user of the {_qanatConfiguration.PlatformLongName}. 
<br /><br />
<a href=""mailto:{_qanatConfiguration.SupportEmail}"">{_qanatConfiguration.SupportEmail}</a>";
            return defaultEmailSignature;
        }

        public string GetSupportNotificationEmailSignature()
        {
            string supportNotificationEmailSignature = $@"<br /><br />
Respectfully, the {_qanatConfiguration.PlatformLongName} team
<br /><br />
***
<br /><br />
You have received this email because you are assigned to receive support notifications within the {_qanatConfiguration.PlatformLongName}. 
<br /><br />
<a href=""mailto:{_qanatConfiguration.SupportEmail}"">{_qanatConfiguration.SupportEmail}</a>";
            return supportNotificationEmailSignature;
        }

        public MailAddress GetDefaultEmailFrom()
        {
            return new MailAddress("donotreply@sitkatech.net", $"{_qanatConfiguration.PlatformLongName}");
        }

        public static void AddBccRecipientsToEmail(MailMessage mailMessage, IEnumerable<string> recipients)
        {
            foreach (var recipient in recipients)
            {
                mailMessage.Bcc.Add(recipient);
            }
        }

        public static void AddCcRecipientsToEmail(MailMessage mailMessage, IEnumerable<string> recipients)
        {
            foreach (var recipient in recipients)
            {
                mailMessage.CC.Add(recipient);
            }
        }
    }
}