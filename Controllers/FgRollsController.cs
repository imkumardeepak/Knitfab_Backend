using AvyyanBackend.Data;
using AvyyanBackend.Models.ProAllot;
using AvyyanBackend.Models.ProductionConfirmation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace AvyyanBackend.Controllers
{
	[ApiController]
	[Route("api/fg-rolls")]
	public class FgRollsController : ControllerBase
	{
		private readonly ApplicationDbContext _context;

		public FgRollsController(ApplicationDbContext context)
		{
			_context = context;
		}

		// =======================
		//   MAIN API ENDPOINT
		// =======================
		[HttpPost("upload")]
		public async Task<IActionResult> UploadVouchers([FromBody] List<VoucherUploadDto> vouchers)
		{
			if (vouchers == null || vouchers.Count == 0)
			{
				return BadRequest(new
				{
					success = false,
					message = "No voucher data received"
				});
			}

			try
			{
				await UploadVoucherData(vouchers);

				return Ok(new
				{
					success = true,
					message = "Data uploaded successfully",
					totalVouchers = vouchers.Count,
					totalRolls = vouchers.Sum(v => v.Rolls.Count)
				});
			}
			catch (Exception ex)
			{
				return BadRequest(new
				{
					success = false,
					message = ex.Message
				});
			}
		}

		// =======================
		//   BUSINESS LOGIC
		// =======================
		private async Task UploadVoucherData(List<VoucherUploadDto> vouchers)
		{
			// STEP 1 – Normalize input (TRIM SPACES)
			var distinctVouchers = vouchers.Select(x => x.VoucherNo.Trim()).Distinct().ToList();
			var distinctLots = vouchers.Select(x => x.LotNo.Trim()).Distinct().ToList();

			// STEP 2 – Load Sales Orders
			var soList = await _context.SalesOrdersWeb
				.Where(x => distinctVouchers.Contains(x.VoucherNumber.Trim()))
				.ToListAsync();

			if (soList.Count == 0)
				throw new Exception("No matching vouchers found in SalesOrdersWeb.");

			// Get SalesOrder IDs
			var soIds = soList.Select(x => x.Id).ToList();

			// STEP 3 – Load SalesOrderItems (correct logic: load ONLY items inside selected vouchers)
			var soItemList = await _context.SalesOrderItemsWeb
				.Where(x => soIds.Contains(x.SalesOrderWebId))
				.ToListAsync();

			// STEP 4 – Load existing lots
			var lotList = await _context.ProductionAllotments
				.Where(x => distinctLots.Contains(x.AllotmentId))
				.ToListAsync();

			// 1. Extract FabricType list from SO items (trimmed, distinct, non-empty)
			// 1. Prepare fabric type list (trim + lower)
			var fabricTypeList = soItemList
				.Select(i => i.FabricType?.Trim().ToLower())
				.Where(s => !string.IsNullOrWhiteSpace(s))
				.Distinct()
				.ToList();

			// 2. Fetch fabric structure master (trim + lower)
			var fabtypestruct = await _context.FabricStructureMasters
				.Where(m => fabricTypeList.Contains(m.Fabricstr.Trim().ToLower()))
				.ToListAsync();




			// STEP 5 – Process each voucher
			foreach (var v in vouchers)
			{
				string voucherNo = v.VoucherNo.Trim();
				string itemName = v.ItemName.Trim();
				string lotNo = v.LotNo.Trim();

				// 1️⃣ Validate Sales Order
				var so = soList.FirstOrDefault(x => x.VoucherNumber.Trim() == voucherNo);
				if (so == null)
					throw new Exception($"Voucher {voucherNo} not found in SalesOrdersWeb.");

				// 2️⃣ Validate Item belongs to the same SalesOrder
				var soItem = soItemList.FirstOrDefault(x =>
					x.SalesOrderWebId == so.Id &&
					x.ItemName.Trim() == itemName
				);

				var fabTyp = fabtypestruct.FirstOrDefault(x =>
					((x.Fabricstr ?? "").Trim().ToLower()) ==
					((soItem.FabricType ?? "").Trim().ToLower())
				);




				if (soItem == null)
					throw new Exception($"Item '{itemName}' not found for voucher {voucherNo}.");

				if (soItem.YarnCount == null)
					throw new Exception($"YarnCount missing for item '{itemName}' for voucher '{voucherNo}'.");



				// 3️⃣ LOT already exists or not
				var lot = lotList.FirstOrDefault(x => x.AllotmentId == lotNo);

				if (lot == null)
				{
					try
					{
						lot = new ProductionAllotment
						{
							AllotmentId = lotNo,
							VoucherNumber = voucherNo,
							ItemName = itemName,
							SalesOrderId = so.Id,
							SalesOrderItemId = soItem.Id,
							SlitLine = soItem.SlitLine,
							FabricType = soItem.FabricType,
							StitchLength = Convert.ToDecimal(soItem.StitchLength),
							ActualQuantity = so.TotalQuantity,
							Composition = soItem.Composition,
							Diameter = soItem.Dia,
							Gauge = soItem.GG,
							Efficiency = fabTyp.Standardeffencny,
							PartyName = so.BuyerName,
							YarnCount = soItem.YarnCount,       // FIXED: YarnCount always correct now
							CreatedDate = DateTime.UtcNow
						};

						_context.ProductionAllotments.Add(lot);
						await _context.SaveChangesAsync();

						lotList.Add(lot); // Add to local list
					}
					catch (Exception ex)
					{

					}
					// Create LOT

				}

				// 4️⃣ Insert Rolls
				foreach (var r in v.Rolls)
				{
					string machineName = r.MachineNo.Trim();
					string rollNo = r.RollNumber.Trim();
					int rollNoInt = 0;
					int.TryParse(rollNo, out rollNoInt);


					var existingRoll = await _context.RollConfirmations
						.FirstOrDefaultAsync(x =>
							x.AllotId == lotNo &&
							x.MachineName == machineName &&
							x.FgRollNo == rollNoInt

						);

					if (existingRoll == null)
					{
						_context.RollConfirmations.Add(new RollConfirmation
						{
							AllotId = lotNo,
							MachineName = machineName,
							FgRollNo = rollNoInt,


							CreatedDate = DateTime.UtcNow
						});
					}
				}

				await _context.SaveChangesAsync();
			}
		}
	}

	// =======================
	// DTOs
	// =======================
	public class VoucherUploadDto
	{
		public string VoucherNo { get; set; }
		public string ItemName { get; set; }
		public string LotNo { get; set; }
		public List<RollDto> Rolls { get; set; } = new();
	}

	public class RollDto
	{
		public string MachineNo { get; set; }
		public string RollNumber { get; set; }
	}
}

