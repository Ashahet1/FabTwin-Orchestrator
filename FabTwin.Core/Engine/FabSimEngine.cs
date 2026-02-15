using FabTwin.Core.Models;

namespace FabTwin.Core.Engine;

public class FabSimEngine
{
    private readonly Dictionary<string, ScenarioState> _activeRuns = new();
    private readonly Random _baseRandom = new();

    public string StartRun(ScenarioId scenarioId, int seed, double severity)
    {
        var runId = $"{scenarioId}_{DateTime.UtcNow:yyyyMMddHHmmss}_{seed}";

        var state = new ScenarioState
        {
            RunId = runId,
            ScenarioId = scenarioId,
            Seed = seed,
            Severity = severity,
            StartTime = DateTime.UtcNow,
            FaultInjected = false,
            ActionApplied = false
        };

        _activeRuns[runId] = state;
        return runId;
    }

    public void InjectFault(string runId)
    {
        if (!_activeRuns.TryGetValue(runId, out var state)) return;

        state.FaultInjected = true;

        switch (state.ScenarioId)
        {
            case ScenarioId.TempDrift:
                state.Temperature = 25.0 + (15.0 * state.Severity);
                state.CalibrationOffset = 0.05 * state.Severity;
                break;

            case ScenarioId.SupplyFault:
                state.PowerSupplyVoltage = 0.0;
                break;

            case ScenarioId.NoiseIntermittent:
                state.NoiseSigma = 0.01 + (0.15 * state.Severity);
                break;
        }
    }

    public void ApplyAction(string runId, ActionCommand action)
    {
        if (!_activeRuns.TryGetValue(runId, out var state)) return;

        state.ActionApplied = true;

        switch (action.ActionType)
        {
            case "COOLDOWN":
                state.Temperature = 25.0;
                break;

            case "RECALIBRATE":
                state.CalibrationOffset = 0.0;
                break;

            case "RESET_SUPPLY":
                state.PowerSupplyVoltage = 3.3;
                break;

            case "REINIT_SEQUENCE":
                state.PowerSupplyVoltage = 3.3;
                break;

            case "RESEAT_CONNECTION":
                state.NoiseSigma = 0.01;
                break;
        }
    }

    public TelemetryEvent GenerateTelemetry(string runId)
    {
        if (!_activeRuns.TryGetValue(runId, out var state))
            throw new InvalidOperationException($"Run {runId} not found");

        var random = new Random(state.Seed + (int)(DateTime.UtcNow - state.StartTime).TotalSeconds);

        double measuredValue = 1.0 + state.CalibrationOffset + (random.NextDouble() - 0.5) * state.NoiseSigma * 2;

        double yieldLoss = 0.0;
        if (state.FaultInjected && !state.ActionApplied)
        {
            yieldLoss = state.ScenarioId switch
            {
                ScenarioId.TempDrift => (state.Temperature - 25.0) * 2.0,
                ScenarioId.SupplyFault => 90.0,
                ScenarioId.NoiseIntermittent => state.NoiseSigma * 100,
                _ => 0.0
            };
        }

        double currentYield = Math.Max(0, state.BaseYield - yieldLoss);

        return new TelemetryEvent(
            DateTime.UtcNow,
            runId,
            "ATE-001",
            state.Temperature,
            state.CalibrationOffset,
            state.PowerSupplyVoltage,
            state.NoiseSigma,
            measuredValue,
            currentYield
        );
    }

    public AlarmEvent? GenerateAlarm(string runId)
    {
        if (!_activeRuns.TryGetValue(runId, out var state)) return null;
        if (!state.FaultInjected || state.ActionApplied) return null;

        return state.ScenarioId switch
        {
            ScenarioId.TempDrift when state.Temperature > 35 => new AlarmEvent(
                DateTime.UtcNow,
                runId,
                "TEMP_HIGH",
                "WARNING",
                $"Temperature out of range: {state.Temperature:F1}°C"
            ),
            ScenarioId.SupplyFault => new AlarmEvent(
                DateTime.UtcNow,
                runId,
                "PSU_FAULT",
                "CRITICAL",
                "Power supply fault detected - test aborted"
            ),
            ScenarioId.NoiseIntermittent when state.NoiseSigma > 0.1 => new AlarmEvent(
                DateTime.UtcNow,
                runId,
                "NOISE_HIGH",
                "WARNING",
                "Intermittent signal noise detected"
            ),
            _ => null
        };
    }

    public ScenarioState? GetState(string runId)
    {
        return _activeRuns.GetValueOrDefault(runId);
    }
}