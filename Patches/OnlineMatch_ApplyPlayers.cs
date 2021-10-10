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
	[HarmonyPatch(typeof(OnlineMatch), "ApplyPlayers")]
	class OnlineMatch_ApplyPlayers
	{

		static void Prefix(OnlineMatch __instance, ref GameSetup gs, ref GameMetaData metaData, ref GameController[] ctrls )
		{
			for (int i = 0; i < metaData.characterMetas.Length; i++)
			{
                if (!UnityEngine.Application.CanStreamedLevelBeLoaded(metaData.characterMetas[i].skins[i].id))
                {
                    metaData.characterMetas[i].skins[i] = metaData.characterMetas[i].skins[0];
				}
			}
		}
	}
}