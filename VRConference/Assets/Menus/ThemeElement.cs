using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Menus{
	
	[ExecuteInEditMode()]
	[System.Serializable]
	public class ThemeElement : MonoBehaviour {
		
		public ThemeSetting themeSettings;
		private Image image;
		private TMP_Text text;
		public bool imageHighlight = false;
		public bool imageBase = false;
		public bool isText = false;
		
		public virtual void Awake()
		{
			
			OnSkinUI();
		}

		public virtual void Update()
		{
			OnSkinUI();
		}

		void OnSkinUI(){
			if (themeSettings == null)
			{
				Debug.Log("Theme missing");
				return;
			}

			if(imageHighlight){
				if (image == null)
				{
					image = GetComponent<Image>();
				}
				image.color = themeSettings.highlightColor;
			}
			
			if(imageBase){
				if (image == null)
				{
					image = GetComponent<Image>();
				}
				image.color = themeSettings.baseColor;
			}
			
			if (isText) {
				if (text == null)
				{
					text = GetComponent<TMP_Text>();
				}
				text.color = themeSettings.textColor;
			}
		}
	}
}