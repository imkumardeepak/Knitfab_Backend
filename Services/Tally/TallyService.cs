using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using TallyERPWebApi.Model;
using static System.Runtime.InteropServices.JavaScript.JSType;

public class TallyService
{
	private readonly HttpClient _httpClient;
	private readonly IConfiguration _configuration;
	private readonly ILogger<TallyService> _logger;
	private readonly IWebHostEnvironment _webHostEnvironment;

	public TallyService(HttpClient httpClient, IConfiguration configuration, ILogger<TallyService> logger, IWebHostEnvironment webHostEnvironment)
	{
		_httpClient = httpClient;
		_configuration = configuration;
		_logger = logger;
		_webHostEnvironment = webHostEnvironment;
	}
	public async Task<bool> GetTestConnection()
	{
		try
		{
			// Send a GET request to the Tally server
			string tallyUrl = _configuration["TallySettings:TallyUrl"] ?? string.Empty;
			var response = await _httpClient.GetAsync(tallyUrl);

			if (response.IsSuccessStatusCode)
			{
				// Server is running
				return true;
			}
			else
			{
				// Server is reachable but returned an error
				return false;
			}
		}
		catch (Exception ex)
		{

			return false;
		}
	}
	public async Task<List<string>> GetOpenCompaniesAsync(string xmlFilePath)
	{
		try
		{
			// Validate Tally URL
			string tallyUrl = _configuration["TallySettings:TallyUrl"] ?? string.Empty;
			if (string.IsNullOrWhiteSpace(tallyUrl) || !Uri.TryCreate(tallyUrl, UriKind.Absolute, out _))
			{
				throw new InvalidOperationException("Invalid Tally URL configured.");
			}

			// Validate XML file
			if (!File.Exists(xmlFilePath))
			{
				throw new FileNotFoundException("The specified XML file was not found.", xmlFilePath);
			}

			// Read XML content
			string xmlContent = await File.ReadAllTextAsync(xmlFilePath);

			// Create HTTP request
			var request = new HttpRequestMessage(HttpMethod.Post, tallyUrl)
			{
				Content = new StringContent(xmlContent, Encoding.UTF8, "text/xml")
			};

			// Send request
			var response = await _httpClient.SendAsync(request);
			response.EnsureSuccessStatusCode();

			var responseContent = await response.Content.ReadAsStringAsync();


			// Log response
			_logger.LogInformation("Tally Response XML: {Response}", responseContent);

			// Parse XML response
			XmlDocument doc = new XmlDocument();
			doc.LoadXml(responseContent);

			var jsonContent = JsonConvert.SerializeXmlNode(doc, Newtonsoft.Json.Formatting.Indented);
			_logger.LogInformation("Converted JSON: {JsonContent}", jsonContent);

			var jsonObject = JObject.Parse(jsonContent);

			// Extract all company names from JSON
			List<string> companyList = new List<string>();

			var companyNodes = jsonObject["ENVELOPE"]?["BODY"]?["DATA"]?["COLLECTION"]?["COMPANY"];
			if (companyNodes is JArray companiesArray)
			{
				// Multiple companies
				companyList = companiesArray
					.Select(company => company["@NAME"]?.ToString()) // Extract the "#text" field
					.Where(name => !string.IsNullOrWhiteSpace(name))
					.ToList();
			}
			else if (companyNodes is JObject singleCompany)
			{
				// Single company
				string companyName = singleCompany["@NAME"]?.ToString();
				if (!string.IsNullOrWhiteSpace(companyName))
				{
					companyList.Add(companyName);
				}
			}

			if (!companyList.Any())
			{
				_logger.LogWarning("No open companies found in Tally response.");
				return new List<string>();
			}

			return companyList;
		}
		catch (HttpRequestException httpEx)
		{
			_logger.LogError(httpEx, "HTTP request to Tally failed.");
			return new List<string> { "Tally server connection failed." };
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "An unexpected error occurred.");
			return new List<string> { "Error retrieving companies from Tally." };
		}
	}
	public async Task<List<string>> GetStockGroup(string xmlFilePath)
	{
		try
		{
			// Validate Tally URL
			string tallyUrl = _configuration["TallySettings:TallyUrl"] ?? string.Empty;
			if (string.IsNullOrWhiteSpace(tallyUrl))
			{
				throw new InvalidOperationException("Tally URL is not configured.");
			}

			// Validate XML file
			if (!File.Exists(xmlFilePath))
			{
				throw new FileNotFoundException("The specified XML file was not found.", xmlFilePath);
			}

			// Read the XML content from the file
			string xmlContent = await File.ReadAllTextAsync(xmlFilePath);

			string companyName = _configuration["TallySettings:CompanyName"] ?? string.Empty;

			if (string.IsNullOrEmpty(companyName))
			{
				throw new InvalidOperationException("Company Name is not configured.");
			}

			xmlContent = xmlContent.Replace("{Cname}", companyName);

			// Create HTTP request
			var request = new HttpRequestMessage(HttpMethod.Post, tallyUrl)
			{
				Content = new StringContent(xmlContent, Encoding.UTF8, "text/xml")
			};

			// Send request and get response
			var response = await _httpClient.SendAsync(request);
			response.EnsureSuccessStatusCode();

			var responseContent = await response.Content.ReadAsStringAsync();
			// Unescape XML special characters
			//responseContent = WebUtility.HtmlDecode(responseContent);
			var groupNames = new List<string>();

			string[] strArrayOne = responseContent.Split('>');

			foreach (string tag in strArrayOne)
			{
				if (tag.Contains("STOCKGROUP NAME"))
				{
					string data = tag.Trim();

					var regex = new Regex(@"NAME=""([^""]+)""");
					foreach (Match match in regex.Matches(data))
					{
						groupNames.Add(match.Groups[1].Value);
					}
				}
			}

			return groupNames;
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "An error occurred while communicating with Tally.");
			throw;
		}
	}
	public async Task<double> GetClosingBalace(string xmlFilePath, string customername1)
	{
		try
		{
			string tallyUrl = _configuration["TallySettings:TallyUrl"] ?? string.Empty;
			if (string.IsNullOrWhiteSpace(tallyUrl))
			{
				throw new InvalidOperationException("Tally URL is not configured.");
			}

			if (!File.Exists(xmlFilePath))
			{
				throw new FileNotFoundException("The specified XML file was not found.", xmlFilePath);
			}

			string xmlContent = await File.ReadAllTextAsync(xmlFilePath);
			xmlContent = xmlContent.Replace("{CustomerName}", customername1);
			string companyName = _configuration["TallySettings:CompanyName"] ?? string.Empty;
			if (string.IsNullOrEmpty(companyName))
			{
				throw new InvalidOperationException("Company name is not configured.");
			}

			xmlContent = xmlContent.Replace("{Cname}", companyName);

			var request = new HttpRequestMessage(HttpMethod.Post, tallyUrl)
			{
				Content = new StringContent(xmlContent, Encoding.UTF8, "text/xml")
			};

			var response = await _httpClient.SendAsync(request);
			response.EnsureSuccessStatusCode();

			var str = await response.Content.ReadAsStringAsync();
			str = RemoveInvalidCharacters(str);

			// Unescape XML special characters
			str = WebUtility.HtmlDecode(str);


			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(str);
			string jsonData = JsonConvert.SerializeXmlNode(xmlDocument, Newtonsoft.Json.Formatting.Indented);
			var jsonObject = JsonConvert.DeserializeObject<JObject>(jsonData);
			RemoveEmptyValues(jsonObject);
			string cleanedJsonData = JsonConvert.SerializeObject(jsonObject, Newtonsoft.Json.Formatting.Indented);

			var data = JObject.Parse(cleanedJsonData);

			// Correct JSON Path
			var ledgerData = data["ENVELOPE"]?["BODY"]?["DATA"]?["TALLYMESSAGE"]?["LEDGER"];

			if (ledgerData != null)
			{
				var customerName = ledgerData["@NAME"]?.ToString()?.Trim();
				if (!string.IsNullOrEmpty(customerName) && customerName.Equals(customername1.Trim(), StringComparison.OrdinalIgnoreCase))
				{
					var closingBalanceField = ledgerData["CLOSINGBALANCE"];
					if (closingBalanceField?["#text"] != null)
					{
						string balanceText = closingBalanceField["#text"]?.ToString()?.Trim();
						if (double.TryParse(balanceText, out double balance))
						{
							return balance; // Return actual balance instead of 0
						}
					}
				}
			}

			return 0; // Default return if no matching ledger is found
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "An error occurred while communicating with Tally.");
			throw;
		}
	}
	public async Task<List<string>> GetVoucherType(string xmlFilePath)
	{
		try
		{
			// Validate Tally URL
			string tallyUrl = _configuration["TallySettings:TallyUrl"] ?? string.Empty;
			if (string.IsNullOrWhiteSpace(tallyUrl))
			{
				throw new InvalidOperationException("Tally URL is not configured.");
			}

			// Validate XML file
			if (!File.Exists(xmlFilePath))
			{
				throw new FileNotFoundException("The specified XML file was not found.", xmlFilePath);
			}

			// Read the XML content from the file
			string xmlContent = await File.ReadAllTextAsync(xmlFilePath);

			string companyName = _configuration["TallySettings:CompanyName"] ?? string.Empty;
			if (string.IsNullOrEmpty(companyName))
			{
				throw new InvalidOperationException("Company name is not configured.");
			}
			xmlContent = xmlContent.Replace("{Cname}", companyName);

			// Create HTTP request
			var request = new HttpRequestMessage(HttpMethod.Post, tallyUrl)
			{
				Content = new StringContent(xmlContent, Encoding.UTF8, "text/xml")
			};

			// Send request and get response
			var response = await _httpClient.SendAsync(request);
			response.EnsureSuccessStatusCode();

			var responseContent = await response.Content.ReadAsStringAsync();

			// Unescape XML special characters
			//responseContent = WebUtility.HtmlDecode(responseContent);

			_logger.LogInformation("Tally Response: {responseContent}", responseContent);

			var groupNames = new List<string>();

			string[] strArrayOne = responseContent.Split('>');

			foreach (string tag in strArrayOne)
			{
				if (tag.Contains("VOUCHERTYPE NAME"))
				{
					string data = tag.Trim();

					var regex = new Regex(@"VOUCHERTYPE NAME=""([^""]+)""");
					foreach (Match match in regex.Matches(data))
					{
						groupNames.Add(match.Groups[1].Value);
					}
				}
			}

			return groupNames;
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "An error occurred while communicating with Tally.");
			throw;
		}
	}
	public async Task<List<string>> GetUOM(string xmlFilePath)
	{
		try
		{
			// Validate Tally URL
			string tallyUrl = _configuration["TallySettings:TallyUrl"] ?? string.Empty;
			if (string.IsNullOrWhiteSpace(tallyUrl))
			{
				throw new InvalidOperationException("Tally URL is not configured.");
			}

			// Validate XML file
			if (!File.Exists(xmlFilePath))
			{
				throw new FileNotFoundException("The specified XML file was not found.", xmlFilePath);
			}

			// Read the XML content from the file
			string xmlContent = await File.ReadAllTextAsync(xmlFilePath);

			string companyName = _configuration["TallySettings:CompanyName"] ?? string.Empty;
			if (string.IsNullOrEmpty(companyName))
			{
				throw new InvalidOperationException("Company Name is not configured.");
			}

			xmlContent = xmlContent.Replace("{Cname}", companyName);

			// Create HTTP request
			var request = new HttpRequestMessage(HttpMethod.Post, tallyUrl)
			{
				Content = new StringContent(xmlContent, Encoding.UTF8, "text/xml")
			};

			// Send request and get response
			var response = await _httpClient.SendAsync(request);
			response.EnsureSuccessStatusCode();

			var responseContent = await response.Content.ReadAsStringAsync();

			// Unescape XML special characters
			//responseContent = WebUtility.HtmlDecode(responseContent);

			var groupNames = new List<string>();

			string[] strArrayOne = responseContent.Split('>');

			foreach (string tag in strArrayOne)
			{
				if (tag.Contains("UNIT NAME"))
				{
					string data = tag.Trim();

					var regex = new Regex(@"NAME=""([^""]+)""");
					foreach (Match match in regex.Matches(data))
					{
						groupNames.Add(match.Groups[1].Value);
					}
				}
			}

			return groupNames;
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "An error occurred while communicating with Tally.");
			throw;
		}
	}
	public async Task<List<string>> GetAccountGroup(string xmlFilePath)
	{
		try
		{
			string tallyUrl = _configuration["TallySettings:TallyUrl"] ?? string.Empty;

			if (string.IsNullOrWhiteSpace(tallyUrl))
			{
				throw new InvalidOperationException("Tally URL is not configured.");
			}

			// Validate XML file
			if (!File.Exists(xmlFilePath))
			{
				throw new FileNotFoundException("The specified XML file was not found.", xmlFilePath);
			}

			// Read the XML content from the file
			string xmlContent = await File.ReadAllTextAsync(xmlFilePath);

			string companyName = _configuration["TallySettings:CompanyName"] ?? string.Empty;
			if (string.IsNullOrEmpty(companyName))
			{
				throw new InvalidOperationException("Company Name is not configured.");
			}
			xmlContent = xmlContent.Replace("{Cname}", companyName);

			// Create HTTP request
			var request = new HttpRequestMessage(HttpMethod.Post, tallyUrl)
			{
				Content = new StringContent(xmlContent, Encoding.UTF8, "text/xml")
			};

			// Send request and get response
			var response = await _httpClient.SendAsync(request);
			response.EnsureSuccessStatusCode();

			var responseContent = await response.Content.ReadAsStringAsync();

			// Unescape XML special characters
			//responseContent = WebUtility.HtmlDecode(responseContent);

			var groupNames = new List<string>();

			string[] strArrayOne = responseContent.Split('>');

			foreach (string tag in strArrayOne)
			{
				if (tag.Contains("GROUP NAME"))
				{
					string data = tag.Trim();

					var regex = new Regex(@"GROUP NAME=""([^""]+)""");
					foreach (Match match in regex.Matches(data))
					{
						groupNames.Add(match.Groups[1].Value);
					}
				}
			}

			return groupNames;
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "An error occurred while communicating with Tally.");
			throw;
		}
	}
	public async Task<List<StockItem>> GetStockItem(string xmlFilePath)
	{
		try
		{
			// Validate Tally URL
			string tallyUrl = _configuration["TallySettings:TallyUrl"] ?? string.Empty;
			if (string.IsNullOrEmpty(tallyUrl))
			{
				throw new InvalidOperationException("Tally URL is not configured.");
			}

			// Validate XML file
			if (!File.Exists(xmlFilePath))
			{
				throw new FileNotFoundException("The specified XML file was not found.", xmlFilePath);
			}

			// Read the XML content from the file
			string xmlContent = await File.ReadAllTextAsync(xmlFilePath);

			string companyName = _configuration["TallySettings:CompanyName"] ?? string.Empty;
			if (string.IsNullOrEmpty(companyName))
			{
				throw new InvalidOperationException("Company Name is not configured.");
			}
			xmlContent = xmlContent.Replace("{Cname}", companyName);

			// Create HTTP request
			var request = new HttpRequestMessage(HttpMethod.Post, tallyUrl)
			{
				Content = new StringContent(xmlContent, Encoding.UTF8, "text/xml")
			};

			// Send request and get response
			var response = await _httpClient.SendAsync(request);
			response.EnsureSuccessStatusCode();

			var responseContent = await response.Content.ReadAsStringAsync();

			responseContent = RemoveInvalidCharacters(responseContent);

			// Unescape XML special characters
			////responseContent = WebUtility.HtmlDecode(responseContent);

			// Parse the cleaned XML response
			var xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(responseContent);

			// Convert XML to JSON
			string jsonData = JsonConvert.SerializeXmlNode(xmlDocument, Newtonsoft.Json.Formatting.Indented);

			// Deserialize JSON into a JObject for manipulation
			var jsonObject = JsonConvert.DeserializeObject<JObject>(jsonData);
			// Remove empty or invalid values recursively
			RemoveEmptyValues(jsonObject);

			// 02/07/2025 by Niraj
			/* // Navigate to the TALLYMESSAGE array
             var tallyMessageArray = jsonObject["ENVELOPE"]?["BODY"]?["DATA"]?["COLLECTION"]?["STOCKITEM"];

             // Convert the TALLYMESSAGE array to a formatted JSON string
             string tallyMessageJson = JsonConvert.SerializeObject(tallyMessageArray, Newtonsoft.Json.Formatting.Indented);
             var finaldata = JsonConvert.DeserializeObject<List<Dictionary<string, dynamic>>>(tallyMessageJson);*/

			// Normalize STOCKITEM to JArray to handle both single and multiple entries
			var stockItemToken = jsonObject["ENVELOPE"]?["BODY"]?["DATA"]?["COLLECTION"]?["STOCKITEM"];
			JArray stockItemArray = stockItemToken switch
			{
				JArray arr => arr,
				JObject obj => new JArray(obj),
				_ => new JArray() // Handle null or unexpected format
			};

			string tallyMessageJson = JsonConvert.SerializeObject(stockItemArray, Newtonsoft.Json.Formatting.Indented);
			var finaldata = JsonConvert.DeserializeObject<List<Dictionary<string, dynamic>>>(tallyMessageJson);


			var voucherList = new List<StockItem>();

			if (finaldata != null)
			{
				foreach (var entry in finaldata)
				{
					// Safely access "HSNDETAILS.LIST" as a dictionary or null
					var hsnData = entry.ContainsKey("HSNDETAILS.LIST") && entry["HSNDETAILS.LIST"] is JObject
								  ? (JObject)entry["HSNDETAILS.LIST"]
								  : null;

					// Safely access nested properties for alias
					string alias = "NA";
					if (entry.ContainsKey("LANGUAGENAME.LIST") && entry["LANGUAGENAME.LIST"] is JObject languageNameList &&
						languageNameList.ContainsKey("NAME.LIST") && languageNameList["NAME.LIST"] is JObject nameList &&
						nameList.ContainsKey("NAME") && nameList["NAME"] is JArray names && names.Count > 1)
					{
						alias = names[1]?.ToString() ?? "NA";
					}

					//GST
					int gstrate = 0;
					//if (entry.ContainsKey("RATEDETAILS.LIST"))
					//{
					//    var rateDetails = entry["GSTDETAILS.LIST"]?["STATEWISEDETAILS.LIST"]?["RATEDETAILS.LIST"];
					//    if (rateDetails is JArray rateArray)
					//    {
					//        foreach (var rateDetail in rateArray)
					//        {
					//            var dutyHead = rateDetail["GSTRATEDUTYHEAD"]?.ToString();
					//            if (dutyHead == "IGST") // Assuming you need IGST rate specifically
					//            {
					//                gstrate = Convert.ToInt32(rateDetail["GSTRATE"]);
					//                break;
					//            }
					//        }
					//    }
					//}
					if (entry.ContainsKey("GSTDETAILS.LIST"))
					{
						var gstDetailsList = entry["GSTDETAILS.LIST"];
						// GSTDETAILS.LIST can be array or object
						var gstDetailsArray = gstDetailsList is JArray ? (JArray)gstDetailsList : new JArray { gstDetailsList };
						foreach (var gstDetail in gstDetailsArray)
						{
							var statewiseDetails = gstDetail["STATEWISEDETAILS.LIST"];
							if (statewiseDetails != null)
							{
								var rateDetails = statewiseDetails["RATEDETAILS.LIST"];

								if (rateDetails is JArray rateArray)
								{
									foreach (var rateDetail in rateArray)
									{
										if (rateDetail["GSTRATEDUTYHEAD"]?.ToString() == "IGST")
										{
											gstrate = Convert.ToInt32(rateDetail["GSTRATE"] ?? "0");
											break;
										}
									}
								}
								else if (rateDetails is JObject singleRate)
								{
									if (singleRate["GSTRATEDUTYHEAD"]?.ToString() == "IGST")
									{
										gstrate = Convert.ToInt32(singleRate["GSTRATE"] ?? "0");
										break;
									}
								}
							}
							if (gstrate > 0)
								break;
						}
					}


					// Create the StockItem object
					var voucher = new StockItem
					{
						name = entry.ContainsKey("@NAME") ? entry["@NAME"]?.ToString() ?? "NA" : "NA",
						GUID = entry.ContainsKey("GUID") ? entry["GUID"]?.ToString() ?? "NA" : "NA",
						openingrate = entry.ContainsKey("OPENINGRATE") && entry["OPENINGRATE"] != null
							 ? Convert.ToDouble(ConvertToInt(entry["OPENINGRATE"].ToString())) : 0,
						openingqnty = (entry.ContainsKey("OPENINGBALANCE") && entry["OPENINGBALANCE"] != null)
						? ExtractNumericPart(entry["OPENINGBALANCE"].ToString()).ToString() : "NA",
						category = RemoveJunkCharacters(entry.ContainsKey("PARENT") ? entry["PARENT"]?.ToString() ?? "NA" : "NA"),
						unit = entry.ContainsKey("BASEUNITS") ? entry["BASEUNITS"]?.ToString() ?? "NA" : "NA",
						hsncode = hsnData?.ContainsKey("HSNCODE") == true ? hsnData["HSNCODE"]?.ToString() ?? "NA" : "NA",
						alias = alias,
						cgst = Convert.ToString(gstrate / 2),
						sgst = Convert.ToString(gstrate / 2),
						igst = Convert.ToString(gstrate),
					};

					voucherList.Add(voucher);
				}
			}



			return voucherList;
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "An error occurred while communicating with Tally.");
			throw;
		}
	}
	static int ExtractNumericPart(string value)
	{
		// Split the string by space and take the first part
		string numericPart = value.Trim().Split(' ')[0];

		// Attempt to convert it to an integer
		return int.TryParse(numericPart, out int result) ? result : 0; // Default to 0 if conversion fails
	}
	static double ConvertToInt(string value)
	{
		// Split the string by '/' and take the first part
		string numericPart = value.Split('/')[0];

		// Attempt to convert it to an integer
		return double.TryParse(numericPart, out double result) ? result : 0; // Default to 0 if conversion fails
	}
	public async Task<List<Ledger>> GetSupplierData(string xmlFilePath)
	{
		try
		{
			var voucherList = new List<Ledger>();

			// Validate Tally URL
			string tallyUrl = _configuration["TallySettings:TallyUrl"] ?? string.Empty;
			if (string.IsNullOrWhiteSpace(tallyUrl))
			{
				throw new InvalidOperationException("Tally URL is not configured.");
			}

			// Validate XML file
			if (!File.Exists(xmlFilePath))
			{
				throw new FileNotFoundException("The specified XML file was not found.", xmlFilePath);
			}

			// Read the XML content from the file
			string xmlContent = await File.ReadAllTextAsync(xmlFilePath);

			string companyName = _configuration["TallySettings:CompanyName"] ?? string.Empty;
			if (string.IsNullOrWhiteSpace(companyName))
			{
				throw new InvalidOperationException("Company Name is not configured.");
			}

			xmlContent = xmlContent.Replace("{Cname}", companyName);

			// Create HTTP request
			var request = new HttpRequestMessage(HttpMethod.Post, tallyUrl)
			{
				Content = new StringContent(xmlContent, Encoding.UTF8, "text/xml")
			};

			//// Send request and get response

			var response = await _httpClient.SendAsync(request);
			response.EnsureSuccessStatusCode();
			var str = await response.Content.ReadAsStringAsync();
			if (str.Contains("\u0005"))
			{
				str = str?.Replace("\u0005", "") ?? "";
			}
			if (str.Contains("\u0004"))
			{
				str = str?.Replace("\u0004", "") ?? "";
			}
			if (str.Contains("\u0004 Not Applicable"))
			{
				str = str?.Replace("\u0004 Not Applicable", "NA") ?? "";
			}

			var responseContent = await response.Content.ReadAsStringAsync();
			responseContent = RemoveInvalidCharacters(responseContent);

			// Unescape XML special characters
			//responseContent = WebUtility.HtmlDecode(responseContent);

			var xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(responseContent);
			string jsonData = JsonConvert.SerializeXmlNode(xmlDocument, Newtonsoft.Json.Formatting.Indented);
			var jsonObject = JsonConvert.DeserializeObject<JObject>(jsonData);
			RemoveEmptyValues(jsonObject);
			var tallyMessages = jsonObject["ENVELOPE"]?["BODY"]?["DATA"]?["COLLECTION"]?["LEDGER"];
			if (tallyMessages != null)
			{

				foreach (var stockItem in tallyMessages)
				{
					//customer_id, customername, contactperson, gstno, contactno, emailid, address, city, state, "Country", pincode, customerid
					var supplierName = stockItem["@NAME"]?.ToString();
					var addressList = stockItem["ADDRESS.LIST"];
					int CREDITLIMIT = (int)Convert.ToDecimal(stockItem["CREDITLIMIT"]);
					string address1 = "NA";

					if (addressList != null)
					{
						if (addressList["ADDRESS"] is JArray addressArray)
						{
							address1 = string.Join(", ", addressArray.Select(a => a["#text"]?.ToString() ?? "").Where(a => !string.IsNullOrWhiteSpace(a)));
						}
						else if (addressList["ADDRESS"]?["#text"] != null)
						{
							address1 = addressList["ADDRESS"]["#text"]?.ToString() ?? string.Empty;
						}
					}

					var found = "NA";
					//var gstNo = stockItem["LEDGSTREGDETAILS.LIST"]?["GSTIN"]?.ToString().Trim() ?? "NA";
					string gstNo = "NA";
					var gstDetailsToken = stockItem["LEDGSTREGDETAILS.LIST"];
					if (gstDetailsToken != null)
					{
						if (gstDetailsToken is JObject gstObj)
						{
							gstNo = gstObj["GSTIN"]?.ToString().Trim() ?? "NA";
						}
						else if (gstDetailsToken is JArray gstArray && gstArray.Count > 0)
						{
							gstNo = gstArray[0]?["GSTIN"]?.ToString().Trim() ?? "NA";
						}

						// Handle any '\u0004 Unknown' edge cases
						if (gstNo.Contains("\u0004 Unknown"))
						{
							gstNo = "NA";
						}
					}
					var contactNo = stockItem["CONTACTDETAILS.LIST"]?["PHONENUMBER"]?.ToString().Trim() ?? "NA";
					var emailId = stockItem["EMAIL"]?.ToString() ?? "NA";
					var contactperson = stockItem["LEDGERCONTACT"]?.ToString() ?? "NA"; // No email ID found in the given JSON structure
					var contactpersonno = stockItem["LEDGERPHONE"]?.ToString() ?? "NA"; // No email ID found in the given JSON structure
					var address = address1;
					var city = "NA";
					//var state = stockItem["LEDMAILINGDETAILS.LIST"]?["STATE"]?.ToString().Trim() ?? "NA";
					//var country = stockItem["LEDMAILINGDETAILS.LIST"]?["COUNTRY"]?.ToString().Trim() ?? "NA";
					//var pincode = stockItem["LEDMAILINGDETAILS.LIST"]?["PINCODE"]?.ToString().Trim() ?? "NA";
					string state = "NA";
					string country = "NA";
					string pincode = "NA";
					var mailingDetails = stockItem["LEDMAILINGDETAILS.LIST"];
					if (mailingDetails != null)
					{
						// If it's a single object
						if (mailingDetails.Type == JTokenType.Object)
						{
							state = mailingDetails["STATE"]?.ToString().Trim() ?? "NA";
							country = mailingDetails["COUNTRY"]?.ToString().Trim() ?? "NA";
							pincode = mailingDetails["PINCODE"]?.ToString().Trim() ?? "NA";
						}
						// If it's an array, use the first object
						else if (mailingDetails.Type == JTokenType.Array)
						{
							var firstItem = mailingDetails.FirstOrDefault();
							if (firstItem != null)
							{
								state = firstItem["STATE"]?.ToString().Trim() ?? "NA";
								country = firstItem["COUNTRY"]?.ToString().Trim() ?? "NA";
								pincode = firstItem["PINCODE"]?.ToString().Trim() ?? "NA";
							}
						}
					}

					if (gstNo != null)
					{
						if (gstNo.Contains("\u0004 Unknown"))
						{
							gstNo = "NA";
						}
					}
					else
					{
						gstNo = "NA";
					}

					// Create a new supplier record
					var voucher = new Ledger
					{
						name2 = "",
						GUID = "",
						type = "Sundry Creditors",
						name1 = supplierName.Trim() ?? "NA",
						gst = gstNo.Trim() ?? "NA",
						phoneno = contactNo.Trim() ?? "NA",
						contactpersonno = contactNo.Trim() ?? "NA",
						contactpersonemail = emailId.Trim() ?? "NA",
						address = address.Trim() ?? "NA",
						city = found.Trim() ?? "NA",
						state = state.Trim() ?? "NA",
						country = country ?? "NA",
						zipcode = pincode.Trim() ?? "NA",
						contactpersonname = contactperson.Trim() ?? "NA",
						creditlimit = (CREDITLIMIT).ToString(),
					};
					voucherList.Add(voucher);
				}
			}
			return voucherList;
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "An error occurred while communicating with Tally.");
			throw;
		}
	}
	public async Task<List<getVouchers>> GetVoucherTypeData(string xmlFilePath)
	{//Sundry Debtors
		try
		{
			// Validate Tally URL
			string tallyUrl = _configuration["TallySettings:TallyUrl"] ?? string.Empty;
			if (string.IsNullOrEmpty(tallyUrl))
			{
				throw new InvalidOperationException("Tally URL is not configured.");
			}

			// Validate XML file
			if (!File.Exists(xmlFilePath))
			{
				throw new FileNotFoundException("The specified XML file was not found.", xmlFilePath);
			}

			// Read the XML content from the file
			string xmlContent = await File.ReadAllTextAsync(xmlFilePath);

			string companyName = _configuration["TallySettings:CompanyName"] ?? string.Empty;
			if (string.IsNullOrEmpty(companyName))
			{
				throw new InvalidOperationException("Company Name is not configured.");
			}
			xmlContent = xmlContent.Replace("{Cname}", companyName);

			// Create HTTP request
			var request = new HttpRequestMessage(HttpMethod.Post, tallyUrl)
			{
				Content = new StringContent(xmlContent, Encoding.UTF8, "text/xml")
			};

			// Send request and get response
			var response = await _httpClient.SendAsync(request);
			response.EnsureSuccessStatusCode();

			var responseContent = await response.Content.ReadAsStringAsync();


			responseContent = RemoveInvalidCharacters(responseContent);

			var xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(responseContent);

			string jsonData = JsonConvert.SerializeXmlNode(xmlDocument, Newtonsoft.Json.Formatting.Indented);

			var jsonObject = JsonConvert.DeserializeObject<JObject>(jsonData);
			RemoveEmptyValues(jsonObject);
			var tallyMessageArray = jsonObject["ENVELOPE"]?["BODY"]?["DATA"]?["COLLECTION"]?["VOUCHERTYPE"];

			string tallyMessageJson = JsonConvert.SerializeObject(tallyMessageArray, Newtonsoft.Json.Formatting.Indented);
			var finaldata = JsonConvert.DeserializeObject<List<Dictionary<string, dynamic>>>(tallyMessageJson);

			var voucherList = new List<getVouchers>();

			if (finaldata != null)
			{
				foreach (var entry in finaldata)
				{
					var voucher = new getVouchers
					{
						vouchername = entry["@NAME"]?.ToString()?.Trim() ?? "NA",
						parent = entry["PARENT"]?.ToString()?.Trim() ?? "NA",
					};
					voucherList.Add(voucher);
				}
			}

			return voucherList;
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "An error occurred while communicating with Tally.");
			throw;
		}
	}
	public async Task<List<Ledger>> GetCustomerData(string xmlFilePath)
	{//Sundry Debtors
		try
		{
			// Validate Tally URL
			string tallyUrl = _configuration["TallySettings:TallyUrl"];
			if (string.IsNullOrWhiteSpace(tallyUrl))
			{
				throw new InvalidOperationException("Tally URL is not configured.");
			}

			// Validate XML file
			if (!File.Exists(xmlFilePath))
			{
				throw new FileNotFoundException("The specified XML file was not found.", xmlFilePath);
			}

			// Read the XML content from the file
			string xmlContent = await File.ReadAllTextAsync(xmlFilePath);

			string companyName = _configuration["TallySettings:CompanyName"]; // Replace with actual company name
			xmlContent = xmlContent.Replace("{Cname}", companyName);

			// Create HTTP request
			var request = new HttpRequestMessage(HttpMethod.Post, tallyUrl)
			{
				Content = new StringContent(xmlContent, Encoding.UTF8, "text/xml")
			};

			// Send request and get response
			var response = await _httpClient.SendAsync(request);
			response.EnsureSuccessStatusCode();

			var responseContent = await response.Content.ReadAsStringAsync();


			responseContent = RemoveInvalidCharacters(responseContent);


			// Unescape XML special characters
			//responseContent = WebUtility.HtmlDecode(responseContent);

			// Parse the cleaned XML response
			var xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(responseContent);

			// Convert XML to JSON
			string jsonData = JsonConvert.SerializeXmlNode(xmlDocument, Newtonsoft.Json.Formatting.Indented);

			// Deserialize JSON into a JObject for manipulation
			var jsonObject = JsonConvert.DeserializeObject<JObject>(jsonData);
			// Remove empty or invalid values recursively
			RemoveEmptyValues(jsonObject);
			// Navigate to the TALLYMESSAGE array
			var tallyMessageArray = jsonObject["ENVELOPE"]?["BODY"]?["DATA"]?["COLLECTION"]?["LEDGER"];

			// Convert the TALLYMESSAGE array to a formatted JSON string
			string tallyMessageJson = JsonConvert.SerializeObject(tallyMessageArray, Newtonsoft.Json.Formatting.Indented);
			var finaldata = JsonConvert.DeserializeObject<List<Dictionary<string, dynamic>>>(tallyMessageJson);

			var voucherList = new List<Ledger>();

			if (finaldata != null)
			{
				foreach (var entry in finaldata)
				{
					// Safely access "HSNDETAILS.LIST" as a dictionary or null
					var hsnData = entry.ContainsKey("ADDRESS.LIST") && entry["ADDRESS.LIST"] is JObject
								  ? (JObject)entry["ADDRESS.LIST"]
								  : null;

					// Safely access nested properties for alias
					string alias = "NA";
					if (entry.ContainsKey("LANGUAGENAME.LIST") && entry["LANGUAGENAME.LIST"] is JObject languageNameList &&
						languageNameList.ContainsKey("NAME.LIST") && languageNameList["NAME.LIST"] is JObject nameList &&
						nameList.ContainsKey("NAME") && nameList["NAME"] is JArray names && names.Count > 1)
					{
						alias = names[1]?.ToString() ?? "NA";
					}

					// Create the StockItem object
					var voucher = new Ledger
					{
						name1 = entry.ContainsKey("@NAME") ? entry["@NAME"]?.ToString()?.Trim() ?? "NA" : "NA",
						GUID = entry.ContainsKey("GUID") ? entry["GUID"]?.ToString()?.Trim() ?? "NA" : "NA",
						type = RemoveJunkCharacters(entry.ContainsKey("PARENT") ? entry["PARENT"]?.ToString()?.Trim() ?? "NA" : "NA"),
						phoneno = entry.ContainsKey("LEDGERMOBILE") ? entry["LEDGERMOBILE"]?.ToString()?.Trim() ?? "NA" : "NA",
						address = hsnData?.ContainsKey("ADDRESS") == true && hsnData["ADDRESS"] is JArray addressArray && addressArray.Count > 0 ? string.Join(", ", addressArray.Select(a => a?["#text"]?.ToString()?.Trim() ?? "NA")) : "NA",

						city = "NA",

						state = entry.ContainsKey("LEDMAILINGDETAILS.LIST") && entry["LEDMAILINGDETAILS.LIST"] is JObject stateObj ? stateObj["STATE"]?.ToString()?.Trim() ?? "NA" : "NA",

						zipcode = entry.ContainsKey("LEDMAILINGDETAILS.LIST") && entry["LEDMAILINGDETAILS.LIST"] is JObject zipObj ? zipObj["PINCODE"]?.ToString()?.Trim() ?? "NA" : "NA",

						country = entry.ContainsKey("LEDMAILINGDETAILS.LIST") && entry["LEDMAILINGDETAILS.LIST"] is JObject countryObj ? countryObj["COUNTRY"]?.ToString()?.Trim() ?? "NA" : "NA",

						contactpersonname = entry.ContainsKey("LEDGERCONTACT") ? entry["LEDGERCONTACT"]?.ToString()?.Trim() ?? "NA" : "NA",

						contactpersonno = entry.ContainsKey("LEDGERPHONE") ? entry["LEDGERPHONE"]?.ToString()?.Trim() ?? "NA" : "NA",

						contactpersonemail = entry.ContainsKey("EMAIL") ? entry["EMAIL"]?.ToString()?.Trim() ?? "NA" : "NA",

						gst = entry.ContainsKey("LEDGSTREGDETAILS.LIST") && entry["LEDGSTREGDETAILS.LIST"] is JObject gstObj ? gstObj["GSTIN"]?.ToString()?.Trim() ?? "NA" : "NA",
					};



					voucherList.Add(voucher);
				}
			}



			return voucherList;
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "An error occurred while communicating with Tally.");
			throw new Exception("An error occurred while communicating with Tally.", ex);
		}
	}
	public async Task<List<Ledger>> GetAllLedger(string xmlFilePath)
	{
		try
		{
			// Validate Tally URL
			string tallyUrl = _configuration["TallySettings:TallyUrl"] ?? string.Empty;
			if (string.IsNullOrWhiteSpace(tallyUrl))
			{
				throw new InvalidOperationException("Tally URL is not configured.");
			}

			// Validate XML file
			if (!File.Exists(xmlFilePath))
			{
				throw new FileNotFoundException("The specified XML file was not found.", xmlFilePath);
			}

			// Read the XML content from the file
			string xmlContent = await File.ReadAllTextAsync(xmlFilePath);

			string companyName = _configuration["TallySettings:CompanyName"] ?? string.Empty;

			if (string.IsNullOrWhiteSpace(companyName))
			{
				throw new InvalidOperationException("Company name is not configured.");
			}

			xmlContent = xmlContent.Replace("{Cname}", companyName);

			// Create HTTP request
			var request = new HttpRequestMessage(HttpMethod.Post, tallyUrl)
			{
				Content = new StringContent(xmlContent, Encoding.UTF8, "text/xml")
			};

			// Send request and get response
			var response = await _httpClient.SendAsync(request);
			response.EnsureSuccessStatusCode();

			var responseContent = await response.Content.ReadAsStringAsync();


			responseContent = RemoveInvalidCharacters(responseContent);

			// Unescape XML special characters
			//responseContent = WebUtility.HtmlDecode(responseContent);

			// Parse the cleaned XML response
			var xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(responseContent);

			// Convert XML to JSON
			string jsonData = JsonConvert.SerializeXmlNode(xmlDocument, Newtonsoft.Json.Formatting.Indented);

			// Deserialize JSON into a JObject for manipulation
			var jsonObject = JsonConvert.DeserializeObject<JObject>(jsonData);
			// Remove empty or invalid values recursively
			RemoveEmptyValues(jsonObject);
			// Navigate to the TALLYMESSAGE array
			var tallyMessageArray = jsonObject["ENVELOPE"]?["BODY"]?["DATA"]?["COLLECTION"]?["LEDGER"];

			// Convert the TALLYMESSAGE array to a formatted JSON string
			string tallyMessageJson = JsonConvert.SerializeObject(tallyMessageArray, Newtonsoft.Json.Formatting.Indented);
			var finaldata = JsonConvert.DeserializeObject<List<Dictionary<string, dynamic>>>(tallyMessageJson);

			var voucherList = new List<Ledger>();

			if (finaldata != null)
			{
				foreach (var entry in finaldata)
				{
					// Safely access "HSNDETAILS.LIST" as a dictionary or null
					var hsnData = entry.ContainsKey("ADDRESS.LIST") && entry["ADDRESS.LIST"] is JObject
								  ? (JObject)entry["ADDRESS.LIST"]
								  : null;

					// Safely access nested properties for alias
					string alias = "NA";
					if (entry.ContainsKey("LANGUAGENAME.LIST") && entry["LANGUAGENAME.LIST"] is JObject languageNameList &&
						languageNameList.ContainsKey("NAME.LIST") && languageNameList["NAME.LIST"] is JObject nameList &&
						nameList.ContainsKey("NAME") && nameList["NAME"] is JArray names && names.Count > 1)
					{
						alias = names[1]?.ToString() ?? "NA";
					}

					// Create the StockItem object
					var voucher = new Ledger
					{
						name1 = entry.ContainsKey("@NAME") ? entry["@NAME"]?.ToString() ?? "NA" : "NA",
						GUID = entry.ContainsKey("GUID") ? entry["GUID"]?.ToString() ?? "NA" : "NA",
						type = RemoveJunkCharacters(entry.ContainsKey("PARENT") ? entry["PARENT"]?.ToString() ?? "NA" : "NA"),
						phoneno = entry.ContainsKey("LEDGERMOBILE") ? entry["LEDGERMOBILE"]?.ToString() ?? "NA" : "NA",
						address = hsnData?.ContainsKey("ADDRESS") == true && hsnData["ADDRESS"] is JArray addressArray && addressArray.Count > 0
								  ? string.Join(", ", addressArray.Select(a => a["#text"]?.ToString() ?? "NA")) : "NA",
						//alias = alias,
					};

					voucherList.Add(voucher);
				}
			}



			return voucherList;
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "An error occurred while communicating with Tally.");
			throw new Exception("An error occurred while communicating with Tally.", ex);
		}
	}
	public async Task<List<Voucher>> GetVoucherAsync(string xmlFilePath)
	{
		try
		{
			// Validate Tally URL
			string tallyUrl = _configuration["TallySettings:TallyUrl"] ?? string.Empty;
			if (string.IsNullOrEmpty(tallyUrl))
			{
				throw new InvalidOperationException("Tally URL is not configured.");
			}

			// Validate XML file
			if (!File.Exists(xmlFilePath))
			{
				throw new FileNotFoundException("The specified XML file was not found.", xmlFilePath);
			}

			// Read the XML content from the file
			string xmlContent = await File.ReadAllTextAsync(xmlFilePath);
			string companyName = _configuration["TallySettings:CompanyName"] ?? string.Empty;
			if (string.IsNullOrEmpty(companyName))
			{
				throw new InvalidOperationException("Company name is not configured.");
			}

			xmlContent = xmlContent.Replace("{Cname}", companyName);
			xmlContent = xmlContent.Replace("{fromdate}", DateTime.UtcNow.AddDays(-10).ToString("yyyyMMdd"));
			xmlContent = xmlContent.Replace("{todate}", DateTime.UtcNow.ToString("yyyyMMdd"));

			// Create HTTP request
			var request = new HttpRequestMessage(HttpMethod.Post, tallyUrl)
			{
				Content = new StringContent(xmlContent, Encoding.UTF8, "text/xml")
			};

			// Send request and get response
			var response = await _httpClient.SendAsync(request);
			response.EnsureSuccessStatusCode();


			var responseContent = await response.Content.ReadAsStringAsync();

			// TallyXmlHelper.SaveXmlForDebugging(responseContent, "tally_response.xml");

			List<Voucher> voucherList = new List<Voucher>();
			var envelope = TallyXmlHelper.DeserializeTallyXml(responseContent);

			if (envelope != null &&
				envelope.Body?.ImportData?.RequestData?.TallyMessages != null)
			{
				foreach (var tallyMessage in envelope.Body.ImportData.RequestData.TallyMessages)
				{
					if (tallyMessage.Voucher != null)
					{
						voucherList.Add(tallyMessage.Voucher);
					}
				}
			}

			return voucherList;

		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "An error occurred while communicating with Tally.");
			throw ex ?? new InvalidOperationException("An error occurred while communicating with Tally.");
		}
	}

	public static string RemoveInvalidCharacters(string xmlContent)
	{
		if (string.IsNullOrEmpty(xmlContent))
			return xmlContent;

		// Remove invalid XML characters (control characters except tab, newline, carriage return)
		return Regex.Replace(xmlContent, @"[\x00-\x08\x0B\x0C\x0E-\x1F]", "");

	}
	public string RemoveJunkCharacters(string input)
	{
		// Remove all control characters except tab, newline, and carriage return
		string pattern = @"[\u0000-\u001F]"; // Matches all control characters (Unicode 0-31)
		return Regex.Replace(input, pattern, string.Empty).Trim();
	}
	void RemoveEmptyValues(JToken token)
	{
		if (token.Type == JTokenType.Object)
		{
			var properties = token.Children<JProperty>().ToList();
			foreach (var prop in properties)
			{
				if (prop.Value.Type == JTokenType.Null ||
					(prop.Value.Type == JTokenType.String && string.IsNullOrWhiteSpace(prop.Value.ToString())))
				{
					prop.Remove(); // Remove null or empty string values
				}
				else
				{
					RemoveEmptyValues(prop.Value); // Recursively clean nested objects or arrays
				}
			}
		}
		else if (token.Type == JTokenType.Array)
		{
			var items = token.Children().ToList();
			foreach (var item in items)
			{
				RemoveEmptyValues(item);
			}
		}
	}
	public async Task<List<string>> GetInvoiceNo(string fileName, string vouchername)
	{
		try
		{
			string xmlContent = "";
			xmlContent = System.IO.File.ReadAllText(fileName);
			string companyName = _configuration["TallySettings:CompanyName"] ?? string.Empty;
			if (string.IsNullOrEmpty(companyName))
			{
				throw new InvalidOperationException("Company name is not configured.");
			}
			xmlContent = xmlContent.Replace("{Cname}", companyName);
			xmlContent = xmlContent.Replace("{BIND_VOUCHER_NAME}", vouchername);
			string tallyUrl = _configuration["TallySettings:TallyUrl"] ?? string.Empty;
			if (string.IsNullOrWhiteSpace(tallyUrl))
			{
				throw new InvalidOperationException("Tally URL is not configured.");
			}
			var request = new HttpRequestMessage(HttpMethod.Post, tallyUrl)
			{
				Content = new StringContent(xmlContent, Encoding.UTF8, "text/xml")
			};
			var response = await _httpClient.SendAsync(request);
			response.EnsureSuccessStatusCode();

			var responseContent = await response.Content.ReadAsStringAsync();

			//ADDED
			responseContent = RemoveInvalidCharacters(responseContent);
			XmlDocument xmlDocument = new XmlDocument();
			xmlDocument.LoadXml(responseContent);
			string jsonData = JsonConvert.SerializeXmlNode(xmlDocument, Newtonsoft.Json.Formatting.Indented);
			var jsonObject = JsonConvert.DeserializeObject<JObject>(jsonData);
			RemoveEmptyValues(jsonObject);
			string cleanedJsonData = JsonConvert.SerializeObject(jsonObject, Newtonsoft.Json.Formatting.Indented);
			var data = JObject.Parse(cleanedJsonData);
			var groupNames = new List<string>();
			var tallyMessage = data["ENVELOPE"]?["BODY"]?["DATA"]?["TALLYMESSAGE"];
			if (tallyMessage is JArray tallyArray)
			{
				// Multiple TALLYMESSAGE
				foreach (var item in tallyArray)
				{
					var voucher = item["VOUCHER"];
					var vouchernumber = voucher?["VOUCHERNUMBER"]?.ToString();
					if (!string.IsNullOrEmpty(vouchernumber))
					{
						groupNames.Add(vouchernumber);
					}
				}
			}
			else if (tallyMessage is JObject tallyObject)
			{
				// Single TALLYMESSAGE
				var voucher = tallyObject["VOUCHER"];
				var vouchernumber = voucher?["VOUCHERNUMBER"]?.ToString();
				if (!string.IsNullOrEmpty(vouchernumber))
				{
					groupNames.Add(vouchernumber);
				}
			}
			return groupNames;
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "An error occurred while communicating with Tally.");
			throw;
		}
	}
	public async Task<List<string>> GetCustomers(string fileName)
	{
		try
		{
			string xmlContent = "";
			xmlContent = System.IO.File.ReadAllText(fileName);

			string companyName = _configuration["TallySettings:CompanyName"] ?? string.Empty;
			if (string.IsNullOrEmpty(companyName))
			{
				throw new InvalidOperationException("Company name is not configured.");
			}
			xmlContent = xmlContent.Replace("{Cname}", companyName);

			// Validate Tally URL
			string tallyUrl = _configuration["TallySettings:TallyUrl"] ?? string.Empty;

			if (string.IsNullOrWhiteSpace(tallyUrl))
			{
				throw new InvalidOperationException("Tally URL is not configured.");
			}
			// Create HTTP request
			var request = new HttpRequestMessage(HttpMethod.Post, tallyUrl)
			{
				Content = new StringContent(xmlContent, Encoding.UTF8, "text/xml")
			};

			// Send request and get response
			var response = await _httpClient.SendAsync(request);
			response.EnsureSuccessStatusCode();

			var responseContent = await response.Content.ReadAsStringAsync();

			var groupNames = new List<string>();

			string[] strArrayOne = responseContent.Split('>');

			foreach (string tag in strArrayOne)
			{
				if (tag.Contains("LEDGER NAME"))
				{
					string data = tag.Trim();

					var regex = new Regex(@"NAME=""([^""]+)""");
					foreach (Match match in regex.Matches(data))
					{
						groupNames.Add(match.Groups[1].Value.Trim());
						//groupNames.Add(match.Groups[1].Value.ToUpper());
					}
				}
			}

			return groupNames;

		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "An error occurred while communicating with Tally.");
			throw;
		}
	}
	public async Task<List<string>> GetSuppliers(string fileName)
	{
		try
		{

			string xmlContent = "";
			xmlContent = System.IO.File.ReadAllText(fileName);

			string companyName = _configuration["TallySettings:CompanyName"] ?? string.Empty;
			if (string.IsNullOrEmpty(companyName))
			{
				throw new InvalidOperationException("Company name is not configured.");
			}
			xmlContent = xmlContent.Replace("{Cname}", companyName);

			// Validate Tally URL
			string tallyUrl = _configuration["TallySettings:TallyUrl"] ?? string.Empty;
			if (string.IsNullOrWhiteSpace(tallyUrl))
			{
				throw new InvalidOperationException("Tally URL is not configured.");
			}
			// Create HTTP request
			var request = new HttpRequestMessage(HttpMethod.Post, tallyUrl)
			{
				Content = new StringContent(xmlContent, Encoding.UTF8, "text/xml")
			};

			// Send request and get response
			var response = await _httpClient.SendAsync(request);
			response.EnsureSuccessStatusCode();

			var responseContent = await response.Content.ReadAsStringAsync();

			var groupNames = new List<string>();

			string[] strArrayOne = responseContent.Split('>');

			foreach (string tag in strArrayOne)
			{
				if (tag.Contains("LEDGER NAME"))
				{
					string data = tag.Trim();

					var regex = new Regex(@"NAME=""([^""]+)""");
					foreach (Match match in regex.Matches(data))
					{
						groupNames.Add(match.Groups[1].Value.Trim());
						//groupNames.Add(match.Groups[1].Value.ToUpper());
					}
				}
			}

			return groupNames;

		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "An error occurred while communicating with Tally.");
			throw;
		}
	}
}
