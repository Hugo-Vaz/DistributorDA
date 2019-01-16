namespace DistDataAcquisition.Model
{
    public class Distributor
    {
        [Identity]
        public int DistibutorID { get; set; }
        public string Name { get; set; }

        //Used for crawling, acces info
        public string UserName { get; set; }
        public string Password { get; set; }
        public string ResellerName { get; set; }
    }
}
