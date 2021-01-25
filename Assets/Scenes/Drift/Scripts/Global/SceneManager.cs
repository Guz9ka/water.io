using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class SceneManager : MonoBehaviour
{
	[SerializeField] 
	private GameObject loadScreenCanvas;
	[SerializeField]
	private Slider sliderLoadProgress;
	[SerializeField] 
	private float loadProgressSpeed;
	
	
	public void LoadScene(int sceneIndex)
	{
		StartCoroutine(LoadSceneAsynchronously(sceneIndex));
	}

	private IEnumerator LoadSceneAsynchronously(int sceneIndex)
	{
		loadScreenCanvas.SetActive(true);
		
		AsyncOperation operation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneIndex);

		while (!operation.isDone)
		{
			float progress = Mathf.Clamp01(operation.progress);
			sliderLoadProgress.value = progress;
			
			yield return null;
		}
	}
}