//    public class VoucherUploadDto
//    {
//        public string VoucherNo { get; set; } = string.Empty;
//        public string ItemName { get; set; } = string.Empty;
//        public string LotNo { get; set; } = string.Empty;
//        public List<RollDto> Rolls { get; set; } = new();
//    }

//    public class RollDto
//    {
//        public string MachineNo { get; set; } = string.Empty;
//        public string RollNumber { get; set; } = string.Empty;
//    }


//    [HttpPost("upload")]
//    public async Task<IActionResult> UploadVouchers([FromBody] List<VoucherUploadDto> vouchers)
//    {
//        if (vouchers == null || vouchers.Count == 0)
//        {
//            return BadRequest(new
//            {
//                success = false,
//                message = "No voucher data received"
//            });
//        }

//        var vno = vouchers.Select(x => x.VoucherNo).Distinct().ToList();
//        var a = await _context.SalesOrdersWeb.Where(x => vno.Contains(x.VoucherNumber)).ToListAsync();
//        var soItemId = vouchers.Select(x => x.ItemName).Distinct().ToList();
//        var b = await _context.SalesOrderItemsWeb.Where(x => soItemId.Contains(x.ItemName)).ToListAsync();
//        var Lot = vouchers.Select(x => x.LotNo).Distinct().ToList();
//        var c  = await _context.ProductionAllotments.Where(x => Lot.Contains(x.AllotmentId)).ToListAsync();
//        var machine = vouchers.Select(x => x.Rolls).Distinct().ToList();
//        var d = await _context.RollConfirmations.Where(x => machine.Contains(x.FgRollNo)).ToListAsync();

//        // ✅ Validate each record
//        foreach (var v in vouchers)
//        {
//            if (string.IsNullOrWhiteSpace(v.VoucherNo))
//                return BadRequest($"Voucher number missing");

//            if (string.IsNullOrWhiteSpace(v.ItemName))
//                return BadRequest($"Item name missing for voucher {v.VoucherNo}");

//            if (string.IsNullOrWhiteSpace(v.LotNo))
//                return BadRequest($"Lot number missing for voucher {v.VoucherNo}");

//            if (v.Rolls == null || v.Rolls.Count == 0)
//                return BadRequest($"No rolls found for voucher {v.VoucherNo}");
//        }

//        // ✅ SAVE TO DATABASE HERE
//        // using EF Core example:
//        // _dbContext.Vouchers.AddRange(mappedEntities);
//        // await _dbContext.SaveChangesAsync();

//        return Ok(new
//        {
//            success = true,
//            message = "Data uploaded successfully",
//            totalVouchers = vouchers.Count,
//            totalRolls = vouchers.Sum(v => v.Rolls.Count)
//        });
//    }


//}

//// Models
//public class VoucherGroup
//{
//    public string VoucherNo { get; set; } = "";
//    public string ItemName { get; set; } = "";
//    public string LotNo { get; set; } = "";
//    public List<FgRoll> Rolls { get; set; } = new();
//}

//public class FgRoll
//{
//    public string MachineNo { get; set; } = "";
//    public string RollNumber { get; set; } = "";
//}
