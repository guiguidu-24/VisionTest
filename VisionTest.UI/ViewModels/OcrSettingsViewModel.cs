using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using VisionTest.Core.Recognition;

namespace VisionTest.UI.ViewModels;

public partial class OcrSettingsViewModel : ObservableObject
{
    // --- Text Constraints ---
    [ObservableProperty] private string _targetText = string.Empty;
    [ObservableProperty] private string _whiteListChar = string.Empty;
    [ObservableProperty] private string _blackListChar = string.Empty;
    [ObservableProperty] private string _regexPattern = string.Empty;

    // --- Engine Configuration ---
    [ObservableProperty] private Language _selectedLang = Language.English;
    [ObservableProperty] private PageSegmentationMode _selectedPSM = PageSegmentationMode.Auto;
    [ObservableProperty] private OcrEngineMode _selectedOEM = OcrEngineMode.Auto;

    // --- Logic Toggles ---
    [ObservableProperty] private bool _useDictionary = false;

    // --- Lists ---
    public ObservableCollection<string> WordList { get; } = new();

    // --- Helper for UI Dropdowns ---
    public IEnumerable<Language> AvailableLanguages => Enum.GetValues<Language>();
    public IEnumerable<PageSegmentationMode> AvailablePSMs => Enum.GetValues<PageSegmentationMode>();
    public IEnumerable<OcrEngineMode> AvailableOEMs => Enum.GetValues<OcrEngineMode>();

    /// <summary>
    /// Factory method to convert these UI settings back into OcrOption.
    /// </summary>
    public OcrOptions CreateOptions()
    {
        return new OcrOptions(
            whiteListChar: WhiteListChar,
            blackListChar: BlackListChar,
            wordList: WordList,
            lang: SelectedLang,
            psm: SelectedPSM,
            oem: SelectedOEM,
            useDictionnary: UseDictionary,
            regexPattern: RegexPattern
        );
    }
}
