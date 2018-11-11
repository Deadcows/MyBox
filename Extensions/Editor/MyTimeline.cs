using System.Linq;
using UnityEditor;
using UnityEngine.Timeline;

namespace MyBox
{
	public static class MyTimeline
	{
		public static void DeleteTracksOfType<T>(this TimelineAsset timeline)
		{
			var tracks = timeline.GetOutputTracks();
			var routeTracks = tracks.Where(t => t is T);
			foreach (var t in routeTracks) timeline.DeleteTrack(t);

			EditorUtility.SetDirty(timeline);
		}

		public static void DeleteAllTracks(this TimelineAsset timeline)
		{
			var tracks = timeline.GetOutputTracks();
			foreach (var t in tracks) timeline.DeleteTrack(t);

			EditorUtility.SetDirty(timeline);
		}
	}
}