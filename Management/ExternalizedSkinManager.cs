// compile with: /reference:UnityEngine.dll /reference:TypeBindConflicts=UnityEngine.CoreModule.dll  
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using HarmonyLib;
using Nick;
using UnityEngine.Networking;
using System.Linq;
using System.Threading.Tasks;
using BepInEx;
using UnityEngine.SceneManagement;
using UnityEngine;
using SweetVictoryToo;

namespace NickSkins.Management
{
	public class ExternalizedSkinManager
	{
		public static string rootCustomSkinsPath;
		public List<CharacterMetaData> charactersingeneral;
		public Scene newScene;
		public Dictionary<string, LoadedSkin> loadedSkins;
		public static ExternalizedSkinManager Instance;

		public ExternalizedSkinManager MeIrl()
        {
			return this;
        }
		public void Init()
		{
			charactersingeneral = new List<CharacterMetaData>();
			rootCustomSkinsPath = Path.Combine(Paths.BepInExRootPath, "CustomSkins");
			Instance = this;

			Task.Run(delegate ()
			{
				// Create the folder if it doesn't exist
				Directory.CreateDirectory(rootCustomSkinsPath);
				// Generate folders, incase any were deleted
				foreach (CharacterMetaData metaknight in UnityEngine.Resources.FindObjectsOfTypeAll(typeof(Nick.CharacterMetaData)))
				{
					charactersingeneral.Add(metaknight);
					Plugin.LogInfo("gonna load up some " + metaknight.id);
					Directory.CreateDirectory(Path.Combine(rootCustomSkinsPath, metaknight.id));
				}
			for (var eye = 0; eye < UnityEngine.Resources.FindObjectsOfTypeAll(typeof(Nick.CharacterMetaData)).Length; eye++)
                {
					LoadFromSubDirectories(charactersingeneral[eye].id);
				}
			}
			);
		}

		public static void LoadFromSubDirectories(string parentFolderName)
		{
			var subDirectories = Directory.GetDirectories(Path.Combine(rootCustomSkinsPath, parentFolderName));

			foreach (string directory in subDirectories)
			{
				var directoryName = Path.GetFileName(directory);

				Plugin.LogInfo($"directory {directoryName} full path {directory}");
				LoadTextureSwapFromFolder(parentFolderName, directoryName);
			}
		}


		public static void LoadSkinsFromFolder(string parentFolderName, string directoryName)
		{
			var subDirectories = Directory.GetDirectories(Path.Combine(rootCustomSkinsPath, parentFolderName, directoryName));

			foreach (string directory in subDirectories)
			{
				var newDirectoryName = Path.GetFileName(directory);

				Plugin.LogInfo($"directory {directoryName} parent folder name {parentFolderName} full path {directory}");

				LoadTextureSwapFromFolder(parentFolderName, directoryName);
			}
		}

		public static void LoadTextureSwapFromFolder(string parentFolderName, string folderName)
		{
			Plugin.LogInfo("new directory is " + folderName);
			string path = Path.Combine(rootCustomSkinsPath, parentFolderName, folderName);
			CharacterMetaData toreplace = null;

			CharacterMetaData GetCharacterById(string id)
			{
				foreach (CharacterMetaData characterMetaData in UnityEngine.Resources.FindObjectsOfTypeAll(typeof(Nick.CharacterMetaData)))
				{
					if (characterMetaData.id == id)
					{
						return characterMetaData;
					}
				}
				Plugin.LogInfo("Character not found: " + id);
				return null;
			}

			toreplace = GetCharacterById(parentFolderName);
			if (toreplace != null)
			{
				Plugin.LogInfo("Character Found:" + parentFolderName);
				CharacterMetaData.CharacterSkinMetaData skinmeta = new CharacterMetaData.CharacterSkinMetaData();
				skinmeta.locNames = toreplace.skins[0].locNames;
				skinmeta.resPortraits = toreplace.skins[0].resPortraits;
				skinmeta.resMediumPortraits = toreplace.skins[0].resMediumPortraits;
				skinmeta.resMiniPortraits = toreplace.skins[0].resMiniPortraits;
				skinmeta.id = folderName;
				GetCharacterById(parentFolderName).skins = toreplace.skins.AddToArray(skinmeta); // end of skin replacing metadata


				SceneManager.LoadScene(toreplace.skins[0].id, LoadSceneMode.Additive);
				ExternalizedSkinManager.Instance.loadedSkins = (Dictionary<string, LoadedSkin>)AccessTools.Field(typeof(LoadedSkin), "loadedSkins").GetValue(Resources.FindObjectsOfTypeAll(typeof(Nick.LoadedSkin)));
				SkinData.TextureSwitch[] temtexturedata = (SkinData.TextureSwitch[])AccessTools.Field(typeof(SkinData), "textureSwitches").GetValue(Resources.FindObjectsOfTypeAll(typeof(Nick.SkinData)));

				foreach (string text in from x in Directory.GetFiles(path)
										where x.ToLower().EndsWith(".png")
										select x)
				{


					foreach (Nick.SkinData.TextureSwitch texavery in temtexturedata)
					{
						for (var ayy = 0; ayy < texavery.textures.Length; ayy++)
						{
							string remember = texavery.textures[ayy].name;
							if (text.ToLower().EndsWith(texavery.textures[ayy].name.ToLower())) {
								texavery.textures[ayy] = BrainFailProductions.PolyFew.AsImpL.TextureLoader.LoadTextureFromUrl("file:///" + path);
							}

						}
					}


				}
				SkinData tempskindata = ExternalizedSkinManager.Instance.loadedSkins[parentFolderName].skin;
				LoadedSkin skintoload = new LoadedSkin();
				skintoload.skinId = folderName;
				skintoload.skin = tempskindata;

				//skintoload.skin = 
				ExternalizedSkinManager.Instance.loadedSkins.Add(folderName, skintoload);
			}
			Plugin.LogInfo($"Found custom skin: {parentFolderName}\\{folderName}");

		}


	}
}