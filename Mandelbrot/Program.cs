using System.Drawing;




// a^2 - b^2 + 2abi


//Max width and height of the image is 23000
//if you wanted to make a 60k image for whatever reason you'd need to tile the image and stitch it together
int width = 2000;
int height = width;

bool JuliaSetMode = true;

double JuliaSetA = -0.8; //Real constant
double JuliaSetB = 0.156; //Imaginary constant


//adjusting these values will zoom in or out to a spot on the mandelbrot set / julia set, think of these bounds as a window
//the ratio between the width and height should be the same as the ratio between the upper and lower bounds for x and y unless you want the set to be stretched
//https://www.geogebra.org/m/mfewjrek
double brotXLowerBound = -0.5;
double brotXUpperBound = 0.5;

double brotYLowerBound = -0.5;
double brotYUpperBound = 0.5;

//this is the offset from the center of the mandelbrot set
//this is used to slide the window around
double xOffset = 0;
double yOffset = 0;

Bitmap bmp = new Bitmap(width, height);

int numThreads = 8;
int regionWidth = width / numThreads;

Thread[] threads = new Thread[numThreads];

object lockObject = new object();

double Map(double value, double inMin, double inMax, double outMin, double outMax)
{
    return (value - inMin) * (outMax - outMin) / (inMax - inMin) + outMin;
}






for(int i = 0; i < numThreads; i++)
{
    int startX = i*regionWidth;
    int endX = startX + regionWidth;

    threads[i] = new Thread((i) =>
    {
        int it;
        for(int x = startX; x < endX; x++)
        {
            for(int y = 0; y < height; y++)
            {
                double a = Map(x, 0, width, brotXLowerBound, brotXUpperBound) + xOffset;
                double b = Map(y, 0, height, brotYLowerBound, brotYUpperBound) + yOffset;

                double real = a * a - b * b;
                double imaginary = 2 * a * b;

                it = 0;
                for(; it < 500; it++)
                {
                    double real2 = real * real - imaginary * imaginary;
                    double imaginary2 = 2 * real * imaginary;

                    if(JuliaSetMode)
                    {
                        real = real2 + JuliaSetA;
                        imaginary = imaginary2 + JuliaSetB;
                    }
                    else
                    {
                        real = real2 + a;
                        imaginary = imaginary2 + b;
                    }

                    if(real * real + imaginary * imaginary > 16)
                    {
                        break;
                    }
                }

                    lock(lockObject)
                    {
                        if(it == 500)
                    {
                        bmp.SetPixel(x, y, Color.Black);
                    }
                    else
                    {
                        int t = (int)Map(it, 0, 500, 0, 255);
                        bmp.SetPixel(x, y, Color.FromArgb(t,t,t));
                    }
                }
                
            }
        }
        
    });

    threads[i].Start();
}

foreach(Thread thread in threads)
{
    thread.Join();
}


string name = "temp";
if(JuliaSetMode)
{
    name = $"JuliaSet{width/1000}k{JuliaSetA}{JuliaSetB}";
}
else
{
    name = $"Mandelbrot{width/1000}k";
}
bmp.Save($"{name}.png");



