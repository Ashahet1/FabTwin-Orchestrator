namespace FabTwin.Core.Models;

public record AlarmEvent(
    DateTime Timestamp,
    string ScenarioRunId,
    string Code,
    string Severity,
    string Message
);