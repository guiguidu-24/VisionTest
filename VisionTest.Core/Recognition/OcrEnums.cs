namespace VisionTest.Core.Recognition;

public enum PageSegmentationMode
{
    OSDOnly = 0,
    AutoOSD = 1,
    AutoOnly = 2,
    Auto = 3,
    SingleColumn = 4,
    SingleBlockVertText = 5,
    SingleBlock = 6,
    SingleLine = 7,
    SingleWord = 8,
    CircleWord = 9,
    SingleChar = 10,
    SparseText = 11,
    SparseTextOSD = 12,
    RawLine = 13
}

public enum  OcrEngineMode
{
    Legacy = 0,
    LSTM = 1,
    LegacyAndLSTM = 2,
    Auto = 3
}