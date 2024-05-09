using System.ComponentModel.DataAnnotations;

namespace WorkoutHelper.Shared.Models
{
    public interface IIDProperty
    {
        public int? Id { get; set; }
    }

    public class Exercise : IIDProperty
    {
        [Key]
        public int? Id { get; set; } = -1;
        public string Name { get; set; } = string.Empty;
    }
}
