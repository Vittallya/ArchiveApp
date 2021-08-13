using MVVM_Core;
using System;
using System.Collections.Generic;
using System.Text;
using Models;
using BL;
using AppContext = Models.AppContext;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using System.Windows.Input;
using MVVM_Core.Validation;
using System.Text.RegularExpressions;
using ArchiveApp.Services;
using ArchiveApp.Resources.Components;

namespace ArchiveApp.ViewModels
{
    public class ProtocolViewModel : BaseViewModel
    {
        

        public bool IsStayActive { get; set; }

        public bool IsNewPeopleRecord { get; set; }
        public bool IsFiledVisible { get; set; } = true;


        #region данные

        public string Fio { get; set; }
        public string BirthPlace { get; set; }
        public Protocol Protocol { get; set; }

        public People People { get; set; }
        #endregion



        public bool IsEdit { get; private set; }

        public ProtocolViewModel(DropDownDataService dropDownDataService)
        {
            this.dropDownDataService = dropDownDataService;
        }


        public void OnAdd()
        {
            var people = Protocol?.People;

            Protocol = new Protocol() { ProtocolDateTime = new DateTime(1937, 10, 10)};
            IsNewPeopleRecord = true;

            if (people == null)
            {
                People = new People { Gender = true, BirthDate = new DateTime(1920, 10, 10) };
            }
            else
            {
                PeopleSearched = people;
                IsNewPeopleRecord = false;
            }

            IsEdit = false;
        }


        public void OnEdit(Protocol item)
        {
            Protocol = item.Clone() as Protocol;
            PeopleSearched = item.People;
            Protocol.People = null;

            Fio = People.Fio;
            IsNewPeopleRecord = false;
            IsEdit = true;
        }
        private void OnAccept()
        {
            ErrorMessage = null;
            if (_isValidation && !validator.IsCorrect)
            {
                OnErrorValid(validator.ErrorMessage);
                return;
            }

            string[] fio = Fio.Split(' ');
            People.Surname = fio[0];
            People.Name = fio[1];
            People.Otchestvo = fio[2];
            //People.BirthPlace = BirthPlace;

            Protocol.People = People;

            Accepted?.Invoke(this);


            if (People.Nationality == null)
            {
                People.Nationality = NationalityText;
                dropDownDataService.AddItem("natio", NationalityText);
            }

            if (People.Party == null)
            {
                People.Party = PartyText;
                dropDownDataService.AddItem("party", PartyText);
            }

            if (People.Education == null)
            {
                People.Education = EducationText;
                dropDownDataService.AddItem("education", EducationText);
            }

            if (People.Family == null)
            {
                People.Family = FamilyText;
                dropDownDataService.AddItem("family", FamilyText);
            }

            if (Protocol.Social == null)
            {
                Protocol.Social = SocialText;
                dropDownDataService.AddItem("social", SocialText);
            }

            if (Protocol.Organ == null)
            {
                Protocol.Organ = OrganText;
                dropDownDataService.AddItem("organ", OrganText);
            }

            if (dropDownDataService.HasChanges)
            {
                dropDownDataService.SaveChanges();
                ReloadDropDownData();
            }
        }

        private void OnCancel()
        {
            Canceled?.Invoke(this);
        }


        #region Команды

        public event Action<ProtocolViewModel> Accepted;
        public event Action<ProtocolViewModel> Canceled;

        private ICommand changeToNewRecordCommand;
        private ICommand changeToExistRecordCommand;
        private ICommand acceptCommand;
        private ICommand cancelCommand;
        public ICommand AcceptCommand => acceptCommand ??= new Command(x => OnAccept());
        public ICommand CancelCommand => cancelCommand ??= new Command(x => OnCancel());
        public ICommand ChangeToNewRecordCommand => changeToNewRecordCommand ??= new Command(x => ChangeToNewRecord());
        public ICommand ChangeToExistRecordCommand => changeToExistRecordCommand ??= new Command(x => ChangeToExistRecord());


        #endregion

        #region Режим работы
        public void ChangeToNewRecord()
        {
            IsFiledVisible = true;
            IsNewPeopleRecord = true;
            People.Id = 0;
        }

        public void ChangeToExistRecord()
        {
            IsNewPeopleRecord = false;
            PeopleUpdate(PeopleSearched);
        }
        public void PeopleUpdate(People people)
        {
            if (people != null)
            {              
                //people.Nationality = AllNationalities.FirstOrDefault(x => x.Id == people.NationalityId);
                //people.Party = AllParty.FirstOrDefault(x => x.Id == people.PartyId);
                //people.Education = AllEducation.FirstOrDefault(x => x.Id == people.EducationId);

                People = people.Clone() as People;
                //BirthPlace = People.BirthPlace;
                //Natio = AllNationalities.FirstOrDefault(x => x.Value == Protocol.People.Nationality);

                IsFiledVisible = true;
            }
            else
            {
                IsFiledVisible = false;
            }
        }

        #endregion

        #region Навигационные свойства

        private People peopleSearched;

