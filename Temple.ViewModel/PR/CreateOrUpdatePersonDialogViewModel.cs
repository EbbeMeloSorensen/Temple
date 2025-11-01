using System.ComponentModel;
using System.Globalization;
using System.Windows;
using MediatR;
using GalaSoft.MvvmLight.Command;
using Craft.UI.Utils;
using Craft.ViewModel.Utils;
using Craft.ViewModels.Dialogs;
using FluentValidation.Validators;
using Temple.Application.People;
using Temple.Domain.Entities.PR;

namespace Temple.ViewModel.PR
{
    public enum CreateOrUpdatePersonDialogViewModelMode
    {
        CreateVariant,
        CreateNew,
        CorrectVariant
    }

    public class CreateOrUpdatePersonDialogViewModel : DialogViewModelBase, IDataErrorInfo
    {
        private static readonly DateTime _maxDateTime = new(9999, 12, 31, 23, 59, 59, DateTimeKind.Utc);

        private readonly IMediator _mediator;
        private CreateOrUpdatePersonDialogViewModelMode _mode;
        private StateOfView _state;
        private IEnumerable<Person> _otherVariants;
        private string _latitude;
        private string _longitude;
        private string _startTime;
        private string _endTime;
        private string _dateRangeError;
        private bool _displayDateRangeError;
        private string _generalError;
        private bool _displayGeneralError;
        private Dictionary<string, string> _errors;

        private AsyncCommand<object> _okCommand;
        private RelayCommand<object> _cancelCommand;

        public Person Person { get; }

        public string FirstName
        {
            get => Person.FirstName;
            set
            {
                Person.FirstName = value;
                Validate();
                RaisePropertyChanged();
            }
        }

        public string Surname
        {
            get => Person.Surname ?? "";
            set
            {
                Person.Surname = value;
                Validate();
                RaisePropertyChanged();
            }
        }

        public string Nickname
        {
            get => Person.Nickname ?? "";
            set
            {
                Person.Nickname = value;
                Validate();
                RaisePropertyChanged();
            }
        }

        public string Address
        {
            get => Person.Address ?? "";
            set
            {
                Person.Address = value;
                Validate();
                RaisePropertyChanged();
            }
        }

        public string ZipCode
        {
            get => Person.ZipCode ?? "";
            set
            {
                Person.ZipCode = value;
                Validate();
                RaisePropertyChanged();
            }
        }

        public string City
        {
            get => Person.City ?? "";
            set
            {
                Person.City = value;
                Validate();
                RaisePropertyChanged();
            }
        }

        public string Category
        {
            get => Person.Category ?? "";
            set
            {
                Person.Category = value;
                Validate();
                RaisePropertyChanged();
            }
        }

        // This property doesn't wrap the Person object, since it might not be able to convert it to a double
        // So the view model has to check that before we involve the business rule catalog
        public string Latitude
        {
            get => _latitude;
            set
            {
                _latitude = value;
                Validate();
                RaisePropertyChanged();
            }
        }

        // This property doesn't wrap the Person object, since it might not be able to convert it to a double
        // So the view model has to check that before we involve the business rule catalog
        public string Longitude
        {
            get => _longitude;
            set
            {
                _longitude = value;
                Validate();
                RaisePropertyChanged();
            }
        }

        public DateTime? Birthday
        {
            get => Person.Birthday;
            set
            {
                Person.Birthday = value;
                Validate();
                RaisePropertyChanged();
            }
        }

        public DateTime? StartDate
        {
            get
            {
                if (Person.Start == default)
                {
                    return null;
                }

                return Person.Start;
            }
            set
            {
                Person.Start = value ?? default;
                Validate();
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(EndDate));
                RaisePropertyChanged(nameof(StartTime));
                RaisePropertyChanged(nameof(EndTime));
            }
        }

