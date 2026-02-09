namespace VisionTest.Core.Input;

/// <summary>
/// Represents virtual key codes for keyboard input.
/// </summary>
public enum KeyCode : ushort
{
    /// <summary>Undefined key</summary>
     Undefined = 0x0000,

    /// <summary>Escape</summary>
    /// <remarks>Available on: Windows, macOS, Linux</remarks>
     Escape = 0x001B,

    /// <summary>F1</summary>
    /// <remarks>Available on: Windows, macOS, Linux</remarks>
     F1 = 0x0070,

    /// <summary>F2</summary>
    /// <remarks>Available on: Windows, macOS, Linux</remarks>
     F2 = 0x0071,

    /// <summary>F3</summary>
    /// <remarks>Available on: Windows, macOS, Linux</remarks>
     F3 = 0x0072,

    /// <summary>F4</summary>
    /// <remarks>Available on: Windows, macOS, Linux</remarks>
     F4 = 0x0073,

    /// <summary>F5</summary>
    /// <remarks>Available on: Windows, macOS, Linux</remarks>
     F5 = 0x0074,

    /// <summary>F6</summary>
    /// <remarks>Available on: Windows, macOS, Linux</remarks>
     F6 = 0x0075,

    /// <summary>F7</summary>
    /// <remarks>Available on: Windows, macOS, Linux</remarks>
     F7 = 0x0076,

    /// <summary>F8</summary>
    /// <remarks>Available on: Windows, macOS, Linux</remarks>
     F8 = 0x0077,

    /// <summary>F9</summary>
    /// <remarks>Available on: Windows, macOS, Linux</remarks>
     F9 = 0x0078,

    /// <summary>F10</summary>
    /// <remarks>Available on: Windows, macOS, Linux</remarks>
     F10 = 0x0079,

    /// <summary>F11</summary>
    /// <remarks>Available on: Windows, macOS, Linux</remarks>
     F11 = 0x007A,

    /// <summary>F12</summary>
    /// <remarks>Available on: Windows, macOS, Linux</remarks>
     F12 = 0x007B,

    /// <summary>F13</summary>
    /// <remarks>Available on: Windows, macOS, Linux</remarks>
     F13 = 0xF000,

    /// <summary>F14</summary>
    /// <remarks>Available on: Windows, macOS, Linux</remarks>
     F14 = 0xF001,

    /// <summary>F15</summary>
    /// <remarks>Available on: Windows, macOS, Linux</remarks>
     F15 = 0xF002,

    /// <summary>F16</summary>
    /// <remarks>Available on: Windows, macOS, Linux</remarks>
     F16 = 0xF003,

    /// <summary>F17</summary>
    /// <remarks>Available on: Windows, macOS, Linux</remarks>
     F17 = 0xF004,

    /// <summary>F18</summary>
    /// <remarks>Available on: Windows, macOS, Linux</remarks>
     F18 = 0xF005,

    /// <summary>F19</summary>
    /// <remarks>Available on: Windows, macOS, Linux</remarks>
     F19 = 0xF006,

    /// <summary>F20</summary>
    /// <remarks>Available on: Windows, macOS, Linux</remarks>
     F20 = 0xF007,

    /// <summary>F21</summary>
    /// <remarks>Available on: Windows, Linux</remarks>
     F21 = 0xF008,

    /// <summary>F22</summary>
    /// <remarks>Available on: Windows, Linux</remarks>
     F22 = 0xF009,

    /// <summary>F23</summary>
    /// <remarks>Available on: Windows, Linux</remarks>
     F23 = 0xF00A,

    /// <summary>F24</summary>
    /// <remarks>Available on: Windows, Linux</remarks>
     F24 = 0xF00B,

    /// <summary>`</summary>
    /// <remarks>Available on: Windows, macOS, Linux</remarks>
     BackQuote = 0x00C0,

    /// <summary>0</summary>
    /// <remarks>Available on: Windows, macOS, Linux</remarks>
     _0 = 0x0030,

    /// <summary>1</summary>
    /// <remarks>Available on: Windows, macOS, Linux</remarks>
     _1 = 0x0031,

    /// <summary>2</summary>
    /// <remarks>Available on: Windows, macOS, Linux</remarks>
     _2 = 0x0032,

