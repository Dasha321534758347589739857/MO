namespace MO_kursasch_25.Models
{
    public class FuncPoint
    {
        public double First { get; set; }
        public double Second { get; set; }
        public double FuncNum { get; set; }

        public FuncPoint(double first, double second)
        {
            First = first;
            Second = second;
        }
    }
}
