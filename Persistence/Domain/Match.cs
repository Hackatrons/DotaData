namespace DotaData.Persistence.Domain
{
    internal class Match
    {
        public long MatchId { get; set; }
        public bool? RadiantWin { get; set; }
        public int? Duration { get; set; }
        public int? PreGameDuration { get; set; }
        public int? StartTime { get; set; }
        public long? MatchSeqNum { get; set; }
        public int? TowerStatusRadiant { get; set; }
        public int? TowerStatusDire { get; set; }
        public int? BarracksStatusRadiant { get; set; }
        public int? BarracksStatusDire { get; set; }
        public int? Cluster { get; set; }
        public int? FirstBloodTime { get; set; }
        public int? LobbyType { get; set; }
        public int? HumanPlayers { get; set; }
        public int? LeagueId { get; set; }
        public int? GameMode { get; set; }
        public int? Flags { get; set; }
        public int? Engine { get; set; }
        public int? RadiantScore { get; set; }
        public int? DireScore { get; set; }
        public int? Patch { get; set; }
        public int? Region { get; set; }
    }
}