    /// <summary>3</summary>
    /// <remarks>Available on: Windows, macOS, Linux</remarks>
     _3 = 0x0033,

    /// <summary>4</summary>
    /// <remarks>Available on: Windows, macOS, Linux</remarks>
     _4 = 0x0034,

    /// <summary>5</summary>
    /// <remarks>Available on: Windows, macOS, Linux</remarks>
     _5 = 0x0035,

    /// <summary>6</summary>
    /// <remarks>Available on: Windows, macOS, Linux</remarks>
     _6 = 0x0036,

    /// <summary>7</summary>
    /// <remarks>Available on: Windows, macOS, Linux</remarks>
     _7 = 0x0037,

    /// <summary>8</summary>
    /// <remarks>Available on: Windows, macOS, Linux</remarks>
     _8 = 0x0038,

    /// <summary>9</summary>
    /// <remarks>Available on: Windows, macOS, Linux</remarks>
     _9 = 0x0039,

    /// <summary>-</summary>
    /// <remarks>Available on: Windows, macOS, Linux</remarks>
     Minus = 0x002D,

    /// <summary>=</summary>
    /// <remarks>Available on: Windows, macOS, Linux</remarks>
     Equals = 0x003D,

    /// <summary>
    /// Backspace (on Windows and Linux) or Delete (on macOS)
    /// </summary>
    /// <remarks>Available on: Windows, macOS, Linux</remarks>
     Backspace = 0x0008,

    /// <summary>Tab</summary>
    /// <remarks>Available on: Windows, macOS, Linux</remarks>
     Tab = 0x0009,

    /// <summary>Caps Lock</summary>
    /// <remarks>Available on: Windows, macOS, Linux</remarks>
     CapsLock = 0x0014,

    /// <summary>A</summary>
    /// <remarks>Available on: Windows, macOS, Linux</remarks>
     A = 0x0041,

    /// <summary>B</summary>
    /// <remarks>Available on: Windows, macOS, Linux</remarks>
     B = 0x0042,

    /// <summary>C</summary>
    /// <remarks>Available on: Windows, macOS, Linux</remarks>
     C = 0x0043,

    /// <summary>D</summary>
    /// <remarks>Available on: Windows, macOS, Linux</remarks>
     D = 0x0044,

    /// <summary>E</summary>
    /// <remarks>Available on: Windows, macOS, Linux</remarks>
     E = 0x0045,

    /// <summary>F</summary>
    /// <remarks>Available on: Windows, macOS, Linux</remarks>
     F = 0x0046,

    /// <summary>G</summary>
    /// <remarks>Available on: Windows, macOS, Linux</remarks>
     G = 0x0047,

    /// <summary>H</summary>
    /// <remarks>Available on: Windows, macOS, Linux</remarks>
     H = 0x0048,

    /// <summary>I</summary>
    /// <remarks>Available on: Windows, macOS, Linux</remarks>
     I = 0x0049,

    /// <summary>J</summary>
    /// <remarks>Available on: Windows, macOS, Linux</remarks>
     J = 0x004A,

    /// <summary>K</summary>
    /// <remarks>Available on: Windows, macOS, Linux</remarks>
     K = 0x004B,

    /// <summary>L</summary>
    /// <remarks>Available on: Windows, macOS, Linux</remarks>
     L = 0x004C,

    /// <summary>M</summary>
    /// <remarks>Available on: Windows, macOS, Linux</remarks>
     M = 0x004D,

    /// <summary>N</summary>
    /// <remarks>Available on: Windows, macOS, Linux</remarks>
     N = 0x004E,

    /// <summary>O</summary>
    /// <remarks>Available on: Windows, macOS, Linux</remarks>
     O = 0x004F,

    /// <summary>P</summary>
    /// <remarks>Available on: Windows, macOS, Linux</remarks>
     P = 0x0050,

    /// <summary>Q</summary>
    /// <remarks>Available on: Windows, macOS, Linux</remarks>
     Q = 0x0051,

    /// <summary>R</summary>
    /// <remarks>Available on: Windows, macOS, Linux</remarks>
     R = 0x0052,

