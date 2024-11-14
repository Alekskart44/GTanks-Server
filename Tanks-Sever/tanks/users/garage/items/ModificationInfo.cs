namespace Tanks_Sever.tanks.users.garage.items
{
    public class ModificationInfo
    {
        public string PreviewId { get; }
        public int Price { get; }
        public int Rank { get; }
        public PropertyItem[] Propertys;

        public ModificationInfo(string previewId, int price, int rank)
        {
            PreviewId = previewId;
            Price = price;
            Rank = rank;
            Propertys = new PropertyItem[1];
        }
    }
}

