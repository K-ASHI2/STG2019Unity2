
namespace Assets.Script.Common
{
    public enum Difficulty
    {
        Easy = 1,
        Normal = 2,
        Hard = 3
    }

    /// <summary>
    /// ���@�̎��
    /// </summary>
    public enum PlayerCharacter
    {
        LunaMagician_A,
        LunaMagician_B,
        SunMagician_A,
        SunMagician_B
    }

    /// <summary>
    /// ����
    /// 2�����ȏ㈵���ꍇ�̓r�b�g���Z�Ŕ���ł���悤�ɂ��Ă���
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