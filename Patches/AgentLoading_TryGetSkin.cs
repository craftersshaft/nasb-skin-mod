// compile with: /reference:UnityEngine.dll /reference:TypeBindConflicts=UnityEngine.CoreModule.dll  
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using HarmonyLib;
using Nick;
using static Nick.MusicMetaData;
using UnityEngine.Networking;
using System.Linq;

namespace NickSkins.Patches
{
	[HarmonyPatch(typeof(AgentLoading), "TryGetSkin")]
	class AgentLoading_TryGetSkin
	{

		static bool Prefix(AgentLoading __instance, ref string id, ref SkinData skin)
		{
			if (UnityEngine.Application.CanStreamedLevelBeLoaded(id))
			{
				return true;
			}
			else {
				SweetVictoryToo.Plugin.LogError("apparently the scene " + id + " doesn't exist");
			id = "skin_apple_default";
			return 	true;
			}
            
		}
	}
}