using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Text;
using System.Xml;
using TallyERPWebApi.Model;
using System.Net;
using System.Diagnostics.Metrics;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Components.Forms;
using System.Globalization;

namespace TallyERPWebApi.Service
{
    public class PostTallyService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<PostTallyService> _logger;

        public PostTallyService(HttpClient httpClient, IConfiguration configuration, ILogger<PostTallyService> logger)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<string> SaveUom(string xmlFilePath, string uom)
        {
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

                string modifiedXmlContent = xmlContent.Replace("<unitname>", uom.Trim());
                //string modifiedXmlContent = xmlContent.Replace("<unitname>", uom.ToUpper().Trim());

                // Create HTTP request
                var request = new HttpRequestMessage(HttpMethod.Post, tallyUrl)
                {
                    Content = new StringContent(modifiedXmlContent, Encoding.UTF8, "text/xml")
                };

                // Send request and get response
                var response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();

                return responseContent;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while communicating with Tally.");
                throw;
            }
        }

        public async Task<string> SaveStockGroup(string xmlFilePath, string stockgroup)
        {
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

                string modifiedXmlContent = xmlContent.Replace("<newstockgroup>", stockgroup.Trim());
                //string modifiedXmlContent = xmlContent.Replace("<newstockgroup>", stockgroup.ToUpper().Trim());

                // Create HTTP request
                var request = new HttpRequestMessage(HttpMethod.Post, tallyUrl)
                {
                    Content = new StringContent(modifiedXmlContent, Encoding.UTF8, "text/xml")
                };

                // Send request and get response
                var response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();

                return responseContent;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while communicating with Tally.");
                throw;
            }
        }

        public async Task<string> SaveStockItem(string xmlFilePath, StockItem stockItem)
        {
            try
            {

                //if (stockItem.name.Trim() == "ATC00590")
                //{
                //    _logger.LogError("An error occurred while communicating with Tally.");
                //}
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

                string xmlContent = await File.ReadAllTextAsync(xmlFilePath);

                string companyName = _configuration["TallySettings:CompanyName"]; // Replace with actual company name               

                string modifiedXmlContent = xmlContent.Trim()
                                .Replace("{Cname}", companyName)
                                .Replace("{BIND_PRODUCT1}", stockItem.alias.Replace("&", "&amp;"))
                                .Replace("{BIND_UNIT}", stockItem.unit.Trim())
                                .Replace("{BIND_CATEGORY}", stockItem.category.Trim())
                                .Replace("{BIND_BALANCE}", stockItem.openingqnty.ToString())
                                .Replace("{BIND_PRODUCT2}", ConvertToHtmlEntities(stockItem.name.Trim()))
                                .Replace("{BIND_CGST}", stockItem.cgst)
                                .Replace("{BIND_SGST}", stockItem.sgst)
                                .Replace("{BIND_IGST}", stockItem.igst)
                                .Replace("{BIND_HSN}", stockItem.hsncode);
                                

               // string safeXml = EscapeAmpersands(modifiedXmlContent);


                // Create HTTP request
                var request = new HttpRequestMessage(HttpMethod.Post, tallyUrl)
                {
                    Content = new StringContent(modifiedXmlContent, Encoding.UTF8, "text/xml")
                };

                // Send request and get response
                var response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();

                return responseContent;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while communicating with Tally.");
                throw;
            }
        }

        public static string ConvertToHtmlEntities(string input)
        {
            return WebUtility.HtmlEncode(input);
        }

        public async Task<string> SaveLedeger(string xmlFilePath, Ledger ledger)
        {
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

                string xmlContent = await File.ReadAllTextAsync(xmlFilePath);

                string companyName = _configuration["TallySettings:CompanyName"]; // Replace with actual company name    

                string modifiedXmlContent = xmlContent.Trim()
                                .Replace("{Cname}", companyName)
                                .Replace("<name1>", ledger.name1.Trim())
                                .Replace("<name2>", ledger.name2.Trim())
                                .Replace("<parent>", ledger.type.ToUpper().Trim())
                                .Replace("<address>", ledger.address.Trim())
                                .Replace("<city>", ledger.city.ToUpper().Trim())
                                .Replace("<country>", ledger.country.ToString())
                                .Replace("<state>", ledger.state.ToString())
                                .Replace("<phoneno>", ledger.phoneno)
                                .Replace("<pincode>", ledger.zipcode)
                                .Replace("<contactpersonno>", ledger.contactpersonno)
                                .Replace("{BIND_CONTACTPERSON_NAME}", ledger.contactpersonname)
                                .Replace("{BIND_CONTACTPERSON_EMAIL}", ledger.contactpersonemail)
                                .Replace("{BIND_CONTACTPERSON_PHONENO}", ledger.contactpersonno)
                                .Replace("{BIND_CREDITLIMIT}", ledger.creditlimit)
                                //var contactperson = stockItem["LEDGERCONTACT"]?.ToString() ?? "NA"; // No email ID found in the given JSON structure
                                //var contactpersonno = stockItem["LEDGERPHONE"]?.ToString() ?? "NA"; // No email ID found in the given JSON structure
                                .Replace("<gst>", ledger.gst)
                                .Replace("&", "&amp;");

                string safeXml = EscapeAmpersands(modifiedXmlContent);


                // Create HTTP request
                var request = new HttpRequestMessage(HttpMethod.Post, tallyUrl)
                {
                    Content = new StringContent(modifiedXmlContent, Encoding.UTF8, "text/xml")
                };

                // Send request and get response
                var response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();

                return responseContent;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while communicating with Tally.");
                throw;
            }
        }

        public async Task<string> SaveVoucher(string xmlFilePath, VoucherSO voucher)
        {
            try
            {
                // Validate Tally URL
                string tallyUrl = _configuration["TallySettings:TallyUrl"];

                string companyName = _configuration["TallySettings:CompanyName"]; // Replace with actual company name    

                if (string.IsNullOrWhiteSpace(tallyUrl))
                {
                    throw new InvalidOperationException("Tally URL is not configured.");
                }

                // Validate XML file
                if (!File.Exists(xmlFilePath))
                {
                    throw new FileNotFoundException("The specified XML file was not found.", xmlFilePath);
                }

                string xmlContent = await File.ReadAllTextAsync(xmlFilePath);
                string addmoreitem = null;

                foreach (var item in voucher.Items)
                {
                    string itemxml = " <ALLINVENTORYENTRIES.LIST>\r\n       <STOCKITEMNAME><itemname></STOCKITEMNAME>\r\n       <GSTOVRDNINELIGIBLEITC>&#4; Not Applicable</GSTOVRDNINELIGIBLEITC>\r\n       <GSTOVRDNISREVCHARGEAPPL>&#4; Not Applicable</GSTOVRDNISREVCHARGEAPPL>\r\n       <GSTOVRDNTAXABILITY>Taxable</GSTOVRDNTAXABILITY>\r\n       <GSTSOURCETYPE>Company</GSTSOURCETYPE>\r\n       <HSNSOURCETYPE>Company</HSNSOURCETYPE>\r\n       <GSTOVRDNSTOREDNATURE/>\r\n       <GSTOVRDNTYPEOFSUPPLY>Goods</GSTOVRDNTYPEOFSUPPLY>\r\n       <GSTRATEINFERAPPLICABILITY>As per Masters/Company</GSTRATEINFERAPPLICABILITY>\r\n       <GSTHSNNAME>12345678</GSTHSNNAME>\r\n       <GSTHSNDESCRIPTION>GST DETAILS</GSTHSNDESCRIPTION>\r\n       <GSTHSNINFERAPPLICABILITY>As per Masters/Company</GSTHSNINFERAPPLICABILITY>\r\n       <ISDEEMEDPOSITIVE>Yes</ISDEEMEDPOSITIVE>\r\n       <ISGSTASSESSABLEVALUEOVERRIDDEN>No</ISGSTASSESSABLEVALUEOVERRIDDEN>\r\n       <STRDISGSTAPPLICABLE>No</STRDISGSTAPPLICABLE>\r\n       <CONTENTNEGISPOS>No</CONTENTNEGISPOS>\r\n       <ISLASTDEEMEDPOSITIVE>Yes</ISLASTDEEMEDPOSITIVE>\r\n       <ISAUTONEGATE>No</ISAUTONEGATE>\r\n       <ISCUSTOMSCLEARANCE>No</ISCUSTOMSCLEARANCE>\r\n       <ISTRACKCOMPONENT>No</ISTRACKCOMPONENT>\r\n       <ISTRACKPRODUCTION>No</ISTRACKPRODUCTION>\r\n       <ISPRIMARYITEM>No</ISPRIMARYITEM>\r\n       <ISSCRAP>No</ISSCRAP>\r\n       <RATE><itemrate></RATE>\r\n       <AMOUNT><itemsum></AMOUNT>\r\n       <ACTUALQTY> <itemqnty></ACTUALQTY>\r\n       <BILLEDQTY> <itemqnty></BILLEDQTY>\r\n       <BATCHALLOCATIONS.LIST>\r\n        <GODOWNNAME>WAREHOUSE 1</GODOWNNAME>\r\n        <BATCHNAME>Primary Batch</BATCHNAME>\r\n        <DESTINATIONGODOWNNAME>WAREHOUSE 1</DESTINATIONGODOWNNAME>\r\n        <INDENTNO>&#4; Not Applicable</INDENTNO>\r\n        <ORDERNO><ordernumber></ORDERNO>\r\n        <TRACKINGNUMBER>&#4; Not Applicable</TRACKINGNUMBER>\r\n        <DYNAMICCSTISCLEARED>No</DYNAMICCSTISCLEARED>\r\n        <AMOUNT><itemsum></AMOUNT>\r\n        <ACTUALQTY> <itemqnty></ACTUALQTY>\r\n        <BILLEDQTY> <itemqnty></BILLEDQTY>\r\n        <ORDERDUEDATE JD=\"45382\" P=\"{Currentdate}\">{Currentdate}</ORDERDUEDATE>\r\n        <ADDITIONALDETAILS.LIST>        </ADDITIONALDETAILS.LIST>\r\n        <VOUCHERCOMPONENTLIST.LIST>        </VOUCHERCOMPONENTLIST.LIST>\r\n       </BATCHALLOCATIONS.LIST>\r\n       <ACCOUNTINGALLOCATIONS.LIST>\r\n        <OLDAUDITENTRYIDS.LIST TYPE=\"Number\">\r\n         <OLDAUDITENTRYIDS>-1</OLDAUDITENTRYIDS>\r\n        </OLDAUDITENTRYIDS.LIST>\r\n        <LEDGERNAME><accoutgroup></LEDGERNAME>\r\n        <GSTCLASS>&#4; Not Applicable</GSTCLASS>\r\n        <ISDEEMEDPOSITIVE>Yes</ISDEEMEDPOSITIVE>\r\n        <LEDGERFROMITEM>No</LEDGERFROMITEM>\r\n        <REMOVEZEROENTRIES>No</REMOVEZEROENTRIES>\r\n        <ISPARTYLEDGER>No</ISPARTYLEDGER>\r\n        <GSTOVERRIDDEN>No</GSTOVERRIDDEN>\r\n        <ISGSTASSESSABLEVALUEOVERRIDDEN>No</ISGSTASSESSABLEVALUEOVERRIDDEN>\r\n        <STRDISGSTAPPLICABLE>No</STRDISGSTAPPLICABLE>\r\n        <STRDGSTISPARTYLEDGER>No</STRDGSTISPARTYLEDGER>\r\n        <STRDGSTISDUTYLEDGER>No</STRDGSTISDUTYLEDGER>\r\n        <CONTENTNEGISPOS>No</CONTENTNEGISPOS>\r\n        <ISLASTDEEMEDPOSITIVE>Yes</ISLASTDEEMEDPOSITIVE>\r\n        <ISCAPVATTAXALTERED>No</ISCAPVATTAXALTERED>\r\n        <ISCAPVATNOTCLAIMED>No</ISCAPVATNOTCLAIMED>\r\n        <AMOUNT><itemsum></AMOUNT>\r\n        <SERVICETAXDETAILS.LIST>        </SERVICETAXDETAILS.LIST>\r\n        <BANKALLOCATIONS.LIST>        </BANKALLOCATIONS.LIST>\r\n        <BILLALLOCATIONS.LIST>        </BILLALLOCATIONS.LIST>\r\n        <INTERESTCOLLECTION.LIST>        </INTERESTCOLLECTION.LIST>\r\n        <OLDAUDITENTRIES.LIST>        </OLDAUDITENTRIES.LIST>\r\n        <ACCOUNTAUDITENTRIES.LIST>        </ACCOUNTAUDITENTRIES.LIST>\r\n        <AUDITENTRIES.LIST>        </AUDITENTRIES.LIST>\r\n        <INPUTCRALLOCS.LIST>        </INPUTCRALLOCS.LIST>\r\n        <DUTYHEADDETAILS.LIST>        </DUTYHEADDETAILS.LIST>\r\n        <EXCISEDUTYHEADDETAILS.LIST>        </EXCISEDUTYHEADDETAILS.LIST>\r\n        <RATEDETAILS.LIST>        </RATEDETAILS.LIST>\r\n        <SUMMARYALLOCS.LIST>        </SUMMARYALLOCS.LIST>\r\n        <CENVATDUTYALLOCATIONS.LIST>        </CENVATDUTYALLOCATIONS.LIST>\r\n        <STPYMTDETAILS.LIST>        </STPYMTDETAILS.LIST>\r\n        <EXCISEPAYMENTALLOCATIONS.LIST>        </EXCISEPAYMENTALLOCATIONS.LIST>\r\n        <TAXBILLALLOCATIONS.LIST>        </TAXBILLALLOCATIONS.LIST>\r\n        <TAXOBJECTALLOCATIONS.LIST>        </TAXOBJECTALLOCATIONS.LIST>\r\n        <TDSEXPENSEALLOCATIONS.LIST>        </TDSEXPENSEALLOCATIONS.LIST>\r\n        <VATSTATUTORYDETAILS.LIST>        </VATSTATUTORYDETAILS.LIST>\r\n        <COSTTRACKALLOCATIONS.LIST>        </COSTTRACKALLOCATIONS.LIST>\r\n        <REFVOUCHERDETAILS.LIST>        </REFVOUCHERDETAILS.LIST>\r\n        <INVOICEWISEDETAILS.LIST>        </INVOICEWISEDETAILS.LIST>\r\n        <VATITCDETAILS.LIST>        </VATITCDETAILS.LIST>\r\n        <ADVANCETAXDETAILS.LIST>        </ADVANCETAXDETAILS.LIST>\r\n        <TAXTYPEALLOCATIONS.LIST>        </TAXTYPEALLOCATIONS.LIST>\r\n       </ACCOUNTINGALLOCATIONS.LIST>\r\n       <DUTYHEADDETAILS.LIST>       </DUTYHEADDETAILS.LIST>\r\n       <RATEDETAILS.LIST>\r\n        <GSTRATEDUTYHEAD>CGST</GSTRATEDUTYHEAD>\r\n        <GSTRATEVALUATIONTYPE>Based on Value</GSTRATEVALUATIONTYPE>\r\n        <GSTRATE> 9</GSTRATE>\r\n       </RATEDETAILS.LIST>\r\n       <RATEDETAILS.LIST>\r\n        <GSTRATEDUTYHEAD>SGST/UTGST</GSTRATEDUTYHEAD>\r\n        <GSTRATEVALUATIONTYPE>Based on Value</GSTRATEVALUATIONTYPE>\r\n        <GSTRATE> 9</GSTRATE>\r\n       </RATEDETAILS.LIST>\r\n       <RATEDETAILS.LIST>\r\n        <GSTRATEDUTYHEAD>IGST</GSTRATEDUTYHEAD>\r\n        <GSTRATEVALUATIONTYPE>Based on Value</GSTRATEVALUATIONTYPE>\r\n        <GSTRATE> 18</GSTRATE>\r\n       </RATEDETAILS.LIST>\r\n       <RATEDETAILS.LIST>\r\n        <GSTRATEDUTYHEAD>Cess</GSTRATEDUTYHEAD>\r\n        <GSTRATEVALUATIONTYPE>&#4; Not Applicable</GSTRATEVALUATIONTYPE>\r\n       </RATEDETAILS.LIST>\r\n       <RATEDETAILS.LIST>\r\n        <GSTRATEDUTYHEAD>State Cess</GSTRATEDUTYHEAD>\r\n        <GSTRATEVALUATIONTYPE>Based on Value</GSTRATEVALUATIONTYPE>\r\n       </RATEDETAILS.LIST>\r\n       <SUPPLEMENTARYDUTYHEADDETAILS.LIST>       </SUPPLEMENTARYDUTYHEADDETAILS.LIST>\r\n       <TAXOBJECTALLOCATIONS.LIST>       </TAXOBJECTALLOCATIONS.LIST>\r\n       <REFVOUCHERDETAILS.LIST>       </REFVOUCHERDETAILS.LIST>\r\n       <EXCISEALLOCATIONS.LIST>       </EXCISEALLOCATIONS.LIST>\r\n       <EXPENSEALLOCATIONS.LIST>       </EXPENSEALLOCATIONS.LIST>\r\n      </ALLINVENTORYENTRIES.LIST>";

                    if (item != null)
                    {
                        string modifiedadditem = itemxml.Trim()

                              .Replace("<itemname>", item.StockItemName.ToUpper().Trim())
                              .Replace("<itemqnty>", item.ActualQty)
                              .Replace("<itemsum>", item.Amount)
                              .Replace("<ordernumber>", "ORDER" + DateTime.Now.ToString("HH:mm:ss").Replace("-", "").Replace("/", "").Replace(":", ""))
                              .Replace("<itemrate>", item.Rate)
                              .Replace("<trackingno>", "TRACK" + DateTime.Now.ToString("HH:mm:ss").Replace("-", "").Replace("/", "").Replace(":", ""))
                              .Replace("<accoutgroup>", voucher.AccountType);


                        addmoreitem += modifiedadditem;

                    }
                }

                string modifiedXmlContent = xmlContent.Trim()
                            .Replace("{Cname}", companyName)
                            .Replace("<vouchertype>", voucher.VoucherType.ToUpper().Trim())
                            .Replace("<date>", voucher.Date.Replace("-", ""))
                            .Replace("<customername>", voucher.PartyName.ToUpper().Trim())
                            .Replace("<documnetno>", "DOC" + DateTime.Now.ToString("HH:mm:ss").Replace("-", "").Replace("/", "").Replace(":", ""))
                            .Replace("<ordernumber>", "ORDER" + DateTime.Now.ToString("HH:mm:ss").Replace("-", "").Replace("/", "").Replace(":", ""))
                            .Replace("<vehicleno>", "MH01XX1234")
                            .Replace("<trackingno>", "TRACK" + DateTime.Now.ToString("HH:mm:ss").Replace("-", "").Replace("/", "").Replace(":", ""))
                            .Replace("<overallamunt>", voucher.overallamount)
                            .Replace("<additem>", addmoreitem)
                            .Replace("&", "&amp;");

                string safeXml = EscapeAmpersands(modifiedXmlContent);

                // Create HTTP request
                var request = new HttpRequestMessage(HttpMethod.Post, tallyUrl)
                {
                    Content = new StringContent(modifiedXmlContent, Encoding.UTF8, "text/xml")
                };

                // Send request and get response
                var response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();

                return responseContent;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while communicating with Tally.");
                throw;
            }
        }

        public async Task<string> SavePO_Order(string xmlFilePath, POorderVoucher voucher)
        {
            try
            {
                DateTime parsedDate = DateTime.ParseExact(voucher.podate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                voucher.podate = parsedDate.ToString("yyyy-MM-dd");

                string tallyUrl = _configuration["TallySettings:TallyUrl"];
                if (string.IsNullOrWhiteSpace(tallyUrl))
                {
                    throw new InvalidOperationException("Tally URL is not configured.");
                }
                if (!File.Exists(xmlFilePath))
                {
                    throw new FileNotFoundException("The specified XML file was not found.", xmlFilePath);
                }
                string xmlContent = await File.ReadAllTextAsync(xmlFilePath);

                string companyName = _configuration["TallySettings:CompanyName"]; // Replace with actual company name    

                string addmoreitem = null;
                //ADDED
                var BIND_VOUCHERTYPE_LEDGER = voucher.EntryType;
                var BIND_VOUCHERTYPE = "Purchase Order";

                string address = voucher.address.Trim();
                int maxLength = 100;
                List<string> addressChunks = new List<string>();
                for (int i = 0; i < address.Length; i += maxLength)
                {
                    addressChunks.Add(address.Substring(i, Math.Min(maxLength, address.Length - i)).Trim());
                }
                var purchase = voucher;
                xmlContent = xmlContent.Replace("<BIND_PARTYNAME>", purchase.suppliername.Replace("&", "&amp;"))
                                              .Replace("{Cname}", companyName.Replace("&", "&amp;"))
                                              .Replace("<BIND_ADD1>", addressChunks.Count > 0 ? addressChunks[0] : "")
                                              .Replace("<BIND_ADD2>", addressChunks.Count > 1 ? addressChunks[1] : "")
                                              .Replace("<BIND_ADD3>", addressChunks.Count > 2 ? addressChunks[2] : "")
                                              .Replace("<BIND_ADD4>", addressChunks.Count > 3 ? addressChunks[3] : "")
                                              .Replace("<BIND_REF>", purchase.pono.Replace("&", "&amp;"))
                                              .Replace("<BIND_GSTNO>", voucher.gstinno.Replace("&", "&amp;"))
                                              .Replace("<BIND_VOUCHERTYPE>", BIND_VOUCHERTYPE.Replace("&", "&amp;"))
                                              .Replace("<BIND_VOUCHERTYPE_LEDGER>", BIND_VOUCHERTYPE_LEDGER.Replace("&", "&amp;"))
                                              .Replace("<BIND_COUNTRY>", voucher.country.Replace("&", "&amp;"))
                                              .Replace("<BIND_STATE>", voucher.state.Replace("&", "&amp;"))
                                              .Replace("<BIND_CON_COUNTRY>", "")
                                              .Replace("<BIND_CON_STATE>", "")
                                              .Replace("<BIND_CON_PINCODE>", voucher.pincode.Replace("&", "&amp;"))
                                              .Replace("<BIND_CONSIGNEEGST>", "")
                                              .Replace("<BIND_COURIER>", "NA")
                                              .Replace("<BIND_DC_DATE>", "")
                                              .Replace("<BIND_DC_NO>", "")
                                              .Replace("<BIND_TOP>", "0")
                                              .Replace("<BIND_TRUCKNO>", "NA")
                                              .Replace("<BIND_BASEAMOUNT>", (purchase.BaseAmount).ToString())
                                              .Replace("<BIND_TOTALAMOUNT>", (purchase.FinalAmount).ToString())
                                              .Replace("<BIND_DISPATCHEDBY>", "NA")
                                              //.Replace("{Currentdate}", DateTime.Now.ToString("yyyyMMdd"));
                                              .Replace("{Currentdate}", voucher.podate.Replace("-", ""));
                StringBuilder productEntries = new StringBuilder();
                StringBuilder LEDFEREntries = new StringBuilder();
                string productTemplate = @"<ALLINVENTORYENTRIES.LIST>
                                                   <STOCKITEMNAME><DESCRIPTION></STOCKITEMNAME>
                                                   <GSTOVRDNINELIGIBLEITC>&#4; Not Applicable</GSTOVRDNINELIGIBLEITC>
                                                   <GSTOVRDNISREVCHARGEAPPL>&#4; Not Applicable</GSTOVRDNISREVCHARGEAPPL>
                                                   <GSTOVRDNTAXABILITY>Taxable</GSTOVRDNTAXABILITY>
                                                   <GSTSOURCETYPE>Stock Item</GSTSOURCETYPE>
                                                   <GSTITEMSOURCE><DESCRIPTION></GSTITEMSOURCE>
                                                   <HSNSOURCETYPE>Company</HSNSOURCETYPE>
                                                   <GSTOVRDNTYPEOFSUPPLY>Goods</GSTOVRDNTYPEOFSUPPLY>
                                                   <GSTRATEINFERAPPLICABILITY>As per Masters/Company</GSTRATEINFERAPPLICABILITY>
                                                   <GSTHSNNAME><BIND_HSN></GSTHSNNAME>
                                                   <GSTHSNDESCRIPTION><DESCRIPTION></GSTHSNDESCRIPTION>
                                                   <GSTHSNINFERAPPLICABILITY>As per Masters/Company</GSTHSNINFERAPPLICABILITY>
                                           <ISDEEMEDPOSITIVE>Yes</ISDEEMEDPOSITIVE>
                                                   <ISGSTASSESSABLEVALUEOVERRIDDEN>No</ISGSTASSESSABLEVALUEOVERRIDDEN>
                                                   <STRDISGSTAPPLICABLE>No</STRDISGSTAPPLICABLE>
                                                   <CONTENTNEGISPOS>No</CONTENTNEGISPOS>
                                           <ISLASTDEEMEDPOSITIVE>Yes</ISLASTDEEMEDPOSITIVE>
                                                   <ISAUTONEGATE>No</ISAUTONEGATE>
                                                   <ISCUSTOMSCLEARANCE>No</ISCUSTOMSCLEARANCE>
                                                   <ISTRACKCOMPONENT>No</ISTRACKCOMPONENT>
                                                   <ISTRACKPRODUCTION>No</ISTRACKPRODUCTION>
                                                   <ISPRIMARYITEM>No</ISPRIMARYITEM>
                                                   <ISSCRAP>No</ISSCRAP>
                                                   <RATE><BIND_RATE></RATE>
                                                   <AMOUNT><BIND_AMOUNT></AMOUNT>
                                                   <ACTUALQTY><BIND_QUANTITY></ACTUALQTY>
                                                   <BILLEDQTY><BIND_QUANTITY></BILLEDQTY>
                                                   <BATCHALLOCATIONS.LIST>
                                                    <GODOWNNAME>Main Location</GODOWNNAME>
                                                    <BATCHNAME>Primary Batch</BATCHNAME>
                                                    <INDENTNO>&#4; Not Applicable</INDENTNO>
                                                    <ORDERNO><BIND_REF></ORDERNO>
                                                    <TRACKINGNUMBER>&#4; Not Applicable</TRACKINGNUMBER>
                                                    <DYNAMICCSTISCLEARED>No</DYNAMICCSTISCLEARED>
                                                    <AMOUNT><BIND_AMOUNT></AMOUNT>
                                                    <ACTUALQTY><BIND_QUANTITY></ACTUALQTY>
                                                    <BILLEDQTY><BIND_QUANTITY></BILLEDQTY>
                                                    <ORDERDUEDATE JD=""45382"" P=""{Currentdate}"">{Currentdate}</ORDERDUEDATE>
                                                    <ADDITIONALDETAILS.LIST>        </ADDITIONALDETAILS.LIST>
                                                    <VOUCHERCOMPONENTLIST.LIST>        </VOUCHERCOMPONENTLIST.LIST>
                                                   </BATCHALLOCATIONS.LIST>
                                                   <ACCOUNTINGALLOCATIONS.LIST>
                                                    <OLDAUDITENTRYIDS.LIST TYPE=""Number"">
                                                     <OLDAUDITENTRYIDS>-1</OLDAUDITENTRYIDS>
                                                    </OLDAUDITENTRYIDS.LIST>
                                                    <LEDGERNAME><BIND_VOUCHERTYPE_LEDGER></LEDGERNAME>
                                                    <GSTCLASS>&#4; Not Applicable</GSTCLASS>
                                          <ISDEEMEDPOSITIVE>Yes</ISDEEMEDPOSITIVE>
                                                    <LEDGERFROMITEM>No</LEDGERFROMITEM>
                                                    <REMOVEZEROENTRIES>No</REMOVEZEROENTRIES>
                                                    <ISPARTYLEDGER>No</ISPARTYLEDGER>
                                                    <GSTOVERRIDDEN>No</GSTOVERRIDDEN>
                                                    <ISGSTASSESSABLEVALUEOVERRIDDEN>No</ISGSTASSESSABLEVALUEOVERRIDDEN>
                                                    <STRDISGSTAPPLICABLE>No</STRDISGSTAPPLICABLE>
                                                    <STRDGSTISPARTYLEDGER>No</STRDGSTISPARTYLEDGER>
                                                    <STRDGSTISDUTYLEDGER>No</STRDGSTISDUTYLEDGER>
                                                    <CONTENTNEGISPOS>No</CONTENTNEGISPOS>
                                           <ISLASTDEEMEDPOSITIVE>Yes</ISLASTDEEMEDPOSITIVE>
                                                    <ISCAPVATTAXALTERED>No</ISCAPVATTAXALTERED>
                                                    <ISCAPVATNOTCLAIMED>No</ISCAPVATNOTCLAIMED>
                                                    <AMOUNT><BIND_AMOUNT></AMOUNT>
                                                    <SERVICETAXDETAILS.LIST>        </SERVICETAXDETAILS.LIST>
                                                    <BANKALLOCATIONS.LIST>        </BANKALLOCATIONS.LIST>
                                                    <BILLALLOCATIONS.LIST>        </BILLALLOCATIONS.LIST>
                                                    <INTERESTCOLLECTION.LIST>        </INTERESTCOLLECTION.LIST>
                                                    <OLDAUDITENTRIES.LIST>        </OLDAUDITENTRIES.LIST>
                                                    <ACCOUNTAUDITENTRIES.LIST>        </ACCOUNTAUDITENTRIES.LIST>
                                                    <AUDITENTRIES.LIST>        </AUDITENTRIES.LIST>
                                                    <INPUTCRALLOCS.LIST>        </INPUTCRALLOCS.LIST>
                                                    <DUTYHEADDETAILS.LIST>        </DUTYHEADDETAILS.LIST>
                                                    <EXCISEDUTYHEADDETAILS.LIST>        </EXCISEDUTYHEADDETAILS.LIST>
                                                    <RATEDETAILS.LIST>        </RATEDETAILS.LIST>
                                                    <SUMMARYALLOCS.LIST>        </SUMMARYALLOCS.LIST>
                                                    <CENVATDUTYALLOCATIONS.LIST>        </CENVATDUTYALLOCATIONS.LIST>
                                                    <STPYMTDETAILS.LIST>        </STPYMTDETAILS.LIST>
                                                    <EXCISEPAYMENTALLOCATIONS.LIST>        </EXCISEPAYMENTALLOCATIONS.LIST>
                                                    <TAXBILLALLOCATIONS.LIST>        </TAXBILLALLOCATIONS.LIST>
                                                    <TAXOBJECTALLOCATIONS.LIST>        </TAXOBJECTALLOCATIONS.LIST>
                                                    <TDSEXPENSEALLOCATIONS.LIST>        </TDSEXPENSEALLOCATIONS.LIST>
                                                    <VATSTATUTORYDETAILS.LIST>        </VATSTATUTORYDETAILS.LIST>
                                                    <COSTTRACKALLOCATIONS.LIST>        </COSTTRACKALLOCATIONS.LIST>
                                                    <REFVOUCHERDETAILS.LIST>        </REFVOUCHERDETAILS.LIST>
                                                    <INVOICEWISEDETAILS.LIST>        </INVOICEWISEDETAILS.LIST>
                                                    <VATITCDETAILS.LIST>        </VATITCDETAILS.LIST>
                                                    <ADVANCETAXDETAILS.LIST>        </ADVANCETAXDETAILS.LIST>
                                                    <TAXTYPEALLOCATIONS.LIST>        </TAXTYPEALLOCATIONS.LIST>
                                                   </ACCOUNTINGALLOCATIONS.LIST>
                                                   <DUTYHEADDETAILS.LIST>       </DUTYHEADDETAILS.LIST>
                                                   <RATEDETAILS.LIST>
                                                    <GSTRATEDUTYHEAD>CGST</GSTRATEDUTYHEAD>
                                                    <GSTRATEVALUATIONTYPE>Based on Value</GSTRATEVALUATIONTYPE>
                                                    <GSTRATE><BIND_CGST_ITEM></GSTRATE>
                                                   </RATEDETAILS.LIST>
                                                   <RATEDETAILS.LIST>
                                                    <GSTRATEDUTYHEAD>SGST/UTGST</GSTRATEDUTYHEAD>
                                                    <GSTRATEVALUATIONTYPE>Based on Value</GSTRATEVALUATIONTYPE>
                                                    <GSTRATE><BIND_SGST_ITEM></GSTRATE>
                                                   </RATEDETAILS.LIST>
                                                   <RATEDETAILS.LIST>
                                                    <GSTRATEDUTYHEAD>IGST</GSTRATEDUTYHEAD>
                                                    <GSTRATEVALUATIONTYPE>Based on Value</GSTRATEVALUATIONTYPE>
                                                    <GSTRATE><BIND_IGST_ITEM></GSTRATE>
                                                   </RATEDETAILS.LIST>
                                                   <RATEDETAILS.LIST>
                                                    <GSTRATEDUTYHEAD>Cess</GSTRATEDUTYHEAD>
                                                    <GSTRATEVALUATIONTYPE>&#4; Not Applicable</GSTRATEVALUATIONTYPE>
                                                   </RATEDETAILS.LIST>
                                                   <RATEDETAILS.LIST>
                                                    <GSTRATEDUTYHEAD>State Cess</GSTRATEDUTYHEAD>
                                                    <GSTRATEVALUATIONTYPE>Based on Value</GSTRATEVALUATIONTYPE>
                                                   </RATEDETAILS.LIST>
                                                   <SUPPLEMENTARYDUTYHEADDETAILS.LIST>       </SUPPLEMENTARYDUTYHEADDETAILS.LIST>
                                                   <TAXOBJECTALLOCATIONS.LIST>       </TAXOBJECTALLOCATIONS.LIST>
                                                   <REFVOUCHERDETAILS.LIST>       </REFVOUCHERDETAILS.LIST>
                                                   <EXCISEALLOCATIONS.LIST>       </EXCISEALLOCATIONS.LIST>
                                                   <EXPENSEALLOCATIONS.LIST>       </EXPENSEALLOCATIONS.LIST>
                                             </ALLINVENTORYENTRIES.LIST>";
                foreach (var product in purchase.POorderItemDetails)
                {

                    var qty = (Convert.ToInt32(product.quantity) + " " + product.uom);
                    var rate = (Convert.ToInt32(product.rate) + "/" + product.uom);
                    productEntries.Append(productTemplate
                        .Replace("{Cname}", companyName)
                        .Replace("<DESCRIPTION>", product.description.Trim().Replace("&", "&amp;"))
                        .Replace("<BIND_VOUCHERTYPE_LEDGER>", BIND_VOUCHERTYPE_LEDGER.Replace("&", "&amp;"))
                        .Replace("<BIND_RATE>", (rate).ToString())
                        .Replace("<BIND_AMOUNT>", "-" + (product.amount).ToString())
                        .Replace("<BIND_REF>", purchase.pono.Replace("&", "&amp;"))
                        .Replace("<BIND_CGST_ITEM>", product.cgst) // Replace CGST value, formatted to 2 decimal places
                        .Replace("<BIND_SGST_ITEM>", product.sgst) // Replace SGST value, formatted to 2 decimal places
                        .Replace("<BIND_IGST_ITEM>", product.igst)
                        .Replace("<BIND_HSN>", product.HSN.Trim().Replace("&", "&amp;"))
                        .Replace("<BIND_QUANTITY>", qty))
                        .Replace("{Currentdate}", voucher.podate.Replace("-", ""));
                }

                xmlContent = xmlContent.Replace("<BIND_ALLINVENTORYENTRIES>", productEntries.ToString());
                xmlContent = xmlContent.Replace("<BIND_FREIGHT_DATA>", "");
                xmlContent = xmlContent.Replace("<BIND_CGST_DATA>", "");
                xmlContent = xmlContent.Replace("<BIND_SGST_DATA>", "");
                xmlContent = xmlContent.Replace("<BIND_IGST_DATA>", "");

                xmlContent = xmlContent.Replace("<BIND_NARRATION>", "NA");
                xmlContent = xmlContent.Replace("<BIND_CARRIERAGENT>", "");
                xmlContent = xmlContent.Replace("<BIND_BILLOFLADNING>", "");
                xmlContent = xmlContent.Replace("<BIND_DISPATCHTHEORUGH>", "");
                xmlContent = xmlContent.Replace("<BIND_DESTIATION>", "");
                xmlContent = xmlContent.Replace("<BIND_OTHERREF>", "");
                xmlContent = xmlContent.Replace("<BIND_MH6666>", "");
                xmlContent = xmlContent.Replace("<BIND_TERMOFPAYMENT>", "");
                xmlContent = xmlContent.Replace("<BIND_TERMOFDILVERY>", "");
                xmlContent = xmlContent.Replace("<BIND_SUPPLIER_INVOICE>", "");//SID/24-25/01983
                xmlContent = xmlContent.Replace("<BIND_INVOICE_DATE>", "");//20250103
                xmlContent = xmlContent.Replace("{Cname}", companyName.Replace("&", "&amp;"));//20250103
                //xmlContent = xmlContent.Replace("&", "&amp;");


                //string safeXml = EscapeAmpersands(xmlContent);


                var request = new HttpRequestMessage(HttpMethod.Post, tallyUrl)
                {
                    Content = new StringContent(xmlContent, System.Text.Encoding.UTF8, "text/xml")
                };

                var response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();

                return responseContent;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while communicating with Tally.");
                throw;
            }
        }

        public async Task<string> SaveSO_Order(string xmlFilePath, SOorderVoucher voucher)
        {
            try
            {
                DateTime parsedDate = DateTime.ParseExact(voucher.sodate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                voucher.sodate = parsedDate.ToString("yyyy-MM-dd");

                string tallyUrl = _configuration["TallySettings:TallyUrl"];
                if (string.IsNullOrWhiteSpace(tallyUrl))
                {
                    throw new InvalidOperationException("Tally URL is not configured.");
                }
                if (!File.Exists(xmlFilePath))
                {
                    throw new FileNotFoundException("The specified XML file was not found.", xmlFilePath);
                }
                string xmlContent = await File.ReadAllTextAsync(xmlFilePath);
                string addmoreitem = null;
                //ADDED
                //var BIND_VOUCHERTYPE_LEDGER = "Purchase (Local)";
                //var BIND_VOUCHERTYPE = "Purchase Order";
                //var BIND_VOUCHERTYPE_LEDGER = "Sales A/c (Local)";
                var BIND_VOUCHERTYPE_LEDGER = voucher.EntryType.Replace("&", "&amp;");
                var BIND_VOUCHERTYPE = "Sales Order";

                string address = voucher.address.Trim().Replace("&", "&amp;");
                int maxLength = 100;
                List<string> addressChunks = new List<string>();
                for (int i = 0; i < address.Length; i += maxLength)
                {
                    addressChunks.Add(address.Substring(i, Math.Min(maxLength, address.Length - i)).Trim());
                }
                var so_inward = voucher;
                string companyName = _configuration["TallySettings:CompanyName"];
                xmlContent = xmlContent.Replace("{Cname}", companyName.Replace("&", "&amp;"));
                xmlContent = xmlContent.Replace("<BIND_PARTYNAME>", so_inward.customername.Replace("&", "&amp;"))
                                               .Replace("<BIND_ADD>", so_inward.address.Replace("&", "&amp;"))
                                               .Replace("<BIND_REF>", so_inward.sono.Replace("&", "&amp;"))
                                               .Replace("<BIND_GSTNO>", so_inward.gstno.Replace("&", "&amp;"))
                                               .Replace("<BIND_VOUCHERTYPE>", BIND_VOUCHERTYPE.Replace("&", "&amp;"))
                                               .Replace("<BIND_VOUCHERTYPE_LEDGER>", BIND_VOUCHERTYPE_LEDGER.Replace("&", "&amp;"))
                                               .Replace("<BIND_COUNTRY>", so_inward.country.Replace("&", "&amp;"))
                                               .Replace("<BIND_STATE>", so_inward.state.Replace("&", "&amp;"))
                                               .Replace("<BIND_CON_COUNTRY>", "")
                                               .Replace("<BIND_CON_STATE>", "")
                                               .Replace("<BIND_CON_PINCODE>", "NA")
                                               .Replace("<BIND_CONSIGNEEGST>", "")
                                               .Replace("<BIND_COURIER>", "NA")
                                               .Replace("<BIND_DC_DATE>", "")
                                               .Replace("<BIND_DC_NO>", "")
                                               .Replace("<BIND_TOP>", "0")
                                               .Replace("<BIND_TRUCKNO>", "NA")
                                               .Replace("<BIND_BASEAMOUNT>", "-" + (so_inward.FinalAmount).ToString())
                                               .Replace("<BIND_TOTALAMOUNT>", (so_inward.FinalAmount).ToString())
                                               .Replace("<BIND_DISPATCHEDBY>", "NA")
                                               //.Replace("{Currentdate}", DateTime.Now.ToString("yyyyMMdd"));
                                               .Replace("{Currentdate}", so_inward.sodate.Replace("-", ""));


                StringBuilder productEntries = new StringBuilder();
                StringBuilder LEDFEREntries = new StringBuilder();
                StringBuilder FreightEntries = new StringBuilder();
                StringBuilder GSTEntries = new StringBuilder();
                string productTemplate = @"<ALLINVENTORYENTRIES.LIST>
                                                   <STOCKITEMNAME><DESCRIPTION></STOCKITEMNAME>
                                                   <GSTOVRDNINELIGIBLEITC>&#4; Not Applicable</GSTOVRDNINELIGIBLEITC>
                                                   <GSTOVRDNISREVCHARGEAPPL>&#4; Not Applicable</GSTOVRDNISREVCHARGEAPPL>
                                                   <GSTOVRDNTAXABILITY>Taxable</GSTOVRDNTAXABILITY>
                                                   <GSTSOURCETYPE>Stock Item</GSTSOURCETYPE>
                                                   <GSTITEMSOURCE><DESCRIPTION></GSTITEMSOURCE>
                                                   <HSNSOURCETYPE>Company</HSNSOURCETYPE>
<GSTOVRDNSTOREDNATURE/>
                                                   <GSTOVRDNTYPEOFSUPPLY>Goods</GSTOVRDNTYPEOFSUPPLY>
                                                   <GSTRATEINFERAPPLICABILITY>As per Masters/Company</GSTRATEINFERAPPLICABILITY>
                                                   <GSTHSNNAME><BIND_HSN></GSTHSNNAME>
                                                   <GSTHSNDESCRIPTION><DESCRIPTION></GSTHSNDESCRIPTION>
                                                   <GSTHSNINFERAPPLICABILITY>As per Masters/Company</GSTHSNINFERAPPLICABILITY>
<ISDEEMEDPOSITIVE>No</ISDEEMEDPOSITIVE>
                                                   <ISGSTASSESSABLEVALUEOVERRIDDEN>No</ISGSTASSESSABLEVALUEOVERRIDDEN>
                                                   <STRDISGSTAPPLICABLE>No</STRDISGSTAPPLICABLE>
                                                   <CONTENTNEGISPOS>No</CONTENTNEGISPOS>
<ISLASTDEEMEDPOSITIVE>No</ISLASTDEEMEDPOSITIVE>
                                                   <ISAUTONEGATE>No</ISAUTONEGATE>
                                                   <ISCUSTOMSCLEARANCE>No</ISCUSTOMSCLEARANCE>
                                                   <ISTRACKCOMPONENT>No</ISTRACKCOMPONENT>
                                                   <ISTRACKPRODUCTION>No</ISTRACKPRODUCTION>
                                                   <ISPRIMARYITEM>No</ISPRIMARYITEM>
                                                   <ISSCRAP>No</ISSCRAP>
                                                   <RATE><BIND_RATE></RATE>
                                                   <AMOUNT><BIND_AMOUNT></AMOUNT>
                                                   <ACTUALQTY><BIND_QUANTITY></ACTUALQTY>
                                                   <BILLEDQTY><BIND_QUANTITY></BILLEDQTY>
                                                   <BATCHALLOCATIONS.LIST>
                                                    <GODOWNNAME>Main Location</GODOWNNAME>
                                                    <BATCHNAME>Primary Batch</BATCHNAME>
                                                    <INDENTNO>&#4; Not Applicable</INDENTNO>
                                                    <ORDERNO><BIND_REF></ORDERNO>
                                                    <TRACKINGNUMBER>&#4; Not Applicable</TRACKINGNUMBER>
                                                    <DYNAMICCSTISCLEARED>No</DYNAMICCSTISCLEARED>
                                                    <AMOUNT><BIND_AMOUNT></AMOUNT>
                                                    <ACTUALQTY><BIND_QUANTITY></ACTUALQTY>
                                                    <BILLEDQTY><BIND_QUANTITY></BILLEDQTY>
                                                    <ORDERDUEDATE JD=""45382"" P=""{Currentdate}"">{Currentdate}</ORDERDUEDATE>
                                                    <ADDITIONALDETAILS.LIST>        </ADDITIONALDETAILS.LIST>
                                                    <VOUCHERCOMPONENTLIST.LIST>        </VOUCHERCOMPONENTLIST.LIST>
                                                   </BATCHALLOCATIONS.LIST>
                                                   <ACCOUNTINGALLOCATIONS.LIST>
                                                    <OLDAUDITENTRYIDS.LIST TYPE=""Number"">
                                                     <OLDAUDITENTRYIDS>-1</OLDAUDITENTRYIDS>
                                                    </OLDAUDITENTRYIDS.LIST>
                                                    <LEDGERNAME><BIND_VOUCHERTYPE_LEDGER></LEDGERNAME>
                                                    <GSTCLASS>&#4; Not Applicable</GSTCLASS>
<ISDEEMEDPOSITIVE>No</ISDEEMEDPOSITIVE>
                                                    <LEDGERFROMITEM>No</LEDGERFROMITEM>
                                                    <REMOVEZEROENTRIES>No</REMOVEZEROENTRIES>
                                                    <ISPARTYLEDGER>No</ISPARTYLEDGER>
                                                    <GSTOVERRIDDEN>No</GSTOVERRIDDEN>
                                                    <ISGSTASSESSABLEVALUEOVERRIDDEN>No</ISGSTASSESSABLEVALUEOVERRIDDEN>
                                                    <STRDISGSTAPPLICABLE>No</STRDISGSTAPPLICABLE>
                                                    <STRDGSTISPARTYLEDGER>No</STRDGSTISPARTYLEDGER>
                                                    <STRDGSTISDUTYLEDGER>No</STRDGSTISDUTYLEDGER>
                                                    <CONTENTNEGISPOS>No</CONTENTNEGISPOS>
<ISLASTDEEMEDPOSITIVE>No</ISLASTDEEMEDPOSITIVE>
                                                    <ISCAPVATTAXALTERED>No</ISCAPVATTAXALTERED>
                                                    <ISCAPVATNOTCLAIMED>No</ISCAPVATNOTCLAIMED>
                                                    <AMOUNT><BIND_AMOUNT></AMOUNT>
                                                    <SERVICETAXDETAILS.LIST>        </SERVICETAXDETAILS.LIST>
                                                    <BANKALLOCATIONS.LIST>        </BANKALLOCATIONS.LIST>
                                                    <BILLALLOCATIONS.LIST>        </BILLALLOCATIONS.LIST>
                                                    <INTERESTCOLLECTION.LIST>        </INTERESTCOLLECTION.LIST>
                                                    <OLDAUDITENTRIES.LIST>        </OLDAUDITENTRIES.LIST>
                                                    <ACCOUNTAUDITENTRIES.LIST>        </ACCOUNTAUDITENTRIES.LIST>
                                                    <AUDITENTRIES.LIST>        </AUDITENTRIES.LIST>
                                                    <INPUTCRALLOCS.LIST>        </INPUTCRALLOCS.LIST>
                                                    <DUTYHEADDETAILS.LIST>        </DUTYHEADDETAILS.LIST>
                                                    <EXCISEDUTYHEADDETAILS.LIST>        </EXCISEDUTYHEADDETAILS.LIST>
                                                    <RATEDETAILS.LIST>        </RATEDETAILS.LIST>
                                                    <SUMMARYALLOCS.LIST>        </SUMMARYALLOCS.LIST>
                                                    <CENVATDUTYALLOCATIONS.LIST>        </CENVATDUTYALLOCATIONS.LIST>
                                                    <STPYMTDETAILS.LIST>        </STPYMTDETAILS.LIST>
                                                    <EXCISEPAYMENTALLOCATIONS.LIST>        </EXCISEPAYMENTALLOCATIONS.LIST>
                                                    <TAXBILLALLOCATIONS.LIST>        </TAXBILLALLOCATIONS.LIST>
                                                    <TAXOBJECTALLOCATIONS.LIST>        </TAXOBJECTALLOCATIONS.LIST>
                                                    <TDSEXPENSEALLOCATIONS.LIST>        </TDSEXPENSEALLOCATIONS.LIST>
                                                    <VATSTATUTORYDETAILS.LIST>        </VATSTATUTORYDETAILS.LIST>
                                                    <COSTTRACKALLOCATIONS.LIST>        </COSTTRACKALLOCATIONS.LIST>
                                                    <REFVOUCHERDETAILS.LIST>        </REFVOUCHERDETAILS.LIST>
                                                    <INVOICEWISEDETAILS.LIST>        </INVOICEWISEDETAILS.LIST>
                                                    <VATITCDETAILS.LIST>        </VATITCDETAILS.LIST>
                                                    <ADVANCETAXDETAILS.LIST>        </ADVANCETAXDETAILS.LIST>
                                                    <TAXTYPEALLOCATIONS.LIST>        </TAXTYPEALLOCATIONS.LIST>
                                                   </ACCOUNTINGALLOCATIONS.LIST>
                                                   <DUTYHEADDETAILS.LIST>       </DUTYHEADDETAILS.LIST>
                                                   <RATEDETAILS.LIST>
                                                    <GSTRATEDUTYHEAD>CGST</GSTRATEDUTYHEAD>
                                                    <GSTRATEVALUATIONTYPE>Based on Value</GSTRATEVALUATIONTYPE>
                                                    <GSTRATE> <BIND_CGST_ITEM></GSTRATE>
                                                   </RATEDETAILS.LIST>
                                                   <RATEDETAILS.LIST>
                                                    <GSTRATEDUTYHEAD>SGST/UTGST</GSTRATEDUTYHEAD>
                                                    <GSTRATEVALUATIONTYPE>Based on Value</GSTRATEVALUATIONTYPE>
                                                    <GSTRATE> <BIND_SGST_ITEM></GSTRATE>
                                                   </RATEDETAILS.LIST>
                                                   <RATEDETAILS.LIST>
                                                    <GSTRATEDUTYHEAD>IGST</GSTRATEDUTYHEAD>
                                                    <GSTRATEVALUATIONTYPE>Based on Value</GSTRATEVALUATIONTYPE>
                                                    <GSTRATE> <BIND_IGST_ITEM></GSTRATE>
                                                   </RATEDETAILS.LIST>
                                                   <RATEDETAILS.LIST>
                                                    <GSTRATEDUTYHEAD>Cess</GSTRATEDUTYHEAD>
                                                    <GSTRATEVALUATIONTYPE>&#4; Not Applicable</GSTRATEVALUATIONTYPE>
                                                   </RATEDETAILS.LIST>
                                                   <RATEDETAILS.LIST>
                                                    <GSTRATEDUTYHEAD>State Cess</GSTRATEDUTYHEAD>
                                                    <GSTRATEVALUATIONTYPE>Based on Value</GSTRATEVALUATIONTYPE>
                                                   </RATEDETAILS.LIST>
                                                   <SUPPLEMENTARYDUTYHEADDETAILS.LIST>       </SUPPLEMENTARYDUTYHEADDETAILS.LIST>
                                                   <TAXOBJECTALLOCATIONS.LIST>       </TAXOBJECTALLOCATIONS.LIST>
                                                   <REFVOUCHERDETAILS.LIST>       </REFVOUCHERDETAILS.LIST>
                                                   <EXCISEALLOCATIONS.LIST>       </EXCISEALLOCATIONS.LIST>
                                                   <EXPENSEALLOCATIONS.LIST>       </EXPENSEALLOCATIONS.LIST>
                                             </ALLINVENTORYENTRIES.LIST>";

                foreach (var product in so_inward.SOorderItemDetails)
                {

                    var qty = (Convert.ToInt32(product.quantity) + " " + product.uom);
                    var rate = (Convert.ToInt32(product.rate) + "/" + product.uom);
                    productEntries.Append(productTemplate
                                .Replace("<DESCRIPTION>", product.description.Trim().Replace("&", "&amp;"))
                                .Replace("<BIND_VOUCHERTYPE_LEDGER>", BIND_VOUCHERTYPE_LEDGER.Replace("&", "&amp;"))
                                .Replace("<BIND_RATE>", (rate).ToString())
                                .Replace("<BIND_AMOUNT>", (product.amount).ToString())
                                //.Replace("<BIND_AMOUNT>", "-" + (product.amount).ToString())
                                .Replace("<BIND_REF>", so_inward.sono.Replace("&", "&amp;"))
                                .Replace("<BIND_CGST_ITEM>", product.cgst.ToString()) // Replace CGST value, formatted to 2 decimal places
                                .Replace("<BIND_SGST_ITEM>", product.sgst.ToString()) // Replace SGST value, formatted to 2 decimal places
                                .Replace("<BIND_IGST_ITEM>", product.igst.ToString())
                                .Replace("<BIND_HSN>", product.HSN.Trim().Replace("&", "&amp;"))
                                .Replace("<BIND_QUANTITY>", qty))
                                .Replace("{Currentdate}", so_inward.sodate.Replace("-", ""));
                }

                xmlContent = xmlContent.Replace("<BIND_ALLINVENTORYENTRIES>", productEntries.ToString());
                xmlContent = xmlContent.Replace("<BIND_CONSIGNEE_NAME>", "");

                //if frieght found  //<BIND_FREIGHT_DATA>
                if (Convert.ToInt32(so_inward.fright) > 0)
                {
                    var freightdata = @"<LEDGERENTRIES.LIST>
                                           <OLDAUDITENTRYIDS.LIST TYPE=""Number"">
                                            <OLDAUDITENTRYIDS>-1</OLDAUDITENTRYIDS>
                                           </OLDAUDITENTRYIDS.LIST>
                                           <LEDGERNAME><BIND_LEDGER_TYPE></LEDGERNAME>
                                           <GSTCLASS>&#4; Not Applicable</GSTCLASS>
                                           <GSTOVRDNISREVCHARGEAPPL>&#4; Not Applicable</GSTOVRDNISREVCHARGEAPPL>
                                           <GSTOVRDNTAXABILITY>Taxable</GSTOVRDNTAXABILITY>
                                           <GSTSOURCETYPE>Ledger</GSTSOURCETYPE>
                                           <GSTLEDGERSOURCE><BIND_LEDGER_TYPE></GSTLEDGERSOURCE>
                                           <HSNSOURCETYPE>Company</HSNSOURCETYPE>
                                           <GSTOVRDNSTOREDNATURE/>
                                           <GSTOVRDNTYPEOFSUPPLY>Services</GSTOVRDNTYPEOFSUPPLY>
                                           <GSTRATEINFERAPPLICABILITY>As per Masters/Company</GSTRATEINFERAPPLICABILITY>
                                           <GSTHSNNAME>12345678</GSTHSNNAME>
                                           <GSTHSNDESCRIPTION>ATCPLGST</GSTHSNDESCRIPTION>
                                           <GSTHSNINFERAPPLICABILITY>As per Masters/Company</GSTHSNINFERAPPLICABILITY>
                                           <ISDEEMEDPOSITIVE>No</ISDEEMEDPOSITIVE>
                                           <LEDGERFROMITEM>No</LEDGERFROMITEM>
                                           <REMOVEZEROENTRIES>No</REMOVEZEROENTRIES>
                                           <ISPARTYLEDGER>No</ISPARTYLEDGER>
                                           <GSTOVERRIDDEN>No</GSTOVERRIDDEN>
                                           <ISGSTASSESSABLEVALUEOVERRIDDEN>No</ISGSTASSESSABLEVALUEOVERRIDDEN>
                                           <STRDISGSTAPPLICABLE>No</STRDISGSTAPPLICABLE>
                                           <STRDGSTISPARTYLEDGER>No</STRDGSTISPARTYLEDGER>
                                           <STRDGSTISDUTYLEDGER>No</STRDGSTISDUTYLEDGER>
                                           <CONTENTNEGISPOS>No</CONTENTNEGISPOS>
                                           <ISLASTDEEMEDPOSITIVE>No</ISLASTDEEMEDPOSITIVE>
                                           <ISCAPVATTAXALTERED>No</ISCAPVATTAXALTERED>
                                           <ISCAPVATNOTCLAIMED>No</ISCAPVATNOTCLAIMED>
                                           <AMOUNT><BIND_LEDGER_VALUE></AMOUNT>
                                           <VATEXPAMOUNT><BIND_LEDGER_VALUE></VATEXPAMOUNT>
                                           <SERVICETAXDETAILS.LIST>       </SERVICETAXDETAILS.LIST>
                                           <BANKALLOCATIONS.LIST>       </BANKALLOCATIONS.LIST>
                                           <BILLALLOCATIONS.LIST>       </BILLALLOCATIONS.LIST>
                                           <INTERESTCOLLECTION.LIST>       </INTERESTCOLLECTION.LIST>
                                           <OLDAUDITENTRIES.LIST>       </OLDAUDITENTRIES.LIST>
                                           <ACCOUNTAUDITENTRIES.LIST>       </ACCOUNTAUDITENTRIES.LIST>
                                           <AUDITENTRIES.LIST>       </AUDITENTRIES.LIST>
                                           <INPUTCRALLOCS.LIST>       </INPUTCRALLOCS.LIST>
                                           <DUTYHEADDETAILS.LIST>       </DUTYHEADDETAILS.LIST>
                                           <EXCISEDUTYHEADDETAILS.LIST>       </EXCISEDUTYHEADDETAILS.LIST>
                                           <RATEDETAILS.LIST>
                                            <GSTRATEDUTYHEAD>CGST</GSTRATEDUTYHEAD>
                                            <GSTRATEVALUATIONTYPE>Based on Value</GSTRATEVALUATIONTYPE>
                                            <GSTRATE> 9</GSTRATE>
                                           </RATEDETAILS.LIST>
                                           <RATEDETAILS.LIST>
                                            <GSTRATEDUTYHEAD>SGST/UTGST</GSTRATEDUTYHEAD>
                                            <GSTRATEVALUATIONTYPE>Based on Value</GSTRATEVALUATIONTYPE>
                                            <GSTRATE> 9</GSTRATE>
                                           </RATEDETAILS.LIST>
                                           <RATEDETAILS.LIST>
                                            <GSTRATEDUTYHEAD>IGST</GSTRATEDUTYHEAD>
                                            <GSTRATEVALUATIONTYPE>Based on Value</GSTRATEVALUATIONTYPE>
                                            <GSTRATE> 18</GSTRATE>
                                           </RATEDETAILS.LIST>
                                           <RATEDETAILS.LIST>
                                            <GSTRATEDUTYHEAD>Cess</GSTRATEDUTYHEAD>
                                            <GSTRATEVALUATIONTYPE>&#4; Not Applicable</GSTRATEVALUATIONTYPE>
                                           </RATEDETAILS.LIST>
                                           <RATEDETAILS.LIST>
                                            <GSTRATEDUTYHEAD>State Cess</GSTRATEDUTYHEAD>
                                            <GSTRATEVALUATIONTYPE>Based on Value</GSTRATEVALUATIONTYPE>
                                           </RATEDETAILS.LIST>
                                           <SUMMARYALLOCS.LIST>       </SUMMARYALLOCS.LIST>
                                           <CENVATDUTYALLOCATIONS.LIST>       </CENVATDUTYALLOCATIONS.LIST>
                                           <STPYMTDETAILS.LIST>       </STPYMTDETAILS.LIST>
                                           <EXCISEPAYMENTALLOCATIONS.LIST>       </EXCISEPAYMENTALLOCATIONS.LIST>
                                           <TAXBILLALLOCATIONS.LIST>       </TAXBILLALLOCATIONS.LIST>
                                           <TAXOBJECTALLOCATIONS.LIST>       </TAXOBJECTALLOCATIONS.LIST>
                                           <TDSEXPENSEALLOCATIONS.LIST>       </TDSEXPENSEALLOCATIONS.LIST>
                                           <VATSTATUTORYDETAILS.LIST>       </VATSTATUTORYDETAILS.LIST>
                                           <COSTTRACKALLOCATIONS.LIST>       </COSTTRACKALLOCATIONS.LIST>
                                           <REFVOUCHERDETAILS.LIST>       </REFVOUCHERDETAILS.LIST>
                                           <INVOICEWISEDETAILS.LIST>       </INVOICEWISEDETAILS.LIST>
                                           <VATITCDETAILS.LIST>       </VATITCDETAILS.LIST>
                                           <ADVANCETAXDETAILS.LIST>       </ADVANCETAXDETAILS.LIST>
                                           <TAXTYPEALLOCATIONS.LIST>       </TAXTYPEALLOCATIONS.LIST>
                                          </LEDGERENTRIES.LIST>";
                    var count = 1;
                    for (int i = 1; i <= count; i++)
                    {
                        FreightEntries.Append(freightdata
                            //.Replace("<BIND_LEDGER_TYPE>", "Freight Charges of Purchase (OMS)")
                            //.Replace("<BIND_LEDGER_TYPE>", "Freight Charges on Sale")
                            .Replace("<BIND_LEDGER_TYPE>", so_inward.Freight_type.Replace("&", "&amp;"))
                            .Replace("<BIND_LEDGER_VALUE>", (so_inward.fright).ToString()));
                    }
                    ;
                    xmlContent = xmlContent.Replace("<BIND_FREIGHT_DATA>", FreightEntries.ToString());
                }
                else
                {
                    xmlContent = xmlContent.Replace("<BIND_FREIGHT_DATA>", "");
                }
                //end

                //if gst found //<BIND_GST_DATA>
                if (so_inward.gst_type == "sgst-cgst" || so_inward.gst_type == "igst")
                {
                    var string_data = "";
                    var gstdata = @"<LEDGERENTRIES.LIST>
                                       <OLDAUDITENTRYIDS.LIST TYPE=""Number"">
                                        <OLDAUDITENTRYIDS>-1</OLDAUDITENTRYIDS>
                                       </OLDAUDITENTRYIDS.LIST>
                                       <APPROPRIATEFOR>&#4; Not Applicable</APPROPRIATEFOR>
                                       <ROUNDTYPE>&#4; Not Applicable</ROUNDTYPE>
                                       <LEDGERNAME><BIND_LEDGER_TYPE></LEDGERNAME>
                                       <GSTCLASS>&#4; Not Applicable</GSTCLASS>
                                       <ISDEEMEDPOSITIVE>No</ISDEEMEDPOSITIVE>
                                       <LEDGERFROMITEM>No</LEDGERFROMITEM>
                                       <REMOVEZEROENTRIES>No</REMOVEZEROENTRIES>
                                       <ISPARTYLEDGER>No</ISPARTYLEDGER>
                                       <GSTOVERRIDDEN>No</GSTOVERRIDDEN>
                                       <ISGSTASSESSABLEVALUEOVERRIDDEN>No</ISGSTASSESSABLEVALUEOVERRIDDEN>
                                       <STRDISGSTAPPLICABLE>No</STRDISGSTAPPLICABLE>
                                       <STRDGSTISPARTYLEDGER>No</STRDGSTISPARTYLEDGER>
                                       <STRDGSTISDUTYLEDGER>No</STRDGSTISDUTYLEDGER>
                                       <CONTENTNEGISPOS>No</CONTENTNEGISPOS>
                                       <ISLASTDEEMEDPOSITIVE>No</ISLASTDEEMEDPOSITIVE>
                                       <ISCAPVATTAXALTERED>No</ISCAPVATTAXALTERED>
                                       <ISCAPVATNOTCLAIMED>No</ISCAPVATNOTCLAIMED>
                                       <AMOUNT><BIND_LEDGER_VALUE></AMOUNT>
                                       <VATEXPAMOUNT><BIND_LEDGER_VALUE></VATEXPAMOUNT>
                                       <SERVICETAXDETAILS.LIST>       </SERVICETAXDETAILS.LIST>
                                       <BANKALLOCATIONS.LIST>       </BANKALLOCATIONS.LIST>
                                       <BILLALLOCATIONS.LIST>       </BILLALLOCATIONS.LIST>
                                       <INTERESTCOLLECTION.LIST>       </INTERESTCOLLECTION.LIST>
                                       <OLDAUDITENTRIES.LIST>       </OLDAUDITENTRIES.LIST>
                                       <ACCOUNTAUDITENTRIES.LIST>       </ACCOUNTAUDITENTRIES.LIST>
                                       <AUDITENTRIES.LIST>       </AUDITENTRIES.LIST>
                                       <INPUTCRALLOCS.LIST>       </INPUTCRALLOCS.LIST>
                                       <DUTYHEADDETAILS.LIST>       </DUTYHEADDETAILS.LIST>
                                       <EXCISEDUTYHEADDETAILS.LIST>       </EXCISEDUTYHEADDETAILS.LIST>
                                       <RATEDETAILS.LIST>       </RATEDETAILS.LIST>
                                       <SUMMARYALLOCS.LIST>       </SUMMARYALLOCS.LIST>
                                       <CENVATDUTYALLOCATIONS.LIST>       </CENVATDUTYALLOCATIONS.LIST>
                                       <STPYMTDETAILS.LIST>       </STPYMTDETAILS.LIST>
                                       <EXCISEPAYMENTALLOCATIONS.LIST>       </EXCISEPAYMENTALLOCATIONS.LIST>
                                       <TAXBILLALLOCATIONS.LIST>       </TAXBILLALLOCATIONS.LIST>
                                       <TAXOBJECTALLOCATIONS.LIST>       </TAXOBJECTALLOCATIONS.LIST>
                                       <TDSEXPENSEALLOCATIONS.LIST>       </TDSEXPENSEALLOCATIONS.LIST>
                                       <VATSTATUTORYDETAILS.LIST>       </VATSTATUTORYDETAILS.LIST>
                                       <COSTTRACKALLOCATIONS.LIST>       </COSTTRACKALLOCATIONS.LIST>
                                       <REFVOUCHERDETAILS.LIST>       </REFVOUCHERDETAILS.LIST>
                                       <INVOICEWISEDETAILS.LIST>       </INVOICEWISEDETAILS.LIST>
                                       <VATITCDETAILS.LIST>       </VATITCDETAILS.LIST>
                                       <ADVANCETAXDETAILS.LIST>       </ADVANCETAXDETAILS.LIST>
                                       <TAXTYPEALLOCATIONS.LIST>       </TAXTYPEALLOCATIONS.LIST>
                                      </LEDGERENTRIES.LIST>";
                    if (so_inward.gst_type == "sgst-cgst")
                    {
                        var gstvalluuu = so_inward.CGST;

                        //CGST
                        GSTEntries.Append(gstdata
                            .Replace("<BIND_LEDGER_TYPE>", "CGST") // Corrected usage of 'item'
                            .Replace("<BIND_LEDGER_VALUE>", (gstvalluuu).ToString())); // Assuming 'gstvalluuu' is a valid variable
                        xmlContent = xmlContent.Replace("<BIND_CGST_DATA>", GSTEntries.ToString());
                        xmlContent = xmlContent.Replace("<BIND_IGST_DATA>", "");
                        //END

                        //SGST
                        GSTEntries.Clear();
                        gstvalluuu = so_inward.SGST;
                        GSTEntries.Append(gstdata
                            .Replace("<BIND_LEDGER_TYPE>", "SGST") // Corrected usage of 'item'
                            .Replace("<BIND_LEDGER_VALUE>", (gstvalluuu).ToString())); // Assuming 'gstvalluuu' is a valid variable
                        xmlContent = xmlContent.Replace("<BIND_SGST_DATA>", GSTEntries.ToString());
                        xmlContent = xmlContent.Replace("<BIND_IGST_DATA>", "");
                        //END
                    }
                    if (so_inward.gst_type == "igst")
                    {
                        //IGST
                        var count = 1;
                        for (int i = 1; i <= count; i++)
                        {
                            var gstvalluuu = so_inward.IGST;
                            GSTEntries.Append(gstdata
                                .Replace("<BIND_LEDGER_TYPE>", "IGST")
                                .Replace("<BIND_LEDGER_VALUE>", (gstvalluuu).ToString()));

                            xmlContent = xmlContent.Replace("<BIND_IGST_DATA>", GSTEntries.ToString());
                            xmlContent = xmlContent.Replace("<BIND_CGST_DATA>", "");
                            xmlContent = xmlContent.Replace("<BIND_SGST_DATA>", "");
                        }
                        //END
                    }
                }
                else
                {
                    xmlContent = xmlContent.Replace("<BIND_CGST_DATA>", "");
                    xmlContent = xmlContent.Replace("<BIND_SGST_DATA>", "");
                    xmlContent = xmlContent.Replace("<BIND_IGST_DATA>", "");
                }
                //end

                xmlContent = xmlContent.Replace("<BIND_FREIGHT_DATA>", "");
                xmlContent = xmlContent.Replace("<BIND_CGST_DATA>", "");
                xmlContent = xmlContent.Replace("<BIND_SGST_DATA>", "");
                xmlContent = xmlContent.Replace("<BIND_IGST_DATA>", "");

                xmlContent = xmlContent.Replace("<BIND_NARRATION>", so_inward.remark.Trim().Replace("&", "&amp;"));
                xmlContent = xmlContent.Replace("<BIND_CARRIERAGENT>", "");
                xmlContent = xmlContent.Replace("<BIND_BILLOFLADNING>", "");
                xmlContent = xmlContent.Replace("<BIND_DISPATCHTHEORUGH>", "");
                xmlContent = xmlContent.Replace("<BIND_DESTIATION>", "");
                xmlContent = xmlContent.Replace("<BIND_OTHERREF>", "");
                xmlContent = xmlContent.Replace("<BIND_MH6666>", "");
                xmlContent = xmlContent.Replace("<BIND_TERMOFPAYMENT>", so_inward.paymentterm.Replace("&", "&amp;"));
                xmlContent = xmlContent.Replace("<BIND_TERMOFDILVERY>", so_inward.termofdilivery.Replace("&", "&amp;"));
                xmlContent = xmlContent.Replace("<BIND_SUPPLIER_INVOICE>", "");//SID/24-25/01983
                xmlContent = xmlContent.Replace("<BIND_INVOICE_DATE>", "");//20250103
                xmlContent = xmlContent.Replace("<BIND_CONSIGNEE_NAME>", "");//20250103

                var request = new HttpRequestMessage(HttpMethod.Post, tallyUrl)
                {
                    Content = new StringContent(xmlContent, System.Text.Encoding.UTF8, "text/xml")
                };

                var response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();

                return responseContent;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while communicating with Tally.");
                throw;
            }
        }

        public async Task<string> SavePO_OrderInvoice(string xmlFilePath, Invoice voucher)
        {
            try
            {
                DateTime parsedDate = DateTime.ParseExact(voucher.orderDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                voucher.orderDate = parsedDate.ToString("yyyy-MM-dd");

                //TALLY UPDATE
                var BIND_VOUCHERTYPE_LEDGER = voucher.EntryType.Replace("&", "&amp;");
                //var BIND_VOUCHERTYPE_LEDGER = "Purchase (Local)";
                var BIND_VOUCHERTYPE = "Purchase";
                var BIND_REF_ORDERNAME = "Purchase Order";

                string tallyUrl = _configuration["TallySettings:TallyUrl"];
                if (string.IsNullOrWhiteSpace(tallyUrl))
                {
                    throw new InvalidOperationException("Tally URL is not configured.");
                }
                if (!File.Exists(xmlFilePath))
                {
                    throw new FileNotFoundException("The specified XML file was not found.", xmlFilePath);
                }
                string xmlContent = await File.ReadAllTextAsync(xmlFilePath);
                string addmoreitem = null;
                //ADDED

                string address = voucher.address.Trim();
                int maxLength = 100;
                List<string> addressChunks = new List<string>();
                for (int i = 0; i < address.Length; i += maxLength)
                {
                    addressChunks.Add(address.Substring(i, Math.Min(maxLength, address.Length - i)).Trim());
                }
                var applicant = voucher;
                string companyName = _configuration["TallySettings:CompanyName"];
                //xmlContent = xmlContent.Replace("{Cname}", companyName);

                xmlContent = xmlContent.Replace("{Cname}", companyName)
                                               .Replace("<BIND_PARTYNAME>", applicant.partyname.Trim().Replace("&", "&amp;"))
                                               .Replace("<BIND_ADD>", applicant.address.Replace("&", "&amp;"))
                                               .Replace("<BIND_ORDER_DT>", applicant.orderDate.Replace("-", ""))
                                              .Replace("<BIND_DC_DT>", applicant.dcDate.Replace("-", ""))
                                              .Replace("<BIND_IN_DT>", applicant.invDate.Replace("-", ""))
                                               .Replace("<BIND_REF>", applicant.refno.Replace("&", "&amp;"))
                                               .Replace("<BIND_GSTNO>", applicant.gstno.Replace("&", "&amp;"))
                                               .Replace("<BIND_VOUCHERTYPE>", BIND_VOUCHERTYPE.Replace("&", "&amp;"))
                                               .Replace("<BIND_REF_ORDERNAME>", BIND_REF_ORDERNAME.Replace("&", "&amp;"))
                                               .Replace("<BIND_COUNTRY>", applicant.country.Replace("&", "&amp;"))
                                               .Replace("<BIND_STATE>", applicant.state.Replace("&", "&amp;"))
                                               .Replace("<BIND_CON_COUNTRY>", "")
                                               .Replace("<BIND_CON_STATE>", "")
                                               .Replace("<BIND_CON_PINCODE>", "")
                                               .Replace("<BIND_CONSIGNEEGST>", "")
                                               .Replace("<BIND_COURIER>", "NA")
                                               .Replace("<BIND_DC_DATE>", "")
                                               .Replace("<BIND_DC_NO>", applicant.dcno.Trim().Replace("&", "&amp;"))
                                               .Replace("<BIND_TOP>", "0")
                                               .Replace("<BIND_TRUCKNO>", applicant.truckno.Trim().Replace("&", "&amp;"))
                                               .Replace("<BIND_BASEAMOUNT>", (applicant.FinalAmount).ToString().Replace("&", "&amp;"))
                                               .Replace("<BIND_DISPATCHEDBY>", applicant.dispatchby.Replace("&", "&amp;"))
                                               .Replace("<BIND_REF_ORDERNAME>", BIND_REF_ORDERNAME.Replace("&", "&amp;"));
                                               //.Replace("{Currentdate}", applicant.invDate.Replace("-", ""));

                StringBuilder productEntries = new StringBuilder();
                StringBuilder LEDFEREntries = new StringBuilder();
                StringBuilder FreightEntries = new StringBuilder();
                StringBuilder GSTEntries = new StringBuilder();

                string productTemplate = @"<ALLINVENTORYENTRIES.LIST>
                                                           <STOCKITEMNAME><BIND_DESCRIPTION></STOCKITEMNAME>
                                                           <GSTOVRDNINELIGIBLEITC>&#4; Not Applicable</GSTOVRDNINELIGIBLEITC>
                                                           <GSTOVRDNISREVCHARGEAPPL>&#4; Not Applicable</GSTOVRDNISREVCHARGEAPPL>
                                                           <GSTOVRDNTAXABILITY>Taxable</GSTOVRDNTAXABILITY>
                                                           <GSTSOURCETYPE>Stock Item</GSTSOURCETYPE>
                                                           <GSTITEMSOURCE><BIND_DESCRIPTION></GSTITEMSOURCE>
                                                           <HSNSOURCETYPE>Company</HSNSOURCETYPE>
                                                           <GSTOVRDNSTOREDNATURE/>
                                                           <GSTOVRDNTYPEOFSUPPLY>Goods</GSTOVRDNTYPEOFSUPPLY>
                                                           <GSTRATEINFERAPPLICABILITY>As per Masters/Company</GSTRATEINFERAPPLICABILITY>
                                                           <GSTHSNNAME><BIND_HSN></GSTHSNNAME>
                                                           <GSTHSNDESCRIPTION>ATCPLGST</GSTHSNDESCRIPTION>
                                                           <GSTHSNINFERAPPLICABILITY>As per Masters/Company</GSTHSNINFERAPPLICABILITY>
                                                           <ISDEEMEDPOSITIVE>Yes</ISDEEMEDPOSITIVE>
                                                           <ISGSTASSESSABLEVALUEOVERRIDDEN>No</ISGSTASSESSABLEVALUEOVERRIDDEN>
                                                           <STRDISGSTAPPLICABLE>No</STRDISGSTAPPLICABLE>
                                                           <CONTENTNEGISPOS>No</CONTENTNEGISPOS>
                                                           <ISLASTDEEMEDPOSITIVE>Yes</ISLASTDEEMEDPOSITIVE>
                                                           <ISAUTONEGATE>No</ISAUTONEGATE>
                                                           <ISCUSTOMSCLEARANCE>No</ISCUSTOMSCLEARANCE>
                                                           <ISTRACKCOMPONENT>No</ISTRACKCOMPONENT>
                                                           <ISTRACKPRODUCTION>No</ISTRACKPRODUCTION>
                                                           <ISPRIMARYITEM>No</ISPRIMARYITEM>
                                                           <ISSCRAP>No</ISSCRAP>
                                                           <RATE><BIND_RATE></RATE>
                                                           <AMOUNT><BIND_AMOUNT></AMOUNT>
                                                           <ACTUALQTY><BIND_QUANTITY></ACTUALQTY>
                                                           <BILLEDQTY><BIND_QUANTITY></BILLEDQTY>
                                                           <BATCHALLOCATIONS.LIST>
                                                            <GODOWNNAME>Main Location</GODOWNNAME>
                                                            <BATCHNAME>Primary Batch</BATCHNAME>
                                                            <INDENTNO>&#4; Not Applicable</INDENTNO>
                                                            <ORDERNO><BIND_REF></ORDERNO>
                                                            <TRACKINGNUMBER>&#4; Not Applicable</TRACKINGNUMBER>
                                                            <DYNAMICCSTISCLEARED>No</DYNAMICCSTISCLEARED>
                                                            <AMOUNT><BIND_AMOUNT></AMOUNT>
                                                            <ACTUALQTY><BIND_QUANTITY></ACTUALQTY>
                                                            <BILLEDQTY><BIND_QUANTITY></BILLEDQTY>
                                                            <ORDERDUEDATE JD=""45382"" P=""{Currentdate}"">{Currentdate}</ORDERDUEDATE>
                                                            <ADDITIONALDETAILS.LIST>        </ADDITIONALDETAILS.LIST>
                                                            <VOUCHERCOMPONENTLIST.LIST>        </VOUCHERCOMPONENTLIST.LIST>
                                                           </BATCHALLOCATIONS.LIST>
                                                           <ACCOUNTINGALLOCATIONS.LIST>
                                                            <OLDAUDITENTRYIDS.LIST TYPE=""Number"">
                                                             <OLDAUDITENTRYIDS>-1</OLDAUDITENTRYIDS>
                                                            </OLDAUDITENTRYIDS.LIST>
                                                            <LEDGERNAME><BIND_VOUCHERTYPE_LEDGER></LEDGERNAME>
                                                            <GSTCLASS>&#4; Not Applicable</GSTCLASS>
                                                            <ISDEEMEDPOSITIVE>Yes</ISDEEMEDPOSITIVE>
                                                            <LEDGERFROMITEM>No</LEDGERFROMITEM>
                                                            <REMOVEZEROENTRIES>No</REMOVEZEROENTRIES>
                                                            <ISPARTYLEDGER>No</ISPARTYLEDGER>
                                                            <GSTOVERRIDDEN>No</GSTOVERRIDDEN>
                                                            <ISGSTASSESSABLEVALUEOVERRIDDEN>No</ISGSTASSESSABLEVALUEOVERRIDDEN>
                                                            <STRDISGSTAPPLICABLE>No</STRDISGSTAPPLICABLE>
                                                            <STRDGSTISPARTYLEDGER>No</STRDGSTISPARTYLEDGER>
                                                            <STRDGSTISDUTYLEDGER>No</STRDGSTISDUTYLEDGER>
                                                            <CONTENTNEGISPOS>No</CONTENTNEGISPOS>
                                                            <ISLASTDEEMEDPOSITIVE>Yes</ISLASTDEEMEDPOSITIVE>
                                                            <ISCAPVATTAXALTERED>No</ISCAPVATTAXALTERED>
                                                            <ISCAPVATNOTCLAIMED>No</ISCAPVATNOTCLAIMED>
                                                            <AMOUNT><BIND_AMOUNT></AMOUNT>
                                                            <SERVICETAXDETAILS.LIST>        </SERVICETAXDETAILS.LIST>
                                                            <BANKALLOCATIONS.LIST>        </BANKALLOCATIONS.LIST>
                                                            <BILLALLOCATIONS.LIST>
			                                                    
		                                                    </BILLALLOCATIONS.LIST>
                                                            <INTERESTCOLLECTION.LIST>        </INTERESTCOLLECTION.LIST>
                                                            <OLDAUDITENTRIES.LIST>        </OLDAUDITENTRIES.LIST>
                                                            <ACCOUNTAUDITENTRIES.LIST>        </ACCOUNTAUDITENTRIES.LIST>
                                                            <AUDITENTRIES.LIST>        </AUDITENTRIES.LIST>
                                                            <INPUTCRALLOCS.LIST>        </INPUTCRALLOCS.LIST>
                                                            <DUTYHEADDETAILS.LIST>        </DUTYHEADDETAILS.LIST>
                                                            <EXCISEDUTYHEADDETAILS.LIST>        </EXCISEDUTYHEADDETAILS.LIST>
                                                            <RATEDETAILS.LIST>        </RATEDETAILS.LIST>
                                                            <SUMMARYALLOCS.LIST>        </SUMMARYALLOCS.LIST>
                                                            <CENVATDUTYALLOCATIONS.LIST>        </CENVATDUTYALLOCATIONS.LIST>
                                                            <STPYMTDETAILS.LIST>        </STPYMTDETAILS.LIST>
                                                            <EXCISEPAYMENTALLOCATIONS.LIST>        </EXCISEPAYMENTALLOCATIONS.LIST>
                                                            <TAXBILLALLOCATIONS.LIST>        </TAXBILLALLOCATIONS.LIST>
                                                            <TAXOBJECTALLOCATIONS.LIST>        </TAXOBJECTALLOCATIONS.LIST>
                                                            <TDSEXPENSEALLOCATIONS.LIST>        </TDSEXPENSEALLOCATIONS.LIST>
                                                            <VATSTATUTORYDETAILS.LIST>        </VATSTATUTORYDETAILS.LIST>
                                                            <COSTTRACKALLOCATIONS.LIST>        </COSTTRACKALLOCATIONS.LIST>
                                                            <REFVOUCHERDETAILS.LIST>        </REFVOUCHERDETAILS.LIST>
                                                            <INVOICEWISEDETAILS.LIST>        </INVOICEWISEDETAILS.LIST>
                                                            <VATITCDETAILS.LIST>        </VATITCDETAILS.LIST>
                                                            <ADVANCETAXDETAILS.LIST>        </ADVANCETAXDETAILS.LIST>
                                                            <TAXTYPEALLOCATIONS.LIST>        </TAXTYPEALLOCATIONS.LIST>
                                                           </ACCOUNTINGALLOCATIONS.LIST>
                                                           <DUTYHEADDETAILS.LIST>       </DUTYHEADDETAILS.LIST>
                                                           <RATEDETAILS.LIST>
                                                            <GSTRATEDUTYHEAD>CGST</GSTRATEDUTYHEAD>
                                                            <GSTRATEVALUATIONTYPE>Based on Value</GSTRATEVALUATIONTYPE>
                                                            <GSTRATE> <BIND_CGST_ITEM></GSTRATE>
                                                           </RATEDETAILS.LIST>
                                                           <RATEDETAILS.LIST>
                                                            <GSTRATEDUTYHEAD>SGST/UTGST</GSTRATEDUTYHEAD>
                                                            <GSTRATEVALUATIONTYPE>Based on Value</GSTRATEVALUATIONTYPE>
                                                            <GSTRATE> <BIND_SGST_ITEM></GSTRATE>
                                                           </RATEDETAILS.LIST>
                                                           <RATEDETAILS.LIST>
                                                            <GSTRATEDUTYHEAD>IGST</GSTRATEDUTYHEAD>
                                                            <GSTRATEVALUATIONTYPE>Based on Value</GSTRATEVALUATIONTYPE>
                                                            <GSTRATE>  <BIND_IGST_ITEM></GSTRATE>
                                                           </RATEDETAILS.LIST>
                                                           <RATEDETAILS.LIST>
                                                            <GSTRATEDUTYHEAD>Cess</GSTRATEDUTYHEAD>
                                                            <GSTRATEVALUATIONTYPE>&#4; Not Applicable</GSTRATEVALUATIONTYPE>
                                                           </RATEDETAILS.LIST>
                                                           <RATEDETAILS.LIST>
                                                            <GSTRATEDUTYHEAD>State Cess</GSTRATEDUTYHEAD>
                                                            <GSTRATEVALUATIONTYPE>Based on Value</GSTRATEVALUATIONTYPE>
                                                           </RATEDETAILS.LIST>
                                                           <SUPPLEMENTARYDUTYHEADDETAILS.LIST>       </SUPPLEMENTARYDUTYHEADDETAILS.LIST>
                                                           <TAXOBJECTALLOCATIONS.LIST>       </TAXOBJECTALLOCATIONS.LIST>
                                                           <REFVOUCHERDETAILS.LIST>       </REFVOUCHERDETAILS.LIST>
                                                           <EXCISEALLOCATIONS.LIST>       </EXCISEALLOCATIONS.LIST>
                                                           <EXPENSEALLOCATIONS.LIST>       </EXPENSEALLOCATIONS.LIST>
                                                          </ALLINVENTORYENTRIES.LIST>
                                                    ";

                foreach (var product in applicant.InvoiceItemDetails)
                {

                    var qty = (Convert.ToInt32(product.qty) + " " + product.uom);
                    var rate = (Convert.ToInt32(product.rate) + "/" + product.uom);
                    //productEntries.Append(productTemplate
                    //            .Replace("<BIND_DESCRIPTION>", product.productname.Trim())
                    //            .Replace("<BIND_VOUCHERTYPE_LEDGER>", BIND_VOUCHERTYPE_LEDGER)
                    //            .Replace("<BIND_RATE>", (rate).ToString())
                    //            .Replace("<BIND_AMOUNT>", (product.amount).ToString())
                    //            .Replace("<BIND_REF>", applicant.refno)
                    //            .Replace("<BIND_CGST_ITEM>", product.cgst.ToString()) // Replace CGST value, formatted to 2 decimal places
                    //            .Replace("<BIND_SGST_ITEM>", product.sgst.ToString()) // Replace SGST value, formatted to 2 decimal places
                    //            .Replace("<BIND_IGST_ITEM>", product.igst.ToString())
                    //            .Replace("<BIND_HSN>", product.hsn.Trim())
                    //            .Replace("<BIND_QUANTITY>", qty));
                    productEntries.Append(productTemplate
                                    .Replace("<BIND_DESCRIPTION>", product.productname.Trim().Replace("&", "&amp;"))
                                    .Replace("<BIND_VOUCHERTYPE_LEDGER>", BIND_VOUCHERTYPE_LEDGER.Replace("&", "&amp;"))
                                    .Replace("<BIND_REF_ORDERNAME>", BIND_REF_ORDERNAME.Replace("&", "&amp;"))
                                    .Replace("<BIND_RATE>", (rate).ToString())
                                    .Replace("<BIND_AMOUNT>", "-" + (product.amount).ToString())
                                    .Replace("<BIND_REF>", applicant.refno.Trim().Replace("&", "&amp;"))
                                    .Replace("<BIND_CGST_ITEM>", product.cgst.ToString()) // Replace CGST value, formatted to 2 decimal
                                    .Replace("<BIND_SGST_ITEM>", product.sgst.ToString()) // Replace SGST value, formatted to 2 decimal 
                                    .Replace("<BIND_IGST_ITEM>", product.igst.ToString())
                                    .Replace("<BIND_HSN>", product.hsn.Trim().Replace("&", "&amp;"))
                                    .Replace("<BIND_QUANTITY>", qty))
                                    .Replace("{Currentdate}", applicant.invDate.Replace("-", ""));
                }

                xmlContent = xmlContent.Replace("<BIND_ALLINVENTORYENTRIES>", productEntries.ToString());

                //if frieght found  //<BIND_FREIGHT_DATA>
                if (Convert.ToInt32(applicant.fright) > 0)
                {
                    var freightdata = @"<LEDGERENTRIES.LIST>
                                           <OLDAUDITENTRYIDS.LIST TYPE=""Number"">
                                            <OLDAUDITENTRYIDS>-1</OLDAUDITENTRYIDS>
                                           </OLDAUDITENTRYIDS.LIST>
                                           <LEDGERNAME><BIND_LEDGER_TYPE></LEDGERNAME>
                                           <GSTCLASS>&#4; Not Applicable</GSTCLASS>
                                           <GSTOVRDNINELIGIBLEITC>&#4; Not Applicable</GSTOVRDNINELIGIBLEITC>
                                           <GSTOVRDNISREVCHARGEAPPL>&#4; Not Applicable</GSTOVRDNISREVCHARGEAPPL>
                                           <GSTOVRDNTAXABILITY>Taxable</GSTOVRDNTAXABILITY>
                                           <GSTSOURCETYPE>Ledger</GSTSOURCETYPE>
                                           <GSTLEDGERSOURCE><BIND_LEDGER_TYPE></GSTLEDGERSOURCE>
                                           <HSNSOURCETYPE>Company</HSNSOURCETYPE>
                                           <GSTOVRDNSTOREDNATURE/>
                                           <GSTOVRDNTYPEOFSUPPLY>Services</GSTOVRDNTYPEOFSUPPLY>
                                           <GSTRATEINFERAPPLICABILITY>As per Masters/Company</GSTRATEINFERAPPLICABILITY>
                                           <GSTHSNNAME>12345678</GSTHSNNAME>
                                           <GSTHSNDESCRIPTION>ATCPLGST</GSTHSNDESCRIPTION>
                                           <GSTHSNINFERAPPLICABILITY>As per Masters/Company</GSTHSNINFERAPPLICABILITY>
                                           <ISDEEMEDPOSITIVE>Yes</ISDEEMEDPOSITIVE>
                                           <LEDGERFROMITEM>No</LEDGERFROMITEM>
                                           <REMOVEZEROENTRIES>No</REMOVEZEROENTRIES>
                                           <ISPARTYLEDGER>No</ISPARTYLEDGER>
                                           <GSTOVERRIDDEN>No</GSTOVERRIDDEN>
                                           <ISGSTASSESSABLEVALUEOVERRIDDEN>No</ISGSTASSESSABLEVALUEOVERRIDDEN>
                                           <STRDISGSTAPPLICABLE>No</STRDISGSTAPPLICABLE>
                                           <STRDGSTISPARTYLEDGER>No</STRDGSTISPARTYLEDGER>
                                           <STRDGSTISDUTYLEDGER>No</STRDGSTISDUTYLEDGER>
                                           <CONTENTNEGISPOS>No</CONTENTNEGISPOS>
                                           <ISLASTDEEMEDPOSITIVE>No</ISLASTDEEMEDPOSITIVE>
                                           <ISCAPVATTAXALTERED>No</ISCAPVATTAXALTERED>
                                           <ISCAPVATNOTCLAIMED>No</ISCAPVATNOTCLAIMED>
                                           <AMOUNT><BIND_LEDGER_VALUE></AMOUNT>
                                           <VATEXPAMOUNT><BIND_LEDGER_VALUE></VATEXPAMOUNT>
                                           <SERVICETAXDETAILS.LIST>       </SERVICETAXDETAILS.LIST>
                                           <BANKALLOCATIONS.LIST>       </BANKALLOCATIONS.LIST>
                                           <BILLALLOCATIONS.LIST>       </BILLALLOCATIONS.LIST>
                                           <INTERESTCOLLECTION.LIST>       </INTERESTCOLLECTION.LIST>
                                           <OLDAUDITENTRIES.LIST>       </OLDAUDITENTRIES.LIST>
                                           <ACCOUNTAUDITENTRIES.LIST>       </ACCOUNTAUDITENTRIES.LIST>
                                           <AUDITENTRIES.LIST>       </AUDITENTRIES.LIST>
                                           <INPUTCRALLOCS.LIST>       </INPUTCRALLOCS.LIST>
                                           <DUTYHEADDETAILS.LIST>       </DUTYHEADDETAILS.LIST>
                                           <EXCISEDUTYHEADDETAILS.LIST>       </EXCISEDUTYHEADDETAILS.LIST>
                                           <RATEDETAILS.LIST>
                                            <GSTRATEDUTYHEAD>CGST</GSTRATEDUTYHEAD>
                                            <GSTRATEVALUATIONTYPE>Based on Value</GSTRATEVALUATIONTYPE>
                                            <GSTRATE> 9</GSTRATE>
                                           </RATEDETAILS.LIST>
                                           <RATEDETAILS.LIST>
                                            <GSTRATEDUTYHEAD>SGST/UTGST</GSTRATEDUTYHEAD>
                                            <GSTRATEVALUATIONTYPE>Based on Value</GSTRATEVALUATIONTYPE>
                                            <GSTRATE> 9</GSTRATE>
                                           </RATEDETAILS.LIST>
                                           <RATEDETAILS.LIST>
                                            <GSTRATEDUTYHEAD>IGST</GSTRATEDUTYHEAD>
                                            <GSTRATEVALUATIONTYPE>Based on Value</GSTRATEVALUATIONTYPE>
                                            <GSTRATE> 18</GSTRATE>
                                           </RATEDETAILS.LIST>
                                           <RATEDETAILS.LIST>
                                            <GSTRATEDUTYHEAD>Cess</GSTRATEDUTYHEAD>
                                            <GSTRATEVALUATIONTYPE>&#4; Not Applicable</GSTRATEVALUATIONTYPE>
                                           </RATEDETAILS.LIST>
                                           <RATEDETAILS.LIST>
                                            <GSTRATEDUTYHEAD>State Cess</GSTRATEDUTYHEAD>
                                            <GSTRATEVALUATIONTYPE>Based on Value</GSTRATEVALUATIONTYPE>
                                           </RATEDETAILS.LIST>
                                           <SUMMARYALLOCS.LIST>       </SUMMARYALLOCS.LIST>
                                           <CENVATDUTYALLOCATIONS.LIST>       </CENVATDUTYALLOCATIONS.LIST>
                                           <STPYMTDETAILS.LIST>       </STPYMTDETAILS.LIST>
                                           <EXCISEPAYMENTALLOCATIONS.LIST>       </EXCISEPAYMENTALLOCATIONS.LIST>
                                           <TAXBILLALLOCATIONS.LIST>       </TAXBILLALLOCATIONS.LIST>
                                           <TAXOBJECTALLOCATIONS.LIST>       </TAXOBJECTALLOCATIONS.LIST>
                                           <TDSEXPENSEALLOCATIONS.LIST>       </TDSEXPENSEALLOCATIONS.LIST>
                                           <VATSTATUTORYDETAILS.LIST>       </VATSTATUTORYDETAILS.LIST>
                                           <COSTTRACKALLOCATIONS.LIST>       </COSTTRACKALLOCATIONS.LIST>
                                           <REFVOUCHERDETAILS.LIST>       </REFVOUCHERDETAILS.LIST>
                                           <INVOICEWISEDETAILS.LIST>       </INVOICEWISEDETAILS.LIST>
                                           <VATITCDETAILS.LIST>       </VATITCDETAILS.LIST>
                                           <ADVANCETAXDETAILS.LIST>       </ADVANCETAXDETAILS.LIST>
                                           <TAXTYPEALLOCATIONS.LIST>       </TAXTYPEALLOCATIONS.LIST>
                                          </LEDGERENTRIES.LIST>";

                    var count = 1;
                    for (int i = 1; i <= count; i++)
                    {
                        FreightEntries.Append(freightdata
                            .Replace("<BIND_LEDGER_TYPE>", applicant.Freight_type.Replace("&", "&amp;"))
                            //.Replace("<BIND_LEDGER_TYPE>", "Freight Charges of Purchase")
                            .Replace("<BIND_LEDGER_VALUE>", "-" + (applicant.fright).ToString()));
                    }
                    ;
                    xmlContent = xmlContent.Replace("<BIND_FREIGHT_DATA>", FreightEntries.ToString());
                }
                else
                {
                    xmlContent = xmlContent.Replace("<BIND_FREIGHT_DATA>", "");
                }
                //end

                //if gst found //<BIND_GST_DATA>
                if (applicant.gst_type == "sgst-cgst" || applicant.gst_type == "igst")
                {
                    var string_data = "";
                    var gstdata = @"<LEDGERENTRIES.LIST>
                                       <OLDAUDITENTRYIDS.LIST TYPE=""Number"">
                                        <OLDAUDITENTRYIDS>-1</OLDAUDITENTRYIDS>
                                       </OLDAUDITENTRYIDS.LIST>
                                       <APPROPRIATEFOR>&#4; Not Applicable</APPROPRIATEFOR>
                                       <ROUNDTYPE>&#4; Not Applicable</ROUNDTYPE>
                                       <LEDGERNAME><BIND_LEDGER_TYPE></LEDGERNAME>
                                       <GSTCLASS>&#4; Not Applicable</GSTCLASS>
                                       <ISDEEMEDPOSITIVE>Yes</ISDEEMEDPOSITIVE>
                                       <LEDGERFROMITEM>No</LEDGERFROMITEM>
                                       <REMOVEZEROENTRIES>No</REMOVEZEROENTRIES>
                                       <ISPARTYLEDGER>No</ISPARTYLEDGER>
                                       <GSTOVERRIDDEN>No</GSTOVERRIDDEN>
                                       <ISGSTASSESSABLEVALUEOVERRIDDEN>No</ISGSTASSESSABLEVALUEOVERRIDDEN>
                                       <STRDISGSTAPPLICABLE>No</STRDISGSTAPPLICABLE>
                                       <STRDGSTISPARTYLEDGER>No</STRDGSTISPARTYLEDGER>
                                       <STRDGSTISDUTYLEDGER>No</STRDGSTISDUTYLEDGER>
                                       <CONTENTNEGISPOS>No</CONTENTNEGISPOS>
                                       <ISLASTDEEMEDPOSITIVE>No</ISLASTDEEMEDPOSITIVE>
                                       <ISCAPVATTAXALTERED>No</ISCAPVATTAXALTERED>
                                       <ISCAPVATNOTCLAIMED>No</ISCAPVATNOTCLAIMED>
                                       <AMOUNT><BIND_LEDGER_VALUE></AMOUNT>
                                       <VATEXPAMOUNT><BIND_LEDGER_VALUE></VATEXPAMOUNT>
                                       <SERVICETAXDETAILS.LIST>       </SERVICETAXDETAILS.LIST>
                                       <BANKALLOCATIONS.LIST>       </BANKALLOCATIONS.LIST>
                                       <BILLALLOCATIONS.LIST>       </BILLALLOCATIONS.LIST>
                                       <INTERESTCOLLECTION.LIST>       </INTERESTCOLLECTION.LIST>
                                       <OLDAUDITENTRIES.LIST>       </OLDAUDITENTRIES.LIST>
                                       <ACCOUNTAUDITENTRIES.LIST>       </ACCOUNTAUDITENTRIES.LIST>
                                       <AUDITENTRIES.LIST>       </AUDITENTRIES.LIST>
                                       <INPUTCRALLOCS.LIST>       </INPUTCRALLOCS.LIST>
                                       <DUTYHEADDETAILS.LIST>       </DUTYHEADDETAILS.LIST>
                                       <EXCISEDUTYHEADDETAILS.LIST>       </EXCISEDUTYHEADDETAILS.LIST>
                                       <RATEDETAILS.LIST>       </RATEDETAILS.LIST>
                                       <SUMMARYALLOCS.LIST>       </SUMMARYALLOCS.LIST>
                                       <CENVATDUTYALLOCATIONS.LIST>       </CENVATDUTYALLOCATIONS.LIST>
                                       <STPYMTDETAILS.LIST>       </STPYMTDETAILS.LIST>
                                       <EXCISEPAYMENTALLOCATIONS.LIST>       </EXCISEPAYMENTALLOCATIONS.LIST>
                                       <TAXBILLALLOCATIONS.LIST>       </TAXBILLALLOCATIONS.LIST>
                                       <TAXOBJECTALLOCATIONS.LIST>       </TAXOBJECTALLOCATIONS.LIST>
                                       <TDSEXPENSEALLOCATIONS.LIST>       </TDSEXPENSEALLOCATIONS.LIST>
                                       <VATSTATUTORYDETAILS.LIST>       </VATSTATUTORYDETAILS.LIST>
                                       <COSTTRACKALLOCATIONS.LIST>       </COSTTRACKALLOCATIONS.LIST>
                                       <REFVOUCHERDETAILS.LIST>       </REFVOUCHERDETAILS.LIST>
                                       <INVOICEWISEDETAILS.LIST>       </INVOICEWISEDETAILS.LIST>
                                       <VATITCDETAILS.LIST>       </VATITCDETAILS.LIST>
                                       <ADVANCETAXDETAILS.LIST>       </ADVANCETAXDETAILS.LIST>
                                       <TAXTYPEALLOCATIONS.LIST>       </TAXTYPEALLOCATIONS.LIST>
                                      </LEDGERENTRIES.LIST>";
                    if (applicant.gst_type == "sgst-cgst")
                    {
                        var gstvalluuu = applicant.cgst;
                        //CGST
                        GSTEntries.Append(gstdata
                            .Replace("<BIND_LEDGER_TYPE>", "CGST") // Corrected usage of 'item'
                            .Replace("<BIND_LEDGER_VALUE>", "-" + (gstvalluuu).ToString())); // Assuming 'gstvalluuu' is a valid variable
                        xmlContent = xmlContent.Replace("<BIND_CGST_DATA>", GSTEntries.ToString());
                        xmlContent = xmlContent.Replace("<BIND_IGST_DATA>", "");
                        //END

                        //SGST
                        GSTEntries.Clear();
                        gstvalluuu = applicant.sgst;
                        GSTEntries.Append(gstdata
                            .Replace("<BIND_LEDGER_TYPE>", "SGST") // Corrected usage of 'item'
                            .Replace("<BIND_LEDGER_VALUE>", "-" + (gstvalluuu).ToString())); // Assuming 'gstvalluuu' is a valid variable
                        xmlContent = xmlContent.Replace("<BIND_SGST_DATA>", GSTEntries.ToString());
                        xmlContent = xmlContent.Replace("<BIND_IGST_DATA>", "");
                        //END
                    }
                    if (applicant.gst_type == "igst")
                    {
                        //IGST
                        var count = 1;
                        for (int i = 1; i <= count; i++)
                        {
                            GSTEntries.Append(gstdata
                                .Replace("<BIND_LEDGER_TYPE>", "IGST")
                                .Replace("<BIND_LEDGER_VALUE>", "-" + (applicant.igst).ToString()));
                            xmlContent = xmlContent.Replace("<BIND_IGST_DATA>", GSTEntries.ToString());
                            xmlContent = xmlContent.Replace("<BIND_CGST_DATA>", "");
                            xmlContent = xmlContent.Replace("<BIND_SGST_DATA>", "");
                        }
                        //END
                    }
                }
                else
                {
                    xmlContent = xmlContent.Replace("<BIND_CGST_DATA>", "");
                    xmlContent = xmlContent.Replace("<BIND_SGST_DATA>", "");
                    xmlContent = xmlContent.Replace("<BIND_IGST_DATA>", "");
                }
                //end
                xmlContent = xmlContent.Replace("<BIND_FREIGHT_DATA>", "");
                xmlContent = xmlContent.Replace("<BIND_CGST_DATA>", "");
                xmlContent = xmlContent.Replace("<BIND_SGST_DATA>", "");
                xmlContent = xmlContent.Replace("<BIND_IGST_DATA>", "");

                xmlContent = xmlContent.Replace("<BIND_NARRATION>", applicant.remark.Replace("&", "&amp;"));
                xmlContent = xmlContent.Replace("<BIND_CARRIERAGENT>", "");
                xmlContent = xmlContent.Replace("<BIND_BILLOFLADNING>", "");
                xmlContent = xmlContent.Replace("<BIND_DISPATCHTHEORUGH>", "");
                xmlContent = xmlContent.Replace("<BIND_DESTIATION>", "");
                xmlContent = xmlContent.Replace("<BIND_OTHERREF>", applicant.refno.Trim().Replace("&", "&amp;"));
                xmlContent = xmlContent.Replace("<BIND_MH6666>", applicant.truckno.Trim().Replace("&", "&amp;"));
                xmlContent = xmlContent.Replace("<BIND_TERMOFPAYMENT>", "");
                xmlContent = xmlContent.Replace("<BIND_TERMOFDILVERY>", "");
                xmlContent = xmlContent.Replace("<BIND_SUPPLIER_INVOICE>", applicant.invoiceno.Trim().Replace("&", "&amp;"));
                xmlContent = xmlContent.Replace("<BIND_INVOICE_DATE>", "");//20250103
                xmlContent = xmlContent.Replace("<BIND_BILLOFENTRYNO>", "");//20250103
                xmlContent = xmlContent.Replace("<BIND_PLACEOFRECEIPTBYSHIPPER>", "");//20250103
                xmlContent = xmlContent.Replace("<BIND_RECEIPT_DOC_NO>", "");//20250103
                xmlContent = xmlContent.Replace("<BIND_PORT_OF_LOADING>", "");//20250103
                xmlContent = xmlContent.Replace("<BIND_PORT_OF_DISCHARGE>", "");//20250103
                xmlContent = xmlContent.Replace("<BIND_REF_ORDERNAME>", BIND_REF_ORDERNAME.Replace("&", "&amp;"));
                xmlContent = xmlContent.Replace("<BIND_VOUCHERTYPE_LEDGER>", BIND_VOUCHERTYPE_LEDGER.Replace("&", "&amp;"));
                xmlContent = xmlContent.Replace("<BIND_REF>", applicant.refno.Trim().Replace("&", "&amp;"));
                

                //string safeXml = EscapeAmpersands(xmlContent);

                var request = new HttpRequestMessage(HttpMethod.Post, tallyUrl)
                {
                    Content = new StringContent(xmlContent, System.Text.Encoding.UTF8, "text/xml")
                };

                var response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();

                return responseContent;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while communicating with Tally.");
                throw;
            }
        }

        public async Task<string> SaveSO_OrderInvoice(string xmlFilePath, Invoice voucher)
        {
            try
            {
                DateTime parsedDate = DateTime.ParseExact(voucher.orderDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                voucher.orderDate = parsedDate.ToString("yyyy-MM-dd");


                //TALLY UPDATE
                var BIND_VOUCHERTYPE_LEDGER = voucher.EntryType.Replace("&", "&amp;");
                //var BIND_VOUCHERTYPE_LEDGER = "Purchase (Local)";
                //var BIND_VOUCHERTYPE_LEDGER = "Sales A/c (Local)";
                var BIND_VOUCHERTYPE = "Sales";
                var BIND_REF_ORDERNAME = "Sales Order";

                string tallyUrl = _configuration["TallySettings:TallyUrl"];
                if (string.IsNullOrWhiteSpace(tallyUrl))
                {
                    throw new InvalidOperationException("Tally URL is not configured.");
                }
                if (!File.Exists(xmlFilePath))
                {
                    throw new FileNotFoundException("The specified XML file was not found.", xmlFilePath);
                }
                string xmlContent = await File.ReadAllTextAsync(xmlFilePath);
                string addmoreitem = null;
                //ADDED

                string address = voucher.address.Trim();
                int maxLength = 100;
                List<string> addressChunks = new List<string>();
                for (int i = 0; i < address.Length; i += maxLength)
                {
                    addressChunks.Add(address.Substring(i, Math.Min(maxLength, address.Length - i)).Trim());
                }
                var applicant = voucher;
                string companyName = _configuration["TallySettings:CompanyName"];
                //xmlContent = xmlContent.Replace("{Cname}", companyName);
                xmlContent = xmlContent.Replace("{Cname}", companyName)
                                              .Replace("<BIND_PARTYNAME>", applicant.partyname.Trim().Replace("&", "&amp;"))
                                              .Replace("<BIND_ADD>", applicant.address.Replace("&", "&amp;"))
                                              .Replace("<BIND_ORDER_DT>", applicant.orderDate.Replace("-", ""))
                                              .Replace("<BIND_DC_DT>", applicant.dcDate.Replace("-", ""))
                                              .Replace("<BIND_IN_DT>", applicant.invDate.Replace("-", ""))
                                              .Replace("<BIND_REF>", applicant.refno.Replace("&", "&amp;"))
                                              .Replace("<BIND_GSTNO>", applicant.gstno.Replace("&", "&amp;"))
                                              .Replace("<BIND_VOUCHERTYPE>", BIND_VOUCHERTYPE.Replace("&", "&amp;"))
                                              .Replace("<BIND_REF_ORDERNAME>", BIND_REF_ORDERNAME.Replace("&", "&amp;"))
                                              .Replace("<BIND_COUNTRY>", applicant.country.Replace("&", "&amp;"))
                                              .Replace("<BIND_STATE>", applicant.state.Replace("&", "&amp;"))
                                              .Replace("<BIND_CON_COUNTRY>", "")
                                              .Replace("<BIND_CON_STATE>", "")
                                              .Replace("<BIND_CON_PINCODE>", "")
                                              .Replace("<BIND_CONSIGNEEGST>", "")
                                              .Replace("<BIND_COURIER>", "NA")
                                              //.Replace("<BIND_DC_DATE>", "")
                                              .Replace("<BIND_DC_NO>", applicant.dcno.Trim().Replace("&", "&amp;"))
                                              .Replace("<BIND_TOP>", "0")
                                              .Replace("<BIND_TRUCKNO>", applicant.truckno.Trim().Replace("&", "&amp;"))
                                              .Replace("<BIND_BASEAMOUNT>", "-" + (applicant.FinalAmount).ToString())
                                              .Replace("<BIND_DISPATCHEDBY>", applicant.dispatchby.Replace("&", "&amp;"))
                                              .Replace("<BIND_REF_ORDERNAME>", BIND_REF_ORDERNAME.Replace("&", "&amp;"));
                                              //.Replace("{Currentdate}", DateTime.Now.ToString("yyyyMMdd"));
                                              //.Replace("{Currentdate}", applicant.invDate.Replace("-", ""));

                StringBuilder productEntries = new StringBuilder();
                StringBuilder LEDFEREntries = new StringBuilder();
                StringBuilder FreightEntries = new StringBuilder();
                StringBuilder GSTEntries = new StringBuilder();

                string productTemplate = @"<ALLINVENTORYENTRIES.LIST>
                                                           <STOCKITEMNAME><BIND_DESCRIPTION></STOCKITEMNAME>
                                                           <GSTOVRDNINELIGIBLEITC>&#4; Not Applicable</GSTOVRDNINELIGIBLEITC>
                                                           <GSTOVRDNISREVCHARGEAPPL>&#4; Not Applicable</GSTOVRDNISREVCHARGEAPPL>
                                                           <GSTOVRDNTAXABILITY>Taxable</GSTOVRDNTAXABILITY>
                                                           <GSTSOURCETYPE>Stock Item</GSTSOURCETYPE>
                                                           <GSTITEMSOURCE><BIND_DESCRIPTION></GSTITEMSOURCE>
                                                           <HSNSOURCETYPE>Company</HSNSOURCETYPE>
                                                           <GSTOVRDNSTOREDNATURE/>
                                                           <GSTOVRDNTYPEOFSUPPLY>Goods</GSTOVRDNTYPEOFSUPPLY>
                                                           <GSTRATEINFERAPPLICABILITY>As per Masters/Company</GSTRATEINFERAPPLICABILITY>
                                                           <GSTHSNNAME><BIND_HSN></GSTHSNNAME>
                                                           <GSTHSNDESCRIPTION>ATCPLGST</GSTHSNDESCRIPTION>
                                                           <GSTHSNINFERAPPLICABILITY>As per Masters/Company</GSTHSNINFERAPPLICABILITY>
                               <ISDEEMEDPOSITIVE>No</ISDEEMEDPOSITIVE>
                                                           <ISGSTASSESSABLEVALUEOVERRIDDEN>No</ISGSTASSESSABLEVALUEOVERRIDDEN>
                                                           <STRDISGSTAPPLICABLE>No</STRDISGSTAPPLICABLE>
                                                           <CONTENTNEGISPOS>No</CONTENTNEGISPOS>
                                <ISLASTDEEMEDPOSITIVE>No</ISLASTDEEMEDPOSITIVE>
                                                           <ISAUTONEGATE>No</ISAUTONEGATE>
                                                           <ISCUSTOMSCLEARANCE>No</ISCUSTOMSCLEARANCE>
                                                           <ISTRACKCOMPONENT>No</ISTRACKCOMPONENT>
                                                           <ISTRACKPRODUCTION>No</ISTRACKPRODUCTION>
                                                           <ISPRIMARYITEM>No</ISPRIMARYITEM>
                                                           <ISSCRAP>No</ISSCRAP>
                                                           <RATE><BIND_RATE></RATE>
                                                           <AMOUNT><BIND_AMOUNT></AMOUNT>
                                                           <ACTUALQTY><BIND_QUANTITY></ACTUALQTY>
                                                           <BILLEDQTY><BIND_QUANTITY></BILLEDQTY>
                                                           <BATCHALLOCATIONS.LIST>
                                                            <GODOWNNAME>Main Location</GODOWNNAME>
                                                            <BATCHNAME>Primary Batch</BATCHNAME>
                                                            <INDENTNO>&#4; Not Applicable</INDENTNO>
                                                            <ORDERNO><BIND_REF></ORDERNO>
                                                            <TRACKINGNUMBER>&#4; Not Applicable</TRACKINGNUMBER>
                                                            <DYNAMICCSTISCLEARED>No</DYNAMICCSTISCLEARED>
                                                            <AMOUNT><BIND_AMOUNT></AMOUNT>
                                                            <ACTUALQTY><BIND_QUANTITY></ACTUALQTY>
                                                            <BILLEDQTY><BIND_QUANTITY></BILLEDQTY>
                                                            <ORDERDUEDATE JD=""45382"" P=""{Currentdate}"">{Currentdate}</ORDERDUEDATE>
                                                            <ADDITIONALDETAILS.LIST>        </ADDITIONALDETAILS.LIST>
                                                            <VOUCHERCOMPONENTLIST.LIST>        </VOUCHERCOMPONENTLIST.LIST>
                                                           </BATCHALLOCATIONS.LIST>
                                                           <ACCOUNTINGALLOCATIONS.LIST>
                                                            <OLDAUDITENTRYIDS.LIST TYPE=""Number"">
                                                             <OLDAUDITENTRYIDS>-1</OLDAUDITENTRYIDS>
                                                            </OLDAUDITENTRYIDS.LIST>
                                                            <LEDGERNAME><BIND_VOUCHERTYPE_LEDGER></LEDGERNAME>
                                                            <GSTCLASS>&#4; Not Applicable</GSTCLASS>
                                               <ISDEEMEDPOSITIVE>No</ISDEEMEDPOSITIVE>
                                                            <LEDGERFROMITEM>No</LEDGERFROMITEM>
                                                            <REMOVEZEROENTRIES>No</REMOVEZEROENTRIES>
                                                            <ISPARTYLEDGER>No</ISPARTYLEDGER>
                                                            <GSTOVERRIDDEN>No</GSTOVERRIDDEN>
                                                            <ISGSTASSESSABLEVALUEOVERRIDDEN>No</ISGSTASSESSABLEVALUEOVERRIDDEN>
                                                            <STRDISGSTAPPLICABLE>No</STRDISGSTAPPLICABLE>
                                                            <STRDGSTISPARTYLEDGER>No</STRDGSTISPARTYLEDGER>
                                                            <STRDGSTISDUTYLEDGER>No</STRDGSTISDUTYLEDGER>
                                                            <CONTENTNEGISPOS>No</CONTENTNEGISPOS>
                                               <ISLASTDEEMEDPOSITIVE>No</ISLASTDEEMEDPOSITIVE>
                                                            <ISCAPVATTAXALTERED>No</ISCAPVATTAXALTERED>
                                                            <ISCAPVATNOTCLAIMED>No</ISCAPVATNOTCLAIMED>
                                                            <AMOUNT><BIND_AMOUNT></AMOUNT>
                                                            <SERVICETAXDETAILS.LIST>        </SERVICETAXDETAILS.LIST>
                                                            <BANKALLOCATIONS.LIST>        </BANKALLOCATIONS.LIST>
                                                            <BILLALLOCATIONS.LIST>
			                                                    
		                                                    </BILLALLOCATIONS.LIST>
                                                            <INTERESTCOLLECTION.LIST>        </INTERESTCOLLECTION.LIST>
                                                            <OLDAUDITENTRIES.LIST>        </OLDAUDITENTRIES.LIST>
                                                            <ACCOUNTAUDITENTRIES.LIST>        </ACCOUNTAUDITENTRIES.LIST>
                                                            <AUDITENTRIES.LIST>        </AUDITENTRIES.LIST>
                                                            <INPUTCRALLOCS.LIST>        </INPUTCRALLOCS.LIST>
                                                            <DUTYHEADDETAILS.LIST>        </DUTYHEADDETAILS.LIST>
                                                            <EXCISEDUTYHEADDETAILS.LIST>        </EXCISEDUTYHEADDETAILS.LIST>
                                                            <RATEDETAILS.LIST>        </RATEDETAILS.LIST>
                                                            <SUMMARYALLOCS.LIST>        </SUMMARYALLOCS.LIST>
                                                            <CENVATDUTYALLOCATIONS.LIST>        </CENVATDUTYALLOCATIONS.LIST>
                                                            <STPYMTDETAILS.LIST>        </STPYMTDETAILS.LIST>
                                                            <EXCISEPAYMENTALLOCATIONS.LIST>        </EXCISEPAYMENTALLOCATIONS.LIST>
                                                            <TAXBILLALLOCATIONS.LIST>        </TAXBILLALLOCATIONS.LIST>
                                                            <TAXOBJECTALLOCATIONS.LIST>        </TAXOBJECTALLOCATIONS.LIST>
                                                            <TDSEXPENSEALLOCATIONS.LIST>        </TDSEXPENSEALLOCATIONS.LIST>
                                                            <VATSTATUTORYDETAILS.LIST>        </VATSTATUTORYDETAILS.LIST>
                                                            <COSTTRACKALLOCATIONS.LIST>        </COSTTRACKALLOCATIONS.LIST>
                                                            <REFVOUCHERDETAILS.LIST>        </REFVOUCHERDETAILS.LIST>
                                                            <INVOICEWISEDETAILS.LIST>        </INVOICEWISEDETAILS.LIST>
                                                            <VATITCDETAILS.LIST>        </VATITCDETAILS.LIST>
                                                            <ADVANCETAXDETAILS.LIST>        </ADVANCETAXDETAILS.LIST>
                                                            <TAXTYPEALLOCATIONS.LIST>        </TAXTYPEALLOCATIONS.LIST>
                                                           </ACCOUNTINGALLOCATIONS.LIST>
                                                           <DUTYHEADDETAILS.LIST>       </DUTYHEADDETAILS.LIST>
                                                           <RATEDETAILS.LIST>
                                                            <GSTRATEDUTYHEAD>CGST</GSTRATEDUTYHEAD>
                                                            <GSTRATEVALUATIONTYPE>Based on Value</GSTRATEVALUATIONTYPE>
                                                            <GSTRATE> <BIND_CGST_ITEM></GSTRATE>
                                                           </RATEDETAILS.LIST>
                                                           <RATEDETAILS.LIST>
                                                            <GSTRATEDUTYHEAD>SGST/UTGST</GSTRATEDUTYHEAD>
                                                            <GSTRATEVALUATIONTYPE>Based on Value</GSTRATEVALUATIONTYPE>
                                                            <GSTRATE> <BIND_SGST_ITEM></GSTRATE>
                                                           </RATEDETAILS.LIST>
                                                           <RATEDETAILS.LIST>
                                                            <GSTRATEDUTYHEAD>IGST</GSTRATEDUTYHEAD>
                                                            <GSTRATEVALUATIONTYPE>Based on Value</GSTRATEVALUATIONTYPE>
                                                            <GSTRATE>  <BIND_IGST_ITEM></GSTRATE>
                                                           </RATEDETAILS.LIST>
                                                           <RATEDETAILS.LIST>
                                                            <GSTRATEDUTYHEAD>Cess</GSTRATEDUTYHEAD>
                                                            <GSTRATEVALUATIONTYPE>&#4; Not Applicable</GSTRATEVALUATIONTYPE>
                                                           </RATEDETAILS.LIST>
                                                           <RATEDETAILS.LIST>
                                                            <GSTRATEDUTYHEAD>State Cess</GSTRATEDUTYHEAD>
                                                            <GSTRATEVALUATIONTYPE>Based on Value</GSTRATEVALUATIONTYPE>
                                                           </RATEDETAILS.LIST>
                                                           <SUPPLEMENTARYDUTYHEADDETAILS.LIST>       </SUPPLEMENTARYDUTYHEADDETAILS.LIST>
                                                           <TAXOBJECTALLOCATIONS.LIST>       </TAXOBJECTALLOCATIONS.LIST>
                                                           <REFVOUCHERDETAILS.LIST>       </REFVOUCHERDETAILS.LIST>
                                                           <EXCISEALLOCATIONS.LIST>       </EXCISEALLOCATIONS.LIST>
                                                           <EXPENSEALLOCATIONS.LIST>       </EXPENSEALLOCATIONS.LIST>
                                                          </ALLINVENTORYENTRIES.LIST>
                                                    ";

                foreach (var product in applicant.InvoiceItemDetails)
                {
                    var qty = (Convert.ToInt32(product.qty) + " " + product.uom);
                    var rate = (Convert.ToInt32(product.rate) + "/" + product.uom);
                    productEntries.Append(productTemplate
                        .Replace("<BIND_DESCRIPTION>", product.productname.Trim().Replace("&", "&amp;"))
                        .Replace("<BIND_VOUCHERTYPE_LEDGER>", BIND_VOUCHERTYPE_LEDGER.Replace("&", "&amp;"))
                        .Replace("<BIND_REF_ORDERNAME>", BIND_REF_ORDERNAME.Replace("&", "&amp;"))
                        .Replace("<BIND_RATE>", (rate).ToString().Replace("&", "&amp;"))
                        .Replace("<BIND_AMOUNT>", (product.amount).ToString())
                        .Replace("<BIND_REF>", applicant.refno.Trim().Replace("&", "&amp;"))
                        .Replace("<BIND_CGST_ITEM>", product.cgst.ToString()) // Replace CGST value, formatted to 2 decimal
                        .Replace("<BIND_SGST_ITEM>", product.sgst.ToString()) // Replace SGST value, formatted to 2 decimal 
                        .Replace("<BIND_IGST_ITEM>", product.igst.ToString())
                        .Replace("<BIND_HSN>", product.hsn.Trim().Replace("&", "&amp;"))
                        .Replace("<BIND_QUANTITY>", qty))
                        .Replace("<BIND_CONSIGNEE_NAME>", "")
                        .Replace("{Currentdate}", applicant.invDate.Replace("-", ""));
                }

                xmlContent = xmlContent.Replace("<BIND_ALLINVENTORYENTRIES>", productEntries.ToString());


                //if frieght found  //<BIND_FREIGHT_DATA>
                if (Convert.ToInt32(applicant.fright) > 0)
                {
                    var freightdata = @"<LEDGERENTRIES.LIST>
                                           <OLDAUDITENTRYIDS.LIST TYPE=""Number"">
                                            <OLDAUDITENTRYIDS>-1</OLDAUDITENTRYIDS>
                                           </OLDAUDITENTRYIDS.LIST>
                                           <LEDGERNAME><BIND_LEDGER_TYPE></LEDGERNAME>
                                           <GSTCLASS>&#4; Not Applicable</GSTCLASS>
                                           <GSTOVRDNINELIGIBLEITC>&#4; Not Applicable</GSTOVRDNINELIGIBLEITC>
                                           <GSTOVRDNISREVCHARGEAPPL>&#4; Not Applicable</GSTOVRDNISREVCHARGEAPPL>
                                           <GSTOVRDNTAXABILITY>Taxable</GSTOVRDNTAXABILITY>
                                           <GSTSOURCETYPE>Ledger</GSTSOURCETYPE>
                                           <GSTLEDGERSOURCE><BIND_LEDGER_TYPE></GSTLEDGERSOURCE>
                                           <HSNSOURCETYPE>Company</HSNSOURCETYPE>
                                           <GSTOVRDNSTOREDNATURE/>
                                           <GSTOVRDNTYPEOFSUPPLY>Services</GSTOVRDNTYPEOFSUPPLY>
                                           <GSTRATEINFERAPPLICABILITY>As per Masters/Company</GSTRATEINFERAPPLICABILITY>
                                           <GSTHSNNAME>12345678</GSTHSNNAME>
                                           <GSTHSNDESCRIPTION>ATCPLGST</GSTHSNDESCRIPTION>
                                           <GSTHSNINFERAPPLICABILITY>As per Masters/Company</GSTHSNINFERAPPLICABILITY>
                                 <ISDEEMEDPOSITIVE>No</ISDEEMEDPOSITIVE>
                                           <LEDGERFROMITEM>No</LEDGERFROMITEM>
                                           <REMOVEZEROENTRIES>No</REMOVEZEROENTRIES>
                                           <ISPARTYLEDGER>No</ISPARTYLEDGER>
                                           <GSTOVERRIDDEN>No</GSTOVERRIDDEN>
                                           <ISGSTASSESSABLEVALUEOVERRIDDEN>No</ISGSTASSESSABLEVALUEOVERRIDDEN>
                                           <STRDISGSTAPPLICABLE>No</STRDISGSTAPPLICABLE>
                                           <STRDGSTISPARTYLEDGER>No</STRDGSTISPARTYLEDGER>
                                           <STRDGSTISDUTYLEDGER>No</STRDGSTISDUTYLEDGER>
                                           <CONTENTNEGISPOS>No</CONTENTNEGISPOS>
                               <ISLASTDEEMEDPOSITIVE>No</ISLASTDEEMEDPOSITIVE>
                                           <ISCAPVATTAXALTERED>No</ISCAPVATTAXALTERED>
                                           <ISCAPVATNOTCLAIMED>No</ISCAPVATNOTCLAIMED>
                                           <AMOUNT><BIND_LEDGER_VALUE></AMOUNT>
                                           <VATEXPAMOUNT><BIND_LEDGER_VALUE></VATEXPAMOUNT>
                                           <SERVICETAXDETAILS.LIST>       </SERVICETAXDETAILS.LIST>
                                           <BANKALLOCATIONS.LIST>       </BANKALLOCATIONS.LIST>
                                           <BILLALLOCATIONS.LIST>       </BILLALLOCATIONS.LIST>
                                           <INTERESTCOLLECTION.LIST>       </INTERESTCOLLECTION.LIST>
                                           <OLDAUDITENTRIES.LIST>       </OLDAUDITENTRIES.LIST>
                                           <ACCOUNTAUDITENTRIES.LIST>       </ACCOUNTAUDITENTRIES.LIST>
                                           <AUDITENTRIES.LIST>       </AUDITENTRIES.LIST>
                                           <INPUTCRALLOCS.LIST>       </INPUTCRALLOCS.LIST>
                                           <DUTYHEADDETAILS.LIST>       </DUTYHEADDETAILS.LIST>
                                           <EXCISEDUTYHEADDETAILS.LIST>       </EXCISEDUTYHEADDETAILS.LIST>
                                           <RATEDETAILS.LIST>
                                            <GSTRATEDUTYHEAD>CGST</GSTRATEDUTYHEAD>
                                            <GSTRATEVALUATIONTYPE>Based on Value</GSTRATEVALUATIONTYPE>
                                            <GSTRATE> 9</GSTRATE>
                                           </RATEDETAILS.LIST>
                                           <RATEDETAILS.LIST>
                                            <GSTRATEDUTYHEAD>SGST/UTGST</GSTRATEDUTYHEAD>
                                            <GSTRATEVALUATIONTYPE>Based on Value</GSTRATEVALUATIONTYPE>
                                            <GSTRATE> 9</GSTRATE>
                                           </RATEDETAILS.LIST>
                                           <RATEDETAILS.LIST>
                                            <GSTRATEDUTYHEAD>IGST</GSTRATEDUTYHEAD>
                                            <GSTRATEVALUATIONTYPE>Based on Value</GSTRATEVALUATIONTYPE>
                                            <GSTRATE> 18</GSTRATE>
                                           </RATEDETAILS.LIST>
                                           <RATEDETAILS.LIST>
                                            <GSTRATEDUTYHEAD>Cess</GSTRATEDUTYHEAD>
                                            <GSTRATEVALUATIONTYPE>&#4; Not Applicable</GSTRATEVALUATIONTYPE>
                                           </RATEDETAILS.LIST>
                                           <RATEDETAILS.LIST>
                                            <GSTRATEDUTYHEAD>State Cess</GSTRATEDUTYHEAD>
                                            <GSTRATEVALUATIONTYPE>Based on Value</GSTRATEVALUATIONTYPE>
                                           </RATEDETAILS.LIST>
                                           <SUMMARYALLOCS.LIST>       </SUMMARYALLOCS.LIST>
                                           <CENVATDUTYALLOCATIONS.LIST>       </CENVATDUTYALLOCATIONS.LIST>
                                           <STPYMTDETAILS.LIST>       </STPYMTDETAILS.LIST>
                                           <EXCISEPAYMENTALLOCATIONS.LIST>       </EXCISEPAYMENTALLOCATIONS.LIST>
                                           <TAXBILLALLOCATIONS.LIST>       </TAXBILLALLOCATIONS.LIST>
                                           <TAXOBJECTALLOCATIONS.LIST>       </TAXOBJECTALLOCATIONS.LIST>
                                           <TDSEXPENSEALLOCATIONS.LIST>       </TDSEXPENSEALLOCATIONS.LIST>
                                           <VATSTATUTORYDETAILS.LIST>       </VATSTATUTORYDETAILS.LIST>
                                           <COSTTRACKALLOCATIONS.LIST>       </COSTTRACKALLOCATIONS.LIST>
                                           <REFVOUCHERDETAILS.LIST>       </REFVOUCHERDETAILS.LIST>
                                           <INVOICEWISEDETAILS.LIST>       </INVOICEWISEDETAILS.LIST>
                                           <VATITCDETAILS.LIST>       </VATITCDETAILS.LIST>
                                           <ADVANCETAXDETAILS.LIST>       </ADVANCETAXDETAILS.LIST>
                                           <TAXTYPEALLOCATIONS.LIST>       </TAXTYPEALLOCATIONS.LIST>
                                          </LEDGERENTRIES.LIST>";
                    var count = 1;
                    for (int i = 1; i <= count; i++)
                    {
                        FreightEntries.Append(freightdata
                            .Replace("<BIND_LEDGER_TYPE>", applicant.Freight_type.Replace("&", "&amp;"))
                            //.Replace("<BIND_LEDGER_TYPE>", "Freight Charges of Purchase")
                            .Replace("<BIND_LEDGER_VALUE>", (applicant.fright).ToString()));
                    }
                    ;
                    xmlContent = xmlContent.Replace("<BIND_FREIGHT_DATA>", FreightEntries.ToString());
                }
                else
                {
                    xmlContent = xmlContent.Replace("<BIND_FREIGHT_DATA>", "");
                }
                //end

                //if gst found //<BIND_GST_DATA>
                if (applicant.gst_type == "sgst-cgst" || applicant.gst_type == "igst")
                {
                    var string_data = "";
                    var gstdata = @"<LEDGERENTRIES.LIST>
                                       <OLDAUDITENTRYIDS.LIST TYPE=""Number"">
                                        <OLDAUDITENTRYIDS>-1</OLDAUDITENTRYIDS>
                                       </OLDAUDITENTRYIDS.LIST>
                                       <APPROPRIATEFOR>&#4; Not Applicable</APPROPRIATEFOR>
                                       <ROUNDTYPE>&#4; Not Applicable</ROUNDTYPE>
                                       <LEDGERNAME><BIND_LEDGER_TYPE></LEDGERNAME>
                                       <GSTCLASS>&#4; Not Applicable</GSTCLASS>
                             <ISDEEMEDPOSITIVE>No</ISDEEMEDPOSITIVE>
                                       <LEDGERFROMITEM>No</LEDGERFROMITEM>
                                       <REMOVEZEROENTRIES>No</REMOVEZEROENTRIES>
                                       <ISPARTYLEDGER>No</ISPARTYLEDGER>
                                       <GSTOVERRIDDEN>No</GSTOVERRIDDEN>
                                       <ISGSTASSESSABLEVALUEOVERRIDDEN>No</ISGSTASSESSABLEVALUEOVERRIDDEN>
                                       <STRDISGSTAPPLICABLE>No</STRDISGSTAPPLICABLE>
                                       <STRDGSTISPARTYLEDGER>No</STRDGSTISPARTYLEDGER>
                                       <STRDGSTISDUTYLEDGER>No</STRDGSTISDUTYLEDGER>
                                       <CONTENTNEGISPOS>No</CONTENTNEGISPOS>
                              <ISLASTDEEMEDPOSITIVE>No</ISLASTDEEMEDPOSITIVE>
                                       <ISCAPVATTAXALTERED>No</ISCAPVATTAXALTERED>
                                        <ISCAPVATNOTCLAIMED>No</ISCAPVATNOTCLAIMED>
                                       <AMOUNT><BIND_LEDGER_VALUE></AMOUNT>
                                       <VATEXPAMOUNT><BIND_LEDGER_VALUE></VATEXPAMOUNT>
                                       <SERVICETAXDETAILS.LIST>       </SERVICETAXDETAILS.LIST>
                                       <BANKALLOCATIONS.LIST>       </BANKALLOCATIONS.LIST>
                                       <BILLALLOCATIONS.LIST>       </BILLALLOCATIONS.LIST>
                                       <INTERESTCOLLECTION.LIST>       </INTERESTCOLLECTION.LIST>
                                       <OLDAUDITENTRIES.LIST>       </OLDAUDITENTRIES.LIST>
                                       <ACCOUNTAUDITENTRIES.LIST>       </ACCOUNTAUDITENTRIES.LIST>
                                       <AUDITENTRIES.LIST>       </AUDITENTRIES.LIST>
                                       <INPUTCRALLOCS.LIST>       </INPUTCRALLOCS.LIST>
                                       <DUTYHEADDETAILS.LIST>       </DUTYHEADDETAILS.LIST>
                                       <EXCISEDUTYHEADDETAILS.LIST>       </EXCISEDUTYHEADDETAILS.LIST>
                                       <RATEDETAILS.LIST>       </RATEDETAILS.LIST>
                                       <SUMMARYALLOCS.LIST>       </SUMMARYALLOCS.LIST>
                                       <CENVATDUTYALLOCATIONS.LIST>       </CENVATDUTYALLOCATIONS.LIST>
                                       <STPYMTDETAILS.LIST>       </STPYMTDETAILS.LIST>
                                       <EXCISEPAYMENTALLOCATIONS.LIST>       </EXCISEPAYMENTALLOCATIONS.LIST>
                                       <TAXBILLALLOCATIONS.LIST>       </TAXBILLALLOCATIONS.LIST>
                                       <TAXOBJECTALLOCATIONS.LIST>       </TAXOBJECTALLOCATIONS.LIST>
                                       <TDSEXPENSEALLOCATIONS.LIST>       </TDSEXPENSEALLOCATIONS.LIST>
                                       <VATSTATUTORYDETAILS.LIST>       </VATSTATUTORYDETAILS.LIST>
                                       <COSTTRACKALLOCATIONS.LIST>       </COSTTRACKALLOCATIONS.LIST>
                                       <REFVOUCHERDETAILS.LIST>       </REFVOUCHERDETAILS.LIST>
                                       <INVOICEWISEDETAILS.LIST>       </INVOICEWISEDETAILS.LIST>
                                       <VATITCDETAILS.LIST>       </VATITCDETAILS.LIST>
                                       <ADVANCETAXDETAILS.LIST>       </ADVANCETAXDETAILS.LIST>
                                       <TAXTYPEALLOCATIONS.LIST>       </TAXTYPEALLOCATIONS.LIST>
                                      </LEDGERENTRIES.LIST>";
                    if (applicant.gst_type == "sgst-cgst")
                    {
                        var gstvalluuu = applicant.cgst;
                        //CGST
                        GSTEntries.Append(gstdata
                            .Replace("<BIND_LEDGER_TYPE>", "CGST") // Corrected usage of 'item'
                            .Replace("<BIND_LEDGER_VALUE>", (gstvalluuu).ToString())); // Assuming 'gstvalluuu' is a valid variable
                        xmlContent = xmlContent.Replace("<BIND_CGST_DATA>", GSTEntries.ToString());
                        xmlContent = xmlContent.Replace("<BIND_IGST_DATA>", "");
                        //END

                        //SGST
                        GSTEntries.Clear();
                        gstvalluuu = applicant.sgst;
                        GSTEntries.Append(gstdata
                            .Replace("<BIND_LEDGER_TYPE>", "SGST") // Corrected usage of 'item'
                            .Replace("<BIND_LEDGER_VALUE>", (gstvalluuu).ToString())); // Assuming 'gstvalluuu' is a valid variable
                        xmlContent = xmlContent.Replace("<BIND_SGST_DATA>", GSTEntries.ToString());
                        xmlContent = xmlContent.Replace("<BIND_IGST_DATA>", "");
                        //END
                    }
                    if (applicant.gst_type == "igst")
                    {
                        //IGST
                        var count = 1;
                        for (int i = 1; i <= count; i++)
                        {
                            GSTEntries.Append(gstdata
                                .Replace("<BIND_LEDGER_TYPE>", "IGST")
                                .Replace("<BIND_LEDGER_VALUE>", (applicant.igst).ToString()));
                            xmlContent = xmlContent.Replace("<BIND_IGST_DATA>", GSTEntries.ToString());
                            xmlContent = xmlContent.Replace("<BIND_CGST_DATA>", "");
                            xmlContent = xmlContent.Replace("<BIND_SGST_DATA>", "");
                        }
                        //END
                    }
                }
                else
                {
                    xmlContent = xmlContent.Replace("<BIND_CGST_DATA>", "");
                    xmlContent = xmlContent.Replace("<BIND_SGST_DATA>", "");
                    xmlContent = xmlContent.Replace("<BIND_IGST_DATA>", "");
                }
                //end
                xmlContent = xmlContent.Replace("<BIND_FREIGHT_DATA>", "");
                xmlContent = xmlContent.Replace("<BIND_CGST_DATA>", "");
                xmlContent = xmlContent.Replace("<BIND_SGST_DATA>", "");
                xmlContent = xmlContent.Replace("<BIND_IGST_DATA>", "");

                xmlContent = xmlContent.Replace("<BIND_NARRATION>", applicant.remark.Replace("&", "&amp;"));
                xmlContent = xmlContent.Replace("<BIND_CARRIERAGENT>", applicant.agent.Replace("&", "&amp;"));
                xmlContent = xmlContent.Replace("<BIND_BILLOFLADNING>", "");
                xmlContent = xmlContent.Replace("<BIND_DISPATCHTHEORUGH>", applicant.dispatchby.Replace("&", "&amp;"));
                xmlContent = xmlContent.Replace("<BIND_DESTIATION>", applicant.destination.Replace("&", "&amp;"));
                xmlContent = xmlContent.Replace("<BIND_OTHERREF>", applicant.refno.Trim().Replace("&", "&amp;"));
                xmlContent = xmlContent.Replace("<BIND_MH6666>", applicant.truckno.Trim().Replace("&", "&amp;"));
                xmlContent = xmlContent.Replace("<BIND_TERMOFPAYMENT>", applicant.paymentterm.Replace("&", "&amp;"));
                xmlContent = xmlContent.Replace("<BIND_TERMOFDILVERY>", applicant.termofdilivery.Replace("&", "&amp;"));
                xmlContent = xmlContent.Replace("<BIND_SUPPLIER_INVOICE>", applicant.invoiceno.Trim().Replace("&", "&amp;"));
                xmlContent = xmlContent.Replace("<BIND_INVOICE_DATE>", "");//20250103
                xmlContent = xmlContent.Replace("<BIND_BILLOFENTRYNO>", "");//20250103
                xmlContent = xmlContent.Replace("<BIND_PLACEOFRECEIPTBYSHIPPER>", "");//20250103
                xmlContent = xmlContent.Replace("<BIND_RECEIPT_DOC_NO>", "");//20250103
                xmlContent = xmlContent.Replace("<BIND_PORT_OF_LOADING>", "");//20250103
                xmlContent = xmlContent.Replace("<BIND_PORT_OF_DISCHARGE>", "");//20250103
                xmlContent = xmlContent.Replace("<BIND_REF_ORDERNAME>", BIND_REF_ORDERNAME.Replace("&", "&amp;"));
                xmlContent = xmlContent.Replace("<BIND_VOUCHERTYPE_LEDGER>", BIND_VOUCHERTYPE_LEDGER.Replace("&", "&amp;"));
                xmlContent = xmlContent.Replace("<BIND_REF>", applicant.refno.Trim().Replace("&", "&amp;"));
                xmlContent = xmlContent.Replace("<BIND_CONSIGNEE_NAME>", "");
                //xmlContent = xmlContent.Replace("&", "&amp;");

                //string safeXml = EscapeAmpersands(xmlContent);

                var request = new HttpRequestMessage(HttpMethod.Post, tallyUrl)
                {
                    Content = new StringContent(xmlContent, System.Text.Encoding.UTF8, "text/xml")
                };

                var response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();

                return responseContent;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while communicating with Tally.");
                throw;
            }
        }

        //public async Task<string> Save_Invoice(string xmlFilePath, Invoice voucher)
        //{
        //    try
        //    {
        //        DateTime parsedDate = DateTime.ParseExact(voucher.orderDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
        //        voucher.orderDate = parsedDate.ToString("yyyy-MM-dd");

        //        //TALLY UPDATE
        //        var BIND_VOUCHERTYPE_LEDGER = voucher.EntryType.Replace("&", "&amp;");
        //        //var BIND_VOUCHERTYPE = "Delivery Note";
        //        var BIND_VOUCHERTYPE = "Sales";
        //        var BIND_REF_ORDERNAME = "Sales Order";

        //        string tallyUrl = _configuration["TallySettings:TallyUrl"];
        //        if (string.IsNullOrWhiteSpace(tallyUrl))
        //        {
        //            throw new InvalidOperationException("Tally URL is not configured.");
        //        }
        //        if (!File.Exists(xmlFilePath))
        //        {
        //            throw new FileNotFoundException("The specified XML file was not found.", xmlFilePath);
        //        }
        //        string xmlContent = await File.ReadAllTextAsync(xmlFilePath);
        //        string addmoreitem = null;
        //        //ADDED

        //        string address = voucher.address.Trim();
        //        int maxLength = 100;
        //        List<string> addressChunks = new List<string>();
        //        for (int i = 0; i < address.Length; i += maxLength)
        //        {
        //            addressChunks.Add(address.Substring(i, Math.Min(maxLength, address.Length - i)).Trim());
        //        }
        //        var applicant = voucher;
        //        string companyName = _configuration["TallySettings:CompanyName"];
        //        xmlContent = xmlContent.Replace("{Cname}", companyName)
        //                                      .Replace("<BIND_PARTYNAME>", applicant.partyname.Trim().Replace("&", "&amp;"))
        //                                      .Replace("<BIND_ADD>", applicant.address.Replace("&", "&amp;"))
        //                                      .Replace("<BIND_ORDER_DT>", applicant.orderDate.Replace("-", ""))
        //                                      .Replace("<BIND_DC_DT>", applicant.dcDate.Replace("-", ""))
        //                                      .Replace("<BIND_IN_DT>", applicant.invDate.Replace("-", ""))
        //                                      .Replace("<BIND_REF>", applicant.refno.Replace("&", "&amp;"))
        //                                      .Replace("<BIND_GSTNO>", applicant.gstno.Replace("&", "&amp;"))
        //                                      .Replace("<BIND_VOUCHERTYPE>", BIND_VOUCHERTYPE.Replace("&", "&amp;"))
        //                                      .Replace("<BIND_REF_ORDERNAME>", BIND_REF_ORDERNAME.Replace("&", "&amp;"))
        //                                      .Replace("<BIND_COUNTRY>", applicant.country.Replace("&", "&amp;"))
        //                                      .Replace("<BIND_STATE>", applicant.state.Replace("&", "&amp;"))
        //                                      .Replace("<BIND_CON_COUNTRY>", "")
        //                                      .Replace("<BIND_CON_STATE>", "")
        //                                      .Replace("<BIND_CON_PINCODE>", "")
        //                                      .Replace("<BIND_CONSIGNEEGST>", "")
        //                                      .Replace("<BIND_COURIER>", "NA")
        //                                      .Replace("<BIND_DC_NO>", applicant.dcno.Trim().Replace("&", "&amp;"))
        //                                      .Replace("<BIND_TOP>", "0")
        //                                      .Replace("<BIND_TRUCKNO>", applicant.truckno.Trim().Replace("&", "&amp;"))
        //                                      .Replace("<BIND_BASEAMOUNT>", "-" + (applicant.FinalAmount).ToString())
        //                                      .Replace("<BIND_DISPATCHEDBY>", applicant.dispatchby.Replace("&", "&amp;"))
        //                                      .Replace("<BIND_REF_ORDERNAME>", BIND_REF_ORDERNAME.Replace("&", "&amp;"));

        //        StringBuilder productEntries = new StringBuilder();
        //        StringBuilder LEDFEREntries = new StringBuilder();
        //        StringBuilder FreightEntries = new StringBuilder();
        //        StringBuilder GSTEntries = new StringBuilder();

        //        string productTemplate = @"<ALLINVENTORYENTRIES.LIST>
        //                                   <STOCKITEMNAME><BIND_DESCRIPTION></STOCKITEMNAME>
        //                                   <GSTOVRDNINELIGIBLEITC>&#4; Not Applicable</GSTOVRDNINELIGIBLEITC>
        //                                   <GSTOVRDNISREVCHARGEAPPL>&#4; Not Applicable</GSTOVRDNISREVCHARGEAPPL>
        //                                   <GSTOVRDNTAXABILITY>Taxable</GSTOVRDNTAXABILITY>
        //                                   <GSTSOURCETYPE>Stock Item</GSTSOURCETYPE>
        //                                   <GSTITEMSOURCE><BIND_DESCRIPTION></GSTITEMSOURCE>
        //                                   <HSNSOURCETYPE>Stock Item</HSNSOURCETYPE>
        //                                   <HSNITEMSOURCE><BIND_DESCRIPTION></HSNITEMSOURCE>
        //                                   <GSTOVRDNSTOREDNATURE/>
        //                                   <GSTOVRDNTYPEOFSUPPLY>Goods</GSTOVRDNTYPEOFSUPPLY>
        //                                   <GSTRATEINFERAPPLICABILITY>As per Masters/Company</GSTRATEINFERAPPLICABILITY>
        //                                   <GSTHSNINFERAPPLICABILITY>As per Masters/Company</GSTHSNINFERAPPLICABILITY>
        //                                   <ISDEEMEDPOSITIVE>No</ISDEEMEDPOSITIVE>
        //                                   <ISGSTASSESSABLEVALUEOVERRIDDEN>No</ISGSTASSESSABLEVALUEOVERRIDDEN>
        //                                   <STRDISGSTAPPLICABLE>No</STRDISGSTAPPLICABLE>
        //                                   <CONTENTNEGISPOS>No</CONTENTNEGISPOS>
        //                                   <ISLASTDEEMEDPOSITIVE>No</ISLASTDEEMEDPOSITIVE>
        //                                   <ISAUTONEGATE>No</ISAUTONEGATE>
        //                                   <ISCUSTOMSCLEARANCE>No</ISCUSTOMSCLEARANCE>
        //                                   <ISTRACKCOMPONENT>No</ISTRACKCOMPONENT>
        //                                   <ISTRACKPRODUCTION>No</ISTRACKPRODUCTION>
        //                                   <ISPRIMARYITEM>No</ISPRIMARYITEM>
        //                                   <ISSCRAP>No</ISSCRAP>
        //                                   <RATE><BIND_RATE></RATE>
        //                                   <AMOUNT><BIND_AMOUNT></AMOUNT>
        //                                   <ACTUALQTY><BIND_QUANTITY></ACTUALQTY>
        //                                   <BILLEDQTY><BIND_QUANTITY></BILLEDQTY>
        //                                   <BATCHALLOCATIONS.LIST>
        //                                    <GODOWNNAME>Main Location</GODOWNNAME>
        //                                    <BATCHNAME>Primary Batch</BATCHNAME>
        //                                    <INDENTNO>&#4; Not Applicable</INDENTNO>
        //                                    <ORDERNO><BIND_REF></ORDERNO>
        //                                    <TRACKINGNUMBER>&#4; Not Applicable</TRACKINGNUMBER>
        //                                    <DYNAMICCSTISCLEARED>No</DYNAMICCSTISCLEARED>
        //                                    <AMOUNT><BIND_QUANTITY></AMOUNT>
        //                                    <ACTUALQTY><BIND_QUANTITY></ACTUALQTY>
        //                                    <BILLEDQTY><BIND_QUANTITY></BILLEDQTY>
        //                                    <ORDERDUEDATE JD=""45747"" P=""{Currentdate}"">{Currentdate}</ORDERDUEDATE>
        //                                    <ADDITIONALDETAILS.LIST>        </ADDITIONALDETAILS.LIST>
        //                                    <VOUCHERCOMPONENTLIST.LIST>        </VOUCHERCOMPONENTLIST.LIST>
        //                                   </BATCHALLOCATIONS.LIST>
        //                                   <ACCOUNTINGALLOCATIONS.LIST>
        //                                    <OLDAUDITENTRYIDS.LIST TYPE=""Number"">
        //                                     <OLDAUDITENTRYIDS>-1</OLDAUDITENTRYIDS>
        //                                    </OLDAUDITENTRYIDS.LIST>
        //                                    <LEDGERNAME><BIND_VOUCHERTYPE_LEDGER></LEDGERNAME>
        //                                    <GSTCLASS>&#4; Not Applicable</GSTCLASS>
        //                                    <ISDEEMEDPOSITIVE>No</ISDEEMEDPOSITIVE>
        //                                    <LEDGERFROMITEM>No</LEDGERFROMITEM>
        //                                    <REMOVEZEROENTRIES>No</REMOVEZEROENTRIES>
        //                                    <ISPARTYLEDGER>No</ISPARTYLEDGER>
        //                                    <GSTOVERRIDDEN>No</GSTOVERRIDDEN>
        //                                    <ISGSTASSESSABLEVALUEOVERRIDDEN>No</ISGSTASSESSABLEVALUEOVERRIDDEN>
        //                                    <STRDISGSTAPPLICABLE>No</STRDISGSTAPPLICABLE>
        //                                    <STRDGSTISPARTYLEDGER>No</STRDGSTISPARTYLEDGER>
        //                                    <STRDGSTISDUTYLEDGER>No</STRDGSTISDUTYLEDGER>
        //                                    <CONTENTNEGISPOS>No</CONTENTNEGISPOS>
        //                                    <ISLASTDEEMEDPOSITIVE>No</ISLASTDEEMEDPOSITIVE>
        //                                    <ISCAPVATTAXALTERED>No</ISCAPVATTAXALTERED>
        //                                    <ISCAPVATNOTCLAIMED>No</ISCAPVATNOTCLAIMED>
        //                                    <AMOUNT><BIND_AMOUNT></AMOUNT>
        //                                    <SERVICETAXDETAILS.LIST>        </SERVICETAXDETAILS.LIST>
        //                                    <BANKALLOCATIONS.LIST>        </BANKALLOCATIONS.LIST>
        //                                    <BILLALLOCATIONS.LIST>        </BILLALLOCATIONS.LIST>
        //                                    <INTERESTCOLLECTION.LIST>        </INTERESTCOLLECTION.LIST>
        //                                    <OLDAUDITENTRIES.LIST>        </OLDAUDITENTRIES.LIST>
        //                                    <ACCOUNTAUDITENTRIES.LIST>        </ACCOUNTAUDITENTRIES.LIST>
        //                                    <AUDITENTRIES.LIST>        </AUDITENTRIES.LIST>
        //                                    <INPUTCRALLOCS.LIST>        </INPUTCRALLOCS.LIST>
        //                                    <DUTYHEADDETAILS.LIST>        </DUTYHEADDETAILS.LIST>
        //                                    <EXCISEDUTYHEADDETAILS.LIST>        </EXCISEDUTYHEADDETAILS.LIST>
        //                                    <RATEDETAILS.LIST>        </RATEDETAILS.LIST>
        //                                    <SUMMARYALLOCS.LIST>        </SUMMARYALLOCS.LIST>
        //                                    <CENVATDUTYALLOCATIONS.LIST>        </CENVATDUTYALLOCATIONS.LIST>
        //                                    <STPYMTDETAILS.LIST>        </STPYMTDETAILS.LIST>
        //                                    <EXCISEPAYMENTALLOCATIONS.LIST>        </EXCISEPAYMENTALLOCATIONS.LIST>
        //                                    <TAXBILLALLOCATIONS.LIST>        </TAXBILLALLOCATIONS.LIST>
        //                                    <TAXOBJECTALLOCATIONS.LIST>        </TAXOBJECTALLOCATIONS.LIST>
        //                                    <TDSEXPENSEALLOCATIONS.LIST>        </TDSEXPENSEALLOCATIONS.LIST>
        //                                    <VATSTATUTORYDETAILS.LIST>        </VATSTATUTORYDETAILS.LIST>
        //                                    <COSTTRACKALLOCATIONS.LIST>        </COSTTRACKALLOCATIONS.LIST>
        //                                    <REFVOUCHERDETAILS.LIST>        </REFVOUCHERDETAILS.LIST>
        //                                    <INVOICEWISEDETAILS.LIST>        </INVOICEWISEDETAILS.LIST>
        //                                    <VATITCDETAILS.LIST>        </VATITCDETAILS.LIST>
        //                                    <ADVANCETAXDETAILS.LIST>        </ADVANCETAXDETAILS.LIST>
        //                                    <TAXTYPEALLOCATIONS.LIST>        </TAXTYPEALLOCATIONS.LIST>
        //                                   </ACCOUNTINGALLOCATIONS.LIST>
        //                                   <DUTYHEADDETAILS.LIST>       </DUTYHEADDETAILS.LIST>
        //                                   <RATEDETAILS.LIST>
        //                                    <GSTRATEDUTYHEAD>CGST</GSTRATEDUTYHEAD>
        //                                    <GSTRATEVALUATIONTYPE>Based on Value</GSTRATEVALUATIONTYPE>
        //                                   </RATEDETAILS.LIST>
        //                                   <RATEDETAILS.LIST>
        //                                    <GSTRATEDUTYHEAD>SGST/UTGST</GSTRATEDUTYHEAD>
        //                                    <GSTRATEVALUATIONTYPE>Based on Value</GSTRATEVALUATIONTYPE>
        //                                   </RATEDETAILS.LIST>
        //                                   <RATEDETAILS.LIST>
        //                                    <GSTRATEDUTYHEAD>IGST</GSTRATEDUTYHEAD>
        //                                    <GSTRATEVALUATIONTYPE>Based on Value</GSTRATEVALUATIONTYPE>
        //                                   </RATEDETAILS.LIST>
        //                                   <RATEDETAILS.LIST>
        //                                    <GSTRATEDUTYHEAD>Cess</GSTRATEDUTYHEAD>
        //                                    <GSTRATEVALUATIONTYPE>&#4; Not Applicable</GSTRATEVALUATIONTYPE>
        //                                   </RATEDETAILS.LIST>
        //                                   <RATEDETAILS.LIST>
        //                                    <GSTRATEDUTYHEAD>State Cess</GSTRATEDUTYHEAD>
        //                                    <GSTRATEVALUATIONTYPE>Based on Value</GSTRATEVALUATIONTYPE>
        //                                   </RATEDETAILS.LIST>
        //                                   <SUPPLEMENTARYDUTYHEADDETAILS.LIST>       </SUPPLEMENTARYDUTYHEADDETAILS.LIST>
        //                                   <TAXOBJECTALLOCATIONS.LIST>       </TAXOBJECTALLOCATIONS.LIST>
        //                                   <REFVOUCHERDETAILS.LIST>       </REFVOUCHERDETAILS.LIST>
        //                                   <EXCISEALLOCATIONS.LIST>       </EXCISEALLOCATIONS.LIST>
        //                                   <EXPENSEALLOCATIONS.LIST>       </EXPENSEALLOCATIONS.LIST>
        //                                  </ALLINVENTORYENTRIES.LIST>";

        //        foreach (var product in applicant.InvoiceItemDetails)
        //        {
        //            var qty = (Convert.ToInt32(product.qty) + " " + product.uom);
        //            var rate = (Convert.ToInt32(product.rate) + "/" + product.uom);
        //            productEntries.Append(productTemplate
        //                .Replace("<BIND_DESCRIPTION>", product.productname.Trim().Replace("&", "&amp;"))
        //                .Replace("<BIND_VOUCHERTYPE_LEDGER>", BIND_VOUCHERTYPE_LEDGER)
        //                .Replace("<BIND_REF_ORDERNAME>", BIND_REF_ORDERNAME)
        //                .Replace("<BIND_RATE>", (rate).ToString())
        //                .Replace("<BIND_AMOUNT>", (product.amount).ToString())
        //                .Replace("<BIND_REF>", applicant.refno.Trim())
        //                .Replace("<BIND_CGST_ITEM>", product.cgst.ToString()) // Replace CGST value, formatted to 2 decimal
        //                .Replace("<BIND_SGST_ITEM>", product.sgst.ToString()) // Replace SGST value, formatted to 2 decimal 
        //                .Replace("<BIND_IGST_ITEM>", product.igst.ToString())
        //                .Replace("<BIND_HSN>", product.hsn.Trim())
        //                .Replace("<BIND_QUANTITY>", qty))
        //                .Replace("<BIND_CONSIGNEE_NAME>", "")
        //                .Replace("{Currentdate}", applicant.invDate.Replace("-", ""));
        //        }

        //        xmlContent = xmlContent.Replace("<BIND_ALLINVENTORYENTRIES>", productEntries.ToString());
        //        xmlContent = xmlContent.Replace("<BIND_NARRATION>", applicant.remark.Replace("&", "&amp;"));
        //        xmlContent = xmlContent.Replace("<BIND_CARRIERAGENT>", applicant.agent.Replace("&", "&amp;"));
        //        xmlContent = xmlContent.Replace("<BIND_BILLOFLADNING>", "");
        //        xmlContent = xmlContent.Replace("<BIND_DISPATCHTHEORUGH>", applicant.dispatchby.Replace("&", "&amp;"));
        //        xmlContent = xmlContent.Replace("<BIND_DESTIATION>", applicant.destination.Replace("&", "&amp;"));
        //        xmlContent = xmlContent.Replace("<BIND_OTHERREF>", applicant.refno.Trim().Replace("&", "&amp;"));
        //        xmlContent = xmlContent.Replace("<BIND_MH6666>", applicant.truckno.Trim().Replace("&", "&amp;"));
        //        xmlContent = xmlContent.Replace("<BIND_TERMOFPAYMENT>", applicant.paymentterm.Replace("&", "&amp;"));
        //        xmlContent = xmlContent.Replace("<BIND_TERMOFDILVERY>", applicant.termofdilivery.Replace("&", "&amp;"));
        //        xmlContent = xmlContent.Replace("<BIND_SUPPLIER_INVOICE>", applicant.invoiceno.Trim().Replace("&", "&amp;"));
        //        xmlContent = xmlContent.Replace("<BIND_INVOICE_DATE>", "");//20250103
        //        xmlContent = xmlContent.Replace("<BIND_BILLOFENTRYNO>", "");//20250103
        //        xmlContent = xmlContent.Replace("<BIND_PLACEOFRECEIPTBYSHIPPER>", "");//20250103
        //        xmlContent = xmlContent.Replace("<BIND_RECEIPT_DOC_NO>", "");//20250103
        //        xmlContent = xmlContent.Replace("<BIND_PORT_OF_LOADING>", "");//20250103
        //        xmlContent = xmlContent.Replace("<BIND_PORT_OF_DISCHARGE>", "");//20250103
        //        xmlContent = xmlContent.Replace("<BIND_REF_ORDERNAME>", BIND_REF_ORDERNAME.Replace("&", "&amp;"));
        //        xmlContent = xmlContent.Replace("<BIND_VOUCHERTYPE_LEDGER>", BIND_VOUCHERTYPE_LEDGER.Replace("&", "&amp;"));
        //        xmlContent = xmlContent.Replace("<BIND_REF>", applicant.refno.Trim().Replace("&", "&amp;"));
        //        xmlContent = xmlContent.Replace("<BIND_CONSIGNEE_NAME>", "");
        //        //xmlContent = xmlContent.Replace("&", "&amp;");

        //        var request = new HttpRequestMessage(HttpMethod.Post, tallyUrl)
        //        {
        //            Content = new StringContent(xmlContent, System.Text.Encoding.UTF8, "text/xml")
        //        };

        //        var response = await _httpClient.SendAsync(request);
        //        response.EnsureSuccessStatusCode();

        //        var responseContent = await response.Content.ReadAsStringAsync();

        //        return responseContent;
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "An error occurred while communicating with Tally.");
        //        throw;
        //    }
        //}

        public async Task<string> Save_Delievery(string xmlFilePath, Invoice voucher)
        {
            try
            {
                DateTime parsedDate = DateTime.ParseExact(voucher.orderDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                voucher.orderDate = parsedDate.ToString("yyyy-MM-dd");

                var BIND_VOUCHERTYPE = voucher.VoucherType.Replace("&", "&amp;");
                var BIND_LEDGERTYPE = voucher.EntryType.Replace("&", "&amp;");

                string tallyUrl = _configuration["TallySettings:TallyUrl"];
                if (string.IsNullOrWhiteSpace(tallyUrl))
                {
                    throw new InvalidOperationException("Tally URL is not configured.");
                }
                if (!File.Exists(xmlFilePath))
                {
                    throw new FileNotFoundException("The specified XML file was not found.", xmlFilePath);
                }
                string xmlContent = await File.ReadAllTextAsync(xmlFilePath);
                string addmoreitem = null;

                string address = voucher.address.Trim();
                int maxLength = 100;
                List<string> addressChunks = new List<string>();
                for (int i = 0; i < address.Length; i += maxLength)
                {
                    addressChunks.Add(address.Substring(i, Math.Min(maxLength, address.Length - i)).Trim());
                }
                var applicant = voucher;
                string companyName = _configuration["TallySettings:CompanyName"];
                xmlContent = xmlContent.Replace("{Cname}", companyName)
                                              .Replace("<BIND_PARTYNAME>", applicant.partyname.Trim().Replace("&", "&amp;"))
                                              .Replace("<BIND_ADD>", applicant.address.Replace("&", "&amp;"))
                                              .Replace("<BIND_ORDER_DT>", applicant.orderDate.Replace("-", ""))
                                              .Replace("<BIND_DC_DT>", applicant.orderDate.Replace("-", ""))
                                              .Replace("<BIND_IN_DT>", applicant.orderDate.Replace("-", ""))
                                              .Replace("<BIND_REF>", applicant.refno.Replace("&", "&amp;"))
                                              .Replace("<BIND_GSTNO>", applicant.gstno.Replace("&", "&amp;"))
                                              .Replace("<BIND_VOUCHERTYPE>", BIND_VOUCHERTYPE.Replace("&", "&amp;"))
                                              .Replace("<BIND_LEDGERTYPE>", BIND_LEDGERTYPE.Replace("&", "&amp;"))
                                              .Replace("<BIND_COURIER>", "NA")
                                              .Replace("<BIND_DC_NO>", applicant.dcno.Trim().Replace("&", "&amp;"))
                                              .Replace("<BIND_TOP>", "0")
                                              .Replace("<BIND_TRUCKNO>", applicant.truckno.Trim().Replace("&", "&amp;"))
                                              .Replace("<BIND_BASEAMOUNT>", "-" + (applicant.FinalAmount).ToString())
                                              .Replace("<BIND_DISPATCHEDBY>", applicant.dispatchby.Replace("&", "&amp;"));

                StringBuilder productEntries22 = new StringBuilder();
                StringBuilder LEDFEREntries = new StringBuilder();
                StringBuilder FreightEntries = new StringBuilder();
                StringBuilder GSTEntries = new StringBuilder();

                string productTemplate = @"      <ALLINVENTORYENTRIES.LIST>
								<STOCKITEMNAME><BIND_DESCRIPTION></STOCKITEMNAME>
								<GSTOVRDNISREVCHARGEAPPL>&#4; Not Applicable</GSTOVRDNISREVCHARGEAPPL>
								<GSTOVRDNTAXABILITY>Taxable</GSTOVRDNTAXABILITY>
								<GSTSOURCETYPE>Stock Item</GSTSOURCETYPE>
								<GSTITEMSOURCE><BIND_DESCRIPTION></GSTITEMSOURCE>
								<HSNSOURCETYPE>Stock Item</HSNSOURCETYPE>
								<HSNITEMSOURCE><BIND_DESCRIPTION></HSNITEMSOURCE>
								<GSTRATEINFERAPPLICABILITY>As per Masters/Company</GSTRATEINFERAPPLICABILITY>
								<GSTHSNINFERAPPLICABILITY>As per Masters/Company</GSTHSNINFERAPPLICABILITY>
								<ISDEEMEDPOSITIVE>No</ISDEEMEDPOSITIVE>
								<ISGSTASSESSABLEVALUEOVERRIDDEN>No</ISGSTASSESSABLEVALUEOVERRIDDEN>
								<STRDISGSTAPPLICABLE>No</STRDISGSTAPPLICABLE>
								<CONTENTNEGISPOS>No</CONTENTNEGISPOS>
								<ISLASTDEEMEDPOSITIVE>No</ISLASTDEEMEDPOSITIVE>
								<ISAUTONEGATE>No</ISAUTONEGATE>
								<ISCUSTOMSCLEARANCE>No</ISCUSTOMSCLEARANCE>
								<ISTRACKCOMPONENT>No</ISTRACKCOMPONENT>
								<ISTRACKPRODUCTION>No</ISTRACKPRODUCTION>
								<ISPRIMARYITEM>No</ISPRIMARYITEM>
								<ISSCRAP>No</ISSCRAP>
								<RATE><BIND_RATE></RATE>
								<AMOUNT><BIND_AMOUNT></AMOUNT>
								<ACTUALQTY><BIND_QUANTITY></ACTUALQTY>
								<BILLEDQTY><BIND_QUANTITY></BILLEDQTY>
								<BATCHALLOCATIONS.LIST>
									<GODOWNNAME>Main Location</GODOWNNAME>
									<BATCHNAME>Primary Batch</BATCHNAME>
									<INDENTNO>&#4; Not Applicable</INDENTNO>
									<ORDERNO>&#4; Not Applicable</ORDERNO>
									<TRACKINGNUMBER>&#4; Not Applicable</TRACKINGNUMBER>
									<DYNAMICCSTISCLEARED>No</DYNAMICCSTISCLEARED>
									<AMOUNT><BIND_AMOUNT></AMOUNT>
									<ACTUALQTY><BIND_QUANTITY></ACTUALQTY>
									<BILLEDQTY><BIND_QUANTITY></BILLEDQTY>
									<ADDITIONALDETAILS.LIST>        </ADDITIONALDETAILS.LIST>
									<VOUCHERCOMPONENTLIST.LIST>        </VOUCHERCOMPONENTLIST.LIST>
								</BATCHALLOCATIONS.LIST>
								<ACCOUNTINGALLOCATIONS.LIST>
									<OLDAUDITENTRYIDS.LIST TYPE=""Number"">
										<OLDAUDITENTRYIDS>-1</OLDAUDITENTRYIDS>
									</OLDAUDITENTRYIDS.LIST>
									<LEDGERNAME><BIND_LEDGERTYPE></LEDGERNAME>
									<GSTCLASS>&#4; Not Applicable</GSTCLASS>
									<ISDEEMEDPOSITIVE>No</ISDEEMEDPOSITIVE>
									<LEDGERFROMITEM>No</LEDGERFROMITEM>
									<REMOVEZEROENTRIES>No</REMOVEZEROENTRIES>
									<ISPARTYLEDGER>No</ISPARTYLEDGER>
									<GSTOVERRIDDEN>No</GSTOVERRIDDEN>
									<ISGSTASSESSABLEVALUEOVERRIDDEN>No</ISGSTASSESSABLEVALUEOVERRIDDEN>
									<STRDISGSTAPPLICABLE>No</STRDISGSTAPPLICABLE>
									<STRDGSTISPARTYLEDGER>No</STRDGSTISPARTYLEDGER>
									<STRDGSTISDUTYLEDGER>No</STRDGSTISDUTYLEDGER>
									<CONTENTNEGISPOS>No</CONTENTNEGISPOS>
									<ISLASTDEEMEDPOSITIVE>No</ISLASTDEEMEDPOSITIVE>
									<ISCAPVATTAXALTERED>No</ISCAPVATTAXALTERED>
									<ISCAPVATNOTCLAIMED>No</ISCAPVATNOTCLAIMED>
									<AMOUNT><BIND_AMOUNT></AMOUNT>
									<SERVICETAXDETAILS.LIST>        </SERVICETAXDETAILS.LIST>
									<BANKALLOCATIONS.LIST>        </BANKALLOCATIONS.LIST>
									<BILLALLOCATIONS.LIST>        </BILLALLOCATIONS.LIST>
									<INTERESTCOLLECTION.LIST>        </INTERESTCOLLECTION.LIST>
									<OLDAUDITENTRIES.LIST>        </OLDAUDITENTRIES.LIST>
									<ACCOUNTAUDITENTRIES.LIST>        </ACCOUNTAUDITENTRIES.LIST>
									<AUDITENTRIES.LIST>        </AUDITENTRIES.LIST>
									<INPUTCRALLOCS.LIST>        </INPUTCRALLOCS.LIST>
									<DUTYHEADDETAILS.LIST>        </DUTYHEADDETAILS.LIST>
									<EXCISEDUTYHEADDETAILS.LIST>        </EXCISEDUTYHEADDETAILS.LIST>
									<RATEDETAILS.LIST>        </RATEDETAILS.LIST>
									<SUMMARYALLOCS.LIST>        </SUMMARYALLOCS.LIST>
									<CENVATDUTYALLOCATIONS.LIST>        </CENVATDUTYALLOCATIONS.LIST>
									<STPYMTDETAILS.LIST>        </STPYMTDETAILS.LIST>
									<EXCISEPAYMENTALLOCATIONS.LIST>        </EXCISEPAYMENTALLOCATIONS.LIST>
									<TAXBILLALLOCATIONS.LIST>        </TAXBILLALLOCATIONS.LIST>
									<TAXOBJECTALLOCATIONS.LIST>        </TAXOBJECTALLOCATIONS.LIST>
									<TDSEXPENSEALLOCATIONS.LIST>        </TDSEXPENSEALLOCATIONS.LIST>
									<VATSTATUTORYDETAILS.LIST>        </VATSTATUTORYDETAILS.LIST>
									<COSTTRACKALLOCATIONS.LIST>        </COSTTRACKALLOCATIONS.LIST>
									<REFVOUCHERDETAILS.LIST>        </REFVOUCHERDETAILS.LIST>
									<INVOICEWISEDETAILS.LIST>        </INVOICEWISEDETAILS.LIST>
									<VATITCDETAILS.LIST>        </VATITCDETAILS.LIST>
									<ADVANCETAXDETAILS.LIST>        </ADVANCETAXDETAILS.LIST>
									<TAXTYPEALLOCATIONS.LIST>        </TAXTYPEALLOCATIONS.LIST>
								</ACCOUNTINGALLOCATIONS.LIST>
								<DUTYHEADDETAILS.LIST>       </DUTYHEADDETAILS.LIST>
								<RATEDETAILS.LIST>
									<GSTRATEDUTYHEAD>CGST</GSTRATEDUTYHEAD>
									<GSTRATEVALUATIONTYPE>Based on Value</GSTRATEVALUATIONTYPE>
								</RATEDETAILS.LIST>
								<RATEDETAILS.LIST>
									<GSTRATEDUTYHEAD>SGST/UTGST</GSTRATEDUTYHEAD>
									<GSTRATEVALUATIONTYPE>Based on Value</GSTRATEVALUATIONTYPE>
								</RATEDETAILS.LIST>
								<RATEDETAILS.LIST>
									<GSTRATEDUTYHEAD>IGST</GSTRATEDUTYHEAD>
									<GSTRATEVALUATIONTYPE>Based on Value</GSTRATEVALUATIONTYPE>
								</RATEDETAILS.LIST>
								<RATEDETAILS.LIST>
									<GSTRATEDUTYHEAD>Cess</GSTRATEDUTYHEAD>
									<GSTRATEVALUATIONTYPE>&#4; Not Applicable</GSTRATEVALUATIONTYPE>
								</RATEDETAILS.LIST>
								<RATEDETAILS.LIST>
									<GSTRATEDUTYHEAD>State Cess</GSTRATEDUTYHEAD>
									<GSTRATEVALUATIONTYPE>Based on Value</GSTRATEVALUATIONTYPE>
								</RATEDETAILS.LIST>
								<SUPPLEMENTARYDUTYHEADDETAILS.LIST>       </SUPPLEMENTARYDUTYHEADDETAILS.LIST>
								<TAXOBJECTALLOCATIONS.LIST>       </TAXOBJECTALLOCATIONS.LIST>
								<REFVOUCHERDETAILS.LIST>       </REFVOUCHERDETAILS.LIST>
								<EXCISEALLOCATIONS.LIST>       </EXCISEALLOCATIONS.LIST>
								<EXPENSEALLOCATIONS.LIST>       </EXPENSEALLOCATIONS.LIST>
							</ALLINVENTORYENTRIES.LIST>";

                foreach (var product in applicant.InvoiceItemDetails)
                {
                    string VALUE = $"{product.UNIT} {product.uom}";

                    //var qty = (Convert.ToInt32(product.UNIT) + " " + product.uom);
                    var ratee = $"{product.rate}/{product.uom}";

                    //var rate = (Convert.ToInt32(product.UNIT) + "/" + product.uom);
                    productEntries22.Append(productTemplate
                        .Replace("<BIND_DESCRIPTION>", product.productname.Trim().Replace("&", "&amp;"))
                        .Replace("<BIND_RATE>", (ratee).ToString())
                        .Replace("<BIND_AMOUNT>", (product.amount).ToString())
                        .Replace("<BIND_QUANTITY>", VALUE))
                        .Replace("{Currentdate}", applicant.orderDate.Replace("-", ""));

                   /* string VALUE = $"{product.UNIT} {product.uom}";
                    var ratee = $"{product.UNIT}/{product.uom}";

                    productEntries22.Append(productTemplate
                        .Replace("<BIND_DESCRIPTION>", product.productname.Trim().Replace("&", "&amp;"))
                        .Replace("<BIND_RATE>", ratee)
                        .Replace("<BIND_AMOUNT>", (product.UNIT * product.RatePerUnit).ToString()) // Optional: auto-calculate amount
                        .Replace("<BIND_QUANTITY>", VALUE));*/
                }

                xmlContent = xmlContent.Replace("<BIND_ALLINVENTORYENTRIES>", productEntries22.ToString());
                xmlContent = xmlContent.Replace("<BIND_NARRATION>", applicant.remark.Replace("&", "&amp;"));
                xmlContent = xmlContent.Replace("<BIND_CARRIERAGENT>", applicant.agent.Replace("&", "&amp;"));
                xmlContent = xmlContent.Replace("<BIND_BILLOFLADNING>", "");
                xmlContent = xmlContent.Replace("<BIND_DISPATCHTHEORUGH>", applicant.dispatchby.Replace("&", "&amp;"));
                xmlContent = xmlContent.Replace("<BIND_DESTINATION>", applicant.destination.Replace("&", "&amp;"));
                xmlContent = xmlContent.Replace("<BIND_OTHERREF>", applicant.refno.Trim().Replace("&", "&amp;"));
                xmlContent = xmlContent.Replace("<BIND_MH6666>", applicant.truckno.Trim().Replace("&", "&amp;"));
                xmlContent = xmlContent.Replace("<BIND_TERMOFPAYMENT>", applicant.paymentterm.Replace("&", "&amp;"));
                xmlContent = xmlContent.Replace("<BIND_TERMOFDILVERY>", applicant.termofdilivery.Replace("&", "&amp;"));
                xmlContent = xmlContent.Replace("<BIND_SUPPLIER_INVOICE>", applicant.invoiceno.Trim().Replace("&", "&amp;"));
                xmlContent = xmlContent.Replace("<BIND_INVOICE_DATE>", "");//20250103
                xmlContent = xmlContent.Replace("<BIND_BILLOFLOADING>", "");//20250103
                xmlContent = xmlContent.Replace("<BIND_DISPATCHDOCNO>", "");//20250103
                xmlContent = xmlContent.Replace("<BIND_REF>", applicant.refno.Trim().Replace("&", "&amp;"));
                xmlContent = xmlContent.Replace("<BIND_CONSIGNEE_NAME>", "");
                xmlContent = xmlContent.Replace("<BIND_LEDGERTYPE>", applicant.EntryType.Trim().Replace("&", "&amp;"));

                var request = new HttpRequestMessage(HttpMethod.Post, tallyUrl)
                {
                    Content = new StringContent(xmlContent, System.Text.Encoding.UTF8, "text/xml")
                };

                var response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var responseContent = await response.Content.ReadAsStringAsync();

                return responseContent;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while communicating with Tally.");
                throw;
            }
        }

        string EscapeAmpersands(string xml)
        {
            // Replace & only if it's NOT followed by one of the valid XML entities
            return Regex.Replace(xml, @"&(?!amp;|lt;|gt;|apos;|quot;)", "&amp;");
        }
    }
}
