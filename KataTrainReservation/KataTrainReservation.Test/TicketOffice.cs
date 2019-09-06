using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KataTrainReservation
{
    public class TicketOffice
    {
        private readonly BookingIdGenerator _bookingIdGenerator;
        private readonly SeatProvider _seatProvider;

        public TicketOffice(BookingIdGenerator bookingIdGenerator, SeatProvider seatProvider)
        {
            _bookingIdGenerator = bookingIdGenerator;
            _seatProvider = seatProvider;
        }
        
        public Reservation MakeReservation(ReservationRequest request)
        {
            var seats = RequestSeats(request.SeatCount);
            return new Reservation(request.TrainId, _bookingIdGenerator.Generate(),  seats);
        }

        private List<Seat> RequestSeats(int requestSeatCount)
        {
            return _seatProvider.BookSeat(requestSeatCount).ToList();
        }
    }
}
