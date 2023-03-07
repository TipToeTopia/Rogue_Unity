using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Unity.Netcode.Runtime")]
[assembly: InternalsVisibleTo("Unity.Netcode.RuntimeTests")]
[assembly: InternalsVisibleTo("Unity.Netcode.EditorTests")]
[assembly: InternalsVisibleTo("Unity.Multiplayer.Tools.MetricTypes")]
[assembly: InternalsVisibleTo("Unity.Multiplayer.Tools.NetStats.Tests.Editor")]
[assembly: InternalsVisibleTo("Unity.Multiplayer.Tools.NetStatsReporting")]
[assembly: InternalsVisibleTo("Unity.Multiplayer.Tools.NetworkProfiler.Runtime")]
[assembly: InternalsVisibleTo("Unity.Multiplayer.Tools.NetworkProfiler.Editor")]
[assembly: InternalsVisibleTo("Unity.Multiplayer.Tools.NetworkProfiler.Tests.Editor")]
[assembly: InternalsVisibleTo("Unity.Multiplayer.Tools.NetStatsMonitor.Component")]
[assembly: InternalsVisibleTo("Unity.Multiplayer.Tools.NetStatsMonitor.Configuration")]
[assembly: InternalsVisibleTo("Unity.Multiplayer.Tools.NetStatsMonitor.Editor")]
[assembly: InternalsVisibleTo("Unity.Multiplayer.Tools.NetStatsMonitor.Implementation")]
[assembly: InternalsVisibleTo("Unity.Multiplayer.Tools.NetStatsMonitor.Tests.Implementation")]
[assembly: InternalsVisibleTo("Unity.Multiplayer.Tools.NetStats.CodeGen")]
[assembly: InternalsVisibleTo("Unity.Netcode.TestHelpers.Runtime")]
[assembly: InternalsVisibleTo("TestProject.RuntimeTests")]
[assembly: InternalsVisibleTo("TestProject.ToolsIntegration.RuntimeTests")]
#if UNITY_EDITOR
[assembly: InternalsVisibleTo("TestProject.EditorTests")]
#endif