using RoomBooking.Core.Entities;
using RoomBooking.Wpf.Common;
using RoomBooking.Wpf.Common.Contracts;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Windows.Input;

namespace RoomBooking.Wpf.ViewModels
{
    public class EditCustomerModel : BaseViewModel
    {
        private Customer _customer;
        private string _lastName;
        private string _firstName;
        private string _iban;

        public string LastName 
        {
            get => _lastName;            
            set
            {
                _lastName = value;
                OnPropertyChanged();
            }
        }
        public string FirstName { get; set; }
        public string Iban { get; set; }

        public EditCustomerModel(IWindowController windowController, Customer customer) : base(windowController)
        {
            _customer = customer;
            LastName = _customer.LastName;
            FirstName = _customer.FirstName;
            Iban = _customer.Iban;
        }

        public override IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            throw new NotImplementedException();
        }
    }
}
