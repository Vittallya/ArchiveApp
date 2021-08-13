using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Models
{
    public class People: ICloneable
    {


        public int Id { get; set; }

        [MaxLength(50)]
        public string Name { get; set; }

        [MaxLength(50)]
        public string Surname { get; set; }

        [MaxLength(50)]
        public string Otchestvo { get; set; }
        public bool Gender { get; set; }

        [MaxLength(150)]
        public string BirthPlace { get; set; }
        public DateTime BirthDate { get; set; }
        public string Nationality { get; set; }
        public string Education { get; set; }
        public string Party { get; set; }
        public string Family { get; set; }

        //public virtual Nationality Nationality { get; set; }

        //public virtual Education Education { get; set; }

        //public virtual Party Party { get; set; }

        //public virtual FamilyType Family { get; set; }

        [NotMapped]
        public string Fio => Surname + " " + Name + " " + Otchestvo;

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
    public class Nationality
    {
        public short Id { get; set; }
        public string Name { get; set; }
    }

    public class Education
    {
        public short Id { get; set; }
        public string Kind { get; set; }
    }

    public class Party
    {
        public short Id { get; set; }
        public string Type { get; set; }
    }

    public class FamilyType
    {
        public short Id { get; set; }
        public string Type { get; set; }
    }
}
