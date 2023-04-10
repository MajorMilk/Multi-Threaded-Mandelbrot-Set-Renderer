using System.Drawing;
using System.Numerics;

int width = 4000, height = 4000;

Bitmap bitmap = new Bitmap(width, height);

 
int exponent = 3;

int theadCount = 8;
int threadWidth = width / theadCount;

Thread[] threads = new Thread[theadCount];

double Map(double num, double min1, double max1, double min2, double max2)
{
    return (num - min1) * (max2 - min2) / (max1 - min1) + min2;
}

Color GetColorForPixel(Complex pixel, int maxIterations, double threshold)
{
    // Set up the roots of the polynomial
    Complex[] roots = { 1, Complex.FromPolarCoordinates(1, 2 * Math.PI / 3), Complex.FromPolarCoordinates(1, -2 * Math.PI / 3) };

    // Initialize variables for tracking the root and iteration count
    Complex root = Complex.Zero;
    int iterations = 0;

    // Iterate the function until it converges or the maximum number of iterations is reached
    while (iterations < maxIterations && Complex.Abs(Complex.Pow(pixel,exponent)-1) > threshold)
    {
        // Calculate the next iteration using the Newton-Raphson method
        root = roots.OrderBy(r => Complex.Abs(pixel - r)).First();
        pixel -= (Complex.Pow(pixel,exponent)-1) / (exponent * Complex.Pow(pixel,exponent-1));
        iterations++;
    }

    // Interpolate the color of the pixel based on the root it converges to
    Color color;
    if (root == 1)
    {
        color = Color.Red; // root 1
    }
    else if (root == Complex.FromPolarCoordinates(1, 2 * Math.PI / 3))
    {
        color = Color.Green; // root 2
    }
    else
    {
        color = Color.Blue; // root 3
    }

    return color;
}




for(int x = 0; x < width; x++)
{
    for(int y = 0; y < height; y++)
    {
        double a = Map(x, 0, width, -2, 2);
        double b = Map(y, 0, height, -2, 2);

        Complex z = new Complex(a, b);

        bitmap.SetPixel(x, y, GetColorForPixel(z, 100, 0.0001));

        
    }
}

bitmap.Save("output.png");