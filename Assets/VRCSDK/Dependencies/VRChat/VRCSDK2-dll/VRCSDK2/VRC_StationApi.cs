using UnityEngine;

namespace VRCSDK2
{
	public class VRC_StationApi : MonoBehaviour
	{
		public delegate bool StationOccupiedDelegate(VRC_StationApi station);

		public delegate VRC_PlayerApi StationOccupantDelegate(VRC_StationApi station);

		public static StationOccupiedDelegate IsStationOccupiedDelegate;

		public static StationOccupantDelegate GetStationOccupant;

		public VRC_StationApi()
			: this()
		{
		}

		public bool IsStationOccupied()
		{
			if (IsStationOccupiedDelegate != null)
			{
				return IsStationOccupiedDelegate(this);
			}
			return false;
		}
	}
}
