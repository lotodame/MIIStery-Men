using UnityEngine;
using UnityEngine.Localization.Settings;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class LocaleDropdown : MonoBehaviour
{
    public TMP_Dropdown dropdown;

    private bool isInitialized = false;

    void Start()
    {
        StartCoroutine(SetupDropdown());
    }

    private IEnumerator SetupDropdown()
    {
        // Wait until localization settings are ready
        yield return LocalizationSettings.InitializationOperation;

        dropdown.ClearOptions();

        List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();

        for (int i = 0; i < LocalizationSettings.AvailableLocales.Locales.Count; i++)
        {
            var locale = LocalizationSettings.AvailableLocales.Locales[i];
            options.Add(new TMP_Dropdown.OptionData(locale.Identifier.CultureInfo.NativeName));
        }

        dropdown.AddOptions(options);

        // Set current locale in dropdown
        dropdown.value = LocalizationSettings.AvailableLocales.Locales.IndexOf(LocalizationSettings.SelectedLocale);
        dropdown.RefreshShownValue();

        dropdown.onValueChanged.AddListener(SetLocale);

        isInitialized = true;
    }

    void SetLocale(int index)
    {
        if (!isInitialized) return;

        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[index];
    }
}
