import os
from dotenv import load_dotenv
from fastapi import FastAPI, Depends
from fastapi.middleware.cors import CORSMiddleware
from sqlalchemy import Column, create_engine, URL, select
from sqlalchemy.orm import sessionmaker, Session
from sqlalchemy.ext.declarative import declarative_base
from sqlalchemy import String, Integer, Boolean, DateTime, BigInteger, Text, Select
import logging

print("FastAPI up and running!")

app = FastAPI()

app.add_middleware(
    CORSMiddleware,
    allow_origins=["*"],
    allow_credentials=True,
    allow_methods=["*"],
    allow_headers=["*"],
)

# Load environment variables from .env file
load_dotenv()

# Read SQLAlchemy connection string from the .env file
SQLALCHEMY_DATABASE_URL = os.getenv("connString")

if not SQLALCHEMY_DATABASE_URL:
    raise ValueError("Environment variable 'sqlConnectionString' is not set")
else:
    print("Environment Variables successfully loaded!")

engine = create_engine(
    SQLALCHEMY_DATABASE_URL,
    echo=True,  # Set to False in production
    future=True
)

SessionLocal = sessionmaker(autocommit=False, autoflush=False, bind=engine)

# SQLAlchemy Base
Base = declarative_base()

# SQLAlchemy Model for Sleeper Players, matches the database schema.
class SleeperPlayers(Base):
    __tablename__ = "SleeperPlayers"
    PlayerId = Column(String(450), primary_key=True, nullable=False)
    Status = Column(Text, nullable=True)
    Birthdate = Column(DateTime, nullable=True)
    Number = Column(Integer, nullable=True)
    YahooId = Column(Integer, nullable=True)
    StatsId = Column(Integer, nullable=True)
    Position = Column(Text, nullable=True)
    SearchFullName = Column(Text, nullable=True)
    Weight = Column(Text, nullable=True)
    SearchFirstName = Column(Text, nullable=True)
    InjuryBodyPart = Column(Text, nullable=True)
    FantasyDataId = Column(Integer, nullable=True)
    YearsExp = Column(Integer, nullable=True)
    LastName = Column(Text, nullable=True)
    RotoworldId = Column(Integer, nullable=True)
    GsisId = Column(Text, nullable=True)
    NewsUpdated = Column(BigInteger, nullable=True)
    EspnId = Column(Integer, nullable=True)
    Hashtag = Column(Text, nullable=True)
    Age = Column(Integer, nullable=True)
    Height = Column(Text, nullable=True)
    TeamId = Column(Integer, nullable=True)
    DepthChartPosition = Column(Text, nullable=True)
    SearchRank = Column(Integer, nullable=True)
    FullName = Column(Text, nullable=True)
    HighSchool = Column(Text, nullable=True)
    DepthChartOrder = Column(Integer, nullable=True)
    RotowireId = Column(Integer, nullable=True)
    College = Column(Text, nullable=True)
    InjuryStatus = Column(Text, nullable=True)
    FirstName = Column(Text, nullable=True)
    IsActive = Column(Boolean, nullable=False)
    SwishId = Column(Integer, nullable=True)
    SearchLastName = Column(Text, nullable=True)
    InjuryNotes = Column(Text, nullable=True)
    BirthCountry = Column(Text, nullable=True)
    SportRadarId = Column(Text, nullable=True)
    LastUpdated = Column(DateTime, nullable=False)

# SQLAlchemy Model for SportsData.io Players, matches the database schema.
class SportsDataIoPlayers(Base):
    __tablename__ = "SportsDataIoPlayers"
    Id = Column(Integer, primary_key=True, nullable=False)
    FantasyPlayerKey = Column(Text, nullable=False)
    PlayerID = Column(Integer, nullable=False)
    Name = Column(Text, nullable=False)
    PlayerTeamId = Column(Integer, nullable=True)
    Position = Column(Text, nullable=False)
    AverageDraftPosition = Column(Integer, nullable=True)
    AverageDraftPositionPPR = Column(Integer, nullable=True)
    ByeWeek = Column(Integer, nullable=True)
    LastSeasonFantasyPoints = Column(Integer, nullable=True)
    ProjectedFantasyPoints = Column(Integer, nullable=True)
    AuctionValue = Column(Integer, nullable=True)
    AuctionValuePPR = Column(Integer, nullable=True)
    AverageDraftPositionIDP = Column(Integer, nullable=True)
    AverageDraftPositionRookie = Column(Integer, nullable=True)
    AverageDraftPositionDynasty = Column(Integer, nullable=True)
    AverageDraftPosition2QB = Column(Integer, nullable=True)
    LastUpdated = Column(DateTime, nullable=False)

