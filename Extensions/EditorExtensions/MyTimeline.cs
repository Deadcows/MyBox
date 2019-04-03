#if UNITY_EDITOR
using System.Linq;
using UnityEditor;
using UnityEngine.Timeline;

namespace MyBox.EditorTools
{
	public static class MyTimeline
	{
		public static TrackAsset GetTrackOfType<T>(this TimelineAsset timeline) where T : TrackAsset, new()
		{
			var tracks = timeline.GetOutputTracks();
			var trackOfType = tracks.FirstOrDefault(t => t is T);
			return trackOfType;
		}
		
		public static TrackAsset[] GetTracksOfType<T>(this TimelineAsset timeline) where T : TrackAsset, new()
		{
			var tracks = timeline.GetOutputTracks();
			return tracks.Where(t => t is T).ToArray();
		}
		
		public static TrackAsset GetOrAddTrackOfType<T>(this TimelineAsset timeline, string nameOfNewTrack) where T : TrackAsset, new()
		{
			var tracks = timeline.GetOutputTracks();
			var trackOfType = tracks.FirstOrDefault(t => t is T);
			if (trackOfType != null) return trackOfType;
			
			trackOfType = timeline.CreateTrack<T>(null, nameOfNewTrack);

			EditorUtility.SetDirty(timeline);
			return trackOfType;
		}
		
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
#endif