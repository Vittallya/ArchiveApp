using MVVM_Core;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Models;

namespace ArchiveApp.ViewModels
{
    public class DisplayGroupViewModel : BaseViewModel
    {
        public IGrouping<string, Protocol> Data { get; private set; }

        public DisplayGroupViewModel()
        {
            var list = new List<Models.Protocol>();
            var a = list.GroupBy(x => x.Organ, Models.AllComparers.GetOrganComparer);
        }
    }
}
