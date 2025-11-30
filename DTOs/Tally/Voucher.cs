namespace TallyERPWebApi.Model
{
	public class VoucherSO
	{
		public string RemoteID { get; set; } = "NA";
		public string VoucherType { get; set; }
		public string Date { get; set; }
		public string PartyName { get; set; }
		public string AccountType { get; set; }
		public string overallamount { get; set; } = "NA";
		public string ledgername { get; set; } = "NA";
		public string partymailingname { get; set; } = "NA";
		public string PERSISTEDVIEW { get; set; } = "NA";
		public string PLACEOFSUPPLY { get; set; } = "NA";
		public string ISGSTOVERRIDDEN { get; set; } = "NA";
		public string ISINVOICE { get; set; } = "NA";
		public string USEFORGAINLOSS { get; set; } = "NA";

		public List<ItemDetails> Items { get; set; }  // List of items

	}
	public class ItemDetails
	{
		public string StockItemName { get; set; }
		public string StockItemDescription { get; set; }
		public string HSN { get; set; }
		public string Rate { get; set; }
		public string Amount { get; set; }
		public string ActualQty { get; set; }
		public string Unit { get; set; } = "NA";
		//public string STOCKLEDGER { get; set; } = "NA";
		//public string STOCKLEDGERAMOUNT { get; set; } = "NA";
	}
}
