namespace LlamAcademy.Minigolf
{
    [System.Serializable]
    public class Par
    {
        public Rating Rating;
        public int Strokes;

        public Par()
        {
        }

        public Par(Rating rating, int strokes)
        {
            Rating = rating;
            Strokes = strokes;
        }
    }
}
