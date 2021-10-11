// compile with: /reference:UnityEngine.dll /reference:TypeBindConflicts=UnityEngine.CoreModule.dll  
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using HarmonyLib;
using Nick;
using NickSkins.Utils;
using UnityEngine.Networking;
using System.Linq;
using System.Threading.Tasks;
using BepInEx;
using UnityEngine.SceneManagement;
using UnityEngine;
using SweetVictoryToo;
using System.Reflection;

namespace NickSkins.Management
{

	public class ExternalizedSkinManager
	{
		public static string rootCustomSkinsPath;
		public List<CharacterMetaData> charactersingeneral;
		public Dictionary<string, LoadedSkin> loadedSkins;
		public static ExternalizedSkinManager Instance;
		public Scene[] sceneList;

		public ExternalizedSkinManager MeIrl()
        {
			return this;
        }
		public void Init()
		{
			charactersingeneral = new List<CharacterMetaData>();
			rootCustomSkinsPath = Path.Combine(Paths.BepInExRootPath, "CustomSkins");
			rootCustomSkinsPath = rootCustomSkinsPath.Replace("\\", "/");
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
			path = path.Replace("\\", "/");
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
				SceneManager.sceneLoaded += OnSceneLoaded;

				void OnSceneLoaded(Scene scene, LoadSceneMode mode)
				{
					if (scene.IsValid() && scene.name != "MenuScene")
					{
						Plugin.LogInfo("Loaded scene " + scene.name);
						GameObject newObject = GameObject.Instantiate(scene.GetRootGameObjects()[0]);
						SceneManager.sceneLoaded -= OnSceneLoaded;
						Plugin.LogInfo("Loaded loadedSkins on " + newObject.name);
						//ExternalizedSkinManager.Instance.loadedSkins = (Dictionary<string, LoadedSkin>)AccessTools.Field(typeof(LoadedSkin), "loadedSkins").GetValue(UnityEngine.Object.FindObjectOfType<Nick.LoadedSkin>());
						//if the bottom command breaks, the top is a backup
						if (ExternalizedSkinManager.Instance.loadedSkins == null)
						{

							ExternalizedSkinManager.Instance.loadedSkins = (Dictionary<string, LoadedSkin>)AccessTools.Field(typeof(LoadedSkin), "loadedSkins").GetValue(newObject.GetComponent<Nick.LoadedSkin>());
						}

						SkinData.TextureSwitch[] temtexturedata = NickSkins.Utils.GetFielder.GetFieldValue<SkinData.TextureSwitch[]>(newObject.GetComponent<Nick.LoadedSkin>().skin, "textureSwitches");

					SkinData tempskindata = ScriptableObject.CreateInstance<SkinData>();
					tempskindata.skinid = folderName;
					SkinData.MeshSwitch[] originalmeshdata = NickSkins.Utils.GetFielder.GetFieldValue<SkinData.MeshSwitch[]>(ExternalizedSkinManager.Instance.loadedSkins[toreplace.skins[0].id].skin, "meshSwitches");
					SkinData.MeshSwitch[] newmeshdata = NickSkins.Utils.GetFielder.GetFieldValue<SkinData.MeshSwitch[]>(tempskindata, "meshSwitches");
					SkinData.TextureSwitch[] originaltexturedata = NickSkins.Utils.GetFielder.GetFieldValue<SkinData.TextureSwitch[]>(ExternalizedSkinManager.Instance.loadedSkins[toreplace.skins[0].id].skin, "textureSwitches");
					SkinData.TextureSwitch[] newtexturedata = NickSkins.Utils.GetFielder.GetFieldValue<SkinData.TextureSwitch[]>(tempskindata, "textureSwitches");
					newtexturedata = originaltexturedata;
					newmeshdata = originalmeshdata;

					foreach (string text in from x in Directory.GetFiles(path)
												where x.ToLower().EndsWith(".png")
												select x)
						{
							
							Plugin.LogInfo("Found PNG " + text);
							string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(text);

							foreach (Nick.SkinData.TextureSwitch texavery in newtexturedata)
							{
								Plugin.LogInfo("Found Texture " + texavery.id);
								for (var ayy = 0; ayy < texavery.textures.Length; ayy++)
								{
									string remember = texavery.textures[ayy].name;
									if (fileNameWithoutExtension.ToLower() == remember.ToLower())
									{
										Texture2D thisoldtexture = BrainFailProductions.PolyFew.AsImpL.TextureLoader.LoadTextureFromUrl(text.Replace("\\", "/"));
										var rawData = System.IO.File.ReadAllBytes(text);
										Texture2D tex = new Texture2D(2, 2); // Create an empty Texture; size doesn't matter (she said)
										tex.LoadRawTextureData(rawData);
										texavery.textures[ayy] = thisoldtexture;
										Plugin.LogInfo("Replaced Texture" + remember);
									}

								}
							}
						}


						LoadedSkin skintoload = new LoadedSkin
                        {
                            skinId = folderName,
                            skin = tempskindata
                        };

                        //skintoload.skin = 
                        ExternalizedSkinManager.Instance.loadedSkins.Add(folderName, skintoload);
						ExternalizedSkinManager.Instance.sceneList.AddItem(SceneManager.GetSceneByName(toreplace.skins[0].id));
						Plugin.LogInfo($"Found custom skin: {parentFolderName}\\{folderName}");
					}

					else
					{
						Plugin.LogInfo("the scene didnt load in time :( ");

					}
				}

			}
		}


	}
}