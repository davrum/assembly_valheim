using System.Collections.Generic;
using GUIFramework;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIGamePad : MonoBehaviour
{
	public KeyCode m_keyCode;

	public string m_zinputKey;

	public GameObject m_hint;

	[Tooltip("The hotkey won't activate if any of these gameobjects are visible")]
	public List<GameObject> m_blockingElements;

	private Button m_button;

	private ISubmitHandler m_submit;

	private Toggle m_toggle;

	private UIGroupHandler m_group;

	[SerializeField]
	private UIGroupHandler alternativeGroupHandler;

	private bool m_blockedByLastFrame;

	private bool m_blockNextFrame;

	private static int m_lastInteractFrame;

	private void Start()
	{
		m_group = GetComponentInParent<UIGroupHandler>();
		m_button = GetComponent<Button>();
		m_submit = GetComponent<ISubmitHandler>();
		m_toggle = GetComponent<Toggle>();
		if ((bool)m_hint)
		{
			m_hint.SetActive(value: false);
		}
	}

	private bool IsInteractive()
	{
		if (m_button != null && !m_button.IsInteractable())
		{
			return false;
		}
		if ((bool)m_toggle)
		{
			if (!m_toggle.IsInteractable())
			{
				return false;
			}
			if ((bool)m_toggle.group && !m_toggle.group.allowSwitchOff && m_toggle.isOn)
			{
				return false;
			}
		}
		if (alternativeGroupHandler != null && alternativeGroupHandler.IsActive)
		{
			return true;
		}
		if ((bool)m_group && !m_group.IsActive)
		{
			return false;
		}
		if (m_submit is GuiInputField && !(m_submit as GuiInputField).interactable)
		{
			return false;
		}
		return true;
	}

	private void Update()
	{
	}

	public bool ButtonPressed()
	{
		if (IsBlocked())
		{
			return false;
		}
		if (m_blockingElements != null)
		{
			foreach (GameObject blockingElement in m_blockingElements)
			{
				if (blockingElement.gameObject.activeInHierarchy)
				{
					return false;
				}
			}
		}
		if (!string.IsNullOrEmpty(m_zinputKey) && ZInput.GetButtonDown(m_zinputKey))
		{
			return true;
		}
		if (m_keyCode != KeyCode.None && ZInput.GetKeyDown(m_keyCode))
		{
			return true;
		}
		return false;
	}

	public bool IsBlocked()
	{
		if (!m_blockedByLastFrame && !m_blockNextFrame)
		{
			if ((bool)Console.instance)
			{
				return Console.IsVisible();
			}
			return false;
		}
		return true;
	}
}
