using System;
using System.Collections.Generic;
using System.Linq;
using KataTrainReservation.Domain;
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
            BookingIdGenerator bookingIdGenerator = new StubBookingIdGenerator("RESA001");
            var train = new Train(Enumerable.Range(1, 10).Select(CoachGenerator).ToList(), "MyTrainId");
            var stubTrainRepository = new StubTrainRepository(new []{train});
            _ticketOffice = new TicketOffice(bookingIdGenerator, stubTrainRepository );
        }
        
        private static Coach CoachGenerator(int numberCoach)
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
        public void ReserveSeats_Should_Book_The_Seats()
        {
            Reservation reservation = _ticketOffice.MakeReservation(new ReservationRequest("MyTrainId", 2));
            Assert.That(reservation.Seats, Is.EquivalentTo(new List<Seat>(){new Seat("01", 1), new Seat("01", 2)} ));
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
        
        
        [Test]
        public void ReserveSeat_Should_Book_Until_70_Percent_In_The_Same_Coach()
        {
            MakeReservations(6);

            Reservation reservation = _ticketOffice.MakeReservation(new ReservationRequest("MyTrainId", 1));
            
            Assert.That(reservation.Seats[0].Coach, Is.EqualTo("01"));
        }

        [Test]
        public void ReserveSeat_Should_Not_Book_More_Than_70_Percent_Of_A_Coach()
        {
            MakeReservations(7);
            
            Reservation reservation = _ticketOffice.MakeReservation(new ReservationRequest("MyTrainId", 2));

            Assert.That(reservation.Seats[0].Coach, Is.EqualTo("02"));
        }
        
        
        [Test]
        public void ReserveSeat_Can_OverBook_More_Than_70_Percent_Of_A_Coach_Given_We_Stay_Bellow_70_Percent_Overall()
        {
            for (int i = 0; i < 10; i++)
            {
                _ticketOffice.MakeReservation(new ReservationRequest("MyTrainId", 5));
            }

            Reservation reservation = _ticketOffice.MakeReservation(new ReservationRequest("MyTrainId", 5));

            Assert.That(reservation.Seats[0].Coach, Is.EqualTo("01"));
        }
        
        [Test]
        public void ReserveSeat_Cannot_Book_More_Seat_In_Coach_Than_Existing_Seat()
        {
            for (int i = 0; i < 10; i++)
            {
                _ticketOffice.MakeReservation(new ReservationRequest("MyTrainId", 5));
            }

            Assert.Throws<ApplicationException>(() =>
            {
                _ticketOffice.MakeReservation(new ReservationRequest("MyTrainId", 6));
            });
            
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
        
        class StubTrainRepository : TrainRepository
        {
            private readonly IEnumerable<Train> _trains;

            public StubTrainRepository(IEnumerable<Train> trains)
            {
                _trains = trains;
            }
            
            
            public Train Get(string trainId)
            {
                return _trains.Single(t => t.TrainId == trainId);
            }
        }
    }
}