        public DateTime? EndDate
        {
            get
            {
                if (Person.End == _maxDateTime)
                {
                    return null;
                }

                return Person.End;
            }
            set
            {
                Person.End = value ?? new DateTime(9999, 12, 31, 23, 59, 59, DateTimeKind.Utc);
                Validate();
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(StartDate));
                RaisePropertyChanged(nameof(StartTime));
                RaisePropertyChanged(nameof(EndTime));
            }
        }

        // This property doesn't wrap the Person object, since it might not be able to convert it to a Time
        // So the view model has to check that before we involve the business rule catalog
        public string StartTime
        {
            get => _startTime;
            set
            {
                _startTime = value;
                Validate();
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(StartDate));
                RaisePropertyChanged(nameof(EndDate));
                RaisePropertyChanged(nameof(EndTime));
            }
        }

        // This property doesn't wrap the Person object, since it might not be able to convert it to a Time
        // So the view model has to check that before we involve the business rule catalog
        public string EndTime
        {
            get => _endTime;
            set
            {
                _endTime = value;
                Validate();
                RaisePropertyChanged(nameof(StartDate));
                RaisePropertyChanged(nameof(EndDate));
                RaisePropertyChanged(nameof(StartTime));
                RaisePropertyChanged();
            }
        }

        public string DateRangeError
        {
            get => _dateRangeError;
            set
            {
                _dateRangeError = value;
                DisplayDateRangeError = !string.IsNullOrEmpty(_dateRangeError);
                RaisePropertyChanged();
            }
        }

        public bool DisplayDateRangeError
        {
            get => _displayDateRangeError;
            set
            {
                _displayDateRangeError = value;
                RaisePropertyChanged();
            }
        }

        public string GeneralError
        {
            get => _generalError;
            set
            {
                _generalError = value;
                DisplayGeneralError = !string.IsNullOrEmpty(_generalError);
                RaisePropertyChanged();
            }
        }

        public bool DisplayGeneralError
        {
            get => _displayGeneralError;
            set
            {
                _displayGeneralError = value;
                RaisePropertyChanged();
            }
        }

        public string Error => null; // Not used

        public CreateOrUpdatePersonDialogViewModel(
            IMediator mediator,
            Person person = null,
            IEnumerable<Person> otherVariants = null)
        {
            _mediator = mediator;

            if (person == null)
            {
                _mode = CreateOrUpdatePersonDialogViewModelMode.CreateNew;
            }
            else if (person.ArchiveID == Guid.Empty)
            {
                _mode = CreateOrUpdatePersonDialogViewModelMode.CreateVariant;
            }
            else
            {
                _mode = CreateOrUpdatePersonDialogViewModelMode.CorrectVariant;
            }

            _otherVariants = otherVariants;

            _latitude = string.Empty;
            _longitude = string.Empty;
            _startTime = "00:00:00";
            _endTime = string.Empty;

            _errors = new Dictionary<string, string>();
            _dateRangeError = string.Empty;
            _generalError = string.Empty;

            Person = person == null
                ? new Person
                {
                    ID = _otherVariants != null && _otherVariants.Any() ? _otherVariants.Last().ID : new Guid(),
                    FirstName = _otherVariants != null && _otherVariants.Any() ? _otherVariants.Last().FirstName : "",
                    Start = DateTime.UtcNow.Date,
                    End = new DateTime(9999, 12, 31, 23, 59, 59, DateTimeKind.Utc)
                }
                : person;
        }

        public AsyncCommand<object> OKCommand
        {
            get { return _okCommand ?? (_okCommand = new AsyncCommand<object>(OK, CanOK)); }
        }

        public RelayCommand<object> CancelCommand
        {
            get { return _cancelCommand ?? (_cancelCommand = new RelayCommand<object>(Cancel, CanCancel)); }
        }

        private async Task OK(
            object parameter)
        {
            UpdateState(StateOfView.Updated);

            if (_errors.Values.Any())
            {
                return;
            }

            try
            {
                switch (_mode)
                {
                    case CreateOrUpdatePersonDialogViewModelMode.CreateNew:
                        var command = new Create.Command { Person = Person };
                        var result = await _mediator.Send(command);

                        if (!result.IsSuccess)
                        {
                            GeneralError = result.Error;
                            return;
                        }
                        break;
                    case CreateOrUpdatePersonDialogViewModelMode.CreateVariant:
                        //await _application.CreatePersonVariant(Person);
                        break;
                    case CreateOrUpdatePersonDialogViewModelMode.CorrectVariant:
                        //await _application.CorrectPersonVariant(Person);
                        break;
                }

                CloseDialogWithResult(parameter as Window, DialogResult.OK);
            }
            catch (Exception e)
            {
                if (e.InnerException != null)
                {
                    GeneralError = e.InnerException.Message;
                }
                else
                {
                    GeneralError = e.Message;
                }
            }
        }

        private bool CanOK(
            object parameter)
        {
            return true;
        }

        private void Cancel(
            object parameter)
        {
            CloseDialogWithResult(parameter as Window, DialogResult.Cancel);
        }

        private bool CanCancel(
            object parameter)
        {
            return true;
        }

        public string this[string columnName]
        {
            get
            {
                // In the basic pattern, validation is done here, but in this design
                // validation is done as soon as a property is updated so that the collection
                // of errors is ready when this indexer is called

                if (_state == StateOfView.Initial)
                {
                    return string.Empty;
                }

                string? error;

                // For these fields, we want to highlight them all if there is a problem with the date range
                if (columnName == nameof(StartDate) ||
                    columnName == nameof(StartTime) ||
                    columnName == nameof(EndDate) ||
                    columnName == nameof(EndTime))
                {
                    _errors.TryGetValue("DateRange", out error);

                    if (string.IsNullOrEmpty(error))
                    {
                        _errors.TryGetValue("ValidTimeIntervals", out error);
                    }
                }
                else
                {
                    _errors.TryGetValue(columnName, out error);
                }

                return error ?? "";
            }
        }

        private void RaisePropertyChanges()
        {
            RaisePropertyChanged(nameof(FirstName));
            RaisePropertyChanged(nameof(Surname));
            RaisePropertyChanged(nameof(Nickname));
            RaisePropertyChanged(nameof(Address));
            RaisePropertyChanged(nameof(ZipCode));
            RaisePropertyChanged(nameof(City));
            RaisePropertyChanged(nameof(Birthday));
            RaisePropertyChanged(nameof(Category));
            RaisePropertyChanged(nameof(Latitude));
            RaisePropertyChanged(nameof(Longitude));
            RaisePropertyChanged(nameof(StartDate));
            RaisePropertyChanged(nameof(StartTime));
            RaisePropertyChanged(nameof(EndDate));
            RaisePropertyChanged(nameof(EndTime));
        }

        private void UpdateState(
            StateOfView state)
        {
            _state = state;
            Validate();
            RaisePropertyChanges();
        }

        private void Validate()
        {
            if (_state != StateOfView.Updated) return;

            _errors.Clear();
            DateRangeError = "";

            // Initially, we ensure that the fields that need to be parsed can be parsed correctly
            if (!Latitude.TryParse(out var latitude, out var error_lat))
            {
                _errors[nameof(Latitude)] = error_lat;
            }

            if (!Longitude.TryParse(out var longitude, out var error_long))
            {
                _errors[nameof(Longitude)] = error_long;
            }

            DateTime startTime = default;
            if (string.IsNullOrEmpty(_startTime))
            {
                DateRangeError = "Start time is required";
                _errors["DateRange"] = DateRangeError;
            }
            else if (!DateTime.TryParseExact(_startTime, "HH:mm:ss", CultureInfo.InvariantCulture,
                    DateTimeStyles.AdjustToUniversal, out startTime))
            {
                DateRangeError = "Time format needs to be HH:mm:ss";
                _errors["DateRange"] = DateRangeError;
            }

            var endTime = EndDate.HasValue
                ? new DateTime()
                : new DateTime(9999, 12, 31, 23, 59, 59, DateTimeKind.Utc);

            if (!string.IsNullOrEmpty(_endTime))
            {
                if (DateTime.TryParseExact(_endTime, "HH:mm:ss", CultureInfo.InvariantCulture,
                        DateTimeStyles.AdjustToUniversal, out var temp))
                {
                    endTime = temp;
                }
                else
                {
                    DateRangeError = "Time format needs to be HH:mm:ss";
                    _errors["DateRange"] = DateRangeError;
                }
            }

            if (_errors.Any())
            {
                return;
            }

            // Then, we set the properties of the Person object and validate it using the business rule catalog
            Person.Latitude = latitude;
            Person.Longitude = longitude;
            Person.Start = Person.Start.Date + startTime.TimeOfDay;
            Person.End = Person.End.Date + endTime.TimeOfDay;

            switch (_mode)
            {
                case CreateOrUpdatePersonDialogViewModelMode.CreateNew:

                    var validator = new Create.CommandValidator();
                    var command = new Create.Command {Person = Person};
                    var result = validator.Validate(command);

                    var failure = result.Errors.FirstOrDefault(_ => _.PropertyName == "Person.FirstName");

                    if (failure != null)
                    {
                        switch (failure.ErrorCode)
                        {
                            case "NotEmptyValidator":
                                _errors["FirstName"] = "First name is required";
                                break;
                            case "MaximumLengthValidator":
                                _errors["FirstName"] = "First name can't exceed 10 characters";
                                break;
                        }
                    }

                    // Old
                    //_errors = _application.CreateNewPerson_ValidateInput(
                    //    Person);
                    break;
                case CreateOrUpdatePersonDialogViewModelMode.CreateVariant:
                    //_errors = _application.CreatePersonVariant_ValidateInput(
                    //    Person,
                    //    _otherVariants,
                    //    out var nonConflictingPersonVariants,
                    //    out var coveredPersonVariants,
                    //    out var trimmedPersonVariants,
                    //    out var newPersonVariants);
                    break;
                case CreateOrUpdatePersonDialogViewModelMode.CorrectVariant:
                    //_errors = _application.CorrectPersonVariant_ValidateInput(
                    //    Person,
                    //    _otherVariants,
                    //    out nonConflictingPersonVariants,
                    //    out coveredPersonVariants,
                    //    out trimmedPersonVariants,
                    //    out newPersonVariants);
                    break;
                default:
                    throw new InvalidOperationException("Only create and correct variant are supported from this dialog");
            }

            if (_errors.TryGetValue("ValidTimeIntervals", out var error))
            {
                DateRangeError = error;
            }
            else if (_errors.TryGetValue("BirthdayConsistency", out error))
            {
                DateRangeError = error;
            }
        }
    }
}
