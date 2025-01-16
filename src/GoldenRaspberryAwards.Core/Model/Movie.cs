namespace GoldenRaspberryAwards.Core.Model;

public class Movie
{
    public int Id { get; set; }
    public int Year { get; set; }
    public string Title { get; set; }
    public string Studios { get; set; }
    public bool IsWinner { get; set; }
    public ICollection<MovieProducer> MovieProducers { get; set; } = new List<MovieProducer>();
}

