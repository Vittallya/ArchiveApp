using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
    public class Protocol: ICloneable, IComparable
    {
        [Key]
        public int Id { get; set; }
        [MaxLength(85)]
        public string ProtocolNumber { get; set; }
        public short ProtocolYear { get; set; }
        public int PeopleId { get; set; }
        public string Punishment { get; set; }
        public string Resolution { get; set; }
        public string Source { get; set; }
        [MaxLength(150)]
        public string ResidentPlace { get; set; }

        public People People { get; set; }

        public object Clone()
        {
            var protocol = this.MemberwiseClone() as Protocol;
            protocol.People = People.Clone() as People;

            return protocol;
        }

        public int CompareTo(object obj)
        {
            return Id.CompareTo(obj);
        }

        public Organ Organ { get; set; }
        public Social Social { get; set; }

        public short? SocialId { get; set; }
        public short? OrganId { get; set; }
        [Timestamp]
        public byte[] RowVersion { get; set; }
    }

    public class Social
    {
        public short Id { get; set; }
        public string Name { get; set; }
    }
    public class Organ
    {
        public short Id { get; set; }
        public string Name { get; set; }
    }

    public enum EducationKind
    {
        None, Medium, High
    }

    public enum PartyStatus
    {
        None, Member, Komsomol
    }

    public enum SocialStatus
    {
        Employee, Peasant, Worker
    }

    public enum FamilyStatus
    {
        MarriedFeemale, MarriedMale, Divorced, Widower
    }
}
