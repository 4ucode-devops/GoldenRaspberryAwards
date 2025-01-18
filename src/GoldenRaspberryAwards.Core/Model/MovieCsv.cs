namespace GoldenRaspberryAwards.Core.Model
{
    public class MovieCsv
    {
        public int Year { get; set; }
        public string Title { get; set; }
        public string Studios { get; set; }
        public string IsWinner { get; set; } = String.Empty;
    }
}
