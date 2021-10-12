using System;
using System.Collections.Generic;
using HarmonyLib;
using Nick;
using UnityEngine;

namespace NickSkins.Patches
{
	// Token: 0x0200000A RID: 10
	[HarmonyPatch(typeof(SlapNetLobbyListings), "CreateLobby")]
	class SlapNetLobbyListings_CreateLobby
	{
		// Token: 0x06000019 RID: 25 RVA: 0x00002ABF File Offset: 0x00000CBF
		private static void Postfix(ref SlapNetLobbyListings __instance)
		{
			SlapNetLobbyListings.Lobby dalobby = NickSkins.Utils.GetFielder.GetPrivateField<SlapNetLobbyListings.Lobby>(__instance, "joinedlobby");
			if (dalobby != null) {
				dalobby.SetData("serverSkins", string.Join(",", Management.ExternalizedSkinManager.yourSkins));
				Management.ExternalizedSkinManager.currServerSkins = Management.ExternalizedSkinManager.yourSkins;
				if (Management.ExternalizedSkinManager.commonSkins == null)
				{
					Management.ExternalizedSkinManager.commonSkins = Management.ExternalizedSkinManager.yourSkins;

				}
			}
		}
	}
}
