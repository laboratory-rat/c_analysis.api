using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MRCryptoCurrencyAnalysis.Controllers
{
    [Route("sns")]
    public class SNSController : Controller
    {
        protected readonly ILogger _logger;

        public SNSController(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger(this.GetType());
        }

        [HttpPost]
        [Route("endpoint")]
        public async Task<string> Endpoint(string id = "")
        {
            try
            {
                string content = string.Empty;
                using (var reader = new StreamReader(Request.Body, Encoding.UTF8))
                {
                    content = await reader.ReadToEndAsync();
                }

                var sm = Amazon.SimpleNotificationService.Util.Message.ParseMessage(content);
                if (sm.Type.Equals("SubscriptionConfirmation")) 
                {
                    _logger.LogInformation("Received Confirm subscription request");
                    if (!string.IsNullOrEmpty(sm.SubscribeURL))
                    {
                        var uri = new Uri(sm.SubscribeURL);
                        _logger.LogInformation("uri:" + uri.ToString());
                        var baseUrl = uri.GetLeftPart(System.UriPartial.Authority);
                        var resource = sm.SubscribeURL.Replace(baseUrl, "");

                        var client = new HttpClient();
                        client.BaseAddress = new Uri(baseUrl);
                        await client.GetAsync(resource);
                    }
                }
                else 
                {
                    _logger.LogWarning("Message received from SNS:" + sm.TopicArn);
                    _logger.LogWarning($"Message id: {sm.MessageId}");
                    _logger.LogWarning($"Full message: {sm.MessageText}");

                    SNSNotification message = JsonConvert.DeserializeObject<SNSNotification>(sm.MessageText);

                    if (message.IsDelivered)
                    {
                        _logger.LogWarning($"Message delivered SNS");
                        _logger.LogWarning($"Source: {message.Mail.Source}");
                        _logger.LogWarning($"Destination: [ {string.Join(", ", message.Mail.Destination)} ]");
                        _logger.LogWarning($"SmtpResponse: {message.Delivery.SmtpResponse}");
                    } 
                    else if (message.IsBounced)
                    {
                        _logger.LogWarning($"Message bounced SNS");
                        _logger.LogWarning($"Source: {message.Mail.Source}");
                        _logger.LogWarning($"Destination: [ {string.Join(", ", message.Mail.Destination)} ]");
                        _logger.LogWarning($"SmtpResponse: [ {string.Join(", ", message.Bounce.BouncedRecipients.Select(x => x.EmailAddress))} ]");
                    }

                }
                //do stuff
                return "Success";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SNS FAIL");
                return "";
            }
        }
    }

    public class SNSNotification
    {
        public string NotificationType { get; set; }

        [JsonIgnore]
        public bool IsDelivered => NotificationType == "Delivery";

        [JsonIgnore]
        public bool IsBounced => NotificationType == "Bounce";

        public SNSMail Mail { get; set; }
        public SNSDelivery Delivery { get; set; }
        public SNSMailBounce Bounce { get; set; }

        public class SNSMail
        {
            public DateTime Timestamp { get; set; }
            public string Source { get; set; }
            public string SourceArn { get; set; }
            public string SourceIp { get; set; }
            public string SendingAccount { get; set; }
            public string MessageId { get; set; }
            public List<string> Destination { get; set; }
            public List<SNSMailHeader> Headers { get; set; }
            public SNSMailCommonHeaders CommonHeaders { get; set; }

            public class SNSMailHeader
            {
                public string Name { get; set; }
                public string Value { get; set; }
            }

            public class SNSMailCommonHeaders
            {
                public List<string> From { get; set; }
                public string Date { get; set; }
                public List<string> To { get; set; }
                public string MessageId { get; set; }
                public string Subject { get; set; }
            }
        }

        public class SNSDelivery
        {
            public DateTime Timestamp { get; set; }
            public long ProcessingTimeMillis { get; set; }
            public List<string> Recipients { get; set; }
            public string SmtpResponse { get; set; }
            public string RemoteMtaIp { get; set; }
            public string ReportingMTA { get; set; }
        }

        public class SNSMailBounce
        {
            public string BounceType { get; set; }
            public string BounceSubType { get; set; }
            public List<NSNMailBounceRecipient> BouncedRecipients { get; set; }
            public DateTime Timestamp { get; set; }
            public string FeedbackId { get; set; }
            public string RemoteMtaIp { get; set; }
            public string ReportingMTA { get; set; }

            public class NSNMailBounceRecipient
            {
                public string EmailAddress { get; set; }
                public string Action { get; set; }
                public string Status { get; set; }
                public string DiagnosticCode { get; set; }
            }
        }
    }
}
