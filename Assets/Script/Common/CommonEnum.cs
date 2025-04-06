
namespace Assets.Script.Common
{
    public enum Difficulty
    {
        Easy = 1,
        Normal = 2,
        Hard = 3
    }

    /// <summary>
    /// 自機の種類
    /// </summary>
    public enum PlayerCharacter
    {
        LunaMagician_A,
        LunaMagician_B,
        SunMagician_A,
        SunMagician_B
    }

    /// <summary>
    /// 向き
    /// 2方向以上扱う場合はビット演算で判定できるようにしておく
    /// </summary>
    public enum Direction
    {
        None = 0x0,
        Left = 0x1,
        Right = 0x2,
        Upper = 0x4,
        Lower = 0x8
    }
}