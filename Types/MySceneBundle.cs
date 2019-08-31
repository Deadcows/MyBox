// ----------------------------------------------------------------------------
// Author: Kaynn, Yeo Wen Qin
// https://github.com/Kaynn-Cahya
// Date:   20/08/2019
// ----------------------------------------------------------------------------

using System.Collections.Generic;
using UnityEngine.SceneManagement;
using MyBox.Internal;

namespace MyBox
{
	/// <summary>
	/// For passing objects between different scenes in Unity.
	/// </summary>
	public static class MySceneBundle
	{
		public enum TransferSceneBundleOption
		{
			TRANSFER_ON_LAST_SCENE,
			TRANSFER_ON_ANY_SCENE_UNLOADED
		}

		/// <summary>
		/// Set the option for when will the scene bundle transfer.
		/// </summary>
		public static TransferSceneBundleOption SceneBundleTransferOption { get; set; }

		private static SceneBundle currentSceneBundle;
		private static SceneBundle nextSceneBundle;

		static MySceneBundle()
		{
			SceneBundleTransferOption = TransferSceneBundleOption.TRANSFER_ON_LAST_SCENE;

			currentSceneBundle = new SceneBundle();
			nextSceneBundle = new SceneBundle();

			SceneManager.sceneUnloaded += PrepareSceneBundleForNextSceneByTransferOptions;
		}

		private static void PrepareSceneBundleForNextSceneByTransferOptions(Scene unloadedScene)
		{
			if (SceneBundleTransferOption == TransferSceneBundleOption.TRANSFER_ON_LAST_SCENE && IsUnloadingLastScene())
			{
				PrepareSceneBundleForNextScene();
			}
			else if (SceneBundleTransferOption == TransferSceneBundleOption.TRANSFER_ON_ANY_SCENE_UNLOADED)
			{
				PrepareSceneBundleForNextScene();
			}
		}

		private static bool IsUnloadingLastScene()
		{
			return SceneManager.sceneCount == 2;
		}

		private static void PrepareSceneBundleForNextScene()
		{
			currentSceneBundle = nextSceneBundle;
			nextSceneBundle = new SceneBundle();
		}

		/// <summary>
		/// Carry over all data in the current bundle to the next bundle.
		/// </summary>
		/// <param name="overrideData">True to override any data in the next bundle.</param>
		public static void CarryOverCurrentBundleToNextBundle(bool overrideData = false)
		{
			nextSceneBundle.BoolData.AddBundleData(currentSceneBundle.BoolData, overrideData);
			nextSceneBundle.IntData.AddBundleData(currentSceneBundle.IntData, overrideData);
			nextSceneBundle.FloatData.AddBundleData(currentSceneBundle.FloatData, overrideData);
			nextSceneBundle.StringData.AddBundleData(currentSceneBundle.StringData, overrideData);
			nextSceneBundle.ObjectData.AddBundleData(currentSceneBundle.ObjectData, overrideData);
		}

		#region Add_Data_Functions

		/// <summary>
		/// Add a string value data that will be carried over to the next scene.
		/// </summary>
		/// <param name="dataKey">The key to identify the data you are storing.</param>
		/// <param name="data"></param>
		/// <param name="overrideIfExists">True to override if a data with the same key already exists</param>
		public static void AddStringDataToBundle(string dataKey, string data, bool overrideIfExists = true)
		{
			nextSceneBundle.StringData.AddData(dataKey, data, overrideIfExists);
		}

		/// <summary>
		/// Add a float value data that will be carried over to the next scene.
		/// </summary>
		/// <param name="dataKey">The key to identify the data you are storing.</param>
		/// <param name="data"></param>
		/// <param name="overrideIfExists">True to override if a data with the same key already exists</param>
		public static void AddFloatDataToBundle(string dataKey, float data, bool overrideIfExists = true)
		{
			nextSceneBundle.FloatData.AddData(dataKey, data, overrideIfExists);
		}

		/// <summary>
		/// Add a integer value data that will be carried over to the next scene.
		/// </summary>
		/// <param name="dataKey">The key to identify the data you are storing.</param>
		/// <param name="data"></param>
		/// <param name="overrideIfExists">True to override if a data with the same key already exists</param>
		public static void AddIntDataToBundle(string dataKey, int data, bool overrideIfExists = true)
		{
			nextSceneBundle.IntData.AddData(dataKey, data, overrideIfExists);
		}

		/// <summary>
		/// Add a bool value data that will be carried over to the next scene.
		/// </summary>
		/// <param name="dataKey">The key to identify the data you are storing.</param>
		/// <param name="data"></param>
		/// <param name="overrideIfExists">True to override if a data with the same key already exists</param>
		public static void AddBoolDataToBundle(string dataKey, bool data, bool overrideIfExists = true)
		{
			nextSceneBundle.BoolData.AddData(dataKey, data, overrideIfExists);
		}

		/// <summary>
		/// Add a object value data that will be carried over to the next scene.
		/// </summary>
		/// <param name="dataKey">The key to identify the data you are storing.</param>
		/// <param name="data"></param>
		/// <param name="overrideIfExists">True to override if a data with the same key already exists</param>
		public static void AddObjectDataToBundle(string dataKey, object data, bool overrideIfExists = true)
		{
			nextSceneBundle.ObjectData.AddData(dataKey, data, overrideIfExists);
		}

		#endregion

		#region Get_Data_Functions

		/// <summary>
		/// Try to fetch a string data from the bundle that was passed from the previous scene.
		/// </summary>
		/// <param name="dataKey">The identifier for the data to fetch</param>
		/// <param name="result">Result of the data (Default or null if it does not exists)</param>
		/// <returns>True if the data with the key exists.</returns>
		public static bool TryGetStringDataFromBundle(string dataKey, out string result)
		{
			return currentSceneBundle.StringData.TryGetData(dataKey, out result);
		}

