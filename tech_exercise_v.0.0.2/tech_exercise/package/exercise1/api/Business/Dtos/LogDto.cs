namespace StargateAPI.Business.Dtos;

public class LogDto
{
    public DateTime Timestamp { get; set; }

    public string Level { get; set; } = string.Empty;
    public string Exception { get; set; } = string.Empty;
    public string RenderedMessage { get; set; } = string.Empty;
    public string Properties { get; set; } = string.Empty;
}
