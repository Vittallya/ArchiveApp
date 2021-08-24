using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Models
{
    public class People: ICloneable, IComparable
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
        public short BirthYear { get; set; }

        [NotMapped]
        public string Fio => Surname + " " + Name + " " + Otchestvo;

        public object Clone()
        {
            return this.MemberwiseClone();
        }

        public int CompareTo(object obj)
        {
            return Id.CompareTo(obj);
        }


        public short? NatioId { get; set; }
        public short? EducationId { get; set; }
        public short? PartyId { get; set; }
        public short? FamilyTypeId { get; set; }

        public Natio Natio { get; set; }
        public Party Party { get; set; }
        public Education Education { get; set; }
        public FamilyType FamilyType { get; set; }


        [Timestamp]
        public byte[] RowVersion { get; set; }
    }
    public class Natio
    {
        public short Id { get; set; }
        public string Name { get; set; }
    }

    public class Education
    {
        public short Id { get; set; }
        public string Name { get; set; }
    }

    public class Party
    {
        public short Id { get; set; }
        public string Name { get; set; }
    }

    public class FamilyType
    {
        public short Id { get; set; }
        public string Name { get; set; }
    }
}
