using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace AvyyanBackend.Migrations
{
    /// <inheritdoc />
    public partial class HSNADDED : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CourierMasters",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CourierName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    ContactPerson = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Phone = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: true),
                    Email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Address = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    GstNo = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    TrackingUrl = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourierMasters", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FabricStructureMasters",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Fabricstr = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Standardeffencny = table.Column<decimal>(type: "numeric(18,5)", precision: 18, scale: 3, nullable: false),
                    FabricCode = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FabricStructureMasters", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Inspections",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AllotId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    MachineName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    RollNo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    ThinPlaces = table.Column<int>(type: "integer", nullable: false),
                    ThickPlaces = table.Column<int>(type: "integer", nullable: false),
                    ThinLines = table.Column<int>(type: "integer", nullable: false),
                    ThickLines = table.Column<int>(type: "integer", nullable: false),
                    DoubleParallelYarn = table.Column<int>(type: "integer", nullable: false),
                    HaidJute = table.Column<int>(type: "integer", nullable: false),
                    ColourFabric = table.Column<int>(type: "integer", nullable: false),
                    Holes = table.Column<int>(type: "integer", nullable: false),
                    DropStitch = table.Column<int>(type: "integer", nullable: false),
                    LycraStitch = table.Column<int>(type: "integer", nullable: false),
                    LycraBreak = table.Column<int>(type: "integer", nullable: false),
                    FFD = table.Column<int>(type: "integer", nullable: false),
                    NeedleBroken = table.Column<int>(type: "integer", nullable: false),
                    KnitFly = table.Column<int>(type: "integer", nullable: false),
                    OilSpots = table.Column<int>(type: "integer", nullable: false),
                    OilLines = table.Column<int>(type: "integer", nullable: false),
                    VerticalLines = table.Column<int>(type: "integer", nullable: false),
                    Grade = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    TotalFaults = table.Column<int>(type: "integer", nullable: false),
                    Remarks = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Flag = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Inspections", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LocationMasters",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Warehousename = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Location = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Sublocation = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Locationcode = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LocationMasters", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MachineManagers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MachineName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Dia = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 3, nullable: false),
                    Gg = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 3, nullable: false),
                    Needle = table.Column<int>(type: "integer", nullable: false),
                    Feeder = table.Column<int>(type: "integer", nullable: false),
                    Rpm = table.Column<decimal>(type: "numeric(18,5)", precision: 18, scale: 3, nullable: false),
                    Constat = table.Column<decimal>(type: "numeric(18,5)", precision: 18, scale: 3, nullable: true),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    MachineType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MachineManagers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProductionAllotments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AllotmentId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    VoucherNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    ItemName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    SalesOrderId = table.Column<int>(type: "integer", nullable: false),
                    SalesOrderItemId = table.Column<int>(type: "integer", nullable: false),
                    ActualQuantity = table.Column<decimal>(type: "numeric(18,3)", precision: 18, scale: 3, nullable: false),
                    YarnCount = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Diameter = table.Column<int>(type: "integer", nullable: false),
                    Gauge = table.Column<int>(type: "integer", nullable: false),
                    FabricType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    SlitLine = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    StitchLength = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Efficiency = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    Composition = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    TotalProductionTime = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    YarnLotNo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Counter = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    ColourCode = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    ReqGreyGsm = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 3, nullable: true),
                    ReqGreyWidth = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 3, nullable: true),
                    ReqFinishGsm = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 3, nullable: true),
                    ReqFinishWidth = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 3, nullable: true),
                    PartyName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    TubeWeight = table.Column<decimal>(type: "numeric(18,3)", precision: 18, scale: 3, nullable: false),
                    ShrinkRapWeight = table.Column<decimal>(type: "numeric(18,3)", precision: 18, scale: 3, nullable: true),
                    TotalWeight = table.Column<decimal>(type: "numeric(18,3)", precision: 18, scale: 3, nullable: true),
                    TapeColor = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    SerialNo = table.Column<string>(type: "text", nullable: true),
                    OtherReference = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductionAllotments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RoleMasters",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RoleName = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleMasters", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RollConfirmations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AllotId = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    MachineName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    RollPerKg = table.Column<decimal>(type: "numeric(18,3)", precision: 18, scale: 3, nullable: false),
                    GreyGsm = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 3, nullable: false),
                    GreyWidth = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 3, nullable: false),
                    BlendPercent = table.Column<decimal>(type: "numeric(5,2)", precision: 18, scale: 3, nullable: false),
                    Cotton = table.Column<decimal>(type: "numeric(5,2)", precision: 18, scale: 3, nullable: false),
                    Polyester = table.Column<decimal>(type: "numeric(5,2)", precision: 18, scale: 3, nullable: false),
                    Spandex = table.Column<decimal>(type: "numeric(5,2)", precision: 18, scale: 3, nullable: false),
                    RollNo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    GrossWeight = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 3, nullable: true),
                    TareWeight = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 3, nullable: true),
                    NetWeight = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 3, nullable: true),
                    FgRollNo = table.Column<int>(type: "integer", nullable: true),
                    IsFGStickerGenerated = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RollConfirmations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SalesOrdersWeb",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    VoucherType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    VoucherNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    OrderDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TermsOfPayment = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    IsJobWork = table.Column<bool>(type: "boolean", nullable: false),
                    SerialNo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    IsProcess = table.Column<bool>(type: "boolean", nullable: false),
                    OrderNo = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    TermsOfDelivery = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    DispatchThrough = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    CompanyName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    CompanyGSTIN = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CompanyState = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    BuyerName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    BuyerGSTIN = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    BuyerState = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    BuyerPhone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    BuyerContactPerson = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    BuyerAddress = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    ConsigneeName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    ConsigneeGSTIN = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    ConsigneeState = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    ConsigneePhone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    ConsigneeContactPerson = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ConsigneeAddress = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Remarks = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    OtherReference = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    TotalQuantity = table.Column<decimal>(type: "numeric(18,3)", precision: 18, scale: 3, nullable: false),
                    TotalAmount = table.Column<decimal>(type: "numeric(18,3)", precision: 18, scale: 3, nullable: false),
                    ProcessDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedBy = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalesOrdersWeb", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ShiftMasters",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ShiftName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    StartTime = table.Column<TimeSpan>(type: "interval", nullable: false),
                    EndTime = table.Column<TimeSpan>(type: "interval", nullable: false),
                    DurationInHours = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShiftMasters", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SlitLineMasters",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SlitLine = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    SlitLineCode = table.Column<char>(type: "char(1)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SlitLineMasters", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StorageCaptures",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    LotNo = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    FGRollNo = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    LocationCode = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Tape = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CustomerName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    IsDispatched = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StorageCaptures", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TapeColorMasters",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TapeColor = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TapeColorMasters", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TransportMasters",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TransportName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    ContactPerson = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    Address = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    VehicleNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    DriverName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    DriverNumber = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    LicenseNumber = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    MaximumCapacityKgs = table.Column<decimal>(type: "numeric(18,3)", precision: 18, scale: 3, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransportMasters", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FirstName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    RoleName = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    PasswordHash = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    PhoneNumber = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    LastLoginAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "YarnTypeMasters",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    YarnType = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    YarnCode = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    ShortCode = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_YarnTypeMasters", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MachineAllocations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProductionAllotmentId = table.Column<int>(type: "integer", nullable: false),
                    MachineName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    MachineId = table.Column<int>(type: "integer", nullable: false),
                    NumberOfNeedles = table.Column<int>(type: "integer", nullable: false),
                    Feeders = table.Column<int>(type: "integer", nullable: false),
                    RPM = table.Column<decimal>(type: "numeric(18,3)", precision: 18, scale: 3, nullable: false),
                    RollPerKg = table.Column<decimal>(type: "numeric(18,3)", precision: 18, scale: 3, nullable: false),
                    TotalLoadWeight = table.Column<decimal>(type: "numeric(18,3)", precision: 18, scale: 3, nullable: false),
                    TotalRolls = table.Column<decimal>(type: "numeric(18,3)", precision: 18, scale: 3, nullable: false),
                    RollBreakdown = table.Column<string>(type: "text", nullable: false),
                    EstimatedProductionTime = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MachineAllocations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MachineAllocations_ProductionAllotments_ProductionAllotment~",
                        column: x => x.ProductionAllotmentId,
                        principalTable: "ProductionAllotments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PageAccesses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RoleId = table.Column<int>(type: "integer", nullable: false),
                    PageName = table.Column<string>(type: "text", nullable: true),
                    IsView = table.Column<bool>(type: "boolean", nullable: false),
                    IsAdd = table.Column<bool>(type: "boolean", nullable: false),
                    IsEdit = table.Column<bool>(type: "boolean", nullable: false),
                    IsDelete = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PageAccesses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PageAccesses_RoleMasters_RoleId",
                        column: x => x.RoleId,
                        principalTable: "RoleMasters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SalesOrderItemsWeb",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SalesOrderWebId = table.Column<int>(type: "integer", nullable: false),
                    ItemName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    HSNCode = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    YarnCount = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Dia = table.Column<int>(type: "integer", nullable: false),
                    GG = table.Column<int>(type: "integer", nullable: false),
                    FabricType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Composition = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    WtPerRoll = table.Column<decimal>(type: "numeric(18,3)", precision: 18, scale: 3, nullable: false),
                    NoOfRolls = table.Column<int>(type: "integer", nullable: false),
                    Rate = table.Column<decimal>(type: "numeric(18,3)", precision: 18, scale: 3, nullable: false),
                    Qty = table.Column<decimal>(type: "numeric(18,3)", precision: 18, scale: 3, nullable: false),
                    Amount = table.Column<decimal>(type: "numeric(18,3)", precision: 18, scale: 3, nullable: false),
                    IGST = table.Column<decimal>(type: "numeric(18,3)", precision: 18, scale: 3, nullable: false),
                    SGST = table.Column<decimal>(type: "numeric(18,3)", precision: 18, scale: 3, nullable: false),
                    CGST = table.Column<decimal>(type: "numeric(18,3)", precision: 18, scale: 3, nullable: false),
                    Remarks = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Unit = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    SlitLine = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    StitchLength = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    DueDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsProcess = table.Column<bool>(type: "boolean", nullable: false),
                    ProcessDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SalesOrderItemsWeb", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SalesOrderItemsWeb_SalesOrdersWeb_SalesOrderWebId",
                        column: x => x.SalesOrderWebId,
                        principalTable: "SalesOrdersWeb",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DispatchPlannings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    LotNo = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    SalesOrderId = table.Column<int>(type: "integer", nullable: false),
                    SalesOrderItemId = table.Column<int>(type: "integer", nullable: false),
                    CustomerName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Tape = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    TotalRequiredRolls = table.Column<decimal>(type: "numeric(18,3)", precision: 18, scale: 3, nullable: false),
                    TotalReadyRolls = table.Column<decimal>(type: "numeric(18,3)", precision: 18, scale: 3, nullable: false),
                    TotalDispatchedRolls = table.Column<decimal>(type: "numeric(18,3)", precision: 18, scale: 3, nullable: false),
                    IsFullyDispatched = table.Column<bool>(type: "boolean", nullable: false),
                    DispatchStartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    DispatchEndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    VehicleNo = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    DriverName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    License = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    MobileNumber = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Remarks = table.Column<string>(type: "text", nullable: false),
                    LoadingNo = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    DispatchOrderId = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    IsTransport = table.Column<bool>(type: "boolean", nullable: false),
                    IsCourier = table.Column<bool>(type: "boolean", nullable: false),
                    TransportId = table.Column<int>(type: "integer", nullable: true),
                    CourierId = table.Column<int>(type: "integer", nullable: true),
                    TransportName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    ContactPerson = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Phone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    MaximumCapacityKgs = table.Column<decimal>(type: "numeric(18,3)", precision: 18, scale: 3, nullable: true),
                    TotalGrossWeight = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 3, nullable: true),
                    TotalNetWeight = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 3, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DispatchPlannings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DispatchPlannings_CourierMasters_CourierId",
                        column: x => x.CourierId,
                        principalTable: "CourierMasters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_DispatchPlannings_TransportMasters_TransportId",
                        column: x => x.TransportId,
                        principalTable: "TransportMasters",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "RollAssignments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    MachineAllocationId = table.Column<int>(type: "integer", nullable: false),
                    ShiftId = table.Column<int>(type: "integer", nullable: false),
                    AssignedRolls = table.Column<decimal>(type: "numeric(18,3)", precision: 18, scale: 3, nullable: false),
                    GeneratedStickers = table.Column<decimal>(type: "numeric(18,3)", precision: 18, scale: 3, nullable: false),
                    RemainingRolls = table.Column<decimal>(type: "numeric(18,3)", precision: 18, scale: 3, nullable: false),
                    OperatorName = table.Column<string>(type: "text", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RollAssignments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RollAssignments_MachineAllocations_MachineAllocationId",
                        column: x => x.MachineAllocationId,
                        principalTable: "MachineAllocations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DispatchedRolls",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DispatchPlanningId = table.Column<int>(type: "integer", nullable: false),
                    LotNo = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    FGRollNo = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    IsLoaded = table.Column<bool>(type: "boolean", nullable: false),
                    LoadedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    LoadedBy = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedBy = table.Column<string>(type: "text", nullable: true),
                    UpdatedBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DispatchedRolls", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DispatchedRolls_DispatchPlannings_DispatchPlanningId",
                        column: x => x.DispatchPlanningId,
                        principalTable: "DispatchPlannings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GeneratedBarcodes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RollAssignmentId = table.Column<int>(type: "integer", nullable: false),
                    Barcode = table.Column<string>(type: "text", nullable: false),
                    RollNumber = table.Column<int>(type: "integer", nullable: false),
                    GeneratedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GeneratedBarcodes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GeneratedBarcodes_RollAssignments_RollAssignmentId",
                        column: x => x.RollAssignmentId,
                        principalTable: "RollAssignments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DispatchedRolls_DispatchPlanningId",
                table: "DispatchedRolls",
                column: "DispatchPlanningId");

            migrationBuilder.CreateIndex(
                name: "IX_DispatchedRolls_FGRollNo",
                table: "DispatchedRolls",
                column: "FGRollNo");

            migrationBuilder.CreateIndex(
                name: "IX_DispatchedRolls_LotNo",
                table: "DispatchedRolls",
                column: "LotNo");

            migrationBuilder.CreateIndex(
                name: "IX_DispatchPlannings_ContactPerson",
                table: "DispatchPlannings",
                column: "ContactPerson");

            migrationBuilder.CreateIndex(
                name: "IX_DispatchPlannings_CourierId",
                table: "DispatchPlannings",
                column: "CourierId");

            migrationBuilder.CreateIndex(
                name: "IX_DispatchPlannings_CustomerName",
                table: "DispatchPlannings",
                column: "CustomerName");

            migrationBuilder.CreateIndex(
                name: "IX_DispatchPlannings_LoadingNo",
                table: "DispatchPlannings",
                column: "LoadingNo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DispatchPlannings_LotNo",
                table: "DispatchPlannings",
                column: "LotNo");

            migrationBuilder.CreateIndex(
                name: "IX_DispatchPlannings_SalesOrderId",
                table: "DispatchPlannings",
                column: "SalesOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_DispatchPlannings_TransportId",
                table: "DispatchPlannings",
                column: "TransportId");

            migrationBuilder.CreateIndex(
                name: "IX_DispatchPlannings_TransportName",
                table: "DispatchPlannings",
                column: "TransportName");

            migrationBuilder.CreateIndex(
                name: "IX_GeneratedBarcodes_RollAssignmentId",
                table: "GeneratedBarcodes",
                column: "RollAssignmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Inspections_AllotId_MachineName_RollNo",
                table: "Inspections",
                columns: new[] { "AllotId", "MachineName", "RollNo" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MachineAllocations_ProductionAllotmentId",
                table: "MachineAllocations",
                column: "ProductionAllotmentId");

            migrationBuilder.CreateIndex(
                name: "IX_MachineManagers_MachineName",
                table: "MachineManagers",
                column: "MachineName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PageAccesses_RoleId",
                table: "PageAccesses",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_RoleMasters_RoleName",
                table: "RoleMasters",
                column: "RoleName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RollAssignments_MachineAllocationId",
                table: "RollAssignments",
                column: "MachineAllocationId");

            migrationBuilder.CreateIndex(
                name: "IX_RollConfirmations_AllotId_MachineName_RollNo",
                table: "RollConfirmations",
                columns: new[] { "AllotId", "MachineName", "RollNo" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SalesOrderItemsWeb_ItemName",
                table: "SalesOrderItemsWeb",
                column: "ItemName");

            migrationBuilder.CreateIndex(
                name: "IX_SalesOrderItemsWeb_SalesOrderWebId",
                table: "SalesOrderItemsWeb",
                column: "SalesOrderWebId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesOrdersWeb_BuyerName",
                table: "SalesOrdersWeb",
                column: "BuyerName");

            migrationBuilder.CreateIndex(
                name: "IX_SalesOrdersWeb_OrderDate",
                table: "SalesOrdersWeb",
                column: "OrderDate");

            migrationBuilder.CreateIndex(
                name: "IX_SalesOrdersWeb_VoucherNumber",
                table: "SalesOrdersWeb",
                column: "VoucherNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StorageCaptures_CustomerName",
                table: "StorageCaptures",
                column: "CustomerName");

            migrationBuilder.CreateIndex(
                name: "IX_StorageCaptures_FGRollNo",
                table: "StorageCaptures",
                column: "FGRollNo");

            migrationBuilder.CreateIndex(
                name: "IX_StorageCaptures_LocationCode",
                table: "StorageCaptures",
                column: "LocationCode");

            migrationBuilder.CreateIndex(
                name: "IX_StorageCaptures_LotNo",
                table: "StorageCaptures",
                column: "LotNo");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DispatchedRolls");

            migrationBuilder.DropTable(
                name: "FabricStructureMasters");

            migrationBuilder.DropTable(
                name: "GeneratedBarcodes");

            migrationBuilder.DropTable(
                name: "Inspections");

            migrationBuilder.DropTable(
                name: "LocationMasters");

            migrationBuilder.DropTable(
                name: "MachineManagers");

            migrationBuilder.DropTable(
                name: "PageAccesses");

            migrationBuilder.DropTable(
                name: "RollConfirmations");

            migrationBuilder.DropTable(
                name: "SalesOrderItemsWeb");

            migrationBuilder.DropTable(
                name: "ShiftMasters");

            migrationBuilder.DropTable(
                name: "SlitLineMasters");

            migrationBuilder.DropTable(
                name: "StorageCaptures");

            migrationBuilder.DropTable(
                name: "TapeColorMasters");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "YarnTypeMasters");

            migrationBuilder.DropTable(
                name: "DispatchPlannings");

            migrationBuilder.DropTable(
                name: "RollAssignments");

            migrationBuilder.DropTable(
                name: "RoleMasters");

            migrationBuilder.DropTable(
                name: "SalesOrdersWeb");

            migrationBuilder.DropTable(
                name: "CourierMasters");

            migrationBuilder.DropTable(
                name: "TransportMasters");

            migrationBuilder.DropTable(
                name: "MachineAllocations");

            migrationBuilder.DropTable(
                name: "ProductionAllotments");
        }
    }
}
