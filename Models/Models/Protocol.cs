using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
    public class Protocol: ICloneable
    {
        [Key]
        public int Id { get; set; }
        [MaxLength(85)]
        public string ProtocolNumber { get; set; }
        public DateTime ProtocolDateTime { get; set; }
        public int PeopleId { get; set; }
        public string Punishment { get; set; }
        public string Resolution { get; set; }
        public string Source { get; set; }

        public People People { get; set; }
        [MaxLength(100)]
        public string Social { get; set; }
        [MaxLength(50)]
        public string Organ { get; set; }

        public object Clone()
        {
            var protocol = this.MemberwiseClone() as Protocol;
            protocol.People = People.Clone() as People;

            return protocol;
        }
    }

    public class Social
    {
        public short Id { get; set; }
        public string Kind { get; set; }
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