    /// <summary>S</summary>
    /// <remarks>Available on: Windows, macOS, Linux</remarks>
     S = 0x0053,

    /// <summary>T</summary>
    /// <remarks>Available on: Windows, macOS, Linux</remarks>
     T = 0x0054,

    /// <summary>U</summary>
    /// <remarks>Available on: Windows, macOS, Linux</remarks>
     U = 0x0055,

    /// <summary>V</summary>
    /// <remarks>Available on: Windows, macOS, Linux</remarks>
     V = 0x0056,

    /// <summary>W</summary>
    /// <remarks>Available on: Windows, macOS, Linux</remarks>
     W = 0x0057,

    /// <summary>X</summary>
    /// <remarks>Available on: Windows, macOS, Linux</remarks>
     X = 0x0058,

    /// <summary>Y</summary>
    /// <remarks>Available on: Windows, macOS, Linux</remarks>
     Y = 0x0059,

    /// <summary>Z</summary>
    /// <remarks>Available on: Windows, macOS, Linux</remarks>
     Z = 0x005A,

    /// <summary>[</summary>
    /// <remarks>Available on: Windows, macOS, Linux</remarks>
     OpenBracket = 0x005B,

    /// <summary>]</summary>
    /// <remarks>Available on: Windows, macOS, Linux</remarks>
     CloseBracket = 0x005C,

    /// <summary>\</summary>
    /// <remarks>Available on: Windows, macOS, Linux</remarks>
     Backslash = 0x005D,

    /// <summary>;</summary>
    /// <remarks>Available on: Windows, macOS, Linux</remarks>
     Semicolon = 0x003B,

    /// <summary>'</summary>
    /// <remarks>Available on: Windows, macOS, Linux</remarks>
     Quote = 0x00DE,

    /// <summary>Enter</summary>
    /// <remarks>Available on: Windows, macOS, Linux</remarks>
     Enter = 0x000A,

    /// <summary>,</summary>
    /// <remarks>Available on: Windows, macOS, Linux</remarks>
     Comma = 0x002C,

    /// <summary>.</summary>
    /// <remarks>Available on: Windows, macOS, Linux</remarks>
     Period = 0x002E,

    /// <summary>/</summary>
    /// <remarks>Available on: Windows, macOS, Linux</remarks>
     Slash = 0x002F,

    /// <summary>Space</summary>
    /// <remarks>Available on: Windows, macOS, Linux</remarks>
     Space = 0x0020,

    /// <summary>
    /// The &lt;&gt; key on the US standard keyboard, or the \| key on the non-US 102-key keyboard,
    /// or the Section key (§) on the macOS ISO keyboard
    /// </summary>
    /// <remarks>Available on: Windows, macOS, Linux</remarks>
     _102 = 0x0099,

    /// <summary>Miscellaneous OEM-specific key</summary>
    /// <remarks>Available on: Windows</remarks>
     Misc = 0x0E01,

    /// <summary>Print Screen</summary>
    /// <remarks>Available on: Windows, Linux</remarks>
     PrintScreen = 0x009A,

    /// <summary>Scroll Lock</summary>
    /// <remarks>Available on: Windows, Linux</remarks>
     ScrollLock = 0x0091,

    /// <summary>Pause</summary>
    /// <remarks>Available on: Windows, Linux</remarks>
     Pause = 0x0013,

    /// <summary>Cancel</summary>
    /// <remarks>Available on: Windows, Linux</remarks>
     Cancel = 0x00D3,

    /// <summary>Help</summary>
    /// <remarks>Available on: Windows, macOS, Linux</remarks>
     Help = 0x009F,

    /// <summary>Insert</summary>
    /// <remarks>Available on: Windows, Linux</remarks>
     Insert = 0x009B,

    /// <summary>
    /// Delete (on Windows and Linux) or Forward Delete (on macOS)
    /// </summary>
    /// <remarks>Available on: Windows, macOS, Linux</remarks>
     Delete = 0x007F,

    /// <summary>Home</summary>
    /// <remarks>Available on: Windows, macOS, Linux</remarks>
     Home = 0x0024,

    /// <summary>End</summary>
    /// <remarks>Available on: Windows, macOS, Linux</remarks>
     End = 0x0023,

