namespace GoldenRaspberryAwards.Core.Model;

public class Movie: EntityBase
{
    public int Year { get; set; }
    public string Title { get; set; }
    public string Studios { get; set; }
    public string IsWinner { get; set; } = String.Empty;
    public ICollection<MovieProducer> MovieProducers { get; set; } = new List<MovieProducer>();
}
