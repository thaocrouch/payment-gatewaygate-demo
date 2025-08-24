using System.Net;
using System.Text;
using Application.Interfaces;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using RabbitMQ.Client;

namespace Application.Services;

public class NotifyService : INotifyService
{
    private readonly EmailOption _emailOption;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly InternalSystemOption _internalSystemOption;
    private readonly ILogger<NotifyService> _logger;
    private readonly RabbitMqOption _rabbitMqOption;

    public NotifyService(ILogger<NotifyService> logger, IOptionsSnapshot<RabbitMqOption> rabbitMqOption, IOptionsSnapshot<EmailOption> emailOption,
        IOptionsSnapshot<InternalSystemOption> internalSystemOption, IHttpClientFactory httpClientFactory)
    {
        _logger = logger;
        _httpClientFactory = httpClientFactory;
        _internalSystemOption = internalSystemOption.Value;
        _emailOption = emailOption.Value;
        _rabbitMqOption = rabbitMqOption.Value;
    }

    public async Task<bool> RabbitmqPublish(IConnection conn, string queue, string routingKey, string message)
    {
        try
        {
            using var channel = await conn.CreateChannelAsync();
            await channel.QueueDeclareAsync(queue, false, false, false);
            var body = Encoding.UTF8.GetBytes(message);
            await channel.BasicPublishAsync("", routingKey, body);
        }
        catch (Exception ex)
        {
            _logger.LogError("{tag} Can't publish message to Rabbitmq: url : {url} - message: {message} - error: {ex}", "NotifyService", _rabbitMqOption.Host, message, ex);
            return false;
        }

        return true;
    }

    public async Task<bool> SendEmail(string toEmail, string subject, string body)
    {
        // Send email to users
        try
        {
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(_emailOption.FromEmail));
            email.To.Add(MailboxAddress.Parse(toEmail));
            email.Subject = subject;
            var bodyBuilder = new BodyBuilder();
            bodyBuilder.HtmlBody = body;
            email.Body = bodyBuilder.ToMessageBody();
            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(_emailOption.Url, _emailOption.Port, SecureSocketOptions.StartTls);
            await smtp.AuthenticateAsync(_emailOption.UserName, _emailOption.Password);
            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }
        catch (Exception ex)
        {
            _logger.LogError("{tag}: Can't send email to: toEmail : {toEmail} - ex: {ex}", "NotifyService", toEmail, ex);
            return false;
        }

        return true;
    }

    public async Task<bool> PushInternalSystem(Order order)
    {
        var url = $"{_internalSystemOption.BaseUrl}/{_internalSystemOption.UrlReceivedOrder}";
        var body = string.Empty;
        // Push order to internal system.
        try
        {
            using (var client = _httpClientFactory.CreateClient(HttpConnection.InternalSystem))
            {
                using (var response = await client.PostAsync(url, new StringContent(order.ToJson(),
                           Encoding.UTF8, "application/json")))
                {
                    body = await response.Content.ReadAsStringAsync();
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        // cast to object response
                        var obj = body.ToObject<object>();
                    }
                    else
                    {
                        // Save to retry table.
                        return false;
                    }

                    _logger.LogInformation("{tag} request: {url} - param : {param} - response: {response}", "backend", url,
                        order.ToString(), body);
                }
            }
        }
        catch (Exception ex)
        {
            // Save to retry table
            _logger.LogError("{tag}: Can't push to internal system: url : {url} - param: {param} - body: {body} - ex: {ex}",
                "NotifyService", url, order.ToJson(), body, ex);
            return false;
        }

        return true;
    }
}