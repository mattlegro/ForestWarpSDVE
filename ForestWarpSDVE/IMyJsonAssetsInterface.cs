namespace ForestWarp
{
    internal interface IMyJsonAssetsInterface
    {
        int GetObjectId(string name);
    }

    public class Api : IMyJsonAssetsInterface
    {
        public int GetObjectId(string name)
        {
            if (Mod.instance.objectIds == null)
                return -1;
            return Mod.instance.objectIds.ContainsKey(name) ? Mod.instance.objectIds[name] : -1;
        }
    }
}