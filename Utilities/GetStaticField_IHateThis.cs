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
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;

namespace NickSkins.Utils
{

	public static class GetFielder
	{

		public static T GetPrivateField<T>(this object obj, string fieldName, Type targetType)
		{
			var prop = targetType.GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
			if (prop == null)
				throw new InvalidOperationException($"{fieldName} is not a member of {targetType.Name}");
			var value = prop.GetValue(obj);
			return (T)value;
		}
		public static T GetPrivateField<T>(this object obj, string fieldName) => obj.GetPrivateField<T>(fieldName, obj.GetType());
		public static object GetCopy(object input)
		{
			using (MemoryStream stream = new MemoryStream())
			{
				BinaryFormatter formatter = new BinaryFormatter();
				formatter.Serialize(stream, input);
				stream.Position = 0;
				return formatter.Deserialize(stream);
			}
		}
	}
}