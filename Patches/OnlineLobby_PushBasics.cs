using System;
using System.Collections.Generic;
using HarmonyLib;
using Nick;
using NickSkins.Utils;
using UnityEngine;

namespace NickSkins.Patches
{
	// Token: 0x0200000A RID: 10
	[HarmonyPatch(typeof(OnlineLobby), "PushBasics")]
	class OnlineLobby_PushBasics
	{
		// Token: 0x06000019 RID: 25 RVA: 0x00002ABF File Offset: 0x00000CBF
		private static bool Prefix(OnlineLobby __instance, ref OnlineLobby.BasicSettings newBasics)
		{
			if (SweetVictoryToo.Plugin.LobbyName.Value != "")
			{
				newBasics.title = SweetVictoryToo.Plugin.LobbyName.Value;
			}
			if (SweetVictoryToo.Plugin.LobbyDescription.Value != "")
			{

				__instance.InvokeMethod("SetLobbyData", "titleupd", __instance.GetPrivateField<ushort>("currentBasicsVersion"), new SlapNetwork.LobbyDataPair[]
			{
				new SlapNetwork.LobbyDataPair("description", SweetVictoryToo.Plugin.LobbyDescription.Value)
			});

			}
			__instance.InvokeMethod("SetLobbyData", "titleupd", __instance.GetPrivateField<ushort>("currentBasicsVersion"), new SlapNetwork.LobbyDataPair[]
			{
				new SlapNetwork.LobbyDataPair("title", newBasics.title),
				new SlapNetwork.LobbyDataPair("pass", newBasics.pass)
			});
			__instance.SetPrivateField("currentBasics", newBasics);
			return false;	
		}
	}
}