    /// <summary>Page Up</summary>
    /// <remarks>Available on: Windows, macOS, Linux</remarks>
     PageUp = 0x0021,

    /// <summary>Page Down</summary>
    /// <remarks>Available on: Windows, macOS, Linux</remarks>
     PageDown = 0x0022,

    /// <summary>Up Arrow</summary>
    /// <remarks>Available on: Windows, macOS, Linux</remarks>
     Up = 0x0026,

    /// <summary>Left Arrow</summary>
    /// <remarks>Available on: Windows, macOS, Linux</remarks>
     Left = 0x0025,

    /// <summary>Right Arrow</summary>
    /// <remarks>Available on: Windows, macOS, Linux</remarks>
     Right = 0x0027,

    /// <summary>Down Arrow</summary>
    /// <remarks>Available on: Windows, macOS, Linux</remarks>
     Down = 0x0028,

    /// <summary>Num Lock</summary>
    /// <remarks>Available on: Windows, Linux</remarks>
     NumLock = 0x0090,

    /// <summary>Num-Pad Clear</summary>
    /// <remarks>Available on: Windows, macOS</remarks>
     NumPadClear = 0x000C,

    /// <summary>Num-Pad /</summary>
    /// <remarks>Available on: Windows, macOS, Linux</remarks>
     NumPadDivide = 0x006F,

    /// <summary>Num-Pad *</summary>
    /// <remarks>Available on: Windows, macOS, Linux</remarks>
     NumPadMultiply = 0x006A,

    /// <summary>Num-Pad -</summary>
    /// <remarks>Available on: Windows, macOS, Linux</remarks>
     NumPadSubtract = 0x006D,

    /// <summary>Num-Pad =</summary>
    /// <remarks>Available on: Windows, macOS, Linux</remarks>
     NumPadEquals = 0x007C,

    /// <summary>Num-Pad +</summary>
    /// <remarks>Available on: Windows, macOS, Linux</remarks>
     NumPadAdd = 0x006B,

    /// <summary>Num-Pad Enter</summary>
    /// <remarks>Available on: Windows, macOS, Linux</remarks>
     NumPadEnter = 0x007D,

    /// <summary>Num-Pad Decimal</summary>
    /// <remarks>Available on: Windows, macOS, Linux</remarks>
     NumPadDecimal = 0x006E,

    /// <summary>Num-Pad Separator</summary>
    /// <remarks>Available on: Windows, Linux</remarks>
     NumPadSeparator = 0x006C,

    /// <summary>Num-Pad 0</summary>
    /// <remarks>Available on: Windows, macOS, Linux</remarks>
     NumPad0 = 0x0060,

    /// <summary>Num-Pad 1</summary>
    /// <remarks>Available on: Windows, macOS, Linux</remarks>
     NumPad1 = 0x0061,

    /// <summary>Num-Pad 2</summary>
    /// <remarks>Available on: Windows, macOS, Linux</remarks>
     NumPad2 = 0x0062,

    /// <summary>Num-Pad 3</summary>
    /// <remarks>Available on: Windows, macOS, Linux</remarks>
     NumPad3 = 0x0063,

    /// <summary>Num-Pad 4</summary>
    /// <remarks>Available on: Windows, macOS, Linux</remarks>
     NumPad4 = 0x0064,

    /// <summary>Num-Pad 5</summary>
    /// <remarks>Available on: Windows, macOS, Linux</remarks>
     NumPad5 = 0x0065,

    /// <summary>Num-Pad 6</summary>
    /// <remarks>Available on: Windows, macOS, Linux</remarks>
     NumPad6 = 0x0066,

    /// <summary>Num-Pad 7</summary>
    /// <remarks>Available on: Windows, macOS, Linux</remarks>
     NumPad7 = 0x0067,

    /// <summary>Num-Pad 8</summary>
    /// <remarks>Available on: Windows, macOS, Linux</remarks>
     NumPad8 = 0x0068,

    /// <summary>Num-Pad 9</summary>
    /// <remarks>Available on: Windows, macOS, Linux</remarks>
     NumPad9 = 0x0069,

    /// <summary>Left Shift</summary>
    /// <remarks>Available on: Windows, macOS, Linux</remarks>
     LeftShift = 0xA010,

