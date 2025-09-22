using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ScrollRect))]
public class ScrollRectSensMod : MonoBehaviour
{
	private ScrollRect scrollRect;

	private int unity6ScrollMod = 120;

	private void Start()
	{
		scrollRect = GetComponent<ScrollRect>();
		scrollRect.scrollSensitivity *= unity6ScrollMod;
	}
}
