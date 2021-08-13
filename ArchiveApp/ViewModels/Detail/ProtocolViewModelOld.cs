//using MVVM_Core;
//using System;
//using System.Collections.Generic;
//using System.Text;
//using Models;
//using System.Linq;
//using System.Windows.Input;
//using MVVM_Core.Validation;
//using ArchiveApp.Resources.Components;

//namespace ArchiveApp.ViewModels
//{
//    public class ProtocolViewModelOld : BaseViewModel
//    {
//        public People SelectedPeople { get; set; }
//        public People SelectedPeopleSearch 
//        { 
//            get => selectedPeopleSearch;
//            set
//            {
//                selectedPeopleSearch = value;
//                OnPropertyChanged();
//                PeopleSearch(value);
//            }
//        }

//        public bool IsEdit { get; set; }

//        public Organ SelectedOrgan { get; set; }

//        public Nationality SelectedNationality { get; set; }
//        public Protocol SelectedProtocol { get; set; }

//        public Nationality[] AllNationalities { get; set; }
//        public Organ[] AllOrgans { get; set; }
//        public People[] AllPeoples { get; set; }


//        public bool IsStayActive { get; set; }

//        public string Message { get; private set; }

//        public bool IsMessageVisible { get; private set; }

//        public string OrganText { get; set; }
//        public string NationalityText { get; set; }

//        public void ShowMessage(string message)
//        {
//            IsMessageVisible = true;
//            Message = message;
//        }
//        public void HideMessage()
//        {
//            IsMessageVisible = false;
//        }

//        public bool IsNewRecordPeople { get; set; }

//        public bool IsSearchOfPeopleEnabled { get; set; }

//        public bool IsPeopleDataFieldsVisible { get; private set; } = true;

//        public int ComboGenderIndex { get; set; }

//        private void PeopleSearch(People people)
//        {
//            if (people != null)
//            {
//                SelectedPeople = people;
//                SelectedNationality = AllNationalities.FirstOrDefault(x => x.Id == people.NationalityId);
//                IsPeopleDataFieldsVisible = true;
//                IsNewRecordPeople = false;
//                SelectedProtocol.PeopleId = people.Id;
//            }
//        }

//        //Результат поиска будет назначен на SelectedPeople
//        public ICommand ChangeToExistRecordOfPeople => new Command(x =>
//        {
//            IsSearchOfPeopleEnabled = true;

//            if(SelectedPeopleSearch != null)
//            {
//                SelectedProtocol.PeopleId = SelectedPeopleSearch.Id;
//                SelectedPeople.Id = SelectedPeopleSearch.Id;
//            }
//            else
//            {
//                IsPeopleDataFieldsVisible = false;
//            }

//            //Это значит, что запись о человеке - существующая
//            //Включение возможности поиска
//            //Если производился поиск, смена Id для свойства SelectedPeople на ранее найденную
//            //Если нет, то лишние поля о человеке скрываются, остается поле ФИО для поиска
//        });

//        public ICommand ChangeToNewRecordOfPeople => new Command(x =>
//        {
//            IsPeopleDataFieldsVisible = true;
//            IsSearchOfPeopleEnabled = false;
//            SelectedProtocol.PeopleId = 0;
//            SelectedProtocol.People = null;
//            SelectedPeople.Id = 0;
//            //Это значит, что запись о человеке - новая
//            //Отключение возможности поиска

//        });

//        public string FIO { get; set; }

//        Validator validator;
//        public void SetupValidator()
//        {
//            validator = new Validator();

//            validator.ForProperty(() => FIO, "ФИО").
//                NotEmpty().
//                Predicate(y => y.Contains(' ') && y.Split(' ').Length == 3, "В поле \"ФИО\" необходимо вписать фамилию, имя и отчество через пробел");
//            validator.ForProperty(() => SelectedPeople.BirthPlace, "Место рождения").NotEmpty();
//            validator.ForProperty(() => SelectedProtocol.Punishment, "Наказание").NotEmpty();
//            validator.ForProperty(() => SelectedProtocol.Resolution, "Постановление").NotEmpty();
//            validator.ForProperty(() => SelectedProtocol.Source, "Источник").NotEmpty();
//        }

//        public bool IsCorrect => validator?.IsCorrect ?? true;

//        public ICommand AcceptCommand { get; set; }
//        public ICommand CancelCommand { get; set; }

//        public Dictionary<string, bool?> GenderValues = new Dictionary<string, bool?> 
//        {
//            {"Мужской", true },
//            {"Женский", true },
//            {"Неопределено", null },
//        };
//        private People selectedPeopleSearch;

//        public Models.PartyStatus[] Parties { get; } = Enum.GetValues(typeof(PartyStatus)).OfType<PartyStatus>().ToArray();
//        public Models.SocialStatus[] Social { get; } = Enum.GetValues(typeof(SocialStatus)).OfType<SocialStatus>().ToArray();
//        public Models.EducationKind[] Education { get; } = Enum.GetValues(typeof(EducationKind)).OfType<EducationKind>().ToArray();
//        public Models.FamilyStatus[] Families { get; } = Enum.GetValues(typeof(FamilyStatus)).OfType<FamilyStatus>().ToArray();
//    }
//}
