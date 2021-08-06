using MVVM_Core;
using System;
using System.Collections.Generic;
using System.Text;
using Models;

namespace ArchiveApp.ViewModels
{
    public class ProtocolViewModel: BaseViewModel
    {
        public People People { get; set; }

        public Organ Organ { get; set; }

        public Nationality Nationality { get; set; }
        public Protocol Protocol { get; set; }

        public string[] Nationalities { get; set; }
        public string[] Organs { get; set; }

        public int NationalityIndex { get; set; }
        public int OrganIndex { get; set; }
    }
}
