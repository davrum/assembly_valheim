using System.Collections.Generic;
using System.Linq;
using GUIFramework;
using UnityEngine;
using UnityEngine.UI;
using Valheim.UI;

namespace Valheim.SettingsGui;

public class RadialSettings : SettingsBase
{
	[Header("Radial")]
	[SerializeField]
	private Button m_back;

	[SerializeField]
	private GuiDropdown m_hoverSelect;

	[SerializeField]
	private Toggle m_persistentBackBtn;

	[SerializeField]
	private Toggle m_animateRadial;

	[SerializeField]
	private GuiDropdown m_spiralEffect;

	[SerializeField]
	private Toggle m_doubleTap;

	[SerializeField]
	private Toggle m_flick;

	[SerializeField]
	private Toggle m_singleUse;

	private Dictionary<HoverSelectSpeedSetting, string> m_hoverSpeedOptionStrings = new Dictionary<HoverSelectSpeedSetting, string>
	{
		{
			HoverSelectSpeedSetting.Off,
			"$radial_speed_off"
		},
		{
			HoverSelectSpeedSetting.Slow,
			"$radial_speed_slow"
		},
		{
			HoverSelectSpeedSetting.Medium,
			"$radial_speed_medium"
		},
		{
			HoverSelectSpeedSetting.Fast,
			"$radial_speed_fast"
		}
	};

	private Dictionary<SpiralEffectIntensitySetting, string> m_sprialOptionStrings = new Dictionary<SpiralEffectIntensitySetting, string>
	{
		{
			SpiralEffectIntensitySetting.Off,
			"$settings_spiral_off"
		},
		{
			SpiralEffectIntensitySetting.Slight,
			"$settings_spiral_slight"
		},
		{
			SpiralEffectIntensitySetting.Normal,
			"$settings_spiral_normal"
		}
	};

	public override void FixBackButtonNavigation(Button backButton)
	{
		SetNavigation(backButton, NavigationDirection.OnUp, m_singleUse);
		SetNavigation(m_singleUse, NavigationDirection.OnDown, backButton);
	}

	public override void FixOkButtonNavigation(Button okButton)
	{
		SetNavigation(okButton, NavigationDirection.OnUp, m_singleUse);
	}

	public override void LoadSettings()
	{
		List<string> list = m_hoverSpeedOptionStrings.Values.ToList();
		int value = list.IndexOf(m_hoverSpeedOptionStrings[(HoverSelectSpeedSetting)PlatformPrefs.GetInt("RadialHoverSpd")]);
		foreach (string item in list.ToList())
		{
			list[list.IndexOf(item)] = Localization.instance.Localize(item);
		}
		m_hoverSelect.ClearOptions();
		m_hoverSelect.AddOptions(list);
		m_hoverSelect.value = value;
		list = m_sprialOptionStrings.Values.ToList();
		value = list.IndexOf(m_sprialOptionStrings[(SpiralEffectIntensitySetting)PlatformPrefs.GetInt("RadialSpiral", 2)]);
		foreach (string item2 in list.ToList())
		{
			list[list.IndexOf(item2)] = Localization.instance.Localize(item2);
		}
		m_spiralEffect.ClearOptions();
		m_spiralEffect.AddOptions(list);
		m_spiralEffect.value = value;
		m_persistentBackBtn.isOn = PlatformPrefs.GetInt("RadialPersistentBackBtn") != 0;
		m_animateRadial.isOn = PlatformPrefs.GetInt("RadialAnimateRadial", 1) != 0;
		m_doubleTap.isOn = PlatformPrefs.GetInt("RadialDoubleTap") != 0;
		m_flick.isOn = PlatformPrefs.GetInt("RadialFlick") != 0;
		m_singleUse.isOn = PlatformPrefs.GetInt("RadialSingleUse") != 0;
		m_flick.onValueChanged.RemoveListener(OnFlickUpdated);
		m_flick.onValueChanged.AddListener(OnFlickUpdated);
		m_doubleTap.onValueChanged.RemoveListener(OnDoubleTapUpdated);
		m_doubleTap.onValueChanged.AddListener(OnDoubleTapUpdated);
		OnDoubleTapUpdated(m_doubleTap.isOn);
	}

	public override void SaveSettings()
	{
		PlatformPrefs.SetInt("RadialPersistentBackBtn", m_persistentBackBtn.isOn ? 1 : 0);
		PlatformPrefs.SetInt("RadialAnimateRadial", m_animateRadial.isOn ? 1 : 0);
		PlatformPrefs.SetInt("RadialDoubleTap", m_doubleTap.isOn ? 1 : 0);
		PlatformPrefs.SetInt("RadialFlick", m_flick.isOn ? 1 : 0);
		PlatformPrefs.SetInt("RadialSingleUse", m_singleUse.isOn ? 1 : 0);
		PlatformPrefs.SetInt("RadialHoverSpd", m_hoverSelect.value);
		PlatformPrefs.SetInt("RadialSpiral", m_spiralEffect.value);
		RadialData.SO.UsePersistantBackBtn = m_persistentBackBtn.isOn;
		RadialData.SO.EnableToggleAnimation = m_animateRadial.isOn;
		RadialData.SO.NudgeSelectedElement = m_animateRadial.isOn;
		RadialData.SO.EnableDoubleClick = m_doubleTap.isOn;
		RadialData.SO.EnableFlick = m_flick.isOn;
		RadialData.SO.EnableSingleUseMode = m_singleUse.isOn;
		RadialData.SO.SpiralEffectInsensity = (SpiralEffectIntensitySetting)m_spiralEffect.value;
		RadialData.SO.HoverSelectSelectionSpeed = (HoverSelectSpeedSetting)m_hoverSelect.value;
		Saved?.Invoke();
	}

	private void OnFlickUpdated(bool value)
	{
		if (value && m_doubleTap.isOn)
		{
			m_doubleTap.isOn = false;
		}
	}

	private void OnDoubleTapUpdated(bool value)
	{
		if (value && m_flick.isOn)
		{
			m_flick.isOn = false;
		}
	}
}
