using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Mandelbrot
{
    public class MandelbrotParallellized : MandelbrotBase
    {
        public override string Name
        {
            get { return "MandelbrotParallellized"; }
        }

        public MandelbrotParallellized(int pixelsX, int pixelsY) : base(pixelsX, pixelsY)
        {
        }

        public override void Compute() // Using Parallell.For
        {
            Tuple<double, double> xRange = Tuple.Create(LowerX, UpperX);
            Tuple<double, double> yRange = Tuple.Create(LowerY, UpperY);
            // Parallelize the outer loop because it includes the inner loop as well
            Parallel.For(0, height, i =>
            {
                double stepx = (xRange.Item2 - xRange.Item1) / width;
                double stepy = (yRange.Item2 - yRange.Item1) / height;

                for (int j = 0; j < height; j++)
                {
                    double tempx = xRange.Item1 + i * stepx;
                    double tempy = yRange.Item1 + j * stepy;
                    int color = Diverge(tempx, tempy);
                    Image[i, j] = MAX_ITERATIONS - color;
                }
            });
        }
    }
}
