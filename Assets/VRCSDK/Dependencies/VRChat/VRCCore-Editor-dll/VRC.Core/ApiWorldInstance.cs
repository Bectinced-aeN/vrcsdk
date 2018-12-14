using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace VRC.Core
{
	public class ApiWorldInstance
	{
		public enum AccessType
		{
			Public,
			FriendsOfGuests,
			FriendsOnly,
			InviteOnly,
			InvitePlus,
			Counter
		}

		public struct InstanceTag
		{
			public string name;

			public string data;

			public InstanceTag(string n = "", string d = null)
			{
				name = n;
				data = d;
			}
		}

		public class AccessDetail
		{
			public string[] tags
			{
				get;
				private set;
			}

			public string shortName
			{
				get;
				private set;
			}

			public string fullName
			{
				get;
				private set;
			}

			public AccessDetail(string[] inTags, string sName, string fName)
			{
				tags = inTags;
				shortName = sName;
				fullName = fName;
			}
		}

		private static Dictionary<AccessType, AccessDetail> accessDetails;

		public ApiWorld instanceWorld;

		public string idWithTags;

		public int count;

		public List<APIUser> users;

		public string instanceName
		{
			get
			{
				Debug.LogError((object)string.Empty);
				return string.Empty;
			}
			set
			{
				Debug.LogError((object)string.Empty);
			}
		}

		public string idOnly => ParseIDFromIDWithTags(idWithTags);

		public string tagsOnly => ParseTagsFromIDWithTags(idWithTags);

		public bool isPublic => GetAccessType() == AccessType.Public;

		public string instanceCreator
		{
			get
			{
				Debug.LogError((object)string.Empty);
				return null;
			}
			set
			{
				Debug.LogError((object)string.Empty);
			}
		}

		public string[] instanceTags
		{
			get
			{
				Debug.LogError((object)string.Empty);
				return new string[0];
			}
			set
			{
				Debug.LogError((object)string.Empty);
			}
		}

		public AccessType InstanceType
		{
			get
			{
				return GetAccessType();
			}
			set
			{
				Debug.LogError((object)string.Empty);
			}
		}

		public ApiWorldInstance(ApiWorld world, string _idWithTags, int _count)
		{
			instanceWorld = world;
			idWithTags = _idWithTags;
			count = _count;
		}

		public ApiWorldInstance(ApiWorld world, Dictionary<string, object> dict, float dataTimestamp)
		{
			instanceWorld = world;
			idWithTags = dict["id"].ToString();
			string error = string.Empty;
			List<object> json = dict["users"] as List<object>;
			List<APIUser> list = users = API.ConvertJsonListToModelList<APIUser>(json, ref error, dataTimestamp);
			count = users.Count;
		}

		static ApiWorldInstance()
		{
			accessDetails = new Dictionary<AccessType, AccessDetail>();
			accessDetails[AccessType.Public] = new AccessDetail(null, "public", "Public");
			accessDetails[AccessType.FriendsOfGuests] = new AccessDetail(new string[1]
			{
				"hidden"
			}, "friends+", "Friends of Guests");
			accessDetails[AccessType.FriendsOnly] = new AccessDetail(new string[1]
			{
				"friends"
			}, "friends", "Friends Only");
			accessDetails[AccessType.InviteOnly] = new AccessDetail(new string[1]
			{
				"private"
			}, "invite", "Invite Only");
			accessDetails[AccessType.InvitePlus] = new AccessDetail(new string[2]
			{
				"private",
				"canRequestInvite"
			}, "invite+", "Invite Plus");
			accessDetails[AccessType.Counter] = new AccessDetail(new string[1]
			{
				"pop"
			}, "popcount", "[Internal Use Only] Population Counter");
		}

		public static string ParseIDFromIDWithTags(string idWithTagsStr)
		{
			if (string.IsNullOrEmpty(idWithTagsStr))
			{
				return null;
			}
			string[] array = idWithTagsStr.Split('~');
			if (array == null || array.Length == 0)
			{
				return idWithTagsStr;
			}
			return array[0];
		}

		public static string ParseTagsFromIDWithTags(string idWithTagsStr)
		{
			if (string.IsNullOrEmpty(idWithTagsStr))
			{
				return null;
			}
			string[] array = idWithTagsStr.Split('~');
			if (array.Length > 1)
			{
				return "~" + array[1].Trim();
			}
			return null;
		}

		private List<InstanceTag> ParseTags(string idWithTags)
		{
			if (string.IsNullOrEmpty(idWithTags))
			{
				return null;
			}
			List<InstanceTag> list = new List<InstanceTag>();
			string[] array = idWithTags.Split('~');
			if (array == null || array.Length > 1)
			{
				for (int i = 1; i < array.Length; i++)
				{
					InstanceTag item;
					if (array[i].Contains('('))
					{
						string[] array2 = array[i].Split('(');
						string n = array2[0];
						string d = array2[1].TrimEnd(')');
						item = new InstanceTag(n, d);
					}
					else
					{
						item = new InstanceTag(array[i]);
					}
					list.Add(item);
				}
			}
			return list;
		}

		public AccessType GetAccessType()
		{
			List<InstanceTag> list = ParseTags(idWithTags);
			if (list != null)
			{
				if (list.Exists((InstanceTag t) => t.name == accessDetails[AccessType.InvitePlus].tags[0]) && list.Exists((InstanceTag t) => t.name == accessDetails[AccessType.InvitePlus].tags[1]))
				{
					return AccessType.InvitePlus;
				}
				if (list.Exists((InstanceTag t) => t.name == accessDetails[AccessType.InviteOnly].tags[0]))
				{
					return AccessType.InviteOnly;
				}
				if (list.Exists((InstanceTag t) => t.name == accessDetails[AccessType.FriendsOnly].tags[0]))
				{
					return AccessType.FriendsOnly;
				}
				if (list.Exists((InstanceTag t) => t.name == accessDetails[AccessType.FriendsOfGuests].tags[0]))
				{
					return AccessType.FriendsOfGuests;
				}
			}
			return AccessType.Public;
		}

		public static AccessDetail GetAccessDetail(AccessType access)
		{
			return accessDetails[access];
		}

		public static string BuildAccessTags(AccessType access, string userId)
		{
			string result = string.Empty;
			if (access == AccessType.Public)
			{
				return result;
			}
			AccessDetail accessDetail = GetAccessDetail(access);
			switch (access)
			{
			case AccessType.Counter:
				result = "~" + accessDetail.tags[0];
				break;
			case AccessType.FriendsOfGuests:
			case AccessType.FriendsOnly:
			case AccessType.InviteOnly:
				result = string.Format("~{0}({1})~nonce({2})", accessDetail.tags[0], userId, new string((from s in Enumerable.Repeat("ABCDEF0123456789", 64)
				select s[Random.Range(0, s.Length)]).ToArray()));
				break;
			case AccessType.InvitePlus:
				result = string.Format("~{0}({1})~{2}~nonce({3})", accessDetail.tags[0], userId, accessDetail.tags[1], new string((from s in Enumerable.Repeat("ABCDEF0123456789", 64)
				select s[Random.Range(0, s.Length)]).ToArray()));
				break;
			}
			return result;
		}

		public string GetInstanceCreator()
		{
			List<InstanceTag> list = ParseTags(idWithTags);
			if (list != null)
			{
				foreach (InstanceTag item in list)
				{
					InstanceTag current = item;
					if (current.name == accessDetails[AccessType.InviteOnly].tags[0] || current.name == accessDetails[AccessType.InvitePlus].tags[0] || current.name == accessDetails[AccessType.FriendsOnly].tags[0] || current.name == accessDetails[AccessType.FriendsOfGuests].tags[0])
					{
						return current.data;
					}
				}
			}
			return null;
		}

		public override string ToString()
		{
			return "[" + idWithTags + ", " + count + "]";
		}

		public string GetTagString()
		{
			Debug.LogError((object)string.Empty);
			return string.Empty;
		}

		public string GetTitle()
		{
			Debug.LogError((object)string.Empty);
			return "title";
		}
	}
}
