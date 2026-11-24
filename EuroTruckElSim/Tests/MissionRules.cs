using System;

internal static class MissionRules
{
    private static void Check(bool condition, string message)
    {
        if (!condition) throw new Exception(message);
    }

    private static void Main()
    {
        var mission = new MissionProgress();
        Check(mission.Current == MissionProgress.Stage.Pickup && !mission.HasCargo, "Starts empty at pickup");
        Check(!mission.Interact(MissionProgress.Stage.Delivery, true), "Cannot deliver before pickup");
        mission.TickCharge(10, true);
        Check(mission.BatteryPercent == 40, "Cannot skip ahead to charging");
        Check(!mission.Interact(MissionProgress.Stage.Pickup, false), "Cannot collect while moving");
        Check(mission.Interact(MissionProgress.Stage.Pickup, true) && mission.HasCargo, "Pickup loads cargo");
        Check(!mission.Interact(MissionProgress.Stage.Pickup, true), "Cannot collect twice");
        Check(!mission.Interact(MissionProgress.Stage.Delivery, false), "Cannot deliver while moving");
        Check(mission.Interact(MissionProgress.Stage.Delivery, true) && !mission.HasCargo, "Delivery unloads cargo");
        mission.TickCharge(2, true);
        Check(mission.BatteryPercent == 60, "Charging rate is ten percentage points per second");
        mission.TickCharge(5, false);
        Check(mission.BatteryPercent == 60, "Leaving or moving pauses charging");
        mission.TickCharge(10, true);
        Check(mission.BatteryPercent == 80 && mission.Current == MissionProgress.Stage.Complete, "Resumes and clamps at target");
        Check(!mission.Interact(MissionProgress.Stage.Delivery, true), "Completed missions cannot repeat");
        mission.TickCharge(10, true);
        Check(mission.BatteryPercent == 80, "Completion is stable");
        var restarted = new MissionProgress();
        Check(restarted.Current == MissionProgress.Stage.Pickup && restarted.BatteryPercent == 40 && !restarted.HasCargo,
            "Fresh scene state resets all progress");
        Console.WriteLine("All mission rule checks passed.");
    }
}
