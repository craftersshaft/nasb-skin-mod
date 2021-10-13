using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System.Reflection;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using NickSkins.Management;
using BepInEx.Configuration;
using Photon.Realtime;
using Photon.Pun;

namespace SweetVictoryToo
{
    [BepInPlugin("craftersshaft.nasbmods.nasbskins", "NASB Skins", "0.0.7")]
    public class Plugin : BaseUnityPlugin
    {
        internal static Plugin Instance;
        internal ExternalizedSkinManager skinmin;
        public static ConfigEntry<string> LobbyName { get; private set; }
        public static ConfigEntry<string> LobbyDescription { get; private set; }
        public static ConfigEntry<bool> UseSkinsOnline { get; private set; }
        private void Awake()
        {
            // Plugin startup logic
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_NAME} is loaded!");
            Logger.LogInfo("BepInEx Path:" + Paths.BepInExRootPath);
            Logger.LogInfo("Is Garfield DLC?" + UnityEngine.Resources.FindObjectsOfTypeAll(typeof(Nick.CharacterMetaData))[21].ToString());
            Instance = this;

            LobbyName = Config.Bind("Lobby", "Lobby Name", string.Empty, "Change this string to set your lobby's name to a new value.");
            UseSkinsOnline = Config.Bind("Skins", "Use Skins Online", false, "Change this to true to add skins to your lobby. Unimplemented!");
            LobbyDescription = Config.Bind("Lobby", "Lobby Description", "This is a Modded Lobby!", "Change this string to set your lobby's description to a new value. Unimplemented for now!");


            //  Harmony harmony = new Harmony(PluginInfo.PLUGIN_GUID);
            // MethodInfo original = AccessTools.Method(typeof(Nick.GameItems), "SpawnItemForTraining");
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
            skinmin = new ExternalizedSkinManager();
            skinmin.Init();
        }


        public ExternalizedSkinManager GetSkins()
        {
            return skinmin;
        }

        internal static void LogDebug(string message) => Instance.Log(message, LogLevel.Debug);
        internal static void LogInfo(string message) => Instance.Log(message, LogLevel.Info);
        internal static void LogError(string message) => Instance.Log(message, LogLevel.Error);
        private void Log(string message, LogLevel logLevel) => Logger.Log(logLevel, message);
    }
}
