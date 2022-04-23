using UnityEditor;

namespace Plugins.VersionTool.MenuItems
{
	public class BuildPostProcessMenu
	{
		private const string RootPath = RootMenu.Path + "Build Postprocess/";

		private const string MI_AutoRename = RootPath + "Auto rename build";

		public const string AutoRename = "BPP_AutoRename";

		[MenuItem(MI_AutoRename, priority = 200)]
		public static void ToggleAutoRename()
		{
			bool newValue = !EditorPrefs.GetBool(AutoRename, false);
			EditorPrefs.SetBool(AutoRename, newValue);
			Menu.SetChecked(MI_AutoRename, newValue);
		}

		[MenuItem(MI_AutoRename, priority = 200, validate = true)]
		public static bool ToggleAutoRenameValidate()
		{
			Menu.SetChecked(MI_AutoRename, EditorPrefs.GetBool(AutoRename, false));
			return true;
		}
	}
}

