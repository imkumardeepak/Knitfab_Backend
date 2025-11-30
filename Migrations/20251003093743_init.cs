using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace AvyyanBackend.Migrations
{
    /// <inheritdoc />
    public partial class init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.CreateTable(
            //    name: "FabricStructureMasters",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(type: "integer", nullable: false)
            //            .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
            //        Fabricstr = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
            //        Standardeffencny = table.Column<decimal>(type: "numeric(18,5)", precision: 18, scale: 3, nullable: false),
            //        FabricCode = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
            //        CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
            //        UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
            //        IsActive = table.Column<bool>(type: "boolean", nullable: false),
            //        CreatedBy = table.Column<string>(type: "text", nullable: true),
            //        UpdatedBy = table.Column<string>(type: "text", nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_FabricStructureMasters", x => x.Id);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "Inspections",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(type: "integer", nullable: false)
            //            .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
            //        AllotId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
            //        MachineName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
            //        RollNo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
            //        ThinPlaces = table.Column<int>(type: "integer", nullable: false),
            //        ThickPlaces = table.Column<int>(type: "integer", nullable: false),
            //        ThinLines = table.Column<int>(type: "integer", nullable: false),
            //        ThickLines = table.Column<int>(type: "integer", nullable: false),
            //        DoubleParallelYarn = table.Column<int>(type: "integer", nullable: false),
            //        HaidJute = table.Column<int>(type: "integer", nullable: false),
            //        ColourFabric = table.Column<int>(type: "integer", nullable: false),
            //        Holes = table.Column<int>(type: "integer", nullable: false),
            //        DropStitch = table.Column<int>(type: "integer", nullable: false),
            //        LycraStitch = table.Column<int>(type: "integer", nullable: false),
            //        LycraBreak = table.Column<int>(type: "integer", nullable: false),
            //        FFD = table.Column<int>(type: "integer", nullable: false),
            //        NeedleBroken = table.Column<int>(type: "integer", nullable: false),
            //        KnitFly = table.Column<int>(type: "integer", nullable: false),
            //        OilSpots = table.Column<int>(type: "integer", nullable: false),
            //        OilLines = table.Column<int>(type: "integer", nullable: false),
            //        VerticalLines = table.Column<int>(type: "integer", nullable: false),
            //        Grade = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
            //        TotalFaults = table.Column<int>(type: "integer", nullable: false),
            //        Remarks = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
            //        CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
            //        Flag = table.Column<bool>(type: "boolean", nullable: false),
            //        CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
            //        UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
            //        IsActive = table.Column<bool>(type: "boolean", nullable: false),
            //        CreatedBy = table.Column<string>(type: "text", nullable: true),
            //        UpdatedBy = table.Column<string>(type: "text", nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Inspections", x => x.Id);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "LocationMasters",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(type: "integer", nullable: false)
            //            .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
            //        Warehousename = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
            //        Location = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
            //        Sublocation = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
            //        Locationcode = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
            //        CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
            //        UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
            //        IsActive = table.Column<bool>(type: "boolean", nullable: false),
            //        CreatedBy = table.Column<string>(type: "text", nullable: true),
            //        UpdatedBy = table.Column<string>(type: "text", nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_LocationMasters", x => x.Id);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "MachineManagers",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(type: "integer", nullable: false)
            //            .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
            //        MachineName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
            //        Dia = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 3, nullable: false),
            //        Gg = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 3, nullable: false),
            //        Needle = table.Column<int>(type: "integer", nullable: false),
            //        Feeder = table.Column<int>(type: "integer", nullable: false),
            //        Rpm = table.Column<decimal>(type: "numeric(18,5)", precision: 18, scale: 3, nullable: false),
            //        Constat = table.Column<decimal>(type: "numeric(18,5)", precision: 18, scale: 3, nullable: true),
            //        Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
            //        CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
            //        UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
            //        IsActive = table.Column<bool>(type: "boolean", nullable: false),
            //        CreatedBy = table.Column<string>(type: "text", nullable: true),
            //        UpdatedBy = table.Column<string>(type: "text", nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_MachineManagers", x => x.Id);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "ProductionAllotments",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(type: "integer", nullable: false)
            //            .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
            //        AllotmentId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
            //        VoucherNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
            //        ItemName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
            //        SalesOrderId = table.Column<int>(type: "integer", nullable: false),
            //        SalesOrderItemId = table.Column<int>(type: "integer", nullable: false),
            //        ActualQuantity = table.Column<decimal>(type: "numeric(18,3)", precision: 18, scale: 3, nullable: false),
            //        YarnCount = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
            //        Diameter = table.Column<int>(type: "integer", nullable: false),
            //        Gauge = table.Column<int>(type: "integer", nullable: false),
            //        FabricType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
            //        SlitLine = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
            //        StitchLength = table.Column<decimal>(type: "numeric(18,3)", precision: 18, scale: 3, nullable: false),
            //        Efficiency = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
            //        Composition = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
            //        TotalProductionTime = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
            //        YarnLotNo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
            //        Counter = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
            //        ColourCode = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
            //        ReqGreyGsm = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 3, nullable: true),
            //        ReqGreyWidth = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 3, nullable: true),
            //        ReqFinishGsm = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 3, nullable: true),
            //        ReqFinishWidth = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 3, nullable: true),
            //        PartyName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
            //        TubeWeight = table.Column<decimal>(type: "numeric(18,3)", precision: 18, scale: 3, nullable: false),
            //        TapeColor = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
            //        SerialNo = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
            //        CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_ProductionAllotments", x => x.Id);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "RoleMasters",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(type: "integer", nullable: false)
            //            .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
            //        RoleName = table.Column<string>(type: "text", nullable: true),
            //        Description = table.Column<string>(type: "text", nullable: true),
            //        IsActive = table.Column<bool>(type: "boolean", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_RoleMasters", x => x.Id);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "RollConfirmations",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(type: "integer", nullable: false)
            //            .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
            //        AllotId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
            //        MachineName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
            //        RollPerKg = table.Column<decimal>(type: "numeric(18,3)", precision: 18, scale: 3, nullable: false),
            //        GreyGsm = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 3, nullable: false),
            //        GreyWidth = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 3, nullable: false),
            //        BlendPercent = table.Column<decimal>(type: "numeric(5,2)", precision: 18, scale: 3, nullable: false),
            //        Cotton = table.Column<decimal>(type: "numeric(5,2)", precision: 18, scale: 3, nullable: false),
            //        Polyester = table.Column<decimal>(type: "numeric(5,2)", precision: 18, scale: 3, nullable: false),
            //        Spandex = table.Column<decimal>(type: "numeric(5,2)", precision: 18, scale: 3, nullable: false),
            //        RollNo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
            //        CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_RollConfirmations", x => x.Id);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "SalesOrders",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(type: "integer", nullable: false)
            //            .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
            //        VchType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
            //        SalesDate = table.Column<DateTime>(type: "timestamp without time zone", maxLength: 20, nullable: false),
            //        Guid = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
            //        GstRegistrationType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
            //        StateName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
            //        PartyName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
            //        PartyLedgerName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
            //        VoucherNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
            //        Reference = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
            //        CompanyAddress = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
            //        BuyerAddress = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
            //        OrderTerms = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
            //        LedgerEntries = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
            //        ProcessFlag = table.Column<int>(type: "integer", nullable: false),
            //        ProcessDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
            //        CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
            //        UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
            //        IsActive = table.Column<bool>(type: "boolean", nullable: false),
            //        CreatedBy = table.Column<string>(type: "text", nullable: true),
            //        UpdatedBy = table.Column<string>(type: "text", nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_SalesOrders", x => x.Id);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "ShiftMasters",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(type: "integer", nullable: false)
            //            .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
            //        ShiftName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
            //        StartTime = table.Column<TimeSpan>(type: "interval", nullable: false),
            //        EndTime = table.Column<TimeSpan>(type: "interval", nullable: false),
            //        DurationInHours = table.Column<int>(type: "integer", nullable: false),
            //        CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
            //        UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
            //        IsActive = table.Column<bool>(type: "boolean", nullable: false),
            //        CreatedBy = table.Column<string>(type: "text", nullable: true),
            //        UpdatedBy = table.Column<string>(type: "text", nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_ShiftMasters", x => x.Id);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "TapeColorMasters",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(type: "integer", nullable: false)
            //            .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
            //        TapeColor = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
            //        CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
            //        UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
            //        IsActive = table.Column<bool>(type: "boolean", nullable: false),
            //        CreatedBy = table.Column<string>(type: "text", nullable: true),
            //        UpdatedBy = table.Column<string>(type: "text", nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_TapeColorMasters", x => x.Id);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "Users",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(type: "integer", nullable: false)
            //            .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
            //        FirstName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
            //        LastName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
            //        RoleName = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
            //        Email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
            //        PasswordHash = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
            //        PhoneNumber = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
            //        LastLoginAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
            //        CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
            //        UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
            //        IsActive = table.Column<bool>(type: "boolean", nullable: false),
            //        CreatedBy = table.Column<string>(type: "text", nullable: true),
            //        UpdatedBy = table.Column<string>(type: "text", nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Users", x => x.Id);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "YarnTypeMasters",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(type: "integer", nullable: false)
            //            .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
            //        YarnType = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
            //        YarnCode = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
            //        ShortCode = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
            //        CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
            //        UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
            //        IsActive = table.Column<bool>(type: "boolean", nullable: false),
            //        CreatedBy = table.Column<string>(type: "text", nullable: true),
            //        UpdatedBy = table.Column<string>(type: "text", nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_YarnTypeMasters", x => x.Id);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "MachineAllocations",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(type: "integer", nullable: false)
            //            .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
            //        ProductionAllotmentId = table.Column<int>(type: "integer", nullable: false),
            //        MachineName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
            //        MachineId = table.Column<int>(type: "integer", nullable: false),
            //        NumberOfNeedles = table.Column<int>(type: "integer", nullable: false),
            //        Feeders = table.Column<int>(type: "integer", nullable: false),
            //        RPM = table.Column<int>(type: "integer", nullable: false),
            //        RollPerKg = table.Column<decimal>(type: "numeric(18,3)", precision: 18, scale: 3, nullable: false),
            //        TotalLoadWeight = table.Column<decimal>(type: "numeric(18,3)", precision: 18, scale: 3, nullable: false),
            //        TotalRolls = table.Column<decimal>(type: "numeric(18,3)", precision: 18, scale: 3, nullable: false),
            //        RollBreakdown = table.Column<string>(type: "text", nullable: false),
            //        EstimatedProductionTime = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_MachineAllocations", x => x.Id);
            //        table.ForeignKey(
            //            name: "FK_MachineAllocations_ProductionAllotments_ProductionAllotment~",
            //            column: x => x.ProductionAllotmentId,
            //            principalTable: "ProductionAllotments",
            //            principalColumn: "Id",
            //            onDelete: ReferentialAction.Cascade);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "PageAccesses",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(type: "integer", nullable: false)
            //            .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
            //        RoleId = table.Column<int>(type: "integer", nullable: false),
            //        PageName = table.Column<string>(type: "text", nullable: true),
            //        IsView = table.Column<bool>(type: "boolean", nullable: false),
            //        IsAdd = table.Column<bool>(type: "boolean", nullable: false),
            //        IsEdit = table.Column<bool>(type: "boolean", nullable: false),
            //        IsDelete = table.Column<bool>(type: "boolean", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_PageAccesses", x => x.Id);
            //        table.ForeignKey(
            //            name: "FK_PageAccesses_RoleMasters_RoleId",
            //            column: x => x.RoleId,
            //            principalTable: "RoleMasters",
            //            principalColumn: "Id",
            //            onDelete: ReferentialAction.Cascade);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "SalesOrderItems",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(type: "integer", nullable: false)
            //            .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
            //        SalesOrderId = table.Column<int>(type: "integer", nullable: false),
            //        StockItemName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
            //        Rate = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
            //        Amount = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
            //        ActualQty = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
            //        BilledQty = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
            //        Descriptions = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
            //        BatchName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
            //        OrderNo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
            //        OrderDueDate = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
            //        ProcessFlag = table.Column<int>(type: "integer", nullable: false),
            //        ProcessDate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_SalesOrderItems", x => x.Id);
            //        table.ForeignKey(
            //            name: "FK_SalesOrderItems_SalesOrders_SalesOrderId",
            //            column: x => x.SalesOrderId,
            //            principalTable: "SalesOrders",
            //            principalColumn: "Id",
            //            onDelete: ReferentialAction.Cascade);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "RollAssignments",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(type: "integer", nullable: false)
            //            .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
            //        MachineAllocationId = table.Column<int>(type: "integer", nullable: false),
            //        ShiftId = table.Column<int>(type: "integer", nullable: false),
            //        AssignedRolls = table.Column<decimal>(type: "numeric(18,3)", precision: 18, scale: 3, nullable: false),
            //        GeneratedStickers = table.Column<decimal>(type: "numeric(18,3)", precision: 18, scale: 3, nullable: false),
            //        RemainingRolls = table.Column<decimal>(type: "numeric(18,3)", precision: 18, scale: 3, nullable: false),
            //        OperatorName = table.Column<string>(type: "text", nullable: false),
            //        Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_RollAssignments", x => x.Id);
            //        table.ForeignKey(
            //            name: "FK_RollAssignments_MachineAllocations_MachineAllocationId",
            //            column: x => x.MachineAllocationId,
            //            principalTable: "MachineAllocations",
            //            principalColumn: "Id",
            //            onDelete: ReferentialAction.Cascade);
            //    });

            //migrationBuilder.CreateTable(
            //    name: "GeneratedBarcode",
            //    columns: table => new
            //    {
            //        Id = table.Column<int>(type: "integer", nullable: false)
            //            .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
            //        RollAssignmentId = table.Column<int>(type: "integer", nullable: false),
            //        Barcode = table.Column<string>(type: "text", nullable: false),
            //        RollNumber = table.Column<int>(type: "integer", nullable: false),
            //        GeneratedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_GeneratedBarcode", x => x.Id);
            //        table.ForeignKey(
            //            name: "FK_GeneratedBarcode_RollAssignments_RollAssignmentId",
            //            column: x => x.RollAssignmentId,
            //            principalTable: "RollAssignments",
            //            principalColumn: "Id",
            //            onDelete: ReferentialAction.Cascade);
            //    });

            //migrationBuilder.CreateIndex(
            //    name: "IX_GeneratedBarcode_RollAssignmentId",
            //    table: "GeneratedBarcode",
            //    column: "RollAssignmentId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_Inspections_AllotId_MachineName_RollNo",
            //    table: "Inspections",
            //    columns: new[] { "AllotId", "MachineName", "RollNo" },
            //    unique: true);

            //migrationBuilder.CreateIndex(
            //    name: "IX_MachineAllocations_ProductionAllotmentId",
            //    table: "MachineAllocations",
            //    column: "ProductionAllotmentId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_MachineManagers_MachineName",
            //    table: "MachineManagers",
            //    column: "MachineName",
            //    unique: true);

            //migrationBuilder.CreateIndex(
            //    name: "IX_PageAccesses_RoleId",
            //    table: "PageAccesses",
            //    column: "RoleId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_RoleMasters_RoleName",
            //    table: "RoleMasters",
            //    column: "RoleName",
            //    unique: true);

            //migrationBuilder.CreateIndex(
            //    name: "IX_RollAssignments_MachineAllocationId",
            //    table: "RollAssignments",
            //    column: "MachineAllocationId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_RollConfirmations_AllotId_MachineName_RollNo",
            //    table: "RollConfirmations",
            //    columns: new[] { "AllotId", "MachineName", "RollNo" },
            //    unique: true);

            //migrationBuilder.CreateIndex(
            //    name: "IX_SalesOrderItems_SalesOrderId",
            //    table: "SalesOrderItems",
            //    column: "SalesOrderId");

            //migrationBuilder.CreateIndex(
            //    name: "IX_SalesOrderItems_StockItemName",
            //    table: "SalesOrderItems",
            //    column: "StockItemName");

            //migrationBuilder.CreateIndex(
            //    name: "IX_SalesOrders_PartyName",
            //    table: "SalesOrders",
            //    column: "PartyName");

            //migrationBuilder.CreateIndex(
            //    name: "IX_SalesOrders_ProcessFlag",
            //    table: "SalesOrders",
            //    column: "ProcessFlag");

            //migrationBuilder.CreateIndex(
            //    name: "IX_SalesOrders_SalesDate",
            //    table: "SalesOrders",
            //    column: "SalesDate");

            //migrationBuilder.CreateIndex(
            //    name: "IX_SalesOrders_VoucherNumber",
            //    table: "SalesOrders",
            //    column: "VoucherNumber",
            //    unique: true);

            //migrationBuilder.CreateIndex(
            //    name: "IX_Users_Email",
            //    table: "Users",
            //    column: "Email",
            //    unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            //    migrationBuilder.DropTable(
            //        name: "FabricStructureMasters");

            //    migrationBuilder.DropTable(
            //        name: "GeneratedBarcode");

            //    migrationBuilder.DropTable(
            //        name: "Inspections");

            //    migrationBuilder.DropTable(
            //        name: "LocationMasters");

            //    migrationBuilder.DropTable(
            //        name: "MachineManagers");

            //    migrationBuilder.DropTable(
            //        name: "PageAccesses");

            //    migrationBuilder.DropTable(
            //        name: "RollConfirmations");

            //    migrationBuilder.DropTable(
            //        name: "SalesOrderItems");

            //    migrationBuilder.DropTable(
            //        name: "ShiftMasters");

            //    migrationBuilder.DropTable(
            //        name: "TapeColorMasters");

            //    migrationBuilder.DropTable(
            //        name: "Users");

            //    migrationBuilder.DropTable(
            //        name: "YarnTypeMasters");

            //    migrationBuilder.DropTable(
            //        name: "RollAssignments");

            //    migrationBuilder.DropTable(
            //        name: "RoleMasters");

            //    migrationBuilder.DropTable(
            //        name: "SalesOrders");

            //    migrationBuilder.DropTable(
            //        name: "MachineAllocations");

            //    migrationBuilder.DropTable(
            //        name: "ProductionAllotments");
        }
    }
}
