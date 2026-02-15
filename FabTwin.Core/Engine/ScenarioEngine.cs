using FabTwin.Core.Models;
namespace FabTwin.Core.Engine;

public class ScenarioState
{
    public string RunId { get; set; } = string.Empty;
    public FabTwin.Core.Models.ScenarioId ScenarioId { get; set; }
    public int Seed { get; set; }
    public double Severity { get; set; }
    public DateTime StartTime { get; set; }
    public bool FaultInjected { get; set; }
    public bool ActionApplied { get; set; }

    // Scenario-specific state
    public double Temperature { get; set; } = 25.0;
    public double CalibrationOffset { get; set; } = 0.0;
    public double PowerSupplyVoltage { get; set; } = 3.3;
    public double NoiseSigma { get; set; } = 0.01;
    public double BaseYield { get; set; } = 95.0;
}