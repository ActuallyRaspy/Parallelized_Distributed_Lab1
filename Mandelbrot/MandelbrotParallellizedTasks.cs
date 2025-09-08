using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Mandelbrot
{
    public class MandelbrotParallellizedTasks : MandelbrotBase
    {
        public override string Name
        {
            get { return "MandelbrotParallellizedTasks"; }
        }

        public MandelbrotParallellizedTasks(int pixelsX, int pixelsY) : base(pixelsX, pixelsY)
        {
        }

        public override void Compute() // Using Tasks
        {
            Tuple<double, double> xRange = Tuple.Create(LowerX, UpperX);
            Tuple<double, double> yRange = Tuple.Create(LowerY, UpperY);

            double stepx = (xRange.Item2 - xRange.Item1) / width;
            double stepy = (yRange.Item2 - yRange.Item1) / height;

            int taskAmount = width; //Get the amount of needed tasks (one task for each amount of width.)

            Task[] taskArray = new Task[taskAmount]; 



            for (int i = 0; i < taskAmount; i++) // For every task, create and run a task
            {
                int taskIndex = i;
                taskArray[taskIndex] = Task.Run(() =>
                {
                    CalcNumbers(xRange, yRange, stepx, stepy, taskIndex);
                });
            }

            Task.WaitAll(taskArray); //Wait for all tasks in the array to be finished
        }



        private void CalcNumbers(Tuple<double, double> xRange, Tuple<double, double> yRange, double stepx, double stepy, int taskIndex)
        {
            for (int j = 0; j < height; j++) //height
            {
                double tempx = xRange.Item1 + taskIndex * stepx;
                double tempy = yRange.Item1 + j * stepy;
                int color = Diverge(tempx, tempy);
                Image[taskIndex, j] = MAX_ITERATIONS - color;
            }
        }
    }
}
