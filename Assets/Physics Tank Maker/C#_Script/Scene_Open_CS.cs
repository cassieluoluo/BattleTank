using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace ChobiAssets.PTM
{
	public class Scene_Open_CS : MonoBehaviour
	{

		public string Scene_Name;
		public Image Fade_Image;

		void Awake ()
		{
			if (Fade_Image) {
				Fade_Image.raycastTarget = false;
				StartCoroutine ("Fade_In");
			}
		}

		public void Button_Push ()
		{
			// Create dictionary.
			Menu_Dictionary_CS dictionaryScript = FindObjectOfType <Menu_Dictionary_CS> ();
			if (dictionaryScript) {
				dictionaryScript.Create_Dictionary ();
			}
			// Disable all the Button.
			Button thisButton = GetComponent <Button> ();
			thisButton.enabled = false;
			Button [] buttons = FindObjectsOfType <Button> ();
			foreach (Button button in buttons) {
				if (button != thisButton) {
					if (button.targetGraphic) {
						button.targetGraphic.enabled = false;
					}
					Text tempText = button.GetComponentInChildren <Text> ();
					if (tempText) {
						tempText.enabled = false;
					}
					button.enabled = false;
				}
			}
			// Start Fadeout.
			StartCoroutine ("Fade_Out");
		}

		IEnumerator Fade_Out ()
		{
			float fadeTime = 1.0f;
			float initialVolume = AudioListener.volume;
			float rate = 1.0f;
			while (rate > 0.0f) {
				AudioListener.volume = initialVolume * rate;
				if (Fade_Image) {
					Color tempColor = Fade_Image.color;
					tempColor.a = 1.0f - rate;
					Fade_Image.color = tempColor;
				}
				rate -= Time.deltaTime / fadeTime;
				yield return null;
			}
			SceneManager.LoadScene (Scene_Name);
		}

		IEnumerator Fade_In ()
		{
			float fadeTime = 1.0f;
			float rate = 1.0f;
			while (rate > 0.0f) {
				if (Fade_Image) {
					Color tempColor = Fade_Image.color;
					tempColor.a = rate;
					Fade_Image.color = tempColor;
				}
				rate -= Time.deltaTime / fadeTime;
				yield return null;
			}
			Color targetColor = Fade_Image.color;
			targetColor.a = 0.0f;
			Fade_Image.color = targetColor;
		}

	}
}
