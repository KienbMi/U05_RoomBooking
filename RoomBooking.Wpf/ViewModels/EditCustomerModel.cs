using RoomBooking.Core.Contracts;
using RoomBooking.Core.Entities;
using RoomBooking.Core.Validations;
using RoomBooking.Persistence;
using RoomBooking.Wpf.Common;
using RoomBooking.Wpf.Common.Contracts;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RoomBooking.Wpf.ViewModels
{
    public class EditCustomerModel : BaseViewModel
    {
        private Customer _customer;
        private string _lastName;
        private string _firstName;
        private string _iban;

        //[MinLength(2, ErrorMessage = "Minimum length of Lastname is 2")]
        //[Required(ErrorMessage = "Lastname is required")]
        public string LastName 
        {
            get => _lastName;            
            set
            {
                _lastName = value;
                OnPropertyChanged();
                Validate();
            }
        }

        public string FirstName 
        {
            get => _firstName; 
            set
            {
                _firstName = value;
                OnPropertyChanged();
                Validate();
            }
        }

        public string Iban 
        {
            get => _iban;           
            set
            {
                _iban = value;
                OnPropertyChanged();
                Validate();
            }
        }

        public EditCustomerModel(IWindowController windowController, Customer customer) : base(windowController)
        {
            _customer = customer;
            InitViewData();
        }

        private void InitViewData()
        {
            LastName = _customer.LastName;
            FirstName = _customer.FirstName;
            Iban = _customer.Iban;
        }

        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrWhiteSpace(LastName))
            {
                yield return new ValidationResult(
                    "Lastname is required",
                    new string[] { nameof(LastName) }
                    );
            }
            else if (LastName.Length < 2)
            {
                yield return new ValidationResult(
                    "Minimum length of Lastname is 2",
                    new string[] { nameof(LastName) }
                    );
            }

            if (!IbanChecker.CheckIban(Iban))
            {
                yield return new ValidationResult(
                    "Iban not valid",
                    new string[] { nameof(Iban) }
                    );
            }
        }

        private async Task SaveCustomer()
        {
            Validate();

            if (IsValid)
            {
                using IUnitOfWork uow = new UnitOfWork();
                _customer.FirstName = FirstName;
                _customer.LastName = LastName;
                _customer.Iban = Iban;
                _customer.Bookings = null;
                uow.Customers.Update(_customer);

                try
                {
                    await uow.SaveAsync();
                    Controller.CloseWindow(this);
                }
                catch (ValidationException validationException)
                {
                    if (validationException.Value is IEnumerable<string> properties)
                    {
                        foreach (var property in properties)
                        {
                            
                            AddErrorsToProperty(property, new List<string> { validationException.ValidationResult.ErrorMessage });
                        }
                    }
                    else
                    {
                        DbError = validationException.ValidationResult.ToString();
                    }
                }
            }
        }

        //Commands
        private ICommand _cmdUndo;
        public ICommand CmdUndo 
        { 
            get
            {
                if(_cmdUndo == null)
                {
                    _cmdUndo = new RelayCommand(
                        execute: _ => InitViewData(),
                        canExecute: _ => true);
                }
                return _cmdUndo;
            }
        }

        private ICommand _cmdSaveChanges;
        public ICommand CmdSaveChanges 
        { 
            get
            {
                if(_cmdSaveChanges == null)
                {
                    _cmdSaveChanges = new RelayCommand(
                        execute: async _ => await SaveCustomer(),
                        canExecute: _ => IsValid
                        );
                }
                return _cmdSaveChanges;
            }
        }
    }
}
