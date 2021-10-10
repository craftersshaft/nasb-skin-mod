﻿using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System.Reflection;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using NickSkins.Management;
namespace SweetVictoryToo
{
    [BepInPlugin("craftersshaft.nasbmods.mousebumper", "Place Bumper on Click", "0.0.1")]
    public class Plugin : BaseUnityPlugin
    {
        internal static Plugin Instance;
        internal ExternalizedSkinManager skinmin;
        private void Awake()
        {
            // Plugin startup logic
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_NAME} is loaded!");
            Logger.LogInfo("BepInEx Path:" + Paths.BepInExRootPath);
            Logger.LogInfo("Is Garfield DLC?" + UnityEngine.Resources.FindObjectsOfTypeAll(typeof(Nick.CharacterMetaData))[21].ToString());
            Instance = this;
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