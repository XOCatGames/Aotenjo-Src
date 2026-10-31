public static class Constants
{
    public static bool IS_DEMO = false;
#if AOTENJO_DEBUG || UNITY_EDITOR
    public static bool DEBUG_MODE = true;
    public static bool CONSOLE_ENABLED = true;
#elif AOTENJO_CONSOLE
    public static bool DEBUG_MODE = false;
    public static bool CONSOLE_ENABLED = true;
#else
    public static bool DEBUG_MODE = false;
    public static bool CONSOLE_ENABLED = false;
#endif

    #region 路径和本地化表格名

    public const string LOCALIZATION_TABLE_YAKU = "YakuInfo";
    public const string LOCALIZATION_TABLE_BOSS = "BossInfo";
    public const string LOCALIZATION_TABLE_GENERAL = "GameTable";
    public const string LOCALIZATION_TABLE_MAIN_MENU = "UIMainMenuTable";
    public const string LOCALIZATION_TABLE_TUTORIAL = "TutorialTable";

    public const string FILE_TILE_BACK_SHEET = "TileMaterials";
    public const string FILE_TILE_FRONT_SHEET = "Tile/TileFont";
    public const string FILE_TILE_MASK_SHEET = "TileMasks";
    public const string FILE_TILE_TRANSFORM_SHEET = "TileTransform";
    public const string PATH_GADGET_TEXTURE_ROOT = "Textures/Gadget";
    public const string FILE_GADGET_SHEET = PATH_GADGET_TEXTURE_ROOT + "/gadget_sheet";
    public const string FILE_BIRD_SHEET = "Tile/BirdFont";

    public const string PATH_ARTIFACT_DATA_ROOT = "Assets/Resources/ArtifactDefinition";
    public const string PATH_YAKU_DATA_ROOT = "Assets/Resources/YakuData";
    public const string PATH_STYLE_SHEET = "Assets/TextMesh Pro/Resources/Style Sheets/Default Style Sheet.asset";
    public const string PATH_TERM_DATA_ROOT = "Assets/Resources/TermDefinition";

    #endregion
}
