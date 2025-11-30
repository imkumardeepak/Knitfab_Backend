using System;
using System.Collections.Generic;
using System.Xml.Serialization;

[XmlRoot(ElementName = "ENVELOPE")]
public class Envelope
{
	[XmlElement(ElementName = "HEADER")]
	public Header Header { get; set; }

	[XmlElement(ElementName = "BODY")]
	public Body Body { get; set; }
}

public class Header
{
	[XmlElement(ElementName = "TALLYREQUEST")]
	public string TallyRequest { get; set; }
}

public class Body
{
	[XmlElement(ElementName = "IMPORTDATA")]
	public ImportData ImportData { get; set; }
}

public class ImportData
{
	[XmlElement(ElementName = "REQUESTDESC")]
	public RequestDesc RequestDesc { get; set; }

	[XmlElement(ElementName = "REQUESTDATA")]
	public RequestData RequestData { get; set; }
}

public class RequestDesc
{
	[XmlElement(ElementName = "REPORTNAME")]
	public string ReportName { get; set; }

	[XmlElement(ElementName = "STATICVARIABLES")]
	public StaticVariables StaticVariables { get; set; }
}

public class StaticVariables
{
	[XmlElement(ElementName = "SVCURRENTCOMPANY")]
	public string CurrentCompany { get; set; }
}

public class RequestData
{
	[XmlElement(ElementName = "TALLYMESSAGE")]
	public List<TallyMessage> TallyMessages { get; set; }
}

public class TallyMessage
{
	[XmlElement(ElementName = "VOUCHER")]
	public Voucher Voucher { get; set; }

	[XmlElement(ElementName = "COMPANY")]
	public Company Company { get; set; }
}

public class Voucher
{
	[XmlAttribute(AttributeName = "REMOTEID")]
	public string RemoteId { get; set; }

	[XmlElement(ElementName = "ADDRESS.LIST")]
	public AddressList AddressList { get; set; }

	[XmlElement(ElementName = "BASICBUYERADDRESS.LIST")]
	public BasicBuyerAddressList BasicBuyerAddressList { get; set; }

	[XmlElement(ElementName = "BASICORDERTERMS.LIST")]
	public BasicOrderTermsList BasicOrderTermsList { get; set; }

	[XmlAttribute(AttributeName = "VCHKEY")]
	public string VchKey { get; set; }

	[XmlAttribute(AttributeName = "VCHTYPE")]
	public string VchType { get; set; }

	[XmlAttribute(AttributeName = "ACTION")]
	public string Action { get; set; }

	[XmlElement(ElementName = "DATE")]
	public string Date { get; set; }

	[XmlElement(ElementName = "GUID")]
	public string Guid { get; set; }

	[XmlElement(ElementName = "GSTREGISTRATIONTYPE")]
	public string GstRegistrationType { get; set; }

	[XmlElement(ElementName = "STATENAME")]
	public string StateName { get; set; }

	[XmlElement(ElementName = "PARTYNAME")]
	public string PartyName { get; set; }

	[XmlElement(ElementName = "PARTYLEDGERNAME")]
	public string PartyLedgerName { get; set; }

	[XmlElement(ElementName = "VOUCHERNUMBER")]
	public string VoucherNumber { get; set; }

	[XmlElement(ElementName = "REFERENCE")]
	public string Reference { get; set; }

	[XmlElement(ElementName = "ALTERID")]
	public string AlterId { get; set; }

	[XmlElement(ElementName = "ALLINVENTORYENTRIES.LIST")]
	public List<AllInventoryEntries> AllInventoryEntries { get; set; }

	[XmlElement(ElementName = "LEDGERENTRIES.LIST")]
	public List<LedgerEntries> LedgerEntries { get; set; }
}
public class AddressList
{
	[XmlElement(ElementName = "ADDRESS")]
	public List<string> Addresses { get; set; }

	[XmlAttribute(AttributeName = "TYPE")]
	public string Type { get; set; }
}

public class BasicBuyerAddressList
{
	[XmlElement(ElementName = "BASICBUYERADDRESS")]
	public List<string> BasicBuyerAddresses { get; set; }

	[XmlAttribute(AttributeName = "TYPE")]
	public string Type { get; set; }
}

