using System;
using System.Linq;
using NUnit.Framework;

namespace KataTrainReservation
{
    [TestFixture]
    public class TicketOfficeTest
    {
        private TicketOffice _ticketOffice;

        [SetUp]
        public void Setup()
        {
            //Generate 10 Coaches with 10 seat (100 seat)
            var coaches = Enumerable.Range(1, 10).Select(CoachGenerator).ToList();
            var seatProvider = new SeatProvider(coaches);
            _ticketOffice = new TicketOffice(new StubBookingIdGenerator("RESA001"), seatProvider);
        }

        private Coach CoachGenerator(int numberCoach)
        {
            return new Coach(numberCoach.ToString("00"), Enumerable.Range(1, 10).ToArray());
        }

        [Test]
        public void ReserveSeats_Should_Book_Ticket_In_The_Requested_Train()
        {
            Reservation reservation = _ticketOffice.MakeReservation(new ReservationRequest("MyTrainId", 2));
            Assert.That(reservation.TrainId, Is.EqualTo("MyTrainId"));
        }

        [Test]
        public void ReserveSeats_Should_Book_The_Requested_Number_Of_Seats()
        {
            Reservation reservation = _ticketOffice.MakeReservation(new ReservationRequest("MyTrainId", 2));
            Assert.That(reservation.Seats, Has.Count.EqualTo(2));
        }


        [Test]
        public void ReserveSeats_Should_Attribute_A_BookingId()
        {
            Reservation reservation = _ticketOffice.MakeReservation(new ReservationRequest("MyTrainId", 2));
            Assert.That(reservation.BookingId, Is.EqualTo("RESA001"));
        }
        
        [Test]
        public void ReserveSeats_Should_Attribute_SeatNumber_In_A_Coach()
        {
            Reservation reservation = _ticketOffice.MakeReservation(new ReservationRequest("MyTrainId", 2));
            
            Assert.That(reservation.Seats[0], Is.EqualTo(new Seat("01", 1)));
            Assert.That(reservation.Seats[1], Is.EqualTo(new Seat("01", 2)));
        }


        [Test]
        public void ReserveSeat_Should_Not_Accept_To_Book_More_Than_70_Percent_For_Same_Train()
        {
            MakeReservations(70);
            Assert.Throws<ApplicationException>(() =>
                _ticketOffice.MakeReservation(new ReservationRequest("MyTrainId", 1)));
        }


        [Test]
        public void ReserveSeat_Should_Book_Seat_In_The_Same_Coach_For_The_Same_Reservation()
        {
            MakeReservations(9);

            Reservation reservation = _ticketOffice.MakeReservation(new ReservationRequest("MyTrainId", 2));
            
            Assert.That(reservation.Seats[0].Coach, Is.EqualTo(reservation.Seats[1].Coach));
        }
        
        

        private void MakeReservations(int nbReservation)
        {
            var reservationRequest = new ReservationRequest("MyTrainId", 1);

            for (int i = 0; i < nbReservation; i++)
            {
                _ticketOffice.MakeReservation(reservationRequest);
            }
        }

        class StubBookingIdGenerator : BookingIdGenerator
        {
            private readonly string _id;

            public StubBookingIdGenerator(string id)
            {
                _id = id;
            }

            public string Generate()
            {
                return _id;
            }
        }
    }
}