		/// <summary>
		/// Try to fetch a float data from the bundle that was passed from the previous scene.
		/// </summary>
		/// <param name="dataKey">The identifier for the data to fetch</param>
		/// <param name="result">Result of the data (Default or null if it does not exists)</param>
		/// <returns>True if the data with the key exists.</returns>
		public static bool TryGetFloatDataFromBundle(string dataKey, out float result)
		{
			return currentSceneBundle.FloatData.TryGetData(dataKey, out result);
		}

		/// <summary>
		/// Try to fetch an integer data from the bundle that was passed from the previous scene.
		/// </summary>
		/// <param name="dataKey">The identifier for the data to fetch</param>
		/// <param name="result">Result of the data (Default or null if it does not exists)</param>
		/// <returns>True if the data with the key exists.</returns>
		public static bool TryGetIntDataFromBundle(string dataKey, out int result)
		{
			return currentSceneBundle.IntData.TryGetData(dataKey, out result);
		}

		/// <summary>
		/// Try to fetch a bool data from the bundle that was passed from the previous scene.
		/// </summary>
		/// <param name="dataKey">The identifier for the data to fetch</param>
		/// <param name="result">Result of the data (Default or null if it does not exists)</param>
		/// <returns>True if the data with the key exists.</returns>
		public static bool TryGetBoolDataFromBundle(string dataKey, out bool result)
		{
			return currentSceneBundle.BoolData.TryGetData(dataKey, out result);
		}

		/// <summary>
		/// Try to fetch an object data from the bundle that was passed from the previous scene.
		/// </summary>
		/// <param name="dataKey">The identifier for the data to fetch</param>
		/// <param name="result">Result of the data (Default or null if it does not exists)</param>
		/// <returns>True if the data with the key exists.</returns>
		public static bool TryGetObjectDataFromBundle(string dataKey, out object result)
		{
			return currentSceneBundle.ObjectData.TryGetData(dataKey, out result);
		}

		#endregion
	}
}

namespace MyBox.Internal
{
	internal class SceneBundle
	{
		private Bundle<string> stringData;
		private Bundle<float> floatData;
		private Bundle<int> intData;
		private Bundle<bool> boolData;

		private Bundle<object> objectData;

		internal SceneBundle()
		{
			stringData = new Bundle<string>();
			floatData = new Bundle<float>();
			intData = new Bundle<int>();
			boolData = new Bundle<bool>();
			objectData = new Bundle<object>();
		}

		internal Bundle<string> StringData
		{
			get { return stringData; }
		}

		internal Bundle<float> FloatData
		{
			get { return floatData; }
		}

		internal Bundle<int> IntData
		{
			get { return intData; }
		}

		internal Bundle<bool> BoolData
		{
			get { return boolData; }
		}

		internal Bundle<object> ObjectData
		{
			get { return objectData; }
		}
	}

	internal class Bundle<T>
	{
		private Dictionary<string, T> bundleData;

		internal Bundle()
		{
			bundleData = new Dictionary<string, T>();
		}

		/// <summary>
		/// Adds a data to this bundle
		/// </summary>
		/// <param name="dataKey">The key to identify this data</param>
		/// <param name="data"></param>
		/// <param name="overrideIfExists">True to override this data if it exists</param>
		internal void AddData(string dataKey, T data, bool overrideIfExists = true)
		{
			bool haveDataWithSameKey = bundleData.ContainsKey(dataKey);

			if (haveDataWithSameKey && overrideIfExists)
			{
				bundleData[dataKey] = data;
			}
			else if (!haveDataWithSameKey)
			{
				bundleData.Add(dataKey, data);
			}
		}

		internal void AddData(KeyValuePair<string, T> keyValuePair, bool overrideIfExists = true)
		{
			bool haveDataWithSameKey = bundleData.ContainsKey(keyValuePair.Key);

			if (haveDataWithSameKey && overrideIfExists)
			{
				bundleData[keyValuePair.Key] = keyValuePair.Value;
			}
			else if (!haveDataWithSameKey)
			{
				bundleData.Add(keyValuePair.Key, keyValuePair.Value);
			}
		}

		internal bool TryGetData(string dataKey, out T result)
		{
			return bundleData.TryGetValue(dataKey, out result);
		}

		/// <summary>
		/// True if a data with the datakey exists
		/// </summary>
		/// <param name="dataKey"></param>
		/// <returns></returns>
		internal bool DataExists(string dataKey)
		{
			return bundleData.ContainsKey(dataKey);
		}

		internal Dictionary<string, T> GetBundleData()
		{
			return new Dictionary<string, T>(new Dictionary<string, T>(bundleData));
		}

		/// <summary>
		/// Add a bundle of data to this bundle
		/// </summary>
		/// <param name="bundle">The data bundle</param>
		/// <param name="overrideIfExists">True to override if any of the data already exists in this data bundle</param>
		internal void AddBundleData(Dictionary<string, T> bundle, bool overrideIfExists)
		{
			foreach (var data in bundle)
			{
				AddData(data, overrideIfExists);
			}
		}

		/// <summary>
		/// Add a bundle of data to this bundle
		/// </summary>
		/// <param name="bundle">The data bundle</param>
		/// <param name="overrideIfExists">True to override the data if it already exists in this data bundle</param>
		internal void AddBundleData(Bundle<T> bundle, bool overrideIfExists)
		{
			AddBundleData(bundle.bundleData, overrideIfExists);
		}
	}
}