    /// <summary>Right Shift</summary>
    /// <remarks>Available on: Windows, macOS, Linux</remarks>
     RightShift = 0xB010,

    /// <summary>Left Control</summary>
    /// <remarks>Available on: Windows, macOS, Linux</remarks>
     LeftControl = 0xA011,

    /// <summary>Right Control</summary>
    /// <remarks>Available on: Windows, macOS, Linux</remarks>
     RightControl = 0xB011,

    /// <summary>
    /// Left Alt (on Windows and Linux) or Option (on macOS)
    /// </summary>
    /// <remarks>Available on: Windows, macOS, Linux</remarks>
     LeftAlt = 0xA012,

    /// <summary>
    /// Right Alt (on Windows and Linux) or Option (on macOS)
    /// </summary>
    /// <remarks>Available on: Windows, macOS, Linux</remarks>
     RightAlt = 0xB012,

    /// <summary>
    /// Left Win (on Windows), Command (on macOS), or Super/Meta (on Linux)
    /// </summary>
    /// <remarks>Available on: Windows, macOS, Linux</remarks>
     LeftMeta = 0xA09D,

    /// <summary>
    /// Right Win (on Windows), Command (on macOS), or Super/Meta (on Linux)
    /// </summary>
    /// <remarks>Available on: Windows, macOS, Linux</remarks>
     RightMeta = 0xB09D,

    /// <summary>Context Menu</summary>
    /// <remarks>Available on: Windows, macOS, Linux</remarks>
     ContextMenu = 0x020D,

    /// <summary>Function</summary>
    /// <remarks>Available on: macOS</remarks>
     Function = 0x020E,

    /// <summary>
    /// Function key when used to change an input source on macOS
    /// </summary>
    /// <remarks>Available on: macOS</remarks>
     ChangeInputSource = 0x020F,

    /// <summary>Power</summary>
    /// <remarks>Available on: macOS, Linux</remarks>
     Power = 0xE05E,

    /// <summary>Sleep</summary>
    /// <remarks>Available on: Windows, Linux</remarks>
     Sleep = 0xE05F,

    /// <summary>Play/Pause Media</summary>
    /// <remarks>Available on: Windows, macOS, Linux</remarks>
     MediaPlay = 0xE022,

    /// <summary>Stop Media</summary>
    /// <remarks>Available on: Windows, Linux</remarks>
     MediaStop = 0xE024,

    /// <summary>Previous Media</summary>
    /// <remarks>Available on: Windows, macOS, Linux</remarks>
     MediaPrevious = 0xE010,

    /// <summary>Next Media</summary>
    /// <remarks>Available on: Windows, macOS, Linux</remarks>
     MediaNext = 0xE019,

    /// <summary>Select Media</summary>
    /// <remarks>Available on: Windows</remarks>
     MediaSelect = 0xE06D,

    /// <summary>Eject Media</summary>
    /// <remarks>Available on: macOS, Linux</remarks>
     MediaEject = 0xE02C,

    /// <summary>Volume Mute</summary>
    /// <remarks>Available on: Windows, macOS, Linux</remarks>
     VolumeMute = 0xE020,

    /// <summary>Volume Down</summary>
    /// <remarks>Available on: Windows, macOS, Linux</remarks>
     VolumeDown = 0xE030,

    /// <summary>Volume Up</summary>
    /// <remarks>Available on: Windows, macOS, Linux</remarks>
     VolumeUp = 0xE02E,

    /// <summary>Launch app 1</summary>
    /// <remarks>Available on: Windows, Linux</remarks>
     App1 = 0xE026,

    /// <summary>Launch app 2</summary>
    /// <remarks>Available on: Windows, Linux</remarks>
     App2 = 0xE027,

    /// <summary>Launch app 3</summary>
    /// <remarks>Available on: Linux</remarks>
     App3 = 0xE028,

    /// <summary>Launch app 4</summary>
    /// <remarks>Available on: Linux</remarks>
     App4 = 0xE029,

    /// <summary>Launch browser</summary>
    /// <remarks>Available on: Linux</remarks>
     AppBrowser = 0xE025,

