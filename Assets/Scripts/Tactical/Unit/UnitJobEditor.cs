using UnityEngine;
using UnityEditor;
using Tactical.Data;

namespace Tactical.Unit {

	[CustomEditor(typeof(UnitJob))]
	public class UnitJobEditor : Editor {

		public override void OnInspectorGUI () {
			var myTarget = (UnitJob) target;

			var selectedIds = myTarget.job != null ? myTarget.job.id : 1;
			var names = EditorLoader.jobs.ConvertAll(item => item.name).ToArray();
			var ids = EditorLoader.jobs.ConvertAll(item => item.id).ToArray();

			var newId = EditorGUILayout.IntPopup("Job", selectedIds, names, ids);
			myTarget.job = EditorLoader.jobs.Find(item => item.id == newId);
		}
	}

}