namespace Origin.Mission
{
    public class HeldItem
    {
        public HeldItemId ItemId { get; }
        public string Name { get; }

        public HeldItem(HeldItemId itemId, string name)
        {
            ItemId = itemId;
            Name = name;
        }

        public string DescribeSelf()
        {
            return "{b}" + Name + "{/b}";
        }
    }
}