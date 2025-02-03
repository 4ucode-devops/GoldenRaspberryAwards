namespace GoldenRaspberryAwards.Core.Model;

public class MovieProducer
{
    public Guid MovieId { get; set; }
    public Movie Movie { get; set; }
    public Guid ProducerId { get; set; }
    public Producer Producer { get; set; }
}
