using System.Collections.Generic;

namespace _5Straight.Data.Models
{
    public class BoardLocation
    {
        public BoardLocation()
        {
            FilledBy = -1;
        }

        public int Number { get; set; }

        public string DisplayNumber { get; set; }

        public bool Filled { get; set; }

        public int FilledBy { get; set; }

        public List<int?> AdjacentLocations { get; set; }
    }
}
