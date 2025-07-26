import os
from dotenv import load_dotenv
from fastapi import FastAPI, Depends
from fastapi.middleware.cors import CORSMiddleware
from sqlalchemy import Column, create_engine, update, select, insert
from sqlalchemy.orm import sessionmaker, Session
from sqlalchemy.ext.declarative import declarative_base
from sqlalchemy import String, Integer, Boolean, DateTime, BigInteger, Text, Select
import logging
from sqlalchemy.exc import SQLAlchemyError
from fastapi import HTTPException

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

class DataStatus(Base):
    __tablename__ = "DataStatus"
    Id = Column(Integer, primary_key=True, autoincrement=True, nullable=False)
    DataSource = Column(Text, nullable=False)
    LastUpdated = Column(DateTime, nullable=False)

class FantasyActivities(Base):
    __tablename__ = "FantasyActivities"
    Id = Column(Integer, primary_key=True, autoincrement=True, nullable=False)
    User = Column(Text, nullable=False)
    PlayerId = Column(Integer, nullable=False)
    IsThumbsUp = Column(Boolean, nullable=False)
    IsThumbsDown = Column(Boolean, nullable=False)
    IsDraftedOnMyTeam = Column(Boolean, nullable=False)
    IsDraftedOnOtherTeam = Column(Boolean, nullable=False)

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

@app.get("/user-players/{user}")
def get_user_players(user: str, db: Session = Depends(get_db)):
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
        Team.Name,
        FantasyActivities.IsThumbsUp,
        FantasyActivities.IsThumbsDown,
        FantasyActivities.IsDraftedOnMyTeam,
        FantasyActivities.IsDraftedOnOtherTeam).join(Team, FantasyProsPlayers.TeamId == Team.Id).join(FantasyActivities, (FantasyActivities.PlayerId == FantasyProsPlayers.PlayerId) & (FantasyActivities.User == user), isouter=True).order_by(FantasyProsPlayers.RankEcr)
    results = db.execute(query).all()

    # Convert tuples to dictionaries
    keys = [
        "PlayerName", "PlayerId", "PlayerByeWeek", "PlayerOwnedAvg", "PlayerPositionId", 
        "PlayerImageUrl", "Tier", "RankEcr", "LastUpdated", "TeamId", "TeamName", 
        "IsThumbsUp", "IsThumbsDown", "IsDraftedOnMyTeam", "IsDraftedOnOtherTeam"
    ]
    formatted_results = [dict(zip(keys, row)) for row in results]

    return formatted_results

