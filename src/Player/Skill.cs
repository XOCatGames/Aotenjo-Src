public class Skill
{
    public const int FENG = 0;
    public const int LIN = 1;
    public const int HUO = 2;
    public const int SHAN = 3;

    public static SkillType[][] SkillTypeMap =
    {
        new[] { SkillType.Feng, SkillType.YaoJiu, SkillType.JiFeng, SkillType.SanYuan },
        new[] { SkillType.Lin, SkillType.TongShun, SkillType.BuGao, SkillType.LianShun },
        new[] { SkillType.Huo, SkillType.JuShu, SkillType.QiMen, SkillType.RanShou },
        new[] { SkillType.Shan, SkillType.DuoGang, SkillType.JieGao, SkillType.TongKe }
    };

    public enum SkillType
    {
        Feng,

        //风
        YaoJiu,
        JiFeng,
        SanYuan,

        Lin,

        //林
        TongShun,
        BuGao,
        LianShun,

        Huo,

        //火
        JuShu,
        QiMen,
        RanShou,

        Shan,

        //山
        DuoGang,
        JieGao,
        TongKe
    }
}