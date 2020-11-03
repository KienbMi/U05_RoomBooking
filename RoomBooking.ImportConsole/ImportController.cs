using RoomBooking.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utils;

namespace RoomBooking.ImportConsole
{
    public static class ImportController
    {
        /// <summary>
        /// Liest die Buchungen mit ihren Räumen und Kunden aus der
        /// csv-Datei ein.
        /// </summary>
        /// <returns></returns>
        public static async Task<IEnumerable<Booking>> ReadBookingsFromCsvAsync()
        {
            string[][] matrix = await MyFile.ReadStringMatrixFromCsvAsync("bookings.csv", true);

            const int Idx_LastName = 0;
            const int Idx_FirstName = 1;
            const int Idx_Iban = 2;
            const int Idx_RoomNumber = 3;
            const int Idx_From = 4;
            const int Idx_To = 5;

            //LastName; FirstName; Iban; RoomNumber; From; To
            //Wiedermann; Alfred; BG80BNBG96611020345678; EDV10; 08:55; 11:45

            List<Customer> customers = matrix
                .GroupBy(line => line[Idx_Iban])
                .Select(grp => new Customer
                {
                    LastName = grp.First()[Idx_LastName],
                    FirstName = grp.First()[Idx_FirstName],
                    Iban = grp.First()[Idx_Iban]
                })
                .ToList();

            List<Room> rooms = matrix
                .GroupBy(line => line[Idx_RoomNumber])
                .Select(grp => new Room
                {
                    RoomNumber = grp.Key
                })
                .ToList();

            List<Booking> bookings = matrix
                .Select(line => new Booking
                {
                    Customer = customers.Single(customer => line[Idx_Iban] == customer.Iban),
                    From = line[Idx_From], 
                    Room = rooms.Single(room => line[Idx_RoomNumber] == room.RoomNumber),
                    To = line[Idx_To]
                })
                .ToList();

            return bookings;
        }
    }
}
