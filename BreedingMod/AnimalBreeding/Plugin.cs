using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;

namespace AnimalBreeding;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class Plugin : BaseUnityPlugin
{
    internal new static ManualLogSource Logger;

    internal static ConfigEntry<int> PopulationCapPerIsland { get; private set; }
    internal static ConfigEntry<float> PregnancyDuration { get; private set; }

    private static Assembly Assembly { get; } = Assembly.GetExecutingAssembly();
        
    private void Awake()
    {
        // Plugin startup logic
        Logger = base.Logger;
        Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");

        var harmony = new Harmony(MyPluginInfo.PLUGIN_GUID);
        harmony.PatchAll(Assembly);

        PopulationCapPerIsland = Config.Bind("General", "Population cap per island",
            100,
            "If the animal population of an island exceeds this value, no more animals can get pregnant.");
        
        PregnancyDuration = Config.Bind("General", "Pregnancy duration (minutes)",
            15f,
            "The number of minutes it takes for an animal to give birth after becoming pregnant. 15 by default.");

    }
}