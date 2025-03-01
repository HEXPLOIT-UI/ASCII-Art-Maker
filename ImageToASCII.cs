using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp;
using System.Text;
using SixLabors.ImageSharp.Processing;

namespace ASCII_Art_Maker;

public class ImageToASCII
{

    public static void Start(string imagePath, int nwidth)
    {
        double aspect = 0.5;

        try
        {
            using (Image<Rgba32> image = Image.Load<Rgba32>(imagePath))
            {
                int imWidth = image.Width;
                int imHeight = image.Height;
                double imasp = (double)imWidth / nwidth;
                int nheight = (int)Math.Round(imHeight * aspect / imasp);

                image.Mutate(x => x.Resize(nwidth, nheight, KnownResamplers.NearestNeighbor));

                string gradient = " .:!/r(lZ4H9W8$@";

                StringBuilder sb = new StringBuilder();

                sb.AppendLine("```");

                for (int y = 0; y < nheight; y++)
                {
                    for (int x = 0; x < nwidth; x++)
                    {
                        Rgba32 pixel = image[x, y];
                        int avg = (int)Math.Round((pixel.R + pixel.G + pixel.B) / 3.0);
                        int index = avg / 16;
                        if (index >= gradient.Length)
                            index = gradient.Length - 1;
                        sb.Append(gradient[index]);
                    }
                    sb.AppendLine();
                }

                sb.Append("```");

                Console.WriteLine(sb.ToString());
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex.Message);
        }
    }
}
