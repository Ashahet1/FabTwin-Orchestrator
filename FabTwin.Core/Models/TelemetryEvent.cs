namespace FabTwin.Core.Models;

public record TelemetryEvent(
    DateTime Timestamp,
    string ScenarioRunId,
    string EquipmentId,
    double Temperature,
    double CalibrationOffset,
    double PowerSupplyVoltage,
    double NoiseSigma,
    double MeasuredValue,
    double YieldPercent
);