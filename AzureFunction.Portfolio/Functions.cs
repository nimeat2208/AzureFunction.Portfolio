using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Net;
using System.Net.Mail;

namespace AzureFunction.Portfolio
{
    public class Functions
    {
        private readonly ILogger _logger;

        public Functions(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<Functions>();
        }

        [Function("SendEmailFunction")]
        public async Task<HttpResponseData> SendEmailFunction([HttpTrigger(
            AuthorizationLevel.Function,
            "get", "post",
            Route = "sendemail")]
        HttpRequestData req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");

            var response = req.CreateResponse();

            string errMsg;

            try
            {
                var smtpInfo = new SmtpSetting
                {
                    SmtpServer = Environment.GetEnvironmentVariable("SmtpServer") ?? string.Empty,
                    SmtpServerPort = Environment.GetEnvironmentVariable("SmtpServerPort") ?? string.Empty,
                    SmtpServerSSL = Environment.GetEnvironmentVariable("SmtpServerSSL") ?? string.Empty,
                    SmtpServerUserName = Environment.GetEnvironmentVariable("SmtpServerUserName") ?? string.Empty,
                    SmtpServerPassword = Environment.GetEnvironmentVariable("SmtpServerPassword") ?? string.Empty,
                    SmtpEmailFrom = Environment.GetEnvironmentVariable("SmtpEmailFrom") ?? string.Empty,
                    SmtpEmailTo = Environment.GetEnvironmentVariable("SmtpEmailTo") ?? string.Empty,
                };

                errMsg = ValidateForSmtpInfo(smtpInfo);
                if (!string.IsNullOrWhiteSpace(errMsg))
                {
                    _logger.LogError(errMsg);
                    response.StatusCode = HttpStatusCode.InternalServerError;
                    await response.WriteAsJsonAsync(new { RtnMsg = errMsg, CurrentTime = DateTime.UtcNow });
                    return response;
                }

                string strReqBody = await new StreamReader(req.Body).ReadToEndAsync();
                var data = JsonConvert.DeserializeObject<Contact>(strReqBody);

                errMsg = ValidateForReqBody(data);
                if (!string.IsNullOrWhiteSpace(errMsg))
                {
                    _logger.LogError(errMsg);
                    response.StatusCode = HttpStatusCode.InternalServerError;
                    await response.WriteAsJsonAsync(new { RtnMsg = errMsg, CurrentTime = DateTime.UtcNow });
                    return response;
                }

                bool smtpServerSSL = bool.Parse(smtpInfo.SmtpServerSSL);

                SmtpClient client = new SmtpClient(smtpInfo.SmtpServer)
                {
                    Port = int.Parse(smtpInfo.SmtpServerPort),
                    Credentials = new NetworkCredential(smtpInfo.SmtpServerUserName, smtpInfo.SmtpServerPassword),
                    EnableSsl = smtpServerSSL
                };

                var mailMsg = new MailMessage
                {
                    From = new MailAddress(smtpInfo.SmtpEmailFrom),
                    Subject = "Contact via Nimeat's Portfolio",
                    Body = $@"<head></head><body><p>[Name] : {data.Name}, [Email] : {data.Email}, [Message] : {data.Msg}</p></body>",
                    IsBodyHtml = true
                };

                mailMsg.To.Add(smtpInfo.SmtpEmailTo);
                mailMsg.ReplyToList.Add(data.Email);
                client.Send(mailMsg);

                response.StatusCode = HttpStatusCode.OK;
                await response.WriteAsJsonAsync(new { RtnMsg = "Success", CurrentTime = DateTime.UtcNow });
            }
            catch (Exception ex)
            {
                await response.WriteAsJsonAsync(new { RtnMsg = ex.Message, CurrentTime = DateTime.UtcNow });
            }

            return response;
        }

        private string ValidateForSmtpInfo(SmtpSetting data)
        {
            string result = string.Empty;

            if (string.IsNullOrWhiteSpace(data.SmtpServer))
            {
                result = "SmtpServer is Empty!";
            }

            if (string.IsNullOrWhiteSpace(data.SmtpServerPort))
            {
                result = "SmtpServerPort is Empty!";
            }

            if (string.IsNullOrWhiteSpace(data.SmtpServerSSL))
            {
                result = "SmtpServerSSL is Empty!";
            }
            else if (!bool.TryParse(data.SmtpServerSSL, out bool _))
            {
                result = "SmtpServerSSL Setting is not a valid boolean value. Must be true of false";
            }

            if (string.IsNullOrWhiteSpace(data.SmtpServerUserName))
            {
                result = "SmtpServerUserName is Empty!";
            }

            if (string.IsNullOrWhiteSpace(data.SmtpServerPassword))
            {
                result = "SmtpServerPassword is Empty!";
            }

            if (string.IsNullOrWhiteSpace(data.SmtpEmailFrom))
            {
                result = "SmtpEmailFrom is Empty!";
            }

            if (string.IsNullOrWhiteSpace(data.SmtpEmailTo))
            {
                result = "SmtpEmailTo is Empty!";
            }

            return result;
        }

        private string ValidateForReqBody(Contact? data)
        {
            string result = string.Empty;

            if (data == null)
            {
                result = "Data is Empty!";
            }
            else if (data.Name == null)
            {
                result = "Data > Name is Empty!";
            }
            else if (data.Email == null)
            {
                result = "Data > Name is Empty!";
            }
            else if (data.Msg == null)
            {
                result = "Data > Name is Empty!";
            }
            return result;
        }
    }
}
