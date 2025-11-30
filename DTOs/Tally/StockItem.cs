namespace TallyERPWebApi.Model
{
    public class StockItem
    {
        public string name { get; set; }

        public string alias { get; set; }

        public string GUID { get; set; } = "NA";

        public string unit { get; set; }

        public string category { get; set; }

        public double openingrate { get; set; }
        public string openingqnty { get; set; }

        public string hsncode { get; set; }
        public string cgst { get; set; }
        public string sgst { get; set; }
        public string igst { get; set; }


    }
}
