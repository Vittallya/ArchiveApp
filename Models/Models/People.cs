using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Models
{
    public class People
    {
        public int Id { get; set; }

        [MaxLength(50)]
        public string Name { get; set; }

        [MaxLength(50)]
        public string Surname { get; set; }

        [MaxLength(50)]
        public string Otchestvo { get; set; }
        public bool? Gender { get; set; }
        public short NationalityId { get; set; }
        public Nationality Nationality { get; set; }

        [MaxLength(150)]
        public string BirthPlace { get; set; }
        public DateTime BirthDate { get; set; }

        [Column(TypeName = "tinyint")]
        public EducationKind Education { get; set; }

        [Column(TypeName = "tinyint")]
        public PartyStatus Party { get; set; }

        [Column(TypeName = "tinyint")]
        public FamilyStatus Family { get; set; }
    }
}
