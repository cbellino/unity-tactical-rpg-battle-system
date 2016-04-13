using UnityEngine;
using Tactical.Core;
using Tactical.Unit.Component;

using Party = System.Collections.Generic.List<UnityEngine.GameObject>;

namespace Tactical.Unit {

	public class LevelGrowthTests : MonoBehaviour {

		private void OnEnable () {
			this.AddObserver(OnLevelChange, Stats.DidChangeNotification(StatType.LVL));
			this.AddObserver(OnExperienceException, Stats.WillChangeNotification(StatType.EXP));
		}

		private void OnDisable () {
			this.RemoveObserver(OnLevelChange, Stats.DidChangeNotification(StatType.LVL));
			this.RemoveObserver(OnExperienceException, Stats.WillChangeNotification(StatType.EXP));
		}

		private void Start () {
			VerifyLevelToExperienceCalculations ();
		}

		private void VerifyLevelToExperienceCalculations () {
			for (int i = 1; i < 100; ++i) {
				int expLvl = ExperienceRank.ExperienceForLevel(i);
				int lvlExp = ExperienceRank.LevelForExperience(expLvl);

				if (lvlExp != i) {
					Debug.Log( string.Format("Mismatch on level:{0} with exp:{1} returned:{2}", i, expLvl, lvlExp) );
				} else {
					Debug.Log(string.Format("Level:{0} = Exp:{1}", lvlExp, expLvl));
				}
			}
		}

		private void OnLevelChange (object sender, object args) {
			var stats = sender as Stats;
			Debug.Log(stats.name + " leveled up!");
		}

		private void OnExperienceException (object sender, object args) {
			GameObject actor = (sender as Stats).gameObject;
			var vce = args as ValueChangeException;
			int roll = UnityEngine.Random.Range(0, 5);
			switch (roll) {
				case 0:
					vce.FlipToggle();
					Debug.Log(string.Format("{0} would have received {1} experience, but we stopped it", actor.name, vce.delta));
					break;
				case 1:
					vce.AddModifier( new AddValueModifier( 0, 1000 ) );
					Debug.Log(string.Format("{0} would have received {1} experience, but we added 1000", actor.name, vce.delta));
					break;
				case 2:
					vce.AddModifier( new MultiplyValueModifier( 0, 2f ) );
					Debug.Log(string.Format("{0} would have received {1} experience, but we multiplied by 2", actor.name, vce.delta));
					break;
				default:
					Debug.Log(string.Format("{0} will receive {1} experience", actor.name, vce.delta));
					break;
			}
		}

	}

}
