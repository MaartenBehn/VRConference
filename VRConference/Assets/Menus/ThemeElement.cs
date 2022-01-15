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
		public bool imageBackground = false;
		public bool imageBackgroundTint = false;
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
			}else if(imageBase){
				if (image == null)
				{
					image = GetComponent<Image>();
				}
				image.color = themeSettings.baseColor;
			}else if(imageBackground){
				if (image == null)
				{
					image = GetComponent<Image>();
				}
				image.color = themeSettings.backgroundColor;
			}else if(imageBackgroundTint){
				if (image == null)
				{
					image = GetComponent<Image>();
				}
				image.color = themeSettings.backgroundTintColor;
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