using System;
using System.Collections.Generic;
using System.Linq;

namespace KataTrainReservation.Domain
{
    public class Train
    {
        private List<Coach> Coaches { get; }
        public string TrainId { get; }

        private const decimal PercentOnlineBookingAuthorizeOnTrain = (decimal) 70.0;


        public Train(List<Coach> coaches, string trainId)
        {
            Coaches = coaches;
            TrainId = trainId;
        }
        
        public IEnumerable<Seat> BookSeat(int nbSeat)
        {
            if(PercentNumberOfSeatsBooked(nbSeat) > PercentOnlineBookingAuthorizeOnTrain) 
                throw new ApplicationException("Cannot book more than 70% of the train");

            return SelectAvailableCoach( nbSeat).PickSeat(nbSeat);
        }

        private Coach SelectAvailableCoach(int nbSeat)
        {

            var availableCoach = Coaches.FirstOrDefault(c => 
                                           c.HasSeatAvailable(nbSeat)) ?? Coaches.FirstOrDefault(c => !c.TotallyFull(nbSeat)
                                       );
            
            if(availableCoach == null) throw new ApplicationException("No coach has the enough seat ");
            return availableCoach;
        }

        private decimal PercentNumberOfSeatsBooked(int nbSeat)
        {
            decimal numberOfSeatsBooked = Coaches.Select(c => c.NumberOfSeatsBooked).Sum();
            decimal numberOfSeats = Coaches.Select(c => c.NumberOfSeats).Sum();
            return ((numberOfSeatsBooked + nbSeat) / numberOfSeats) * 100;
        }
    }
}