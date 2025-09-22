using SoftReferenceableAssets.SceneManagement;
using UnityEngine;

public class EntryPointSceneLoader : MonoBehaviour
{
	[SerializeField]
	private SceneReference m_scene;

	private void Start()
	{
		ZLog.Log("Loading first scene");
		SceneManager.LoadScene(m_scene);
	}
}
