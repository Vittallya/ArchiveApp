using System;
using System.Collections.Generic;
using System.Text;

namespace Models.Abstract
{
    public interface ISecondaryTable
    {
        byte Id { get; set; }
        string Name { get; set; }
    }
}
