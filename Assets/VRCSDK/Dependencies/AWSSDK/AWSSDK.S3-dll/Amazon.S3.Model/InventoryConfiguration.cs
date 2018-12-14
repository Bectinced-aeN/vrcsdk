using System.Collections.Generic;

namespace Amazon.S3.Model
{
	public class InventoryConfiguration
	{
		private InventoryDestination inventoryDestination;

		private bool isEnabled;

		private InventoryFilter inventoryFilter;

		private string inventoryId;

		private InventoryIncludedObjectVersions inventoryIncludedObjectVersions;

		private List<InventoryOptionalField> inventoryOptionalFields = new List<InventoryOptionalField>();

		private InventorySchedule inventorySchedule;

		public InventoryDestination Destination
		{
			get
			{
				return inventoryDestination;
			}
			set
			{
				inventoryDestination = value;
			}
		}

		public bool IsEnabled
		{
			get
			{
				return isEnabled;
			}
			set
			{
				isEnabled = value;
			}
		}

		public InventoryFilter InventoryFilter
		{
			get
			{
				return inventoryFilter;
			}
			set
			{
				inventoryFilter = value;
			}
		}

		public string InventoryId
		{
			get
			{
				return inventoryId;
			}
			set
			{
				inventoryId = value;
			}
		}

		public InventoryIncludedObjectVersions IncludedObjectVersions
		{
			get
			{
				return inventoryIncludedObjectVersions;
			}
			set
			{
				inventoryIncludedObjectVersions = value;
			}
		}

		public List<InventoryOptionalField> InventoryOptionalFields
		{
			get
			{
				return inventoryOptionalFields;
			}
			set
			{
				inventoryOptionalFields = value;
			}
		}

		public InventorySchedule Schedule
		{
			get
			{
				return inventorySchedule;
			}
			set
			{
				inventorySchedule = value;
			}
		}

		internal bool IsSetDestination()
		{
			return inventoryDestination != null;
		}

		internal bool IsSetInventoryFilter()
		{
			return inventoryFilter != null;
		}

		internal bool IsSetInventoryId()
		{
			return !string.IsNullOrEmpty(inventoryId);
		}

		internal bool IsSetIncludedObjectVersions()
		{
			return inventoryIncludedObjectVersions != null;
		}

		internal bool IsSetInventoryOptionalFields()
		{
			return inventoryOptionalFields.Count > 0;
		}

		internal bool IsSetSchedule()
		{
			return inventorySchedule != null;
		}
	}
}
