using Microsoft.AspNetCore.Mvc;
using ReelDesigner.Models;
using System.Globalization;
using System.Text;

namespace ReelDesigner.Controllers
{
    public class ReelController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return View(new ReelModel());
        }

        [HttpPost]
        public IActionResult Index(ReelModel model)
        {
            // Basic validation
            if (model.BarrelDiameter >= model.FlangeDiameter)
                ModelState.AddModelError(nameof(model.BarrelDiameter), "Barrel must be smaller than flange.");
            if (!ModelState.IsValid)
                return View(model);

            // Generate SVG string and pass via ViewData (or ViewModel if you prefer)
            ViewData["SvgSide"] = GenerateSvg(model);
            ViewData["SvgFront"] = GenerateFrontViewSvg(model);
            return View(model);
        }

        private string GenerateSvg(ReelModel m)
        {
            // Side view: concentric circles (flange outer, barrel, arbor)
            // We’ll scale so the flange fits within 360px square with padding.
            const int canvas = 400;               // px
            const int pad = 20;                   // px padding
            double maxDraw = canvas - pad * 2;    // drawing area
            double scale = maxDraw / m.FlangeDiameter; // px per mm
            double cx = canvas / 2.0;
            double cy = canvas / 2.0;

            // Radii in px
            double rFlange = (m.FlangeDiameter / 2.0) * scale;
            double rBarrel = (m.BarrelDiameter / 2.0) * scale;
            double rArbor = (m.ArborHoleDiameter / 2.0) * scale;

            // For a bit of visual detail, draw a flange thickness ring
            double rFlangeInner = Math.Max(rFlange - (m.FlangeThickness * scale), rBarrel + 2);

            // Dimension helpers
            string mm(double v) => v.ToString("0.#", CultureInfo.InvariantCulture);
            string px(double v) => v.ToString("0.###", CultureInfo.InvariantCulture);

            var sb = new StringBuilder();
            sb.AppendLine($@"<svg xmlns=""http://www.w3.org/2000/svg"" width=""{canvas}"" height=""{canvas}"" viewBox=""0 0 {canvas} {canvas}"" aria-label=""Wooden reel side view"">");

            // Background
            sb.AppendLine($@"  <rect x=""0"" y=""0"" width=""{canvas}"" height=""{canvas}"" fill=""white"" />");

            // Flange outer
            sb.AppendLine($@"  <circle cx=""{px(cx)}"" cy=""{px(cy)}"" r=""{px(rFlange)}"" fill=""#f7f7f7"" stroke=""#333"" stroke-width=""1.5"" />");

            // Flange inner (thickness ring)
            sb.AppendLine($@"  <circle cx=""{px(cx)}"" cy=""{px(cy)}"" r=""{px(rFlangeInner)}"" fill=""white"" stroke=""#999"" stroke-width=""1"" />");

            // Barrel (core)
            sb.AppendLine($@"  <circle cx=""{px(cx)}"" cy=""{px(cy)}"" r=""{px(rBarrel)}"" fill=""#eaeaea"" stroke=""#555"" stroke-width=""1.2"" />");

            // Arbor hole
            sb.AppendLine($@"  <circle cx=""{px(cx)}"" cy=""{px(cy)}"" r=""{px(rArbor)}"" fill=""white"" stroke=""#333"" stroke-width=""1.2"" />");

            // Cross hair for center
            sb.AppendLine($@"  <line x1=""{px(cx - 8)}"" y1=""{px(cy)}"" x2=""{px(cx + 8)}"" y2=""{px(cy)}"" stroke=""#aaa"" />");
            sb.AppendLine($@"  <line x1=""{px(cx)}"" y1=""{px(cy - 8)}"" x2=""{px(cx)}"" y2=""{px(cy + 8)}"" stroke=""#aaa"" />");

            // DIMENSIONS: Flange diameter (vertical)
            double xDim1 = pad * 0.8;
            sb.AppendLine($@"  <line x1=""{px(xDim1)}"" y1=""{px(cy - rFlange)}"" x2=""{px(xDim1)}"" y2=""{px(cy + rFlange)}"" stroke=""#222"" stroke-width=""1"" />");
            sb.AppendLine($@"  <line x1=""{px(xDim1 - 6)}"" y1=""{px(cy - rFlange)}"" x2=""{px(xDim1 + 6)}"" y2=""{px(cy - rFlange)}"" stroke=""#222"" />");
            sb.AppendLine($@"  <line x1=""{px(xDim1 - 6)}"" y1=""{px(cy + rFlange)}"" x2=""{px(xDim1 + 6)}"" y2=""{px(cy + rFlange)}"" stroke=""#222"" />");
            sb.AppendLine($@"  <text x=""{px(xDim1 + 8)}"" y=""{px(cy)}"" dominant-baseline=""middle"" font-size=""12"" fill=""#000"">{mm(m.FlangeDiameter)} mm (ბარაბნის Ø)</text>");

            // DIMENSIONS: Barrel diameter (right side vertical)
            double xDim2 = canvas - pad * 0.8;
            sb.AppendLine($@"  <line x1=""{px(xDim2)}"" y1=""{px(cy - rBarrel)}"" x2=""{px(xDim2)}"" y2=""{px(cy + rBarrel)}"" stroke=""#444"" stroke-width=""1"" />");
            sb.AppendLine($@"  <line x1=""{px(xDim2 - 6)}"" y1=""{px(cy - rBarrel)}"" x2=""{px(xDim2 + 6)}"" y2=""{px(cy - rBarrel)}"" stroke=""#444"" />");
            sb.AppendLine($@"  <line x1=""{px(xDim2 - 6)}"" y1=""{px(cy + rBarrel)}"" x2=""{px(xDim2 + 6)}"" y2=""{px(cy + rBarrel)}"" stroke=""#444"" />");
            sb.AppendLine($@"  <text x=""{px(xDim2 - 8)}"" y=""{px(cy)}"" dominant-baseline=""middle"" text-anchor=""end"" font-size=""12"" fill=""#000"">{mm(m.BarrelDiameter)} mm (გულის Ø)</text>");

            // DIMENSIONS: Arbor hole (top horizontal)
            double yDimTop = pad * 0.8;
            sb.AppendLine($@"  <line x1=""{px(cx - rArbor)}"" y1=""{px(yDimTop)}"" x2=""{px(cx + rArbor)}"" y2=""{px(yDimTop)}"" stroke=""#444"" />");
            sb.AppendLine($@"  <line x1=""{px(cx - rArbor)}"" y1=""{px(yDimTop - 6)}"" x2=""{px(cx - rArbor)}"" y2=""{px(yDimTop + 6)}"" stroke=""#444"" />");
            sb.AppendLine($@"  <line x1=""{px(cx + rArbor)}"" y1=""{px(yDimTop - 6)}"" x2=""{px(cx + rArbor)}"" y2=""{px(yDimTop + 6)}"" stroke=""#444"" />");
            sb.AppendLine($@"  <text x=""{px(cx)}"" y=""{px(yDimTop - 8)}"" text-anchor=""middle"" font-size=""12"">{mm(m.ArborHoleDiameter)} mm (ნახვრეტის დიამეტრი)</text>");

// // DIMENSIONS: Barrel Thickness (difference between barrel radius and arbor radius)
// double yDimBT = cy + rBarrel + 40; // place it below barrel circle
// sb.AppendLine($@"  <line x1=""{px(cx + rArbor)}"" y1=""{px(yDimBT)}"" x2=""{px(cx + rBarrel)}"" y2=""{px(yDimBT)}"" stroke=""#d22"" stroke-width=""1"" />");
// sb.AppendLine($@"  <line x1=""{px(cx + rArbor)}"" y1=""{px(yDimBT - 6)}"" x2=""{px(cx + rArbor)}"" y2=""{px(yDimBT + 6)}"" stroke=""#d22"" />");
// sb.AppendLine($@"  <line x1=""{px(cx + rBarrel)}"" y1=""{px(yDimBT - 6)}"" x2=""{px(cx + rBarrel)}"" y2=""{px(yDimBT + 6)}"" stroke=""#d22"" />");
// sb.AppendLine($@"  <text x=""{px(cx + (rArbor + rBarrel) / 2)}"" y=""{px(yDimBT - 8)}"" text-anchor=""middle"" font-size=""12"" fill=""#d22"">{mm(m.BarrelThickness)} mm (ბარაბნის სისქე)</text>");


            // LABEL: Width (not visible in side view) — show as note
            //sb.AppendLine($@"  <text x=""{px(cx)}"" y=""{px(canvas - pad)}"" text-anchor=""middle"" font-size=""12"" fill=""#222"">Traverse Width: {mm(m.Width)} mm</text>");

            // Scale note
            sb.AppendLine($@"  <text x=""{px(canvas - pad)}"" y=""{px(canvas - pad)}"" text-anchor=""end"" font-size=""10"" fill=""#666"">Scale: {px(scale)} px/mm</text>");

            sb.AppendLine("</svg>");
            return sb.ToString();
        }
        
