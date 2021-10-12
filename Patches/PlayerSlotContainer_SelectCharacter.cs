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
	[HarmonyPatch(typeof(PlayerSlotContainer), "SelectCharacter")]
	class PlayerSlotContainer_SelectCharacter
	{

		static void Prefix(PlayerSlotContainer __instance, ref CharacterMetaData chmd)
		{
			for (int i = 0; i < chmd.skins.Length; i++)
			{
                if (!UnityEngine.Application.CanStreamedLevelBeLoaded(chmd.skins[i].id))
                {
					//if (!SweetVictoryToo.Plugin.Instance.skinmin.loadedSkins.ContainsKey(chmd.skins[i].id)) {
						SweetVictoryToo.Plugin.LogError("apparently the scene " + chmd.skins[i].id + " doesn't exist");
						//chmd.skins[i] = chmd.skins[0];
					//}
				}
			}
		}
	}
}