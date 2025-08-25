using System.ComponentModel.DataAnnotations;

namespace ReelDesigner.Models
{
    public class ReelModel
    {
        [Display(Name = "ბარაბნის დიამეტრი (mm)")]
        [Range(50, 5000)]
        public double FlangeDiameter { get; set; } = 800;

        [Display(Name = "გულის დიამეტრი (mm)")]
        [Range(10, 4999)]
        public double BarrelDiameter { get; set; } = 300;

        [Display(Name = "შიდა სიგანე (mm)")]
        [Range(10, 5000)]
        public double Width { get; set; } = 600;

        [Display(Name = "ნახვრეტის დიამეტრი (mm)")]
        [Range(5, 500)]
        public double ArborHoleDiameter { get; set; } = 50;

        [Display(Name = "გვერდის სისქე (mm)")]
        [Range(5, 100)]
        public double FlangeThickness { get; set; } = 30;

        [Display(Name = "გულის სისქე (mm)")]
        [Range(5, 200)]
        public double DrumThickness { get; set; } = 50;
    }
}

