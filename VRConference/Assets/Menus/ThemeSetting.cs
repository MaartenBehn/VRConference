using UnityEngine;

namespace Menus{
	[CreateAssetMenu(menuName = "ThemeSettings")]
	[System.Serializable]
	public class ThemeSetting : ScriptableObject {
		
		public Color highlightColor;
		public Color baseColor;
		public Color backgroundColor;
		public Color backgroundTintColor;
		public Color32 textColor;
	}
}