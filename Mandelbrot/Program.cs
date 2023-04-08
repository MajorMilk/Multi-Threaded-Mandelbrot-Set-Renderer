using System.Drawing;




// a^2 - b^2 + 2abi


//height can be different but the set will be skewed
int width = 10000;
int height = width;



//adjusting these values will zoom in or out to a spot on the mandelbrot set
//the distance between these values is the zoom level and both should be the same
//https://www.geogebra.org/m/mfewjrek
double brotXLowerBound = -1.5d;
double brotXUpperBound = 0.5d;

double brotYLowerBound = -1d;
double brotYUpperBound = 1d;

//this is the offset from the center of the mandelbrot set
double xOffset = 0.0d;
double yOffset = 0.0d;

Bitmap bmp = new Bitmap(width, height);

int numThreads = 8;
int regionWidth = width / numThreads;

Thread[] threads = new Thread[numThreads];

object lockObject = new object();

double Map(double value, double inMin, double inMax, double outMin, double outMax)
{
    return (value - inMin) * (outMax - outMin) / (inMax - inMin) + outMin;
}




using(Graphics graphics = Graphics.FromImage(bmp))
{
    graphics.Clear(Color.Black);

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

                        real = real2 + a;
                        imaginary = imaginary2 + b;

                        if(real * real + imaginary * imaginary > 32)
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
                            bmp.SetPixel(x, y, Color.FromArgb((int)Map(x,0,width,0,128), (int)Map(y,0,height,0,128), (int)Map(it,0,500,0,255)));
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
    
    
    bmp.Save($"mandelbrot{width/1000}k.png");
}


