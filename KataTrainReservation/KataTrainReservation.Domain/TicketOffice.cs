using System.Linq;

namespace KataTrainReservation.Domain
{
    public class TicketOffice
    {
        private readonly BookingIdGenerator _bookingIdGenerator;
        private readonly TrainRepository _trainRepository;

        public TicketOffice(BookingIdGenerator bookingIdGenerator, TrainRepository trainRepository)
        {
            _bookingIdGenerator = bookingIdGenerator;
            _trainRepository = trainRepository;
        }
        
        public Reservation MakeReservation(ReservationRequest request)
        {
            var train = _trainRepository.Get(request.TrainId);
            var seats = train.BookSeat(train, request.SeatCount);
            return new Reservation(train.TrainId, _bookingIdGenerator.Generate(),  seats.ToList());
        }
    }
}
