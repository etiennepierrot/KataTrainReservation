using System.Collections.Generic;
using System.Linq;

namespace KataTrainReservation
{
    public class Coach
    {
        

        private readonly Queue<int> _availablesSeatNumber;
        private readonly string _id;
        public int NumberOfSeats { get; }
        public int NumberOfSeatsBooked => NumberOfSeats - _availablesSeatNumber.Count;


        public Coach(string id, int[] availablesSeatNumber)
        {
            _id = id;
            NumberOfSeats = availablesSeatNumber.Length;
            _availablesSeatNumber = new Queue<int>();
            foreach (var seatNumber in availablesSeatNumber)
            {
                _availablesSeatNumber.Enqueue(seatNumber);
            }
        }

        public IEnumerable<Seat> PickSeat(int nbSeats)
        {
            return Enumerable.Range(0, nbSeats).Select(i => new Seat(_id, _availablesSeatNumber.Dequeue()));
        }

        public bool HasSeatAvailable(int nbSeatToBook)
        {
            return _availablesSeatNumber.Count >= nbSeatToBook;
        }
    }
}