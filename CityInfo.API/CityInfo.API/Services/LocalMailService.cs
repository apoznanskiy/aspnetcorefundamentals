namespace CityInfo.API.Services;

public class LocalMailService : IMailService
{
    private readonly string _mailTo;
    private readonly string _mailFrom;

    public LocalMailService(IConfiguration configuration)
    {
        _mailTo = configuration["MailSettings:MailTo"];
        _mailFrom = configuration["MailSettings:MailFrom"];
    }

    public void Send(string subject, string message)
    {
        Console.WriteLine($"Mail from {_mailFrom} to {_mailTo}, subject {subject}, message: {message}.");
    }
}
