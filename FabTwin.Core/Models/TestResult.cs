namespace FabTwin.Core.Models;

public record TestResult(
    DateTime Timestamp,
    string ScenarioRunId,
    string ParamName,
    double Measured,
    double SpecMin,
    double SpecMax,
    bool PassFail
);