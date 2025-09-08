using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Utilities;

namespace Mandelbrot
{
    public abstract class MandelbrotBase : ICompute
    {
        public abstract string Name { get; }

        public double LowerX { get; set; }
        public double UpperX { get; set; }
        public double LowerY { get; set; }
        public double UpperY { get; set; }

        public int[,] Image { get; protected set; }

        protected MandelbrotBase(int pixelsX, int pixelsY)
        {
            width = pixelsX;
            height = pixelsY;
            Image = new int[pixelsX, pixelsY];
            LowerX = -1.0;
            UpperX = 1.0;
            LowerY = -1.0;
            UpperY = 1.0;
        }

        public abstract void Compute();

        protected void Compute(Tuple<double, double> xRange, Tuple<double, double> yRange, int[,] image)
        {
            int widthPixels = image.GetLength(0); // Width of the window
            int heightPixels = image.GetLength(1); // Height of the window
            double stepx = (xRange.Item2 - xRange.Item1) / widthPixels; // Max width - min width divided by the window's size. This gives the "true" steps between pixels.
            double stepy = (yRange.Item2 - yRange.Item1) / heightPixels; // Max height - min height divided by the window's size. 

            for (int i = 0; i < widthPixels; i++) {      // For every 1 pixel in the horizontal plane/x/width. Will be run (n) times where n is the amount of pixels.
                for (int j = 0; j < heightPixels; j++) { // For every 1 pixel in the vertical plane/y/height. Draw 1 column at a time. 
                                                         // Will be run (h*n) times where h is the amount of pixels in height and n is the amount of pixels in width.

                    double tempx = xRange.Item1 + i * stepx; // xRangeMin + (i * stepx) =
                                                             // startingWidthPixel + (widthPixel * truePixelOffset) =
                                                             // startingWidthPixel + pixelOffset =
                                                             // currentIterationsWidthPixel = tempx
                    double tempy = yRange.Item1 + j * stepy;
                    int color = Diverge(tempx, tempy);
                    image[i, j] = MAX_ITERATIONS - color;
                }
            }
        }

        protected int Diverge(double cx, double cy)
        {
            int iter = 0;
            double vx = cx, vy = cy; // Avoid sharing data implicitly (through method parameter), initialize new variables for the input parameters.
            while (iter < MAX_ITERATIONS && (vx*vx + vy*vy) < 16) { //Default "magic number" was 4, put it at 16 for now
                double tx = vx * vx - vy * vy + cx; //(x^2 - y^2 + cx) math woohoo :))))))
                double ty = 2 * vx * vy + cy;
                vx = tx;
                vy = ty;
                iter++;
            }
            return iter;
        }
        protected const int MAX_ITERATIONS = 255;
        protected readonly int width;
        protected readonly int height;
    }
}