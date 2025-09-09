using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Mandelbrot
{
    public class MandelbrotParallellizedLoadBalanced : MandelbrotBase
    {
        public override string Name
        {
            get { return "MandelbrotParallellizedLoadBalanced"; }
        }

        public MandelbrotParallellizedLoadBalanced(int pixelsX, int pixelsY) : base(pixelsX, pixelsY)
        {
        }

        public override void Compute() // Using Parallell.For
        {

            Tuple<double, double> xRange = Tuple.Create(LowerX, UpperX);
            Tuple<double, double> yRange = Tuple.Create(LowerY, UpperY);
            double stepx = (xRange.Item2 - xRange.Item1) / width; 
            double stepy = (yRange.Item2 - yRange.Item1) / height;  

            int tileSize = 12; //Set the size for each tile. 12 seems to be the only one capable of hitting 0.14s
            BlockingCollection<(int, int)> tiles = new BlockingCollection<(int startX, int startY)>();
            //Initialize the collection for tiles. Each tile is denoted by its top-left pixel.
            // The keywords for this is that it is "thread-safe" and that it supports a "producer/consumer" model.
            // That means that threads can interact with the collectionwithout interfering with eachother.

            for (int y = 0; y < height; y += tileSize) //Iterate through the length of the tile's Y by steps of the tileSize to place the starting point correctly.
            {
                for (int x = 0; x < width; x += tileSize) // Same here but iterate on the width, by the tilesize.
                {
                    tiles.Add((x, y)); //Add the starting point coordinates to the collection, this point is the "tile" (the tile will be built from the point)
                }

            }
            tiles.CompleteAdding(); //Tell the collection that we are done and deny any new additions

            Parallel.ForEach(tiles.GetConsumingEnumerable(), tile => //Threads will spawn and pull the next available tile to be worked on.
                                                                     //Slow threads wont hold back the faster ones because they are sharing the queue.
            {
                for (int dy = 0; dy < tileSize && tile.Item2 + dy < height; dy++) //For every tile-point in the Y. Also Make sure we dont go over the image edge 
                {
                    for (int dx = 0; dx < tileSize && tile.Item1 + dx < width; dx++)
                    {
                        int px = tile.Item1 + dx;
                        int py = tile.Item2 + dy;

                        double tempx = xRange.Item1 + px * stepx;
                        double tempy = yRange.Item1 + py * stepy;

                        int color = Diverge(tempx, tempy);
                        Image[px, py] = MAX_ITERATIONS - color;
                    }
                }
            });
        }
    }
}
