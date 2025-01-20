namespace GoldenRaspberryAwards.Core.Model;

public class Producer: EntityBase
{
    public string Name { get; set; }
    public ICollection<MovieProducer> MovieProducers { get; set; } = new List<MovieProducer>();
}