# SQLAlchemy Model for FantasyPros Players, matches the database schema.
class FantasyProsPlayers(Base):
    __tablename__ = "FantasyProsPlayers"
    Id = Column(Integer, primary_key=True, autoincrement=True, nullable=False)
    PlayerId = Column(Integer, nullable=False)
    PlayerName = Column(Text, nullable=False)
    SportsdataId = Column(Text, nullable=False)
    PlayerTeamId = Column(Text, nullable=False)
    TeamId = Column(Integer, nullable=False)
    PlayerPositionId = Column(Text, nullable=False)
    PlayerPositions = Column(Text, nullable=False)
    PlayerShortName = Column(Text, nullable=False)
    PlayerEligibility = Column(Text, nullable=False)
    PlayerYahooPositions = Column(Text, nullable=False)
    PlayerPageUrl = Column(Text, nullable=False)
    PlayerFilename = Column(Text, nullable=False)
    PlayerSquareImageUrl = Column(Text, nullable=False)
    PlayerImageUrl = Column(Text, nullable=False)
    PlayerYahooId = Column(Text, nullable=False)
    CbsPlayerId = Column(Text, nullable=False)
    PlayerByeWeek = Column(Text, nullable=True)
    PlayerOwnedAvg = Column(Integer, nullable=False)
    PlayerOwnedEspn = Column(Integer, nullable=False)
    PlayerOwnedYahoo = Column(Integer, nullable=False)
    RankEcr = Column(Integer, nullable=False)
    RankMin = Column(Text, nullable=False)
    RankMax = Column(Text, nullable=False)
    RankAve = Column(Text, nullable=False)
    RankStd = Column(Text, nullable=False)
    PosRank = Column(Text, nullable=False)
    Tier = Column(Integer, nullable=False)
    LastUpdated = Column(DateTime, nullable=False)

class Team(Base):
    __tablename__ = "Teams"
    Id = Column(Integer, primary_key=True, autoincrement=True, nullable=False)
    Name = Column(Text, nullable=False)
    Abbreviation = Column(Text, nullable=False)

# Dependency for FastAPI routes
def get_db():
    db: Session = SessionLocal()
    try:
        yield db
    finally:
        db.close()

@app.get("/")
def read_root():
    return {"Hello": "World"}

#Right now, focus on Fantasy Pros data.
@app.get("/players")
def get_players(db: Session = Depends(get_db)):
    query = select(
        FantasyProsPlayers.PlayerName,
        FantasyProsPlayers.PlayerId,
        FantasyProsPlayers.PlayerByeWeek,
        FantasyProsPlayers.PlayerOwnedAvg,
        FantasyProsPlayers.PlayerPositionId,
        FantasyProsPlayers.PlayerImageUrl,
        FantasyProsPlayers.Tier,
        FantasyProsPlayers.RankEcr,
        FantasyProsPlayers.LastUpdated,
        FantasyProsPlayers.TeamId,
        Team.Name).join(Team, FantasyProsPlayers.TeamId == Team.Id).order_by(FantasyProsPlayers.RankEcr)
    results = db.execute(query).all()
    
    # Convert tuples to dictionaries
    keys = [
        "PlayerName", "PlayerId", "PlayerByeWeek", "PlayerOwnedAvg", "PlayerPositionId", 
        "PlayerImageUrl", "Tier", "RankEcr", "LastUpdated", "TeamId", "Name"
    ]
    formatted_results = [dict(zip(keys, row)) for row in results]

    return formatted_results


@app.get("/player/{player_id}")
def get_player(player_id: int, db: Session = Depends(get_db)):
    query = select(FantasyProsPlayers).where(FantasyProsPlayers.PlayerId == player_id)
    results = db.execute(query).scalars().all()
    return results

@app.get("/sleeper-players")
def get_sleeper_players(db: Session = Depends(get_db)):
    query = select(SleeperPlayers)
    results = db.execute(query).scalars().all()
    return results

@app.get("/simple-sleeper-players")
def get_simple_sleeper_players(db: Session = Depends(get_db)):
    query = select(
        SleeperPlayers.PlayerId,
        SleeperPlayers.FullName,
        SleeperPlayers.IsActive,
        SleeperPlayers.SearchRank,
        SleeperPlayers.YahooId,
        SleeperPlayers.EspnId,
        SleeperPlayers.Position,
        SleeperPlayers.College,
        SleeperPlayers.TeamId,
        SleeperPlayers.LastUpdated
    ).where(SleeperPlayers.SearchRank < 9999999).order_by(SleeperPlayers.SearchRank)
    
    results = db.execute(query).all()  # Fetch all rows as tuples

    # Convert tuples to dictionaries
    keys = [
        "PlayerId", "FullName", "IsActive", "SearchRank", "YahooId", 
        "EspnId", "Position", "College", "TeamId", "LastUpdated"
    ]
    formatted_results = [dict(zip(keys, row)) for row in results]

    return formatted_results

@app.get("/sportsdataio-players")
def get_sportsdataio_players(db: Session = Depends(get_db)):
    query = select(SportsDataIoPlayers)
    results = db.execute(query).scalars().all()
    return results

@app.get("/fantasypros-players")
def get_fantasypros_players(db: Session = Depends(get_db)):
    query = select(FantasyProsPlayers)
    results = db.execute(query).scalars().all()
    return results

logging.basicConfig()
logging.getLogger("sqlalchemy.engine").setLevel(logging.INFO)