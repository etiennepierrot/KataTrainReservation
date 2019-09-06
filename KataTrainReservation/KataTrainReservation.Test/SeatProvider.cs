using System;
using System.Collections.Generic;
using System.Linq;

namespace KataTrainReservation
{
    public class SeatProvider
    {
        private const decimal PercentOnlineBookingAuthorizeOnTrain = (decimal) 70.0;
        private readonly IEnumerable<Coach> Coaches;

        public SeatProvider(IEnumerable<Coach> coaches)
        {
            Coaches = coaches;
        }

        public IEnumerable<Seat> BookSeat(int nbSeat)
        {
            if(PercentNumberOfSeatsBooked(nbSeat) > PercentOnlineBookingAuthorizeOnTrain) 
                throw new ApplicationException("Cannot book more than 70% of the train");

            return Coaches.First(c => c.HasSeatAvailable(nbSeat)).PickSeat(nbSeat);
        }

        private decimal PercentNumberOfSeatsBooked(int nbSeat)
        {
            decimal numberOfSeatsBooked = Coaches.Select(c => c.NumberOfSeatsBooked).Sum();
            decimal numberOfSeats = Coaches.Select(c => c.NumberOfSeats).Sum();
            return ((numberOfSeatsBooked + nbSeat) / numberOfSeats) * 100;
        }
    }
}