private string GenerateFrontViewSvg(ReelModel m)
{
    const int canvasW = 600;
    const int canvasH = 400;
    const int pad = 30;

    double maxFlange = m.FlangeDiameter;
    double maxWidth = m.Width;

    double scaleY = (canvasH - pad * 2) / maxFlange;
    double scaleX = (canvasW - pad * 2) / maxWidth;
    double scale = Math.Min(scaleX, scaleY);

    double cx = canvasW / 2.0;
    double cy = canvasH / 2.0;

    double halfWidth = (m.Width / 2.0) * scale;
    double rFlange = (m.FlangeDiameter / 2.0) * scale;
    double rBarrel = (m.BarrelDiameter / 2.0) * scale;
    double rArbor = (m.ArborHoleDiameter / 2.0) * scale;

    string px(double v) => v.ToString("0.###", System.Globalization.CultureInfo.InvariantCulture);

    var sb = new System.Text.StringBuilder();
    sb.AppendLine($@"<svg xmlns=""http://www.w3.org/2000/svg"" width=""{canvasW}"" height=""{canvasH}"" viewBox=""0 0 {canvasW} {canvasH}"">");

    // Barrel rectangle
    sb.AppendLine($@"  <rect x=""{px(cx - halfWidth)}"" y=""{px(cy - rBarrel)}"" width=""{px(m.Width * scale)}"" height=""{px(rBarrel * 2)}"" fill=""#fdf2b3"" stroke=""#555"" />");

    // Left flange (rectangle line)
    sb.AppendLine($@"  <rect x=""{px(cx - halfWidth - m.FlangeThickness * scale)}"" y=""{px(cy - rFlange)}"" width=""{px(m.FlangeThickness * scale)}"" height=""{px(rFlange * 2)}"" fill=""#eee"" stroke=""#333"" />");
    sb.AppendLine($@"  <text x=""{px(cx - halfWidth - (m.FlangeThickness * scale / 2))}"" 
                        y=""{px(cy - rFlange - 10)}"" 
                        text-anchor=""middle"" font-size=""12"" fill=""black"">
                        {m.FlangeThickness} mm (გვერდის სისქე)
                  </text>");

// Right Flange Thickness Label
sb.AppendLine($@"  <text x=""{px(cx + halfWidth + (m.FlangeThickness * scale / 2))}"" 
                        y=""{px(cy - rFlange - 10)}"" 
                        text-anchor=""middle"" font-size=""12"" fill=""black"">
                        {m.FlangeThickness} mm
                  </text>");
    // Right flange
    sb.AppendLine($@"  <rect x=""{px(cx + halfWidth)}"" y=""{px(cy - rFlange)}"" width=""{px(m.FlangeThickness * scale)}"" height=""{px(rFlange * 2)}"" fill=""#eee"" stroke=""#333"" />");

    // Arbor hole centerline (optional)
    //sb.AppendLine($@"  <circle cx=""{px(cx)}"" cy=""{px(cy)}"" r=""{px(rArbor)}"" fill=""white"" stroke=""#333"" />");

    // Dimension line for width
    sb.AppendLine($@"  <line x1=""{px(cx - halfWidth)}"" y1=""{px(cy + rFlange + 20)}"" x2=""{px(cx + halfWidth)}"" y2=""{px(cy + rFlange + 20)}"" stroke=""#222"" />");
    sb.AppendLine($@"  <line x1=""{px(cx - halfWidth)}"" y1=""{px(cy + rFlange + 15)}"" x2=""{px(cx - halfWidth)}"" y2=""{px(cy + rFlange + 25)}"" stroke=""#222"" />");
    sb.AppendLine($@"  <line x1=""{px(cx + halfWidth)}"" y1=""{px(cy + rFlange + 15)}"" x2=""{px(cx + halfWidth)}"" y2=""{px(cy + rFlange + 25)}"" stroke=""#222"" />");
    sb.AppendLine($@"  <text x=""{px(cx)}"" y=""{px(cy + rFlange + 35)}"" text-anchor=""middle"" font-size=""12"">{m.Width} mm (შიდა სიგანე)</text>");

    sb.AppendLine("</svg>");
    return sb.ToString();
}


    }
}
