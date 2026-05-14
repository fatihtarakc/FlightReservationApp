using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace App.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "admins",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    surname = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    is_super_admin = table.Column<bool>(type: "boolean", nullable: false),
                    entity_status = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    created_by = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    created_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "NOW()"),
                    deleted_by = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    deleted_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    modified_by = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    modified_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    identity_id = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_admins", x => x.id);
                    table.CheckConstraint("CK_Admin_CreatedBy_MinLength", "char_length(created_by) >= 5");
                    table.CheckConstraint("CK_Admin_Email_MinLength", "char_length(email) >= 5 AND email ~ '@'");
                    table.CheckConstraint("CK_Admin_Name_MinLength", "char_length(name) >= 2");
                    table.CheckConstraint("CK_Admin_Surname_MinLength", "char_length(surname) >= 2");
                });

            migrationBuilder.CreateTable(
                name: "airlines",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    iata_code = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: false),
                    icao_code = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    country = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    logo_url = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    website = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    entity_status = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    created_by = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    created_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "NOW()"),
                    deleted_by = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    deleted_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    modified_by = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    modified_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_airlines", x => x.id);
                    table.CheckConstraint("CK_Airline_CreatedBy_MinLength", "char_length(created_by) >= 5");
                    table.CheckConstraint("CK_Airline_IataCode_Pattern", "iata_code ~ '^[A-Z0-9]{2}$'");
                    table.CheckConstraint("CK_Airline_IcaoCode_Pattern", "icao_code ~ '^[A-Z]{3}$'");
                    table.CheckConstraint("CK_Airline_Name_Pattern", "name ~ '^[A-Za-z0-9 &-]+$' AND char_length(name) >= 2");
                });

            migrationBuilder.CreateTable(
                name: "airports",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    iata_code = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    icao_code = table.Column<string>(type: "character varying(4)", maxLength: 4, nullable: false),
                    city = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    country = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    time_zone = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    latitude = table.Column<double>(type: "double precision", nullable: false),
                    longitude = table.Column<double>(type: "double precision", nullable: false),
                    entity_status = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    created_by = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    created_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "NOW()"),
                    deleted_by = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    deleted_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    modified_by = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    modified_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_airports", x => x.id);
                    table.CheckConstraint("CK_Airport_City_MinLength", "char_length(city) >= 2");
                    table.CheckConstraint("CK_Airport_Country_MinLength", "char_length(country) >= 2");
                    table.CheckConstraint("CK_Airport_CreatedBy_MinLength", "char_length(created_by) >= 5");
                    table.CheckConstraint("CK_Airport_IataCode_Pattern", "iata_code ~ '^[A-Z]{3}$'");
                    table.CheckConstraint("CK_Airport_IcaoCode_Pattern", "icao_code ~ '^[A-Z]{4}$'");
                    table.CheckConstraint("CK_Airport_Name_MinLength", "char_length(name) >= 3");
                });

            migrationBuilder.CreateTable(
                name: "app_users",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    surname = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    phone_number = table.Column<string>(type: "character varying(16)", maxLength: 16, nullable: false),
                    birth_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    user_status = table.Column<int>(type: "integer", nullable: false, defaultValue: 2),
                    preferred_notification_channel = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    passport_number = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    national_id = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    nationality = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    entity_status = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    created_by = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    created_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "NOW()"),
                    deleted_by = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    deleted_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    modified_by = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    modified_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    identity_id = table.Column<string>(type: "character varying(450)", maxLength: 450, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_app_users", x => x.id);
                    table.CheckConstraint("CK_AppUser_BirthDate_Age", "birth_date <= CURRENT_DATE - INTERVAL '18 years'");
                    table.CheckConstraint("CK_AppUser_CreatedBy_MinLength", "char_length(created_by) >= 5");
                    table.CheckConstraint("CK_AppUser_Email_MinLength", "char_length(email) >= 5 AND email ~ '@'");
                    table.CheckConstraint("CK_AppUser_Name_Pattern", "name ~ '^[A-Za-z -]+$' AND char_length(name) >= 2");
                    table.CheckConstraint("CK_AppUser_PhoneNumber_Pattern", "phone_number ~ '^\\+[0-9]+$' AND char_length(phone_number) >= 8");
                    table.CheckConstraint("CK_AppUser_Surname_Pattern", "surname ~ '^[A-Za-z -]+$' AND char_length(surname) >= 2");
                });

            migrationBuilder.CreateTable(
                name: "identity_roles",
                columns: table => new
                {
                    id = table.Column<string>(type: "text", nullable: false),
                    name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    normalized_name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    concurrency_stamp = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_identity_roles", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "identity_users",
                columns: table => new
                {
                    id = table.Column<string>(type: "text", nullable: false),
                    user_name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    normalized_user_name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    normalized_email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    email_confirmed = table.Column<bool>(type: "boolean", nullable: false),
                    password_hash = table.Column<string>(type: "text", nullable: true),
                    security_stamp = table.Column<string>(type: "text", nullable: true),
                    concurrency_stamp = table.Column<string>(type: "text", nullable: true),
                    phone_number = table.Column<string>(type: "text", nullable: true),
                    phone_number_confirmed = table.Column<bool>(type: "boolean", nullable: false),
                    two_factor_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    lockout_end = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    lockout_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    access_failed_count = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_identity_users", x => x.id);
                    table.CheckConstraint("CK_IdentityUser_Email_MinLength", "char_length(email) >= 5 AND email ~ '@'");
                });

            migrationBuilder.CreateTable(
                name: "manufacturers",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    country = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    entity_status = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    created_by = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    created_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "NOW()"),
                    deleted_by = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    deleted_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    modified_by = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    modified_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_manufacturers", x => x.id);
                    table.CheckConstraint("CK_Manufacturer_Country_MinLength", "char_length(country) >= 2");
                    table.CheckConstraint("CK_Manufacturer_CreatedBy_MinLength", "char_length(created_by) >= 5");
                    table.CheckConstraint("CK_Manufacturer_Name_MinLength", "char_length(name) >= 2");
                });

            migrationBuilder.CreateTable(
                name: "routes",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    distance_km = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    estimated_duration = table.Column<TimeSpan>(type: "interval", nullable: false),
                    departure_airport_id = table.Column<Guid>(type: "uuid", nullable: false),
                    arrival_airport_id = table.Column<Guid>(type: "uuid", nullable: false),
                    entity_status = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    created_by = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    created_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "NOW()"),
                    deleted_by = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    deleted_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    modified_by = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    modified_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_routes", x => x.id);
                    table.CheckConstraint("CK_Route_CreatedBy_MinLength", "char_length(created_by) >= 5");
                    table.CheckConstraint("CK_Route_DifferentAirports", "departure_airport_id <> arrival_airport_id");
                    table.CheckConstraint("CK_Route_DistanceKm_Positive", "distance_km > 0");
                    table.CheckConstraint("CK_Route_EstimatedDuration_Positive", "estimated_duration > '00:00:00'::interval");
                    table.ForeignKey(
                        name: "fk_routes_airports_arrival_airport_id",
                        column: x => x.arrival_airport_id,
                        principalTable: "airports",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_routes_airports_departure_airport_id",
                        column: x => x.departure_airport_id,
                        principalTable: "airports",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "verification_codes",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    code = table.Column<string>(type: "character varying(6)", maxLength: 6, nullable: false),
                    channel = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    purpose = table.Column<int>(type: "integer", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    expires_at = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    attempt_count = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    app_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    entity_status = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    created_by = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    created_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "NOW()"),
                    deleted_by = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    deleted_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    modified_by = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    modified_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_verification_codes", x => x.id);
                    table.CheckConstraint("CK_VerificationCode_AttemptCount_Range", "attempt_count BETWEEN 0 AND 3");
                    table.CheckConstraint("CK_VerificationCode_Code_Pattern", "code ~ '^[0-9]{6}$'");
                    table.CheckConstraint("CK_VerificationCode_CreatedBy_MinLength", "char_length(created_by) >= 5");
                    table.CheckConstraint("CK_VerificationCode_ExpiresAt", "expires_at > created_date");
                    table.ForeignKey(
                        name: "fk_verification_codes_app_users_app_user_id",
                        column: x => x.app_user_id,
                        principalTable: "app_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    role_id = table.Column<string>(type: "text", nullable: false),
                    claim_type = table.Column<string>(type: "text", nullable: true),
                    claim_value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_asp_net_role_claims", x => x.id);
                    table.ForeignKey(
                        name: "fk_asp_net_role_claims_asp_net_roles_role_id",
                        column: x => x.role_id,
                        principalTable: "identity_roles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<string>(type: "text", nullable: false),
                    claim_type = table.Column<string>(type: "text", nullable: true),
                    claim_value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_asp_net_user_claims", x => x.id);
                    table.ForeignKey(
                        name: "fk_asp_net_user_claims_asp_net_users_user_id",
                        column: x => x.user_id,
                        principalTable: "identity_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    login_provider = table.Column<string>(type: "text", nullable: false),
                    provider_key = table.Column<string>(type: "text", nullable: false),
                    provider_display_name = table.Column<string>(type: "text", nullable: true),
                    user_id = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_asp_net_user_logins", x => new { x.login_provider, x.provider_key });
                    table.ForeignKey(
                        name: "fk_asp_net_user_logins_asp_net_users_user_id",
                        column: x => x.user_id,
                        principalTable: "identity_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    user_id = table.Column<string>(type: "text", nullable: false),
                    role_id = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_asp_net_user_roles", x => new { x.user_id, x.role_id });
                    table.ForeignKey(
                        name: "fk_asp_net_user_roles_asp_net_roles_role_id",
                        column: x => x.role_id,
                        principalTable: "identity_roles",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_asp_net_user_roles_asp_net_users_user_id",
                        column: x => x.user_id,
                        principalTable: "identity_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    user_id = table.Column<string>(type: "text", nullable: false),
                    login_provider = table.Column<string>(type: "text", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    value = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_asp_net_user_tokens", x => new { x.user_id, x.login_provider, x.name });
                    table.ForeignKey(
                        name: "fk_asp_net_user_tokens_asp_net_users_user_id",
                        column: x => x.user_id,
                        principalTable: "identity_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "models",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    body_type = table.Column<int>(type: "integer", nullable: false),
                    max_passenger_capacity = table.Column<int>(type: "integer", nullable: false),
                    economy_seats = table.Column<int>(type: "integer", nullable: false),
                    premium_economy_seats = table.Column<int>(type: "integer", nullable: false),
                    business_seats = table.Column<int>(type: "integer", nullable: false),
                    first_class_seats = table.Column<int>(type: "integer", nullable: false),
                    max_range_km = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: false),
                    manufacturer_id = table.Column<Guid>(type: "uuid", nullable: false),
                    entity_status = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    created_by = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    created_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "NOW()"),
                    deleted_by = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    deleted_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    modified_by = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    modified_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_models", x => x.id);
                    table.CheckConstraint("CK_Model_CreatedBy_MinLength", "char_length(created_by) >= 5");
                    table.CheckConstraint("CK_Model_MaxPassengerCapacity_Range", "max_passenger_capacity BETWEEN 1 AND 1000");
                    table.CheckConstraint("CK_Model_MaxRangeKm_Positive", "max_range_km > 0");
                    table.CheckConstraint("CK_Model_Name_MinLength", "char_length(name) >= 2");
                    table.CheckConstraint("CK_Model_SeatCounts_NonNegative", "economy_seats >= 0 AND premium_economy_seats >= 0 AND business_seats >= 0 AND first_class_seats >= 0");
                    table.ForeignKey(
                        name: "fk_models_manufacturers_manufacturer_id",
                        column: x => x.manufacturer_id,
                        principalTable: "manufacturers",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "schedules",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    code = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    valid_from = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    valid_to = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    days_of_week = table.Column<int>(type: "integer", nullable: false),
                    departure_time = table.Column<TimeSpan>(type: "interval", nullable: false),
                    time_zone = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    route_id = table.Column<Guid>(type: "uuid", nullable: false),
                    entity_status = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    created_by = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    created_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "NOW()"),
                    deleted_by = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    deleted_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    modified_by = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    modified_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_schedules", x => x.id);
                    table.CheckConstraint("CK_Schedule_Code_Pattern", "code ~ '^[A-Za-z0-9 .&-]+$' AND char_length(code) >= 3");
                    table.CheckConstraint("CK_Schedule_CreatedBy_MinLength", "char_length(created_by) >= 5");
                    table.CheckConstraint("CK_Schedule_ValidFromValidTo", "valid_to IS NULL OR valid_to > valid_from");
                    table.ForeignKey(
                        name: "fk_schedules_routes_route_id",
                        column: x => x.route_id,
                        principalTable: "routes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "aircrafts",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    tail_number = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    manufacture_year = table.Column<int>(type: "integer", nullable: false),
                    aircraft_status = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    airline_id = table.Column<Guid>(type: "uuid", nullable: false),
                    model_id = table.Column<Guid>(type: "uuid", nullable: false),
                    entity_status = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    created_by = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    created_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "NOW()"),
                    deleted_by = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    deleted_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    modified_by = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    modified_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_aircrafts", x => x.id);
                    table.CheckConstraint("CK_Aircraft_CreatedBy_MinLength", "char_length(created_by) >= 5");
                    table.CheckConstraint("CK_Aircraft_ManufactureYear_Range", "manufacture_year >= 1950 AND manufacture_year <= EXTRACT(YEAR FROM NOW())::int + 2");
                    table.CheckConstraint("CK_Aircraft_TailNumber_Pattern", "tail_number ~ '^[A-Z0-9-]+$' AND char_length(tail_number) >= 2");
                    table.ForeignKey(
                        name: "fk_aircrafts_airlines_airline_id",
                        column: x => x.airline_id,
                        principalTable: "airlines",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_aircrafts_models_model_id",
                        column: x => x.model_id,
                        principalTable: "models",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "flights",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    number = table.Column<string>(type: "character varying(6)", maxLength: 6, nullable: false),
                    departure_date_time = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    arrival_date_time = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    duration = table.Column<TimeSpan>(type: "interval", nullable: false),
                    base_economy_price = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: false),
                    base_premium_economy_price = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: false),
                    base_business_price = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: false),
                    base_first_class_price = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: false),
                    currency = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    flight_status = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    cancellation_reason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    gate = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    terminal = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    aircraft_id = table.Column<Guid>(type: "uuid", nullable: false),
                    airline_id = table.Column<Guid>(type: "uuid", nullable: false),
                    schedule_id = table.Column<Guid>(type: "uuid", nullable: false),
                    entity_status = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    created_by = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    created_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "NOW()"),
                    deleted_by = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    deleted_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    modified_by = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    modified_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_flights", x => x.id);
                    table.CheckConstraint("CK_Flight_CreatedBy_MinLength", "char_length(created_by) >= 5");
                    table.CheckConstraint("CK_Flight_Duration", "arrival_date_time > departure_date_time");
                    table.CheckConstraint("CK_Flight_Number_Pattern", "number ~ '^[A-Z0-9]{3,6}$'");
                    table.CheckConstraint("CK_Flight_Prices_Positive", "base_economy_price > 0 AND base_premium_economy_price > 0 AND base_business_price > 0 AND base_first_class_price > 0");
                    table.ForeignKey(
                        name: "fk_flights_aircrafts_aircraft_id",
                        column: x => x.aircraft_id,
                        principalTable: "aircrafts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_flights_airlines_airline_id",
                        column: x => x.airline_id,
                        principalTable: "airlines",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_flights_schedules_schedule_id",
                        column: x => x.schedule_id,
                        principalTable: "schedules",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "seats",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    row = table.Column<int>(type: "integer", nullable: false),
                    column = table.Column<int>(type: "integer", nullable: false),
                    seat_class = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    is_window_seat = table.Column<bool>(type: "boolean", nullable: false),
                    is_aisle_seat = table.Column<bool>(type: "boolean", nullable: false),
                    has_extra_leg_room = table.Column<bool>(type: "boolean", nullable: false),
                    aircraft_id = table.Column<Guid>(type: "uuid", nullable: false),
                    entity_status = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    created_by = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    created_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "NOW()"),
                    deleted_by = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    deleted_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    modified_by = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    modified_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_seats", x => x.id);
                    table.CheckConstraint("CK_Seat_CreatedBy_MinLength", "char_length(created_by) >= 5");
                    table.CheckConstraint("CK_Seat_Row_Range", "row BETWEEN 1 AND 200");
                    table.ForeignKey(
                        name: "fk_seats_aircrafts_aircraft_id",
                        column: x => x.aircraft_id,
                        principalTable: "aircrafts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "bookings",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    pnr_number = table.Column<string>(type: "character varying(6)", maxLength: 6, nullable: false),
                    total_price = table.Column<decimal>(type: "numeric(12,2)", precision: 12, scale: 2, nullable: false),
                    currency = table.Column<int>(type: "integer", nullable: false),
                    booking_status = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    cancellation_reason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    check_in_time = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    boarding_pass_number = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    is_reminder_sent7days = table.Column<bool>(type: "boolean", nullable: false),
                    is_reminder_sent24hours = table.Column<bool>(type: "boolean", nullable: false),
                    app_user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    flight_id = table.Column<Guid>(type: "uuid", nullable: false),
                    seat_id = table.Column<Guid>(type: "uuid", nullable: false),
                    entity_status = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    created_by = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    created_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: false, defaultValueSql: "NOW()"),
                    deleted_by = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    deleted_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    modified_by = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: true),
                    modified_date = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_bookings", x => x.id);
                    table.CheckConstraint("CK_Booking_CreatedBy_MinLength", "char_length(created_by) >= 5");
                    table.CheckConstraint("CK_Booking_PnrNumber_Pattern", "pnr_number ~ '^[A-Z0-9]{6}$'");
                    table.CheckConstraint("CK_Booking_TotalPrice_Positive", "total_price > 0");
                    table.ForeignKey(
                        name: "fk_bookings_app_users_app_user_id",
                        column: x => x.app_user_id,
                        principalTable: "app_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_bookings_flights_flight_id",
                        column: x => x.flight_id,
                        principalTable: "flights",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_bookings_seats_seat_id",
                        column: x => x.seat_id,
                        principalTable: "seats",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_admins_email",
                table: "admins",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_admins_identity_id",
                table: "admins",
                column: "identity_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_aircrafts_airline_id",
                table: "aircrafts",
                column: "airline_id");

            migrationBuilder.CreateIndex(
                name: "ix_aircrafts_model_id",
                table: "aircrafts",
                column: "model_id");

            migrationBuilder.CreateIndex(
                name: "ix_aircrafts_tail_number",
                table: "aircrafts",
                column: "tail_number",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_airlines_iata_code",
                table: "airlines",
                column: "iata_code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_airlines_icao_code",
                table: "airlines",
                column: "icao_code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_airlines_name",
                table: "airlines",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_airports_iata_code",
                table: "airports",
                column: "iata_code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_airports_icao_code",
                table: "airports",
                column: "icao_code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_airports_name",
                table: "airports",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_app_users_email",
                table: "app_users",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_app_users_identity_id",
                table: "app_users",
                column: "identity_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_app_users_phone_number",
                table: "app_users",
                column: "phone_number",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_asp_net_role_claims_role_id",
                table: "AspNetRoleClaims",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "ix_asp_net_user_claims_user_id",
                table: "AspNetUserClaims",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_asp_net_user_logins_user_id",
                table: "AspNetUserLogins",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_asp_net_user_roles_role_id",
                table: "AspNetUserRoles",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "ix_bookings_app_user_id",
                table: "bookings",
                column: "app_user_id");

            migrationBuilder.CreateIndex(
                name: "ix_bookings_flight_id_seat_id",
                table: "bookings",
                columns: new[] { "flight_id", "seat_id" },
                unique: true,
                filter: "booking_status != 3");

            migrationBuilder.CreateIndex(
                name: "ix_bookings_pnr_number",
                table: "bookings",
                column: "pnr_number",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_bookings_seat_id",
                table: "bookings",
                column: "seat_id");

            migrationBuilder.CreateIndex(
                name: "ix_flights_aircraft_id",
                table: "flights",
                column: "aircraft_id");

            migrationBuilder.CreateIndex(
                name: "ix_flights_airline_id",
                table: "flights",
                column: "airline_id");

            migrationBuilder.CreateIndex(
                name: "ix_flights_number_departure_date_time",
                table: "flights",
                columns: new[] { "number", "departure_date_time" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_flights_schedule_id",
                table: "flights",
                column: "schedule_id");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "identity_roles",
                column: "normalized_name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "identity_users",
                column: "normalized_email");

            migrationBuilder.CreateIndex(
                name: "ix_identity_users_email",
                table: "identity_users",
                column: "email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "identity_users",
                column: "normalized_user_name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_manufacturers_name",
                table: "manufacturers",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_models_manufacturer_id_name",
                table: "models",
                columns: new[] { "manufacturer_id", "name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_routes_arrival_airport_id",
                table: "routes",
                column: "arrival_airport_id");

            migrationBuilder.CreateIndex(
                name: "ix_routes_departure_airport_id_arrival_airport_id",
                table: "routes",
                columns: new[] { "departure_airport_id", "arrival_airport_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_schedules_code",
                table: "schedules",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_schedules_route_id",
                table: "schedules",
                column: "route_id");

            migrationBuilder.CreateIndex(
                name: "ix_seats_aircraft_id_row_column",
                table: "seats",
                columns: new[] { "aircraft_id", "row", "column" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_verification_codes_app_user_id",
                table: "verification_codes",
                column: "app_user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "admins");

            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "bookings");

            migrationBuilder.DropTable(
                name: "verification_codes");

            migrationBuilder.DropTable(
                name: "identity_roles");

            migrationBuilder.DropTable(
                name: "identity_users");

            migrationBuilder.DropTable(
                name: "flights");

            migrationBuilder.DropTable(
                name: "seats");

            migrationBuilder.DropTable(
                name: "app_users");

            migrationBuilder.DropTable(
                name: "schedules");

            migrationBuilder.DropTable(
                name: "aircrafts");

            migrationBuilder.DropTable(
                name: "routes");

            migrationBuilder.DropTable(
                name: "airlines");

            migrationBuilder.DropTable(
                name: "models");

            migrationBuilder.DropTable(
                name: "airports");

            migrationBuilder.DropTable(
                name: "manufacturers");
        }
    }
}
