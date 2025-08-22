namespace WebClient.Models;

public class Result<T>
{
    public int code { get; set; } = 0;
    public string message { get; set; } = string.Empty;
    public T? data { get; set; }
}