        public People PeopleSearched
        {
            get => peopleSearched;
            set
            {
                if (value != null)
                {
                    peopleSearched = value?.Clone() as People;
                    OnPropertyChanged();
                    PeopleUpdate(peopleSearched);
                }
            }
        }

        public string NationalityText { get; set; }
        public string OrganText { get; set; }
        public string FamilyText { get; set; }
        public string PartyText { get; set; }
        public string EducationText { get; set; }
        public string SocialText { get; set; }

        #endregion

        #region Источники данных

        public void LoadItemsScources(People[] peoples)
        {
            AllPeoples = peoples;
            ReloadDropDownData();
        }

        private void ReloadDropDownData()
        {
            AllNationalities = dropDownDataService.GetUnits("natio");
            AllFamily = dropDownDataService.GetUnits("family");
            AllParty = dropDownDataService.GetUnits("party");
            AllEducation = dropDownDataService.GetUnits("education");


            AllOrgans = dropDownDataService.GetUnits("organs");
            AllSocial = dropDownDataService.GetUnits("social");
        }

        public People[] AllPeoples { get; private set; }
        //todo если окно остается открытым, и была добавлена запись о человеке, то это свойство надо обновить
        public RootUnitsItem[] AllNationalities { get; set; }
        public RootUnitsItem[] AllFamily { get; set; }
        public RootUnitsItem[] AllSocial { get; set; }
        public RootUnitsItem[] AllEducation { get; set; }
        public RootUnitsItem[] AllParty { get; set; }
        public RootUnitsItem[] AllOrgans { get; set; }



        public Dictionary<string, bool> Genders { get; } = new Dictionary<string, bool>
        {
            {"Мужской", true },
            {"Женский", false },
        };

        #endregion

        #region Валидация

        public string ErrorMessage { get; private set; }

        private Validator validator;
        private Action<string> _errorFunc;
        private bool _isValidation;
        private readonly DropDownDataService dropDownDataService;

        private void OnErrorValid(string msg)
        {
            _errorFunc?.Invoke(msg);
        }

        private void DefaultErrorHandler(string msg)
        {
            ErrorMessage = msg;
        }

        public void SetupValidator(Action<string> errorFunc = null)
        {
            _isValidation = true;
            _errorFunc = errorFunc ?? DefaultErrorHandler;
            validator = new Validator();

            //var regex = new Regex(@"[а-я]{1,}\s{1,}[а-я]{1,}\s{1,}[а-я]{1,}", RegexOptions.Compiled);

            validator.ForProperty(() => Fio, "ФИО").NotEmpty().
                Match(@"[А-Яа-я]{1,}\s{1}[А-Яа-я]{1,}\s{1}[А-Яа-я]{1,}", "Поле \"ФИО\" должно содержать фамилию, имя и отчество, вписанные через пробел");

            validator.ForProperty(() => People.BirthPlace, "Место рождения").NotEmpty().LengthLessThan(150);
            validator.ForProperty(() => PeopleSearched?.ToString(), "ФИО").Predicate(x => !string.IsNullOrEmpty(x) || IsNewPeopleRecord,
                "Необходимо выбрать существующую запись о человеке в поле \"ФИО\" либо перейти в режим добавления записи");
            validator.ForProperty(() => EducationText, "").
                Predicate(x => !string.IsNullOrEmpty(x) || People.Education != null,
                "Для поля \"Образование\" необходимо выбрать значение из существующих или написать новое");

            validator.ForProperty(() => PartyText, "").
                Predicate(x => !string.IsNullOrEmpty(x) || People.Party != null,
                "Для поля \"Партийность\" необходимо выбрать значение из существующих или написать новое");

            validator.ForProperty(() => FamilyText, "").
                Predicate(x => !string.IsNullOrEmpty(x) || People.Family != null,
                "Для поля \"Семейное положение\" необходимо выбрать значение из существующих или написать новое");

            validator.ForProperty(() => NationalityText, "").
                Predicate(x => !string.IsNullOrEmpty(x) || People.Nationality != null,
                "Для поля \"Национальность\" необходимо выбрать значение из существующих или написать новое");

            validator.ForProperty(() => SocialText, "").
                Predicate(x => !string.IsNullOrEmpty(x) || Protocol.Social != null,
                "Для поля \"Социальное положение на момент ареста\" необходимо выбрать значение из существующих или написать новое");

            validator.ForProperty(() => OrganText, "").
                Predicate(x => !string.IsNullOrEmpty(x) || Protocol.Organ != null,
                "Для поля \"Судебный орган\" необходимо выбрать значение из существующих или написать новое");

            validator.ForProperty(() => Protocol.ProtocolNumber, "Номер протокола").NotEmpty();
            validator.ForProperty(() => Protocol.Punishment, "Наказание").NotEmpty();
            validator.ForProperty(() => Protocol.Resolution, "Постановление").NotEmpty();
            validator.ForProperty(() => Protocol.Source, "Источник").NotEmpty();
        }

        #endregion
    }
}
