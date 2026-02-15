using FabTwin.Core.Models;

namespace FabTwin.Core.Data;

public class EventStore
{
    private readonly List<TelemetryEvent> _telemetry = new();
    private readonly List<AlarmEvent> _alarms = new();
    private readonly List<TestResult> _testResults = new();
    private readonly List<ActionCommand> _actions = new();

    public void Append(TelemetryEvent evt) => _telemetry.Add(evt);
    public void Append(AlarmEvent evt) => _alarms.Add(evt);
    public void Append(TestResult evt) => _testResults.Add(evt);
    public void Append(ActionCommand evt) => _actions.Add(evt);

    public List<TelemetryEvent> QueryTelemetry(string runId, DateTime start, DateTime end)
        => _telemetry.Where(e => e.ScenarioRunId == runId && e.Timestamp >= start && e.Timestamp <= end).ToList();

    public List<AlarmEvent> QueryAlarms(string runId, DateTime start, DateTime end)
        => _alarms.Where(e => e.ScenarioRunId == runId && e.Timestamp >= start && e.Timestamp <= end).ToList();

    public List<TestResult> QueryTestResults(string runId)
        => _testResults.Where(e => e.ScenarioRunId == runId).ToList();

    public List<ActionCommand> GetActions(string runId)
        => _actions.Where(e => e.ScenarioRunId == runId).ToList();
}