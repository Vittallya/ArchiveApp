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
using System.Threading.Tasks;

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
            Protocol = new Protocol() { ProtocolYear = Years[86], Punishment = Punishments[2]};
            IsNewPeopleRecord = true;

            if (People == null)
            {
                People = new People { Gender = true, BirthYear = Years[46] };
            }
            else
            {
                PeopleSearched = People;
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
        private async void OnAccept()
        {
            ErrorMessage = null;
            if (_isValidation && !validator.IsCorrect)
            {
                OnErrorValid(validator.ErrorMessage);
                return;
            }

            Protocol.People = People;

            await Accepted?.Invoke(this);


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

            if (IsNewPeopleRecord)
            {
                var list = AllPeoples.ToList();
                list.Add(People);
                AllPeoples = list.ToArray();                
                People = null;
                //todo непонятная строка
            }

            if (IsEdit)
            {
                OnEdit(Protocol);
            }
            else
            {
                OnAdd();
            }
        }

        private void OnCancel()
        {
            Canceled?.Invoke(this);
        }


        #region Команды

        public event Func<ProtocolViewModel, Task> Accepted;
        public event Action<ProtocolViewModel> Canceled;

        private ICommand changeToNewRecordCommand;
        private ICommand changeToExistRecordCommand;
        private ICommand acceptCommand;
        private ICommand cancelCommand;
        private ICommand clerSearchCommand;
        public ICommand AcceptCommand => acceptCommand ??= new Command(x => OnAccept());
        public ICommand CancelCommand => cancelCommand ??= new Command(x => OnCancel());
        public ICommand ChangeToNewRecordCommand => changeToNewRecordCommand ??= new Command(x => ChangeToNewRecord());
        public ICommand ChangeToExistRecordCommand => changeToExistRecordCommand ??= new Command(x => ChangeToExistRecord());
        public ICommand ClearSearchCommand => clerSearchCommand ??= new Command(x => ClearSearch());


        #endregion

        #region Режим работы
        public void ChangeToNewRecord()
        {
            IsFiledVisible = true;
            IsNewPeopleRecord = true;
            People.Id = 0;
        }

        public bool IsClearBtnVis => !IsNewPeopleRecord && peopleSearched != null;

        public bool IsFioVis => !IsNewPeopleRecord && peopleSearched == null;

        public void ChangeToExistRecord()
        {
            IsNewPeopleRecord = false;
            PeopleUpdate(PeopleSearched);
        }

        public void ClearSearch()
        {
            IsFiledVisible = false;
            PeopleSearched = null;
        }

        public void PeopleUpdate(People people)
        {
            if (people != null)
            {                              
                People = people.Clone() as People;
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
                peopleSearched = value?.Clone() as People;
                OnPropertyChanged();
                PeopleUpdate(peopleSearched);                
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
        public RootUnitsItem[] AllNationalities { get; set; }
        public RootUnitsItem[] AllFamily { get; set; }
        public RootUnitsItem[] AllSocial { get; set; }
        public RootUnitsItem[] AllEducation { get; set; }
        public RootUnitsItem[] AllParty { get; set; }
        public RootUnitsItem[] AllOrgans { get; set; }


        public short[] Years { get; } = Enumerable.Range(1850, 141).Select(x => (short)x).ToArray();

        public string[] Punishments { get; } = new string[] { "Расстрел", "10 лет", "8 лет", "5 лет", "3 года", "Отправлено на доследование", "Другое" };


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


            validator.ForProperty(() => Fio, "").Predicate(y => IsNewPeopleRecord || peopleSearched != null, 
                "Находясь в режиме существующей записи о человеке необходимо выбрать запись о человеке");

            validator.ForProperty(() => People.Surname, "Фамилия").NotEmpty().LengthLessThan(150);
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
                "Для поля \"Кем работал(-а) на момент ареста\" необходимо выбрать значение из существующих или написать новое");

            validator.ForProperty(() => OrganText, "").
                Predicate(x => !string.IsNullOrEmpty(x) || Protocol.Organ != null,
                "Для поля \"Судебный орган\" необходимо выбрать значение из существующих или написать новое");

            validator.ForProperty(() => Protocol.ProtocolNumber, "По каким статьям УК РСФСР осужден").NotEmpty();
            validator.ForProperty(() => Protocol.ResidentPlace, "Место проживания на момент ареста").NotEmpty();
            validator.ForProperty(() => Protocol.Punishment, "Наказание").NotEmpty();
            validator.ForProperty(() => Protocol.Resolution, "Постановление").NotEmpty();
            //validator.ForProperty(() => Protocol.Source, "Источник").NotEmpty();
        }

        #endregion
    }
}
