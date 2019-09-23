namespace Oi
{
    public struct GitCommand
    {
        public readonly string Name;
        public readonly string Description;

        public GitCommand(string name, string desc)
        {
            Name = name;
            Description = desc;
        }
    }
}