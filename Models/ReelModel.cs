using System.ComponentModel.DataAnnotations;

namespace ReelDesigner.Models
{
    public class ReelModel
    {
        [Display(Name = "ბარაბნის დიამეტრი (mm)")]
        [Range(50, 5000)]
        public double FlangeDiameter { get; set; } = 1400;

        [Display(Name = "გულის დიამეტრი (mm)")]
        [Range(10, 4999)]
        public double BarrelDiameter { get; set; } = 700;

        [Display(Name = "შიდა სიგანე (mm)")]
        [Range(10, 5000)]
        public double Width { get; set; } = 880;

        [Display(Name = "ნახვრეტის დიამეტრი (mm)")]
        [Range(5, 500)]
        public double ArborHoleDiameter { get; set; } = 100;

        [Display(Name = "გვერდის სისქე (mm)")]
        [Range(5, 100)]
        public double FlangeThickness { get; set; } = 60;

        [Display(Name = "გულის სისქე (mm)")]
        [Range(5, 200)]
        public double DrumThickness { get; set; } = 30;
    }
}
