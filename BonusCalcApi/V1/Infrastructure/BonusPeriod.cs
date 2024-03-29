using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BonusCalcApi.V1.Infrastructure
{
    public class BonusPeriod
    {
        [Key]
        [StringLength(10)]
        public string Id { get; set; }

        public DateTime StartAt { get; set; }

        public int Year { get; set; }

        public int Number { get; set; }

        public DateTime? ClosedAt { get; set; }

        [StringLength(100)]
        public string ClosedBy { get; set; }

        public List<Week> Weeks { get; set; }

        public List<BandChange> BandChanges { get; set; }

        public bool IsClosed => ClosedAt != null;
    }
}
