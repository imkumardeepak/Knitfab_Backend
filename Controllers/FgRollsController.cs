﻿using AvyyanBackend.Data;
using AvyyanBackend.Models;
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
		public async Task<IActionResult> UploadVouchers([FromBody] List<VoucherGroupDto> vouchers)
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
				var result = await UploadVoucherData(vouchers);
				return Ok(result);
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
		private async Task<object> UploadVoucherData(List<VoucherGroupDto> vouchers)
		{
			var results = new List<object>();
			int totalProcessedVouchers = 0;
			int totalProcessedRolls = 0;
			int totalErrors = 0;

			// STEP 1 – Normalize input (TRIM SPACES)
			var distinctVouchers = vouchers.Select(x => x.voucherNo.Trim()).Distinct().ToList();
			var distinctLots = vouchers.Select(x => x.lotNo.Trim()).Distinct().ToList();

			// STEP 2 – Load Sales Orders
			var soList = await _context.SalesOrdersWeb
				.Where(x => distinctVouchers.Contains(x.VoucherNumber.Trim()))
				.ToListAsync();

			if (soList.Count == 0)
			{
				return new
				{
					success = false,
					message = "No matching Sales Orders found for the provided vouchers.",
					processedVouchers = 0,
					processedRolls = 0,
					errors = 1
				};
			}

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

			// Track entities to be added
			var newLots = new List<ProductionAllotment>();
			var newRollConfirmations = new List<RollConfirmation>();
			var newStorageCaptures = new List<StorageCapture>();

			// STEP 5 – Process each voucher
			foreach (var v in vouchers)
			{
				string voucherNo = v.voucherNo.Trim();
				string itemName = v.itemName.Trim();
				string lotNo = v.lotNo.Trim();

				try
				{
					// 1️⃣ Validate Sales Order
					var so = soList.FirstOrDefault(x => x.VoucherNumber.Trim() == voucherNo);
					if (so == null)
					{
						results.Add(new
						{
							voucherNo = voucherNo,
							success = false,
							message = $"Sales Order with VoucherNumber '{voucherNo}' not found."
						});
						totalErrors++;
						continue;
					}

					// 2️⃣ Validate Item belongs to the same SalesOrder
					var soItem = soItemList.FirstOrDefault(x =>
					x.SalesOrderWebId == so.Id &&
					x.ItemName.Trim() == itemName
				);

					if (soItem == null)
					{
						results.Add(new
						{
							voucherNo = voucherNo,
							success = false,
							message = $"Item '{itemName}' not found in SalesOrderItemsWeb."
						});
						totalErrors++;
						continue;
					}

					if (soItem.YarnCount == null)
					{
						results.Add(new
						{
							voucherNo = voucherNo,
							success = false,
							message = $"YarnCount not found in SalesOrderItemsWeb."
						});
						totalErrors++;
						continue;
					}

					var fabTyp = fabtypestruct.FirstOrDefault(x =>
						((x.Fabricstr ?? "").Trim().ToLower()) ==
						((soItem.FabricType ?? "").Trim().ToLower())
					);

					// 3️⃣ LOT already exists or not
					var lot = lotList.FirstOrDefault(x => x.AllotmentId == lotNo);

					if (lot == null)
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
							StitchLength = soItem.StitchLength,
							ActualQuantity = so.TotalQuantity,
							Composition = soItem.Composition,
							Diameter = soItem.Dia,
							Gauge = soItem.GG,
							Efficiency = fabTyp?.Standardeffencny ?? 0,
							PartyName = so.BuyerName,
							YarnCount = soItem.YarnCount,
							TapeColor = v.tape,
							CreatedDate = DateTime.UtcNow
						};

						newLots.Add(lot);
						lotList.Add(lot); // Add to local list for subsequent references
					}

					int rollCount = 0;
					// 4️⃣ Prepare Rolls for bulk insert
					foreach (FgRollDto r in v.rolls)
					{
						string machineName = r.machineNo.Trim();
						string rollNo = r.rollNumber.Trim();
						int rollNoInt = 0;
						int.TryParse(rollNo, out rollNoInt);

						// Check if roll confirmation already exists
						var existingRoll = await _context.RollConfirmations
							.AnyAsync(x =>
								x.AllotId == lotNo &&
								x.MachineName == machineName &&
								x.FgRollNo == rollNoInt);

						if (!existingRoll)
						{
							newRollConfirmations.Add(new RollConfirmation
							{
								AllotId = lotNo,
								MachineName = machineName,
								FgRollNo = rollNoInt,
								RollNo = rollNoInt.ToString(),
								NetWeight = Convert.ToDecimal(r.netWt),
								GrossWeight = Convert.ToDecimal(r.grossWt),
								TareWeight= Convert.ToDecimal(r.grossWt) - Convert.ToDecimal(r.netWt),
								IsFGStickerGenerated = true,
								CreatedDate = DateTime.UtcNow
							});
						}

						// Check if storage capture already exists
						var existingStorageRoll = await _context.StorageCaptures
							.AnyAsync(x => x.LocationCode == v.location.Trim() && x.FGRollNo == rollNoInt.ToString() && x.LotNo == lotNo);

						if (!existingStorageRoll)
						{
							newStorageCaptures.Add(new StorageCapture
							{
								LocationCode = v.location.Trim(),
								FGRollNo = rollNoInt.ToString(),
								LotNo = lotNo,
								CreatedAt = DateTime.UtcNow,
								Tape = v.tape,
								IsDispatched = r.isDispatched ?? false,
								CustomerName = so.BuyerName,
								UpdatedAt = DateTime.UtcNow
							});
						}

						rollCount++;
					}

					results.Add(new
					{
						voucherNo = voucherNo,
						success = true,
						message = $"Voucher {voucherNo} processed successfully.",
						rollsProcessed = rollCount
					});

					totalProcessedVouchers++;
					totalProcessedRolls += rollCount;
				}
				catch (Exception ex)
				{
					results.Add(new
					{
						voucherNo = voucherNo,
						success = false,
						message = $"Error processing voucher {voucherNo}: {ex.Message}"
					});
					totalErrors++;
				}
			}

			// Bulk insert all new entities
			try
			{
				if (newLots.Any())
				{
					_context.ProductionAllotments.AddRange(newLots);
				}

				if (newRollConfirmations.Any())
				{
					_context.RollConfirmations.AddRange(newRollConfirmations);
				}

				if (newStorageCaptures.Any())
				{
					_context.StorageCaptures.AddRange(newStorageCaptures);
				}

				if (newLots.Any() || newRollConfirmations.Any() || newStorageCaptures.Any())
				{
					await _context.SaveChangesAsync();
				}
			}
			catch (Exception ex)
			{
				return new
				{
					success = false,
					message = $"Error saving data to database: {ex.Message}",
					processedVouchers = totalProcessedVouchers,
					processedRolls = totalProcessedRolls,
					errors = totalErrors + 1
				};
			}

			return new
			{
				success = totalErrors == 0,
				message = totalErrors == 0 ? "All vouchers processed successfully" : $"Processing completed with {totalErrors} errors",
				processedVouchers = totalProcessedVouchers,
				totalVouchers = vouchers.Count,
				processedRolls = totalProcessedRolls,
				totalRolls = vouchers.Sum(v => v.rolls.Count),
				errors = totalErrors,
				details = results
			};
		}
	}

	// =======================
	// DTOs
	// =======================
	public class VoucherGroupDto
	{
		public string voucherNo { get; set; }
		public string itemName { get; set; }
		public string lotNo { get; set; }
		public string tape { get; set; }
		public string location { get; set; }
		public List<FgRollDto> rolls { get; set; } = new();
	}

	public class FgRollDto
	{
		public string machineNo { get; set; }
		public string rollNumber { get; set; }
		public string grossWt { get; set; }
		public string netWt { get; set; }
		public bool? isDispatched { get; set; }
	}
}

