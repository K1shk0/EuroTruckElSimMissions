# EuroTruckElSim

## Three simple missions

This prototype extends the existing controllable 2D truck on `andreas-truck`.
Open `project.godot` with the Godot **.NET** editor matching the project's
Godot.NET.Sdk version (currently 4.7.2), build the C# project, and press F5.

Drive with A/D or the left/right arrows. Follow the on-screen direction hint.

1. **Pickup:** park fully inside the yellow bay at x=600 and press E to load one crate.
2. **Delivery:** park inside the blue bay at x=1200 and press E to unload the crate.
3. **Charge:** park inside the green bay at x=1800. The battery starts at 40%
   and charges at 10 percentage points per second until 80%. Leaving the bay
   or moving pauses charging; returning resumes it.

Missions must be completed in order. Driving through a bay does not complete
a mission. The HUD displays the objective, cargo, battery and completion.
The Restart missions button resets the scene and all mission state.

`Scripts/MissionProgress.cs` holds the mission rules. `Scripts/MissionManager.cs`
connects them to the truck, parking bays and HUD. Move the Pickup, Delivery and
Charger Marker2D nodes in `Scene/main.tscn` to reposition stops along the road.
The bays assume the existing horizontal 200x80 truck collider and no scaling.

Battery is a simple mission value: there is no energy consumption, charging
cost, persistent inventory or reward system yet. No Excel data is required.
The truck's missing speed and null handling override have also been fixed.

## Verification

Run `dotnet build` to compile the game. Run
`dotnet run --project Tests/MissionRules.csproj` for the standalone mission-rule
checks (ordering, moving versus parked, cargo, charging pause/resume and completion).

In Godot, also play through all three bays. Try E outside a bay and while
moving, visit delivery before pickup, leave the charger halfway through,
and use Restart missions. These scene/input checks require a Godot playtest;
the standalone tests cover the mission rules only.
