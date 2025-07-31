using CsvHelper.Configuration.Attributes;
using eID.PJS.Contracts.Results.Admin;
using Newtonsoft.Json;

namespace eID.PJS.API.DTOs;

public class LogToUserResultDTO
{
    [Name("ID на събитие")]
    public string EventId { get; set; }
    [Name("Дата")]
    public DateTime EventDate { get; set; }
    [Name("Тип на събитие")]
    public string EventType { get; set; }
    [Name("Извикващата система - идентификатор")]
    public string RequesterSystemId { get; set; }
    [Name("Извикващата система - име")]
    public string RequesterSystemName { get; set; }
    [Name("Номер за свързаност между операциите")]
    public string CorrelationId { get; set; }
    [Name("Съобщение")]
    public string Message { get; set; }
    [Name("Идентификатор на извършващия действието")]
    public string RequesterUserId { get; set; }
    [Name("Идентификатор на гражданина")]
    public string TargetUserId { get; set; }
    [Name("Други данни")]
    public string EventPayload { get; set; }

    public static LogToUserResultDTO Create(LogToUserResult source)
        => new LogToUserResultDTO()
        {
            EventId = source.EventId,
            EventDate = source.EventDate,
            EventType = source.EventType,
            RequesterSystemId = source.RequesterSystemId,
            CorrelationId = source.CorrelationId,
            Message = source.Message,
            RequesterSystemName = source.RequesterSystemName,
            RequesterUserId = source.RequesterUserId,
            TargetUserId = source.TargetUserId,
            EventPayload = JsonConvert.SerializeObject(source.EventPayload)
        };
}
