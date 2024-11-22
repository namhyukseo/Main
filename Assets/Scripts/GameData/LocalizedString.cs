
using Framework;

enum LocalizedStringID
{
    [LocalizedString(Text = "게임 종료?")]
    ID_QUIT_GAME,
    [LocalizedString(Text = "게임을 종료합니까?")]
    ID_QUIT_GAME_MESSAGE,
}

static class ExtensionClass
{
    public static string ToLocalizedString(this LocalizedStringID _enum)
    {
        return LocalizeStringTable<LocalizedStringID>.Instance.ToLoclizedString(_enum);
    }
}