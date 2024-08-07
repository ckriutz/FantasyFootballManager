﻿// <auto-generated />
using System;
using FantasyFootballManager.DataService.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace FantasyFootballManager.DataService.Migrations
{
    [DbContext(typeof(FantasyDbContext))]
    partial class FantasyDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.6")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("FantasyFootballManager.DataService.Models.DataStatus", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("DataSource")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("LastUpdated")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.ToTable("DataStatus");
                });

            modelBuilder.Entity("FantasyFootballManager.DataService.Models.FantasyPlayer", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<bool>("IsOnMyTeam")
                        .HasColumnType("bit");

                    b.Property<bool>("IsTaken")
                        .HasColumnType("bit");

                    b.Property<bool>("IsThumbsDown")
                        .HasColumnType("bit");

                    b.Property<bool>("IsThumbsUp")
                        .HasColumnType("bit");

                    b.Property<DateTime>("LastUpdated")
                        .HasColumnType("datetime2");

                    b.Property<int?>("PickNumber")
                        .HasColumnType("int");

                    b.Property<int?>("PickRound")
                        .HasColumnType("int");

                    b.Property<string>("PickedBy")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PlayerId")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("FantasyPlayers");
                });

            modelBuilder.Entity("FantasyFootballManager.DataService.Models.FantasyProsPlayer", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("CbsPlayerId")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasAnnotation("Relational:JsonPropertyName", "cbs_player_id");

                    b.Property<DateTime>("LastUpdated")
                        .HasColumnType("datetime2");

                    b.Property<string>("PlayerByeWeek")
                        .HasColumnType("nvarchar(max)")
                        .HasAnnotation("Relational:JsonPropertyName", "player_bye_week");

                    b.Property<string>("PlayerEligibility")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasAnnotation("Relational:JsonPropertyName", "player_eligibility");

                    b.Property<string>("PlayerFilename")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasAnnotation("Relational:JsonPropertyName", "player_filename");

                    b.Property<int>("PlayerId")
                        .HasColumnType("int")
                        .HasAnnotation("Relational:JsonPropertyName", "player_id");

                    b.Property<string>("PlayerImageUrl")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasAnnotation("Relational:JsonPropertyName", "player_image_url");

                    b.Property<string>("PlayerName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasAnnotation("Relational:JsonPropertyName", "player_name");

                    b.Property<double>("PlayerOwnedAvg")
                        .HasColumnType("float")
                        .HasAnnotation("Relational:JsonPropertyName", "player_owned_avg");

                    b.Property<double>("PlayerOwnedEspn")
                        .HasColumnType("float")
                        .HasAnnotation("Relational:JsonPropertyName", "player_owned_espn");

                    b.Property<int>("PlayerOwnedYahoo")
                        .HasColumnType("int")
                        .HasAnnotation("Relational:JsonPropertyName", "player_owned_yahoo");

                    b.Property<string>("PlayerPageUrl")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasAnnotation("Relational:JsonPropertyName", "player_page_url");

                    b.Property<string>("PlayerPositionId")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasAnnotation("Relational:JsonPropertyName", "player_position_id");

                    b.Property<string>("PlayerPositions")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasAnnotation("Relational:JsonPropertyName", "player_positions");

                    b.Property<string>("PlayerShortName")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasAnnotation("Relational:JsonPropertyName", "player_short_name");

                    b.Property<string>("PlayerSquareImageUrl")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasAnnotation("Relational:JsonPropertyName", "player_square_image_url");

                    b.Property<string>("PlayerTeamId")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasAnnotation("Relational:JsonPropertyName", "player_team_id");

                    b.Property<string>("PlayerYahooId")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasAnnotation("Relational:JsonPropertyName", "player_yahoo_id");

                    b.Property<string>("PlayerYahooPositions")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasAnnotation("Relational:JsonPropertyName", "player_yahoo_positions");

                    b.Property<string>("PosRank")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasAnnotation("Relational:JsonPropertyName", "pos_rank");

                    b.Property<string>("RankAve")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasAnnotation("Relational:JsonPropertyName", "rank_ave");

                    b.Property<int>("RankEcr")
                        .HasColumnType("int")
                        .HasAnnotation("Relational:JsonPropertyName", "rank_ecr");

                    b.Property<string>("RankMax")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasAnnotation("Relational:JsonPropertyName", "rank_max");

                    b.Property<string>("RankMin")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasAnnotation("Relational:JsonPropertyName", "rank_min");

                    b.Property<string>("RankStd")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasAnnotation("Relational:JsonPropertyName", "rank_std");

                    b.Property<string>("SportsdataId")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasAnnotation("Relational:JsonPropertyName", "sportsdata_id");

                    b.Property<int>("TeamId")
                        .HasColumnType("int");

                    b.Property<int>("Tier")
                        .HasColumnType("int")
                        .HasAnnotation("Relational:JsonPropertyName", "tier");

                    b.HasKey("Id");

                    b.HasIndex("TeamId");

                    b.ToTable("FantasyProsPlayers");
                });

            modelBuilder.Entity("FantasyFootballManager.DataService.Models.SleeperPlayer", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int?>("Age")
                        .HasColumnType("int")
                        .HasAnnotation("Relational:JsonPropertyName", "age");

                    b.Property<string>("BirthCountry")
                        .HasColumnType("nvarchar(max)")
                        .HasAnnotation("Relational:JsonPropertyName", "birth_country");

                    b.Property<DateTime?>("Birthdate")
                        .HasColumnType("datetime2")
                        .HasAnnotation("Relational:JsonPropertyName", "birth_date");

                    b.Property<string>("College")
                        .HasColumnType("nvarchar(max)")
                        .HasAnnotation("Relational:JsonPropertyName", "college");

                    b.Property<int?>("DepthChartOrder")
                        .HasColumnType("int")
                        .HasAnnotation("Relational:JsonPropertyName", "depth_chart_order");

                    b.Property<string>("DepthChartPosition")
                        .HasColumnType("nvarchar(max)")
                        .HasAnnotation("Relational:JsonPropertyName", "depth_chart_position");

                    b.Property<int?>("EspnId")
                        .HasColumnType("int")
                        .HasAnnotation("Relational:JsonPropertyName", "espn_id");

                    b.Property<int?>("FantasyDataId")
                        .HasColumnType("int")
                        .HasAnnotation("Relational:JsonPropertyName", "fantasy_data_id");

                    b.Property<string>("FirstName")
                        .HasColumnType("nvarchar(max)")
                        .HasAnnotation("Relational:JsonPropertyName", "first_name");

                    b.Property<string>("FullName")
                        .HasColumnType("nvarchar(max)")
                        .HasAnnotation("Relational:JsonPropertyName", "full_name");

                    b.Property<string>("GsisId")
                        .HasColumnType("nvarchar(max)")
                        .HasAnnotation("Relational:JsonPropertyName", "gsis_id");

                    b.Property<string>("Hashtag")
                        .HasColumnType("nvarchar(max)")
                        .HasAnnotation("Relational:JsonPropertyName", "hashtag");

                    b.Property<string>("Height")
                        .HasColumnType("nvarchar(max)")
                        .HasAnnotation("Relational:JsonPropertyName", "height");

                    b.Property<string>("HighSchool")
                        .HasColumnType("nvarchar(max)")
                        .HasAnnotation("Relational:JsonPropertyName", "high_school");

                    b.Property<string>("InjuryBodyPart")
                        .HasColumnType("nvarchar(max)")
                        .HasAnnotation("Relational:JsonPropertyName", "injury_body_part");

                    b.Property<string>("InjuryNotes")
                        .HasColumnType("nvarchar(max)")
                        .HasAnnotation("Relational:JsonPropertyName", "injury_notes");

                    b.Property<string>("InjuryStatus")
                        .HasColumnType("nvarchar(max)")
                        .HasAnnotation("Relational:JsonPropertyName", "injury_status");

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit")
                        .HasAnnotation("Relational:JsonPropertyName", "active");

                    b.Property<string>("LastName")
                        .HasColumnType("nvarchar(max)")
                        .HasAnnotation("Relational:JsonPropertyName", "last_name");

                    b.Property<DateTime>("LastUpdated")
                        .HasColumnType("datetime2");

                    b.Property<long?>("NewsUpdated")
                        .HasColumnType("bigint")
                        .HasAnnotation("Relational:JsonPropertyName", "news_updated");

                    b.Property<int?>("Number")
                        .HasColumnType("int")
                        .HasAnnotation("Relational:JsonPropertyName", "number");

                    b.Property<string>("PlayerId")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasAnnotation("Relational:JsonPropertyName", "player_id");

                    b.Property<string>("Position")
                        .HasColumnType("nvarchar(max)")
                        .HasAnnotation("Relational:JsonPropertyName", "position");

                    b.Property<int?>("RotowireId")
                        .HasColumnType("int")
                        .HasAnnotation("Relational:JsonPropertyName", "rotowire_id");

                    b.Property<int?>("RotoworldId")
                        .HasColumnType("int")
                        .HasAnnotation("Relational:JsonPropertyName", "rotoworld_id");

                    b.Property<string>("SearchFirstName")
                        .HasColumnType("nvarchar(max)")
                        .HasAnnotation("Relational:JsonPropertyName", "search_first_name");

                    b.Property<string>("SearchFullName")
                        .HasColumnType("nvarchar(max)")
                        .HasAnnotation("Relational:JsonPropertyName", "search_full_name");

                    b.Property<string>("SearchLastName")
                        .HasColumnType("nvarchar(max)")
                        .HasAnnotation("Relational:JsonPropertyName", "search_last_name");

                    b.Property<int?>("SearchRank")
                        .HasColumnType("int")
                        .HasAnnotation("Relational:JsonPropertyName", "search_rank");

                    b.Property<string>("SportRadarId")
                        .HasColumnType("nvarchar(max)")
                        .HasAnnotation("Relational:JsonPropertyName", "sportradar_id");

                    b.Property<int?>("StatsId")
                        .HasColumnType("int")
                        .HasAnnotation("Relational:JsonPropertyName", "stats_id");

                    b.Property<string>("Status")
                        .HasColumnType("nvarchar(max)")
                        .HasAnnotation("Relational:JsonPropertyName", "status");

                    b.Property<int?>("SwishId")
                        .HasColumnType("int")
                        .HasAnnotation("Relational:JsonPropertyName", "swish_id");

                    b.Property<int?>("TeamId")
                        .HasColumnType("int");

                    b.Property<string>("Weight")
                        .HasColumnType("nvarchar(max)")
                        .HasAnnotation("Relational:JsonPropertyName", "weight");

                    b.Property<int?>("YahooId")
                        .HasColumnType("int")
                        .HasAnnotation("Relational:JsonPropertyName", "yahoo_id");

                    b.Property<int?>("YearsExp")
                        .HasColumnType("int")
                        .HasAnnotation("Relational:JsonPropertyName", "years_exp");

                    b.HasKey("Id");

                    b.HasIndex("TeamId");

                    b.ToTable("SleeperPlayers");
                });

            modelBuilder.Entity("FantasyFootballManager.DataService.Models.SportsDataIoPlayer", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<double?>("AuctionValue")
                        .HasColumnType("float")
                        .HasAnnotation("Relational:JsonPropertyName", "AuctionValue");

                    b.Property<double?>("AuctionValuePPR")
                        .HasColumnType("float")
                        .HasAnnotation("Relational:JsonPropertyName", "AuctionValuePPR");

                    b.Property<double?>("AverageDraftPosition")
                        .HasColumnType("float")
                        .HasAnnotation("Relational:JsonPropertyName", "AverageDraftPosition");

                    b.Property<double?>("AverageDraftPosition2QB")
                        .HasColumnType("float")
                        .HasAnnotation("Relational:JsonPropertyName", "AverageDraftPosition2QB");

                    b.Property<double?>("AverageDraftPositionDynasty")
                        .HasColumnType("float")
                        .HasAnnotation("Relational:JsonPropertyName", "AverageDraftPositionDynasty");

                    b.Property<double?>("AverageDraftPositionIDP")
                        .HasColumnType("float")
                        .HasAnnotation("Relational:JsonPropertyName", "AverageDraftPositionIDP");

                    b.Property<double?>("AverageDraftPositionPPR")
                        .HasColumnType("float")
                        .HasAnnotation("Relational:JsonPropertyName", "AverageDraftPositionPPR");

                    b.Property<double?>("AverageDraftPositionRookie")
                        .HasColumnType("float")
                        .HasAnnotation("Relational:JsonPropertyName", "AverageDraftPositionRookie");

                    b.Property<int?>("ByeWeek")
                        .HasColumnType("int")
                        .HasAnnotation("Relational:JsonPropertyName", "ByeWeek");

                    b.Property<string>("FantasyPlayerKey")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasAnnotation("Relational:JsonPropertyName", "FantasyPlayerKey");

                    b.Property<double?>("LastSeasonFantasyPoints")
                        .HasColumnType("float")
                        .HasAnnotation("Relational:JsonPropertyName", "LastSeasonFantasyPoints");

                    b.Property<DateTime>("LastUpdated")
                        .HasColumnType("datetime2");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasAnnotation("Relational:JsonPropertyName", "Name");

                    b.Property<int>("PlayerID")
                        .HasColumnType("int")
                        .HasAnnotation("Relational:JsonPropertyName", "PlayerID");

                    b.Property<int?>("PlayerTeamId")
                        .HasColumnType("int");

                    b.Property<string>("Position")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasAnnotation("Relational:JsonPropertyName", "Position");

                    b.Property<double?>("ProjectedFantasyPoints")
                        .HasColumnType("float")
                        .HasAnnotation("Relational:JsonPropertyName", "ProjectedFantasyPoints");

                    b.HasKey("Id");

                    b.HasIndex("PlayerTeamId");

                    b.ToTable("SportsDataIoPlayers");
                });

            modelBuilder.Entity("FantasyFootballManager.DataService.Models.Team", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<string>("Abbreviation")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Teams");
                });

            modelBuilder.Entity("FantasyFootballManager.DataService.Models.FantasyProsPlayer", b =>
                {
                    b.HasOne("FantasyFootballManager.DataService.Models.Team", "Team")
                        .WithMany()
                        .HasForeignKey("TeamId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Team");
                });

            modelBuilder.Entity("FantasyFootballManager.DataService.Models.SleeperPlayer", b =>
                {
                    b.HasOne("FantasyFootballManager.DataService.Models.Team", "Team")
                        .WithMany()
                        .HasForeignKey("TeamId");

                    b.Navigation("Team");
                });

            modelBuilder.Entity("FantasyFootballManager.DataService.Models.SportsDataIoPlayer", b =>
                {
                    b.HasOne("FantasyFootballManager.DataService.Models.Team", "PlayerTeam")
                        .WithMany()
                        .HasForeignKey("PlayerTeamId");

                    b.Navigation("PlayerTeam");
                });
#pragma warning restore 612, 618
        }
    }
}
