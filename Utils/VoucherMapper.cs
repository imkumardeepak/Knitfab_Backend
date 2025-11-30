using AvyyanBackend.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using TallyERPWebApi.Model;

public class VoucherMapper
{
	public static SalesOrder MapToDatabaseModel(Voucher xmlVoucher)
	{
		if (xmlVoucher == null) return null;

		return new SalesOrder
		{
			VchType = xmlVoucher.VchType ?? "",
			SalesDate = ParseDate(xmlVoucher.Date),
			Guid = xmlVoucher.Guid ?? "",
			GstRegistrationType = xmlVoucher.GstRegistrationType ?? "",
			StateName = xmlVoucher.StateName ?? "",
			PartyName = xmlVoucher.PartyName ?? "",
			PartyLedgerName = xmlVoucher.PartyLedgerName ?? "",
			VoucherNumber = xmlVoucher.VoucherNumber ?? "",
			Reference = xmlVoucher.Reference ?? "",
			CompanyAddress = JoinAddressLines(xmlVoucher.AddressList?.Addresses),
			BuyerAddress = JoinAddressLines(xmlVoucher.BasicBuyerAddressList?.BasicBuyerAddresses),
			OrderTerms = JoinTerms(xmlVoucher.BasicOrderTermsList?.BasicOrderTerms),
			LedgerEntries = JoinLedgerEntries(xmlVoucher.LedgerEntries),
			Items = MapItems(xmlVoucher.AllInventoryEntries) ?? new List<SalesOrderItem>()
		};

	}

	private static DateTime ParseDate(string dateString)
	{
		if (string.IsNullOrEmpty(dateString)) return DateTime.MinValue;

		try
		{
			return DateTime.ParseExact(dateString, "yyyyMMdd", CultureInfo.InvariantCulture);
		}
		catch
		{
			return DateTime.MinValue;
		}
	}

	private static string JoinAddressLines(List<string> lines)
	{
		return lines != null && lines.Any() ?
			string.Join(" | ", lines.Where(l => !string.IsNullOrEmpty(l))) :
			"";
	}

	private static string JoinTerms(List<string> terms)
	{
		return terms != null && terms.Any() ?
			string.Join(" | ", terms.Where(t => !string.IsNullOrEmpty(t))) :
			"";
	}

	private static string JoinLedgerEntries(List<LedgerEntries> entries)
	{
		return entries != null && entries.Any() ?
			string.Join(" | ", entries
				.Where(e => e != null && !string.IsNullOrEmpty(e.LedgerName) && !string.IsNullOrEmpty(e.Amount))
				.Select(e => $"{e.LedgerName}:{e.Amount}")) :
			"";
	}

	private static List<SalesOrderItem> MapItems(List<AllInventoryEntries> inventoryEntries)
	{
		if (inventoryEntries == null || !inventoryEntries.Any())
			return new List<SalesOrderItem>();

		return inventoryEntries
			.Where(item => item != null)
			.Select(item => new SalesOrderItem
			{
				StockItemName = item.StockItemName ?? "",
				Rate = item.Rate ?? "",
				Amount = item.Amount ?? "",
				ActualQty = item.ActualQty ?? "",
				BilledQty = item.BilledQty ?? "",
				Descriptions = JoinDescriptions(item.BasicUserDescriptionList?.BasicUserDescriptions),
				BatchName = item.BatchAllocations?.FirstOrDefault()?.BatchName ?? "",
				OrderNo = item.BatchAllocations?.FirstOrDefault()?.OrderNo ?? "",
				OrderDueDate = item.BatchAllocations?.FirstOrDefault()?.OrderDueDate?.Value ?? ""
			})
			.ToList();
	}

	private static string JoinDescriptions(List<string> descriptions)
	{
		return descriptions != null && descriptions.Any() ?
			string.Join(" | ", descriptions.Where(d => !string.IsNullOrEmpty(d))) :
			"";
	}
}