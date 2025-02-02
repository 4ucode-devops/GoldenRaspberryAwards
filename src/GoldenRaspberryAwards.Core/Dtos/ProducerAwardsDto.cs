namespace GoldenRaspberryAwards.Core.Dtos;

public class ProducerIntervalResultDTO
{
    public List<ProducerIntervalDTO> Min { get; set; } = new List<ProducerIntervalDTO>();
    public List<ProducerIntervalDTO> Max { get; set; } = new List<ProducerIntervalDTO>();
}

public class ProducerIntervalDTO
{
    public string Producer { get; set; }
    public int Interval { get; set; }
    public int PreviousWin { get; set; }
    public int FollowingWin { get; set; }
}
