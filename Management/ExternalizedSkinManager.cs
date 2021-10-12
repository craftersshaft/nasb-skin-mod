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
		public Dictionary<string, Scene> sceneList = new();
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
						GameObject newObject = new GameObject("loadedCustomSkin");
						Nick.LoadedSkin loadyskin = newObject.AddComponent<Nick.LoadedSkin>() as Nick.LoadedSkin;
						loadyskin.skinId = folderName;
						loadyskin.skin = ScriptableObject.CreateInstance<SkinData>();
						loadyskin.skin.skinid = folderName.Substring(folderName.LastIndexOf("_") + 1);
						SceneManager.sceneLoaded -= OnSceneLoaded;
						Plugin.LogInfo("Loaded loadedSkins on " + newObject.name);
						//ExternalizedSkinManager.Instance.loadedSkins = (Dictionary<string, LoadedSkin>)AccessTools.Field(typeof(LoadedSkin), "loadedSkins").GetValue(UnityEngine.Object.FindObjectOfType<Nick.LoadedSkin>());
						//if the bottom command breaks, the top is a backup
						if (ExternalizedSkinManager.Instance.loadedSkins == null)
						{

							ExternalizedSkinManager.Instance.loadedSkins = (Dictionary<string, LoadedSkin>)AccessTools.Field(typeof(LoadedSkin), "loadedSkins").GetValue(loadyskin);
						}

					SkinData.TextureSwitch[] temtexturedata = NickSkins.Utils.GetFielder.GetPrivateField<SkinData.TextureSwitch[]>(loadyskin.skin, "textureSwitches");
						if (temtexturedata == null)
                        {
							loadyskin.skin.SetPrivateField("textureSwitches", new SkinData.TextureSwitch[25]);
						}

					SkinData tempskindata = ScriptableObject.CreateInstance<SkinData>();
					tempskindata.skinid = folderName.Substring(folderName.LastIndexOf("_")+1);
					tempskindata.name = folderName;
					SkinData.MeshSwitch[] originalmeshdata = NickSkins.Utils.GetFielder.GetPrivateField<SkinData.MeshSwitch[]>(ExternalizedSkinManager.Instance.loadedSkins[toreplace.skins[0].id].skin, "meshSwitches");
					SkinData.MeshSwitch[] newmeshdata = new SkinData.MeshSwitch[originalmeshdata.Length];
					SkinData.MeshSwitch[] newobjmeshdata = NickSkins.Utils.GetFielder.GetPrivateField<SkinData.MeshSwitch[]>(loadyskin.skin, "meshSwitches");
						SweetVictoryToo.Plugin.LogInfo("loadedskin skinid is " + loadyskin.skinId);
					SkinData.TextureSwitch[] originaltexturedata = NickSkins.Utils.GetFielder.GetPrivateField<SkinData.TextureSwitch[]>(ExternalizedSkinManager.Instance.loadedSkins[toreplace.skins[0].id].skin, "textureSwitches");
					SkinData.TextureSwitch[] newtexturedata = new SkinData.TextureSwitch[originaltexturedata.Length];
					SkinData.TextureSwitch[] newobjtexturedata = NickSkins.Utils.GetFielder.GetPrivateField<SkinData.TextureSwitch[]>(loadyskin.skin, "textureSwitches");
					newobjtexturedata = new SkinData.TextureSwitch[originaltexturedata.Length];

						//Array.Copy(originaltexturedata, newtexturedata, originaltexturedata.Length);
						//Array.Copy(originalmeshdata, newmeshdata, originalmeshdata.Length);
						//originalmeshdata.CopyTo(newmeshdata, 0);
						//originaltexturedata.CopyTo(newtexturedata, 0);
						//newmeshdata = originalmeshdata.Select(a => (SkinData.MeshSwitch)a.Clone()).ToArray();
						//newmeshdata = Array.ConvertAll(originalmeshdata, a => (SkinData.MeshSwitch)a.Clone());
						Texture2D duplicateTexture(Texture2D source)
						{
							RenderTexture renderTex = RenderTexture.GetTemporary(
										source.width,
										source.height,
										0,
										RenderTextureFormat.Default,
										RenderTextureReadWrite.Linear);

							Graphics.Blit(source, renderTex);
							RenderTexture previous = RenderTexture.active;
							RenderTexture.active = renderTex;
							Texture2D readableText = new Texture2D(source.width, source.height);
							readableText.ReadPixels(new Rect(0, 0, renderTex.width, renderTex.height), 0, 0);
							readableText.Apply();
							readableText.name = source.name;
							RenderTexture.active = previous;
							RenderTexture.ReleaseTemporary(renderTex);
							return readableText;
						}
						for (var ariba = 0; ariba < originalmeshdata.Length; ariba++)
						{
							var newmesh = new SkinData.MeshSwitch();
							newmesh.id = originalmeshdata[ariba].id;
							Plugin.LogInfo("Cloning Mesh " + newmesh.id);
							var submes = new Mesh[originalmeshdata[ariba].meshes.Length];
							for (var tempgraphy = 0; tempgraphy < originalmeshdata[ariba].meshes.Length; tempgraphy++) { submes[tempgraphy] = originalmeshdata[ariba].meshes[tempgraphy]; }
							newmesh.meshes = submes;
							newmeshdata[ariba] = newmesh;
						}
						for (var temporary = 0; temporary < originaltexturedata.Length; temporary++)
						{
							var newtex = new SkinData.TextureSwitch();
							newtex.id = originaltexturedata[temporary].id;
							Plugin.LogInfo("Cloning Texture " + newtex.id);
							var subtex = new Texture2D[originaltexturedata[temporary].textures.Length];
							for (var subway = 0; subway < originaltexturedata[temporary].textures.Length; subway++) {
								subtex[subway] = duplicateTexture(originaltexturedata[temporary].textures[subway]);
							}
							newtex.textures = subtex;
							newtexturedata[temporary] = newtex;
						}
						newobjmeshdata = newmeshdata;
						newobjtexturedata = newtexturedata;
						Plugin.LogInfo("Texture Data Length:" + newtexturedata.Length);
						foreach (string text in from x in Directory.GetFiles(path)
												where x.ToLower().EndsWith(".png")
												select x)
						{
                            path = path.Replace("\\", "/");

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

						loadyskin.skinId = folderName;
						SkinData.TextureSwitch[] tempskindatatextures = NickSkins.Utils.GetFielder.GetPrivateField<SkinData.TextureSwitch[]>(tempskindata, "textureSwitches");
						SkinData.MeshSwitch[] tempskindatameshes = NickSkins.Utils.GetFielder.GetPrivateField<SkinData.MeshSwitch[]>(tempskindata, "meshSwitches");
						tempskindatameshes = newmeshdata;
						tempskindatatextures = newtexturedata;
						loadyskin.skin = tempskindata;

						var loadytexs = NickSkins.Utils.GetFielder.GetPrivateField<SkinData.TextureSwitch[]>(loadyskin.skin, "textureSwitches");

						loadytexs = newtexturedata; //a doublecheck
						loadyskin.skin.SetPrivateField("textureSwitches", loadytexs);
						loadyskin.skin.SetPrivateField("meshSwitches", newmeshdata);
						Plugin.LogInfo("new textureswitches should be " + loadytexs.Length);

						
						//skintoload.skin = 
						ExternalizedSkinManager.Instance.loadedSkins.Add(folderName, loadyskin);
						ExternalizedSkinManager.Instance.sceneList.Add(toreplace.skins[0].id, SceneManager.GetSceneByName(toreplace.skins[0].id));
						ExternalizedSkinManager.Instance.sceneList.Add(folderName, scene);

						string dacurrentConfig = GameObject.Find("Agent Loader").GetComponent<Nick.AgentLoading>().idScenes.scenesConfigFile.text + (folderName+":"+toreplace.skins[0].id+ "\n");
						TextAsset configFinalized = new TextAsset(dacurrentConfig);
						GameObject.Find("Agent Loader").GetComponent<Nick.AgentLoading>().idScenes.scenesConfigFile = new TextAsset(dacurrentConfig);
						GameObject.Find("Agent Loader").GetComponent<Nick.AgentLoading>().idScenes.IdDict.Add(folderName, toreplace.skins[0].id);

						var loadstates = (Dictionary<string, Nick.AgentLoading.LoadState>)AccessTools.Field(typeof(AgentLoading), "loadStates").GetValue(GameObject.Find("Agent Loader").GetComponent<Nick.AgentLoading>());
						var loadstatus = new AgentLoading.LoadState();
						//loadstatus.phase = AgentLoading.LoadPhase.Loaded;
						loadstates.Add(folderName, loadstatus);
						//GameObject.Find("Agent Loader").GetComponent<Nick.AgentLoading>().SetPrivateField("loadStates", loadstates);

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