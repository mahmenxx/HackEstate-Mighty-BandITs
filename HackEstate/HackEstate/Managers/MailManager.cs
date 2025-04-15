using System.Net.Mail;
using System.Net;
using Microsoft.Extensions.Configuration;
using System;

namespace HackEstate.Managers
{
    public class MailManager
    {
        private readonly string _mailSender;
        private readonly string _mailAppPassword;

        public MailManager(IConfiguration configuration)
        {
            _mailSender = configuration["EmailSettings:MailSender"];
            _mailAppPassword = configuration["EmailSettings:MailSenderAppPassword"];
        }

        public bool SendEmail(string recipient, string subject, string msgBody, ref string errResponse)
        {
            try
            {
                using (var message = new MailMessage())
                {
                    var smtp = new SmtpClient
                    {
                        Port = 587,
                        Host = "smtp.gmail.com",
                        EnableSsl = true,
                        UseDefaultCredentials = false,
                        Credentials = new NetworkCredential(_mailSender, _mailAppPassword),
                        DeliveryMethod = SmtpDeliveryMethod.Network
                    };

                    message.From = new MailAddress(_mailSender, "EvenTahan");
                    message.To.Add(new MailAddress(recipient));
                    message.Subject = subject;
                    message.IsBodyHtml = true;
                    message.Body = msgBody;

                    smtp.Send(message);
                    errResponse = "Message Sent";
                    return true;
                }
            }
            catch (Exception ex)
            {
                errResponse = ex.Message;
                return false;
            }
        }

        public bool SendMessageToEmail(string recipientEmail, string subject, string firstName, string senderRole, string senderName, string message, ref string errResponse)
        {
            string welcomeTemplate = $@"<!DOCTYPE html>
                    <html lang='en'>
                    <head>
                        <meta charset='UTF-8'>
                        <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                        <title>Welcome to EvenTahan!</title>
                    </head>
                    <body style='font-family: Arial, sans-serif; background-color: #f4f4f4; margin: 0; padding: 0;'>
                        <div style='max-width: 600px; margin: 20px auto; background-color: #fff; border-radius: 10px; overflow: hidden;'>
                            <div style='background-color: #70c778; color: #333; text-align: center; padding: 20px 0;'>
                                <h1 style='margin: 0; font-size: 24px;'>Your {senderRole} has messaged you.</h1>
                            </div>
                            <div style='padding: 20px;'>
                                <p style='font-size: 16px;'>Hi, {firstName}! This is EvenTahan!</p>
                                <p style='font-size: 16px;'>{senderName} has sent you a message.  </p>
                                <p style='font-size: 16px;'>Message: {message}</p>
                            </div>
                        </div>
                    </body>
                    </html>";

            welcomeTemplate = welcomeTemplate.Replace("{firstName}", firstName).Replace("{firstName}", firstName)
                                     .Replace("{senderName}", senderName)
                                     .Replace("{message}", message);

            return SendEmail(recipientEmail, subject, welcomeTemplate, ref errResponse);
        }

        public bool SendAnnouncementEmail(string recipientEmail, string subject, string firstName, string note, string date, ref string errResponse)
        {
            string welcomeTemplate = $@"<!DOCTYPE html>
                    <html lang='en'>
                    <head>
                        <meta charset='UTF-8'>
                        <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                        <title>Welcome to Babag EcoHub!</title>
                    </head>
                    <body style='font-family: Arial, sans-serif; background-color: #f4f4f4; margin: 0; padding: 0;'>
                        <div style='max-width: 600px; margin: 20px auto; background-color: #fff; border-radius: 10px; overflow: hidden;'>
                            <div style='background-color: #70c778; color: #333; text-align: center; padding: 20px 0;'>
                                <h1 style='margin: 0; font-size: 24px;'>Babag EcoHub Announcement!</h1>
                            </div>
                            <div style='padding: 20px;'>
                                <p style='font-size: 16px;'>Hi, {firstName}! This is Babag EcoHub, since you've subscribed to our alert notification, a Babag Official has a reminder/announcmeent for you: </p>
                                <p style='font-size: 16px;'> {note}. Date: {date}. </p>
                                <p style='font-size: 16px;'>Be updated only here in Babag EcoHub!</p>
                            </div>
                        </div>
                    </body>
                    </html>";

            welcomeTemplate = welcomeTemplate.Replace("{firstName}", firstName).Replace("{note}", note)
                                     .Replace("{date}", date);

            return SendEmail(recipientEmail, subject, welcomeTemplate, ref errResponse);
        }

