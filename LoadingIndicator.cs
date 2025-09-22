using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadingIndicator : MonoBehaviour
{
	public static LoadingIndicator s_instance;

	[SerializeField]
	public bool m_showProgressIndicator = true;

	[SerializeField]
	private bool m_visibleInitially;

	[SerializeField]
	private float m_visibilityFadeTime = 0.2f;

	[SerializeField]
	private float m_maxDeltaTime = 1f / 30f;

	[SerializeField]
	private Image m_spinner;

	[SerializeField]
	private Image m_progressIndicator;

	[SerializeField]
	private Image m_background;

	[SerializeField]
	private TMP_Text m_text;

	private bool m_visible;

	private float m_progress;

	private float m_spinnerVisibility;

	private float m_progressVisibility;

	private float m_progressSmoothVelocity;

	private Color m_progressIndicatorOriginalColor;

	private Color m_spinnerOriginalColor;

	private Color m_backgroundOriginalColor;

	private Color m_textOriginalColor;

	public static bool IsCompletelyInvisible
	{
		get
		{
			if (s_instance == null)
			{
				return true;
			}
			if (s_instance.m_spinnerVisibility == 0f)
			{
				return s_instance.m_progressVisibility == 0f;
			}
			return false;
		}
	}

	private void Awake()
	{
		ZLog.Log("Initializing loading indicator instance");
		if (s_instance == null)
		{
			s_instance = this;
		}
		else
		{
			ZLog.LogWarning("Loading indicator instance already set up! Not setting the instance.");
		}
	}

	private void OnDestroy()
	{
		ZLog.Log("Destroying loading indicator instance");
		if (s_instance == this)
		{
			s_instance = null;
		}
		else
		{
			ZLog.LogWarning("Loading indicator instance did not match! Not removing the instance.");
		}
	}

	private void LateUpdate()
	{
	}

	private void UpdateGUIVisibility()
	{
		Color spinnerOriginalColor = m_spinnerOriginalColor;
		spinnerOriginalColor.a *= m_spinnerVisibility;
		m_spinner.color = spinnerOriginalColor;
		spinnerOriginalColor = m_progressIndicatorOriginalColor;
		spinnerOriginalColor.a *= m_progressVisibility;
		m_progressIndicator.color = spinnerOriginalColor;
		spinnerOriginalColor = m_backgroundOriginalColor;
		spinnerOriginalColor.a *= m_progressVisibility;
		m_background.color = spinnerOriginalColor;
		spinnerOriginalColor = m_textOriginalColor;
		spinnerOriginalColor.a *= m_progressVisibility;
		m_text.color = spinnerOriginalColor;
	}

	public static void SetVisibility(bool visible)
	{
	}

	public static void SetProgressVisibility(bool visible)
	{
	}

	public static void SetProgress(float progress)
	{
	}

	public static void SetText(string progressText)
	{
	}
}