    /// <summary>Launch calculator</summary>
    /// <remarks>Available on: Linux</remarks>
     AppCalculator = 0xE021,

    /// <summary>Launch mail</summary>
    /// <remarks>Available on: Windows, Linux</remarks>
     AppMail = 0xE06C,

    /// <summary>Browser Search</summary>
    /// <remarks>Available on: Windows, Linux</remarks>
     BrowserSearch = 0xE065,

    /// <summary>Browser Home</summary>
    /// <remarks>Available on: Windows, Linux</remarks>
     BrowserHome = 0xE032,

    /// <summary>Browser Back</summary>
    /// <remarks>Available on: Windows, Linux</remarks>
     BrowserBack = 0xE06A,

    /// <summary>Browser Forward</summary>
    /// <remarks>Available on: Windows, Linux</remarks>
     BrowserForward = 0xE069,

    /// <summary>Browser Stop</summary>
    /// <remarks>Available on: Windows</remarks>
     BrowserStop = 0xE068,

    /// <summary>Browser Refresh</summary>
    /// <remarks>Available on: Windows, Linux</remarks>
     BrowserRefresh = 0xE067,

    /// <summary>Browser Favorites</summary>
    /// <remarks>Available on: Windows, Linux</remarks>
     BrowserFavorites = 0xE066,

    /// <summary>IME Katakana/Hiragana toggle</summary>
    /// <remarks>Available on: Linux</remarks>
     KatakanaHiragana = 0x0106,

    /// <summary>IME Katakana mode</summary>
    /// <remarks>Available on: Linux</remarks>
     Katakana = 0x00F1,

    /// <summary>IME Hiragana mode</summary>
    /// <remarks>Available on: Linux</remarks>
     Hiragana = 0x00F2,

    /// <summary>IME Kana mode</summary>
    /// <remarks>Available on: Windows, macOS</remarks>
     Kana = 0x0015,

    /// <summary>IME Kanji mode</summary>
    /// <remarks>Available on: Windows</remarks>
    [Obsolete(" Hanja should be used instead")]
     Kanji = 0x0019,

    /// <summary>IME Hangul mode</summary>
    /// <remarks>Available on: Windows, Linux</remarks>
    [Obsolete(" Kana should be used instead")]
     Hangul = 0x00E9,

    /// <summary>IME Junja mode</summary>
    /// <remarks>Available on: Windows</remarks>
     Junja = 0x00E8,

    /// <summary>IME Final mode</summary>
    /// <remarks>Available on: Windows</remarks>
     Final = 0x00E7,

    /// <summary>IME Hanja mode</summary>
    /// <remarks>Available on: Windows, Linux</remarks>
     Hanja = 0x00E6,

    /// <summary>IME Accept</summary>
    /// <remarks>Available on: Windows</remarks>
     Accept = 0x001E,

    /// <summary>IME Convert (henkan)</summary>
    /// <remarks>Available on: Windows, Linux</remarks>
     Convert = 0x001C,

    /// <summary>IME Non-Convert (muhenkan)</summary>
    /// <remarks>Available on: Windows, Linux</remarks>
     NonConvert = 0x001D,

    /// <summary>IME On</summary>
    /// <remarks>Available on: Windows</remarks>
     ImeOn = 0x0109,

    /// <summary>IME Off</summary>
    /// <remarks>Available on: Windows</remarks>
     ImeOff = 0x0108,

    /// <summary>IME Mode Change</summary>
    /// <remarks>Available on: Windows, Linux</remarks>
     ModeChange = 0x0107,

    /// <summary>IME Process</summary>
    /// <remarks>Available on: Windows</remarks>
     Process = 0x0105,

    /// <summary>IME Alphanumeric mode (eisū)</summary>
    /// <remarks>Available on: macOS</remarks>
     Alphanumeric = 0x00F0,

    /// <summary>_</summary>
    /// <remarks>Available on: macOS, Linux</remarks>
     Underscore = 0x020B,

    /// <summary>Yen</summary>
    /// <remarks>Available on: macOS, Linux</remarks>
     Yen = 0x020C,

    /// <summary>JP Comma</summary>
    /// <remarks>Available on: macOS, Linux</remarks>
     JpComma = 0x0210
}