@app.get("/my-players/{user}")
def get_my_players(user: str, db: Session = Depends(get_db)):
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
        Team.Name,
        FantasyActivities.IsThumbsUp,
        FantasyActivities.IsThumbsDown,
        FantasyActivities.IsDraftedOnMyTeam,
        FantasyActivities.IsDraftedOnOtherTeam).where(FantasyActivities.User == user, FantasyActivities.IsDraftedOnMyTeam == True).join(Team, FantasyProsPlayers.TeamId == Team.Id).join(FantasyActivities, (FantasyActivities.PlayerId == FantasyProsPlayers.PlayerId) & (FantasyActivities.User == user)).order_by(FantasyProsPlayers.RankEcr)
    results = db.execute(query).all()

    # Convert tuples to dictionaries
    keys = [
        "PlayerName", "PlayerId", "PlayerByeWeek", "PlayerOwnedAvg", "PlayerPositionId", 
        "PlayerImageUrl", "Tier", "RankEcr", "LastUpdated", "TeamId", "TeamName", 
        "IsThumbsUp", "IsThumbsDown", "IsDraftedOnMyTeam", "IsDraftedOnOtherTeam"
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

@app.get('/datastatuses')
def get_datastatuses(db: Session = Depends(get_db)):
    query = select(DataStatus)
    results = db.execute(query).scalars().all()
    return results

@app.get('/fantasyactivities/{user}')
def get_fantasy_activities(user: str, db: Session = Depends(get_db)):
    query = select(FantasyActivities).where(FantasyActivities.User == user)
    results = db.execute(query).scalars().all()
    return results

@app.post('/player/claim/{player_id}')
def claim_player(player_id: int, user: str, db: Session = Depends(get_db)):
    # First, check if there is an existing record for this player and user.
    existing_fantasy_activity = db.execute(
        select(FantasyActivities).where(
            FantasyActivities.PlayerId == player_id,
            FantasyActivities.User == user
        )
    ).scalars().first()

    if existing_fantasy_activity:
        # Update existing record
        query = (
            update(FantasyActivities)
            .where(FantasyActivities.Id == existing_fantasy_activity.Id)
            .values(
                IsThumbsUp=False,
                IsThumbsDown=False,
                IsDraftedOnMyTeam=True,
                IsDraftedOnOtherTeam=False
            )
            .returning(
                FantasyActivities.Id,
                FantasyActivities.User,
                FantasyActivities.PlayerId,
                FantasyActivities.IsThumbsUp,
                FantasyActivities.IsThumbsDown,
                FantasyActivities.IsDraftedOnMyTeam,
                FantasyActivities.IsDraftedOnOtherTeam
            )
        )
    else:
        # Insert the new record and return the inserted row
        query = insert(FantasyActivities).values(
            User=user,
            PlayerId=player_id,
            IsThumbsUp=False,
            IsThumbsDown=False,
            IsDraftedOnMyTeam=True,
            IsDraftedOnOtherTeam=False
        ).returning(
            FantasyActivities.Id,
            FantasyActivities.User,
            FantasyActivities.PlayerId,
            FantasyActivities.IsThumbsUp,
            FantasyActivities.IsThumbsDown,
            FantasyActivities.IsDraftedOnMyTeam,
            FantasyActivities.IsDraftedOnOtherTeam
        )
    result = db.execute(query).first()
    db.commit()

    # Return the inserted record as a dictionary
    return result._asdict() if result else {"error": "Failed to claim player"}
    
@app.post('/player/notclaimed/{player_id}')
def reset_player(player_id: int, user: str, db: Session = Depends(get_db)):
    existing_fantasy_activity = db.execute(
        select(FantasyActivities).where(
            FantasyActivities.PlayerId == player_id,
            FantasyActivities.User == user
        )
    ).scalars().first()

    if existing_fantasy_activity:
        # Update existing record
        query = (
            update(FantasyActivities)
            .where(FantasyActivities.Id == existing_fantasy_activity.Id)
            .values(
                IsDraftedOnMyTeam=False,
                IsDraftedOnOtherTeam=True
            )
        ).returning(
            FantasyActivities.Id,
            FantasyActivities.User,
            FantasyActivities.PlayerId,
            FantasyActivities.IsThumbsUp,
            FantasyActivities.IsThumbsDown,
            FantasyActivities.IsDraftedOnMyTeam,
            FantasyActivities.IsDraftedOnOtherTeam
        )
    else:
        query = insert(FantasyActivities).values(
            User=user,
            PlayerId=player_id,
            IsThumbsUp=False,
            IsThumbsDown=False,
            IsDraftedOnMyTeam=False,
            IsDraftedOnOtherTeam=True
        ).returning(
        FantasyActivities.Id,
        FantasyActivities.User,
        FantasyActivities.PlayerId,
        FantasyActivities.IsThumbsUp,
        FantasyActivities.IsThumbsDown,
        FantasyActivities.IsDraftedOnMyTeam,
        FantasyActivities.IsDraftedOnOtherTeam
    )
    result = db.execute(query).first()
    db.commit()
    # Return the inserted record as a dictionary
    return result._asdict() if result else {"error": "Failed to claim player"}

@app.post('/player/resetstatus/{player_id}')
def not_claim_player(player_id: int, user: str, db: Session = Depends(get_db)):
    existing_fantasy_activity = db.execute(
        select(FantasyActivities).where(
            FantasyActivities.PlayerId == player_id,
            FantasyActivities.User == user
        )
    ).scalars().first()

    if existing_fantasy_activity:
        # Update existing record
        query = (
            update(FantasyActivities)
            .where(FantasyActivities.Id == existing_fantasy_activity.Id)
            .values(
                IsDraftedOnMyTeam=False,
                IsDraftedOnOtherTeam=False
            )
        ).returning(
            FantasyActivities.Id,
            FantasyActivities.User,
            FantasyActivities.PlayerId,
            FantasyActivities.IsThumbsUp,
            FantasyActivities.IsThumbsDown,
            FantasyActivities.IsDraftedOnMyTeam,
            FantasyActivities.IsDraftedOnOtherTeam
        )
    else:
        query = insert(FantasyActivities).values(
            User=user,
            PlayerId=player_id,
            IsThumbsUp=False,
            IsThumbsDown=False,
            IsDraftedOnMyTeam=False,
            IsDraftedOnOtherTeam=False
        ).returning(
        FantasyActivities.Id,
        FantasyActivities.User,
        FantasyActivities.PlayerId,
        FantasyActivities.IsThumbsUp,
        FantasyActivities.IsThumbsDown,
        FantasyActivities.IsDraftedOnMyTeam,
        FantasyActivities.IsDraftedOnOtherTeam
    )
    result = db.execute(query).first()
    db.commit()
    # Return the inserted record as a dictionary
    return result._asdict() if result else {"error": "Failed to reset player"}

@app.post('/player/thumbsup/{player_id}')
def thumbs_up_player(player_id: int, user: str, db: Session = Depends(get_db)):
    existing_fantasy_activity = db.execute(
        select(FantasyActivities).where(
            FantasyActivities.PlayerId == player_id,
            FantasyActivities.User == user
        )
    ).scalars().first()

    if existing_fantasy_activity:
        # Okay, so a record exists, and thumbsUp is already true, lets set it to false.
        th = True
        if existing_fantasy_activity.IsThumbsUp: # type: ignore
            th = False

        # Update existing record
        query = (
            update(FantasyActivities)
            .where(FantasyActivities.Id == existing_fantasy_activity.Id)
            .values(
                IsThumbsUp=th,
                IsThumbsDown=False
            )
        ).returning(
            FantasyActivities.Id,
            FantasyActivities.User,
            FantasyActivities.PlayerId,
            FantasyActivities.IsThumbsUp,
            FantasyActivities.IsThumbsDown,
            FantasyActivities.IsDraftedOnMyTeam,
            FantasyActivities.IsDraftedOnOtherTeam
        )
    else:
        query = insert(FantasyActivities).values(
            User=user,
            PlayerId=player_id,
            IsThumbsUp=True,
            IsThumbsDown=False,
            IsDraftedOnMyTeam=False,
            IsDraftedOnOtherTeam=False
        ).returning(
        FantasyActivities.Id,
        FantasyActivities.User,
        FantasyActivities.PlayerId,
        FantasyActivities.IsThumbsUp,
        FantasyActivities.IsThumbsDown,
        FantasyActivities.IsDraftedOnMyTeam,
        FantasyActivities.IsDraftedOnOtherTeam
    )
    result = db.execute(query).first()
    db.commit()
    # Return the inserted record as a dictionary
    return result._asdict() if result else {"error": "Failed to thumbs up player"}

@app.post('/player/thumbsdown/{player_id}')
def thumbs_down_player(player_id: int, user: str, db: Session = Depends(get_db)):
    existing_fantasy_activity = db.execute(
        select(FantasyActivities).where(
            FantasyActivities.PlayerId == player_id,
            FantasyActivities.User == user
        )
    ).scalars().first()

    if existing_fantasy_activity:
        # Update existing record
        td = True
        if existing_fantasy_activity.IsThumbsDown: # type: ignore
            td = False
        query = (
            update(FantasyActivities)
            .where(FantasyActivities.Id == existing_fantasy_activity.Id)
            .values(
                IsThumbsUp=False,
                IsThumbsDown=True
            )
        ).returning(
            FantasyActivities.Id,
            FantasyActivities.User,
            FantasyActivities.PlayerId,
            FantasyActivities.IsThumbsUp,
            FantasyActivities.IsThumbsDown,
            FantasyActivities.IsDraftedOnMyTeam,
            FantasyActivities.IsDraftedOnOtherTeam
        )
    else:
        query = insert(FantasyActivities).values(
            User=user,
            PlayerId=player_id,
            IsThumbsUp=False,
            IsThumbsDown=True,
            IsDraftedOnMyTeam=False,
            IsDraftedOnOtherTeam=False
        ).returning(
        FantasyActivities.Id,
        FantasyActivities.User,
        FantasyActivities.PlayerId,
        FantasyActivities.IsThumbsUp,
        FantasyActivities.IsThumbsDown,
        FantasyActivities.IsDraftedOnMyTeam,
        FantasyActivities.IsDraftedOnOtherTeam
    )
    result = db.execute(query).first()
    db.commit()
    # Return the inserted record as a dictionary
    return result._asdict() if result else {"error": "Failed to thumbs down player"}