namespace MatrixScreensaver.Engine.Matrix;

public static class AlphabetProvider
{
    public static char[] GetAlphabet(string alphabet) => alphabet.Trim().ToLowerInvariant() switch
    {
        "hex" => "0123456789ABCDEF".ToCharArray(),
        "binary" => "01".ToCharArray(),
        "runes" => "ᚠᚢᚦᚨᚱᚲᚷᚹᚺᚾᛁᛃᛇᛈᛉᛊᛏᛒᛖᛗᛚᛜᛟᛞ".ToCharArray(),
        "demon" => "☠⛧☥†‡※◬◈◇◆⟡⟐⟁⧫0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray(),
        _ => "アイウエオカキクケコサシスセソタチツテトナニヌネノマミムメモヤユヨラリルレロワン0123456789".ToCharArray()
    };
}
