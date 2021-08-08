using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
    public class Protocol
    {
        [Key]
        public int Id { get; set; }
        public int ProtocolNumber { get; set; }
        public DateTime ProtocolDateTime { get; set; }
        public short OrganId { get; set; }
        public Organ Organ { get; set; }  
        public int PeopleId { get; set; }
        public People People { get; set; }        

        [Column(TypeName = "tinyint")]
        public SocialStatus Social { get; set; }
        public string Punishment { get; set; }
        public string Resolution { get; set; }
        public string Source { get; set; }
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
