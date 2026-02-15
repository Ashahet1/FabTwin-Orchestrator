namespace FabTwin.Core.Models;

public record RetestReport(
    string ScenarioRunId,
    DateTime BaselineStart,
    DateTime BaselineEnd,
    DateTime RetestStart,
    DateTime RetestEnd,
    Dictionary<string, double> Deltas,
    bool Resolved,
    string Summary
);