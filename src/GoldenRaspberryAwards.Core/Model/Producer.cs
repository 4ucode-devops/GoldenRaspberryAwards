namespace GoldenRaspberryAwards.Core.Model
{
    public class Producer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<MovieProducer> MovieProducers { get; set; } = new List<MovieProducer>();
    }
}
