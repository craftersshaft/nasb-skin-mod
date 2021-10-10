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
	[HarmonyPatch(typeof(PlayerSlotContainer), "Update")]
	class PlayerSlotContainer_Update
	{

		static void Prefix(PlayerSlotContainer __instance, ref MenuTextContent ___playerTagText)
		{
			
            if (__instance.playerCursor.menuInput.IsButtonPress(MenuAction.ActionButton.Opt2))
			{
				__instance.PlayerSetup.skin += 1;
				if (__instance.PlayerSetup.skin > (__instance.Character.skins.Length - 1)) { __instance.PlayerSetup.skin = 0; };
				___playerTagText.SetString(Localization.abbr_player + (__instance.playerSlotIndex + 1).ToString() + "\nSkin: " + (__instance.PlayerSetup.skin + 1).ToString());
			}
		}
	}
}