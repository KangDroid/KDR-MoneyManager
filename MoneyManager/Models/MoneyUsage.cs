using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace MoneyManager.Models
{
    [ExcludeFromCodeCoverage]
    public class MoneyUsage
    {
        [Key]
        public string GradId { get; set; }
        public DateTime UserDate { get; set; }
        public string CardType { get; set; }
        public string UsedLocation { get; set; }
        public long MoneyUsed { get; set; }
        public string UsedType { get; set; }
    }
}