using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneFadeInOut : MonoBehaviour {

	public float fadeSpeed = 1.5f;
	private bool sceneStarting = true;

	void Awake() {
		GUITexture gTT = this.GetComponent<GUITexture> ();
		gTT.pixelInset = new Rect (0f, 0f, Screen.width, Screen.height);
	}

	void Update() {
		if (sceneStarting) {
			StartScene ();
		}
	}

	void FadeToClear() {
		GUITexture gTT = this.GetComponent<GUITexture> ();
		gTT.color = Color.Lerp (gTT.color, Color.clear, fadeSpeed * Time.deltaTime);
	}

	void FadeToBlack() {
		GUITexture gTT = this.GetComponent<GUITexture> ();
		gTT.color = Color.Lerp (gTT.color, Color.black, fadeSpeed * Time.deltaTime);
	}

	void StartScene() {

		GUITexture gTT = this.GetComponent<GUITexture> ();

		FadeToClear ();

		if (gTT.color.a <= 0.05f) {
			gTT.color = Color.clear;
			gTT.enabled = false;
			sceneStarting = false;
		}
	}

	public void EndScene() {
		
		GUITexture gTT = this.GetComponent<GUITexture> ();
//		gTT.enabled = true;
		FadeToBlack ();

		if (gTT.color.a >= 0.95f) {
//			Application.LoadLevel (1);
			SceneManager.LoadScene (1);
		}
	}
}
