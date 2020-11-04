using RoomBooking.Core.Contracts;
using RoomBooking.Core.Entities;
using RoomBooking.Persistence;
using RoomBooking.Wpf.Common;
using RoomBooking.Wpf.Common.Contracts;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media.Animation;

namespace RoomBooking.Wpf.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private Booking _selectedBooking;
        private Room _selectedRoom;
        private ObservableCollection<Room> _rooms;
        private ObservableCollection<Booking> _bookings;

        public Booking SelectedBooking 
        { 
            get
            {
                return _selectedBooking;
            }
            set
            {
                _selectedBooking = value;
                OnPropertyChanged(nameof(SelectedBooking));
            }
        }

        public Room SelectedRoom
        {
            get
            {
                return _selectedRoom;
            }
            set
            {
                _selectedRoom = value;
                OnPropertyChanged(nameof(SelectedRoom));
                OnNewRoomSelectedAsync().ContinueWith(x => { });
            }
        }

        private async Task OnNewRoomSelectedAsync()
        {
            if (_selectedRoom == null)
            {
                return;
            }

            Booking selectedBookingTmp = SelectedBooking;

            using (IUnitOfWork uow = new UnitOfWork())
            {
                var bookings = await uow.Bookings.GetByRoomWithCustomerAsync(_selectedRoom.Id);
                Bookings = new ObservableCollection<Booking>(bookings);
            }
            if (selectedBookingTmp == null)
                SelectedBooking = Bookings.First();
            else
                SelectedBooking = selectedBookingTmp;
        }

        public ObservableCollection<Room> Rooms 
        { 
            get
            {
                return _rooms;
            }
            set
            {
                _rooms = value;
                OnPropertyChanged(nameof(Rooms));
            }
        }

        public ObservableCollection<Booking> Bookings 
        { 
            get
            {
                return _bookings;
            }
            set
            {
                _bookings = value;
                OnPropertyChanged(nameof(Bookings));
            }
        }

        public MainViewModel(IWindowController windowController) : base(windowController)
        {
        }

        private async Task LoadDataAsync()
        {
            using (IUnitOfWork uow = new UnitOfWork())
            {
                var rooms = await uow.Rooms.GetAllAsync();
                Rooms = new ObservableCollection<Room>(rooms);
                _selectedRoom = Rooms.First();
                await OnNewRoomSelectedAsync();
            }
        }

        public static async Task<MainViewModel> CreateAsync(IWindowController windowController)
        {
            var viewModel = new MainViewModel(windowController);
            await viewModel.LoadDataAsync();
            return viewModel;
        }

        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            throw new NotImplementedException();
        }

        //Commands
        private ICommand _cmdEditCustomer;
        public ICommand CmdEditCustomer 
        { 
            get
            {
                if (_cmdEditCustomer == null)
                {
                    _cmdEditCustomer = new RelayCommand(
                        execute: async _ => 
                        {
                            var window = new EditCustomerModel(Controller, SelectedBooking.Customer);
                            window.Controller.ShowWindow(window, true);
                            await OnNewRoomSelectedAsync();
                        },
                        canExecute: _ => SelectedBooking != null);
                }      
                return _cmdEditCustomer;
            }
        }
    }
}
