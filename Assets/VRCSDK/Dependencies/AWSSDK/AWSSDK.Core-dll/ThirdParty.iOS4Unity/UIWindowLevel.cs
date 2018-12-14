namespace ThirdParty.iOS4Unity
{
	public static class UIWindowLevel
	{
		public static float Alert => ObjC.GetFloatConstant(ObjC.Libraries.UIKit, "UIWindowLevelAlert");

		public static float Normal => ObjC.GetFloatConstant(ObjC.Libraries.UIKit, "UIWindowLevelNormal");

		public static float StatusBar => ObjC.GetFloatConstant(ObjC.Libraries.UIKit, "UIWindowLevelStatusBar");
	}
}