        public bool SendAlertAdminEmail(string recipientEmail, string subject, string firstName, string report ,ref string errResponse)
        {
            string welcomeTemplate = $@"<!DOCTYPE html>
                    <html lang='en'>
                    <head>
                        <meta charset='UTF-8'>
                        <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                        <title>Welcome to Babag EcoHub!</title>
                    </head>
                    <body style='font-family: Arial, sans-serif; background-color: #f4f4f4; margin: 0; padding: 0;'>
                        <div style='max-width: 600px; margin: 20px auto; background-color: #fff; border-radius: 10px; overflow: hidden;'>
                            <div style='background-color: #70c778; color: #333; text-align: center; padding: 20px 0;'>
                                <h1 style='margin: 0; font-size: 24px;'>Babag EcoHub Incident Report</h1>
                            </div>
                            <div style='padding: 20px;'>
                                <p style='font-size: 16px;'>Hi, Official {firstName}! This is Babag EcoHub, an incident has been reported within your barangay.</p>
                                <p style='font-size: 16px;'>Check our website at <a href='https://babagecohub-001-site1.mtempurl.com/Account/Login'>https://babagecohub-001-site1.mtempurl.com/Account/Login</a> to approve this report ({report}).</p>
                                < p style='font-size: 16px;'>Thank you for helping Brgy. Babag become safer than ever!</p>
                            </div>
                        </div>
                    </body>
                    </html>";

            welcomeTemplate = welcomeTemplate.Replace("{firstName}", firstName).Replace("{report}", report);

            return SendEmail(recipientEmail, subject, welcomeTemplate, ref errResponse);
        }

        public bool SendAlertDriverEmail(string recipientEmail, string subject, string firstName, ref string errResponse)
        {
            string welcomeTemplate = $@"<!DOCTYPE html>
                    <html lang='en'>
                    <head>
                        <meta charset='UTF-8'>
                        <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                        <title>Welcome to Babag EcoHub!</title>
                    </head>
                    <body style='font-family: Arial, sans-serif; background-color: #f4f4f4; margin: 0; padding: 0;'>
                        <div style='max-width: 600px; margin: 20px auto; background-color: #fff; border-radius: 10px; overflow: hidden;'>
                            <div style='background-color: #70c778; color: #333; text-align: center; padding: 20px 0;'>
                                <h1 style='margin: 0; font-size: 24px;'>Babag EcoHub Reminder!</h1>
                            </div>
                            <div style='padding: 20px;'>
                                <p style='font-size: 16px;'>Hi, {firstName}! This is Babag EcoHub, a Babag Official has a reminded you about your garbage collection schedule today! </p>
                                <p style='font-size: 16px;'>Be updated only here in Babag EcoHub! Thank you for your service!</p>
                            </div>
                        </div>
                    </body>
                    </html>";

            welcomeTemplate = welcomeTemplate.Replace("{firstName}", firstName);

            return SendEmail(recipientEmail, subject, welcomeTemplate, ref errResponse);
        }


        public bool SendReportGarbageAdminEmail(string recipientEmail, string subject, string firstName, string senderName, ref string errResponse)
        {
            string welcomeTemplate = $@"<!DOCTYPE html>
                    <html lang='en'>
                    <head>
                        <meta charset='UTF-8'>
                        <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                        <title>Welcome to Babag EcoHub!</title>
                    </head>
                    <body style='font-family: Arial, sans-serif; background-color: #f4f4f4; margin: 0; padding: 0;'>
                        <div style='max-width: 600px; margin: 20px auto; background-color: #fff; border-radius: 10px; overflow: hidden;'>
                            <div style='background-color: #70c778; color: #333; text-align: center; padding: 20px 0;'>
                                <h1 style='margin: 0; font-size: 24px;'>Babag EcoHub Missed Collection Report</h1>
                            </div>
                            <div style='padding: 20px;'>
                                <p style='font-size: 16px;'>Hi, Official {firstName}! This is Babag EcoHub, a report from {senderName} has been submitted within your barangay about a missed garbage collection.</p>
                                <p style='font-size: 16px;'>Check our website at <a href='https://babagecohub-001-site1.mtempurl.com/Account/Login'>https://babagecohub-001-site1.mtempurl.com//Account/Login</a> to respond to this report.</p>
                                <p style='font-size: 16px;'>Thank you for helping Brgy. Babag become cleaner than ever!</p>
                            </div>
                        </div>
                    </body>
                    </html>";

            welcomeTemplate = welcomeTemplate.Replace("{firstName}", firstName).Replace("{senderName}", senderName);

            return SendEmail(recipientEmail, subject, welcomeTemplate, ref errResponse);
        }

        public bool SendResponseToReportGarbageEmail(string recipientEmail, string subject, string firstName, string response, ref string errResponse)
        {
            string welcomeTemplate = $@"<!DOCTYPE html>
                    <html lang='en'>
                    <head>
                        <meta charset='UTF-8'>
                        <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                        <title>Welcome to Babag EcoHub!</title>
                    </head>
                    <body style='font-family: Arial, sans-serif; background-color: #f4f4f4; margin: 0; padding: 0;'>
                        <div style='max-width: 600px; margin: 20px auto; background-color: #fff; border-radius: 10px; overflow: hidden;'>
                            <div style='background-color: #70c778; color: #333; text-align: center; padding: 20px 0;'>
                                <h1 style='margin: 0; font-size: 24px;'>Babag EcoHub Report Response</h1>
                            </div>
                            <div style='padding: 20px;'>
                                <p style='font-size: 16px;'>Hi, {firstName}! This is Babag EcoHub, a response from your previous report of missed garbage collection has been responded by our KaBabag: </p>
                                <p style='font-size: 16px;'>Response: </p>
                                <p style='font-size: 16px;'>{response}</p>
                            </div>
                        </div>
                    </body>
                    </html>";

            welcomeTemplate = welcomeTemplate.Replace("{firstName}", firstName).Replace("{response}", response);

            return SendEmail(recipientEmail, subject, welcomeTemplate, ref errResponse);
        }
    }
}
