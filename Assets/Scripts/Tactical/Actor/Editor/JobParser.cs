using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using Tactical.Core;
using Tactical.Core.Component;
using Tactical.Actor.Component;

namespace Tactical.Actor.Editor {

	public static class JobParser {

		[MenuItem("Pre Production/Parse Jobs")]
		public static void Parse() {
			CreateDirectories();
			ParseStartingStats();
			ParseGrowthStats();
			AssetDatabase.SaveAssets();
			AssetDatabase.Refresh();
		}

		private static void CreateDirectories () {
			if (!AssetDatabase.IsValidFolder("Assets/Resources/Jobs")) {
				AssetDatabase.CreateFolder("Assets/Resources", "Jobs");
			}
		}

		private static void ParseStartingStats () {
			string readPath = string.Format("{0}/Settings/JobStartingStats.csv", Application.dataPath);
			string[] readText = File.ReadAllLines(readPath);
			for (int i = 1; i < readText.Length; ++i) {
				PartsStartingStats(readText[i]);
			}
		}

		private static void PartsStartingStats (string line) {
			string[] elements = line.Split(',');
			GameObject obj = GetOrCreate(elements[0]);
			Job job = obj.GetComponent<Job>();
			for (int i = 1; i < Job.statOrder.Length + 1; ++i) {
				job.baseStats[i - 1] = Convert.ToInt32(elements[i]);
			}

			StatModifierFeature move = GetFeature (obj, StatType.MOV);
			move.amount = Convert.ToInt32(elements[8]);

			StatModifierFeature jump = GetFeature (obj, StatType.JMP);
			jump.amount = Convert.ToInt32(elements[9]);
		}

		private static void ParseGrowthStats () {
			string readPath = string.Format("{0}/Settings/JobGrowthStats.csv", Application.dataPath);
			string[] readText = File.ReadAllLines(readPath);
			for (int i = 1; i < readText.Length; ++i) {
				ParseGrowthStats(readText[i]);
			}
		}

		private static void ParseGrowthStats (string line) {
			string[] elements = line.Split(',');
			GameObject obj = GetOrCreate(elements[0]);
			Job job = obj.GetComponent<Job>();
			for (int i = 1; i < elements.Length; ++i) {
				job.growStats[i - 1] = Convert.ToSingle(elements[i]);
			}
		}

		private static StatModifierFeature GetFeature (GameObject obj, StatType type) {
			StatModifierFeature[] smf = obj.GetComponents<StatModifierFeature>();
			for (int i = 0; i < smf.Length; ++i) {
				if (smf[i].type == type) {
					return smf[i];
				}
			}

			StatModifierFeature feature = obj.AddComponent<StatModifierFeature>();
			feature.type = type;
			return feature;
		}

		private static GameObject GetOrCreate (string jobName) {
			string fullPath = string.Format("Assets/Resources/Jobs/{0}.prefab", jobName);
			GameObject obj = AssetDatabase.LoadAssetAtPath<GameObject>(fullPath);
			if (obj == null) {
				obj = Create(fullPath);
			}
			return obj;
		}

		private static GameObject Create (string fullPath) {
			GameObject instance = new GameObject ("temp");
			instance.AddComponent<Job>();
			GameObject prefab = PrefabUtility.CreatePrefab( fullPath, instance );
			GameObject.DestroyImmediate(instance);
			return prefab;
		}

	}

}