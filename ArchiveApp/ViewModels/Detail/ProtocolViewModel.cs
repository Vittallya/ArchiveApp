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
using BL.DbHandling;

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
            Protocol = new Protocol() { ProtocolYear = Years[87], Punishment = Punishments[2] };
            IsNewPeopleRecord = true;

            if (People == null)
            {
                People = new People { Gender = true, BirthYear = Years[40] };
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
        
        private void CheckOuter()
        {
            if (People.Natio == null)
            {
                People.Natio = NatioNew;
            }
            if (People.Education == null)
            {
                People.Education = EducationNew;
            }
            if (People.Party == null)
            {
                People.Party = PartyNew;
            }
            if (People.FamilyType == null)
            {
                People.FamilyType = FamilyNew;
            }
            if (Protocol.Organ == null)
            {
                Protocol.Organ = OrganNew;
            }
            if (Protocol.Social == null)
            {
                Protocol.Social = SocialNew;
            }
        }
        
        private T[] CheckUpdateSource<T>(bool hasValue, T[] data, T newRecord)
        {
            if (!hasValue)
            {
                return UpdateDataSource(data, newRecord);
            }
            else
                return data;
        }

        private async void OnAccept()
        {
            ErrorMessage = null;
            if (_isValidation && !validator.IsCorrect)
            {
                OnErrorValid(validator.ErrorMessage);
                return;
            }
            CheckOuter();

            Protocol.People = People;


            if (!await Accepted?.Invoke(this))
                return;

            AllNatio = CheckUpdateSource(People.NatioId.HasValue, AllNatio, NatioNew);
            AllEducation = CheckUpdateSource(People.EducationId.HasValue, AllEducation, EducationNew);
            AllParty = CheckUpdateSource(People.PartyId.HasValue, AllParty, PartyNew);
            AllFamily = CheckUpdateSource(People.FamilyTypeId.HasValue, AllFamily, FamilyNew);
            AllSocial = CheckUpdateSource(Protocol.SocialId.HasValue, AllSocial, SocialNew);
            AllOrgans = CheckUpdateSource(Protocol.OrganId.HasValue, AllOrgans, OrganNew);

            if (IsNewPeopleRecord)
            {
                AllPeoples = UpdateDataSource(AllPeoples, People);
                People = null;
            }

            if (IsEdit)
            {
                NatioNew = new Natio { Name = NatioNew.Name };
                EducationNew = new Education { Name = EducationNew.Name };
                PartyNew = new Party { Name = PartyNew.Name };
                FamilyNew = new FamilyType { Name = FamilyNew.Name };
                OrganNew = new Organ { Name = OrganNew.Name };
                SocialNew = new Social { Name = SocialNew.Name };
                OnEdit(Protocol);
            }
            else
            {
                NatioNew = new Natio();
                EducationNew = new Education();
                PartyNew = new Party();
                FamilyNew = new FamilyType();
                OrganNew = new Organ();
                SocialNew = new Social();
                OnAdd();
            }
            
        }

        internal void OnUpdate(Protocol protocol)
        {
            Protocol = protocol.Clone() as Protocol;
            if (!IsNewPeopleRecord)
            {
                PeopleUpdate(Protocol.People);
            }
            Protocol.People = null;
        }

        private T[] UpdateDataSource<T>(T[] items, T newItem)
        {
            var newArr = new T[items.Length + 1];
            items.CopyTo(newArr, 0);
            newArr[items.Length] = newItem;
            return newArr;
        }

        private void OnCancel()
        {
            Canceled?.Invoke(this);
        }


        #region Команды

        public event Func<ProtocolViewModel, Task<bool>> Accepted;
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
                Fio = null;
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

        public Natio NatioNew { get; set; } = new Natio();
        public Party PartyNew { get; set; } = new Party();
        public Education EducationNew { get; set; } = new Education();
        public FamilyType FamilyNew { get; set; } = new FamilyType();
        public Social SocialNew { get; set; } = new Social();
        public Organ OrganNew { get; set; } = new Organ();

        

        //public short? EducationId { get; set; }
        //public short? PartyId { get; set; }
        //public short? PartyId { get; set; }

        #endregion

        #region Источники данных

        public void LoadItemsScources(UnitOfWork unitOfWork)
        {
            var uw = unitOfWork;
            AllPeoples = uw.Peoples.LoadItems().ToArray();
            AllNatio = uw.Natios.ToArray();
            AllFamily = uw.FamilyTypes.ToArray();
            AllParty = uw.Parties.ToArray();
            AllEducation = uw.Educations.ToArray();


            AllOrgans = uw.Organs.ToArray();
            AllSocial = uw.Socials.ToArray();
        }

        public People[] AllPeoples { get; private set; }
        public Models.Natio[] AllNatio { get; set; }
        public FamilyType[] AllFamily { get; set; }
        public Social[] AllSocial { get; set; }
        public Education[] AllEducation { get; set; }
        public Party[] AllParty { get; set; }
        public Organ[] AllOrgans { get; set; }



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
            //validator.ForProperty(() => EducationText, "").
            //    Predicate(x => !string.IsNullOrEmpty(x) || People.Education != null,
            //    "Для поля \"Образование\" необходимо выбрать значение из существующих или написать новое");

            //validator.ForProperty(() => PartyText, "").
            //    Predicate(x => !string.IsNullOrEmpty(x) || People.Party != null,
            //    "Для поля \"Партийность\" необходимо выбрать значение из существующих или написать новое");

            //validator.ForProperty(() => FamilyText, "").
            //    Predicate(x => !string.IsNullOrEmpty(x) || People.Family != null,
            //    "Для поля \"Семейное положение\" необходимо выбрать значение из существующих или написать новое");

            //validator.ForProperty(() => NationalityText, "").
            //    Predicate(x => !string.IsNullOrEmpty(x) || People.Nationality != null,
            //    "Для поля \"Национальность\" необходимо выбрать значение из существующих или написать новое");

            //validator.ForProperty(() => SocialText, "").
            //    Predicate(x => !string.IsNullOrEmpty(x) || Protocol.Social != null,
            //    "Для поля \"Кем работал(-а) на момент ареста\" необходимо выбрать значение из существующих или написать новое");

            //validator.ForProperty(() => OrganText, "").
            //    Predicate(x => !string.IsNullOrEmpty(x) || Protocol.Organ != null,
            //    "Для поля \"Судебный орган\" необходимо выбрать значение из существующих или написать новое");

            validator.ForProperty(() => Protocol.ProtocolNumber, "По каким статьям УК РСФСР осужден").NotEmpty();
            validator.ForProperty(() => Protocol.ResidentPlace, "Место проживания на момент ареста").NotEmpty();
            validator.ForProperty(() => Protocol.Punishment, "Наказание").NotEmpty();
            validator.ForProperty(() => Protocol.Resolution, "Постановление").NotEmpty();
            //validator.ForProperty(() => Protocol.Source, "Источник").NotEmpty();
        }

        #endregion
        public byte? Test { get; set; }

        public ICommand TestCommand => new Command(x =>
        {
            byte? test = Test;
        });
    }

}
