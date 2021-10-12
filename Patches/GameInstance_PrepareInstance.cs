using System;
using System.Collections.Generic;
using HarmonyLib;
using Nick;
using UnityEngine;

namespace NickSkins.Patches
{
	// Token: 0x0200000A RID: 10
	[HarmonyPatch(typeof(GameInstance), "CopyState")]
	class GameInstance_CopyState
	{
		// Token: 0x06000019 RID: 25 RVA: 0x00002ABF File Offset: 0x00000CBF
		private static void Postfix(ref GameInstance __instance)
		{
			var agents = NickSkins.Utils.GetFielder.GetPrivateField<List<GameAgent>>(__instance, "agents");
			foreach (GameAgent gameAgent in agents)
			{
				if (gameAgent.name.StartsWith("char_"))
                {
					Renderer[] componentsInChildren = gameAgent.gameObject.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(0).GetComponentsInChildren<Renderer>(true);

				}
			}
		}
	}
}