public class BasicOrderTermsList
{
	[XmlElement(ElementName = "BASICORDERTERMS")]
	public List<string> BasicOrderTerms { get; set; }

	[XmlAttribute(AttributeName = "TYPE")]
	public string Type { get; set; }
}
public class AllInventoryEntries
{
	[XmlElement(ElementName = "BASICUSERDESCRIPTION.LIST")]
	public BasicUserDescriptionList BasicUserDescriptionList { get; set; }

	[XmlElement(ElementName = "STOCKITEMNAME")]
	public string StockItemName { get; set; }

	[XmlElement(ElementName = "RATE")]
	public string Rate { get; set; }

	[XmlElement(ElementName = "AMOUNT")]
	public string Amount { get; set; }

	[XmlElement(ElementName = "ACTUALQTY")]
	public string ActualQty { get; set; }

	[XmlElement(ElementName = "BILLEDQTY")]
	public string BilledQty { get; set; }

	[XmlElement(ElementName = "BATCHALLOCATIONS.LIST")]
	public List<BatchAllocations> BatchAllocations { get; set; }

	[XmlElement(ElementName = "ACCOUNTINGALLOCATIONS.LIST")]
	public List<AccountingAllocations> AccountingAllocations { get; set; }

	[XmlElement(ElementName = "RATEDETAILS.LIST")]
	public List<RateDetails> RateDetails { get; set; }
}
public class BasicUserDescriptionList
{
	[XmlElement(ElementName = "BASICUSERDESCRIPTION")]
	public List<string> BasicUserDescriptions { get; set; }

	[XmlAttribute(AttributeName = "TYPE")]
	public string Type { get; set; }
}
public class BatchAllocations
{
	[XmlElement(ElementName = "BATCHNAME")]
	public string BatchName { get; set; }

	[XmlElement(ElementName = "ORDERNO")]
	public string OrderNo { get; set; }

	[XmlElement(ElementName = "ORDERDUEDATE")]
	public OrderDueDate OrderDueDate { get; set; }

	[XmlElement(ElementName = "AMOUNT")]
	public string Amount { get; set; }

	[XmlElement(ElementName = "ACTUALQTY")]
	public string ActualQty { get; set; }

	[XmlElement(ElementName = "BILLEDQTY")]
	public string BilledQty { get; set; }
}

public class OrderDueDate
{
	[XmlAttribute(AttributeName = "JD")]
	public string Jd { get; set; }

	[XmlAttribute(AttributeName = "P")]
	public string P { get; set; }

	[XmlText]
	public string Value { get; set; }
}

public class AccountingAllocations
{
	[XmlElement(ElementName = "LEDGERNAME")]
	public string LedgerName { get; set; }

	[XmlElement(ElementName = "ISDEEMEDPOSITIVE")]
	public string IsDeemedPositive { get; set; }

	[XmlElement(ElementName = "AMOUNT")]
	public string Amount { get; set; }
}

public class RateDetails
{
	[XmlElement(ElementName = "GSTRATEDUTYHEAD")]
	public string GstRateDutyHead { get; set; }

	[XmlElement(ElementName = "GSTRATEVALUATIONTYPE")]
	public string GstRateValuationType { get; set; }

	[XmlElement(ElementName = "GSTRATE")]
	public string GstRate { get; set; }
}

public class LedgerEntries
{
	[XmlElement(ElementName = "LEDGERNAME")]
	public string LedgerName { get; set; }

	[XmlElement(ElementName = "ISDEEMEDPOSITIVE")]
	public string IsDeemedPositive { get; set; }

	[XmlElement(ElementName = "AMOUNT")]
	public string Amount { get; set; }
}

public class Company
{
	[XmlElement(ElementName = "REMOTECMPINFO.LIST")]
	public RemoteCmpInfoList RemoteCmpInfoList { get; set; }
}

public class RemoteCmpInfoList
{
	[XmlElement(ElementName = "NAME")]
	public string Name { get; set; }

	[XmlElement(ElementName = "REMOTECMPNAME")]
	public string RemoteCmpName { get; set; }

	[XmlElement(ElementName = "REMOTECMPSTATE")]
	public string RemoteCmpState { get; set; }
}