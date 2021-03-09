using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageFiltering.ImageProcessing
{

    partial class FunctionFilter : ImageFilter
    {
        public static FunctionFilter Identity { get
            {
                return new FunctionFilter((byte input) => input, "Identity");
            } }

        public static FunctionFilter Inversion
        {
            get
            {
                return new FunctionFilter((byte input) => (byte)(255 - input), "Inversion");
            }
        }

        public static FunctionFilter BrightnessCorrection(int delta)
        {
            return new FunctionFilter((byte input) => (byte)Math.Min(Math.Max(input + delta, 0), 255), $"Brightness Correction delta={delta}");
        }

        public static FunctionFilter ContrastEnchantment(float multiplier)
        {
            return new FunctionFilter(
                (byte input) =>
                {
                    float val = ((float)input / 256) - 0.5f;
                    val *= multiplier;
                    val = (val + 0.5f) * 256;
                    return (byte)Math.Min(Math.Max(Math.Round(val), 0), 255);
                },
                $"Contrast Enchantment {multiplier}x");
        }

        public static FunctionFilter GammaCorrection(float exponent)
        {
            return new FunctionFilter(
                (byte input) =>
                {
                    float val = (float)input / 256;
                    val = (float)Math.Pow(val, exponent);
                    val *= 256;
                    return (byte)Math.Min(Math.Max(Math.Round(val), 0), 255);
                },
                $"Gamma Correction lambda={exponent}");
        }
    }
}
