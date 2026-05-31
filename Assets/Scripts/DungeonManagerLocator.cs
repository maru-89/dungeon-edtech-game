public static class DungeonManagerLocator
{
    public static IDungeonManager Instance =>
        DungeonManager.Instance as IDungeonManager ??
        TutorialDungeonManager.Instance as IDungeonManager;
}