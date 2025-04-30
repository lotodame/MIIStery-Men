using UnityEngine;
using UnityEngine.Localization.Settings;
using TMPro;

public class LocaleDropdown : MonoBehaviour
{
    public TMP_Dropdown dropdown;

    void Start()
    {
        dropdown.ClearOptions();

        // Add all available locales to dropdown
        for (int i = 0; i < LocalizationSettings.AvailableLocales.Locales.Count; i++)
        {
            var locale = LocalizationSettings.AvailableLocales.Locales[i];
            dropdown.options.Add(new TMP_Dropdown.OptionData(locale.LocaleName));
        }

        dropdown.onValueChanged.AddListener(SetLocale);
        dropdown.value = LocalizationSettings.AvailableLocales.Locales.IndexOf(LocalizationSettings.SelectedLocale);
    }

    void SetLocale(int index)
    {
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[index];
    }
}
