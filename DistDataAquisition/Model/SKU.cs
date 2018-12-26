namespace DistDataAquisition.Model
{
    public class SKU
    {
        [Identity]
        public int SKUID { get; set; }
        public string PartNumber { get; set; }
        public string Description { get; set; }
    }
}
