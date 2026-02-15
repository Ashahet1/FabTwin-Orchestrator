namespace FabTwin.Core.Models;

public record ActionCommand(
    string ScenarioRunId,
    string ActionType,
    Dictionary<string, object> Parameters,
    string Reason
);