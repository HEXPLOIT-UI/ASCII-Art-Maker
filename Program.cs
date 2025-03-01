using System.Text;

namespace ASCII_Art_Maker;

class Program
{

    static void Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;
        Console.InputEncoding = Encoding.UTF8;
        Console.WriteLine("select ASCII art generation mode:");
        Console.WriteLine("[1] - Image to ASCII art");
        Console.WriteLine("[2] - Video to ASCII video");
        if (int.TryParse(Console.ReadLine(), out int mode))
        {
            Console.WriteLine("Enter path of file or drag and drop it to console (if you using desktop enviroment)");
            string? path = Console.ReadLine();
            if (path == null) return;
            path = Path.GetFullPath(path.Trim('"'));
            if (!File.Exists(path))
            {
                Console.WriteLine($"file {path} not found");
                return;
            }
            int dw = 150;
            Console.WriteLine($"Enter width (press enter to skip, default is {dw})");
            int width = int.TryParse(Console.ReadLine(), out int w) ? w : dw;
            switch (mode)
            {
                case 1:
                    ImageToASCII.Start(path, width);
                    break;
                case 2:
                    VideoToASCII.Start(path, width);
                    break;
                default:
                    Console.WriteLine("invalid mode");
                    break;
            }
        } 
        else
        {
            Console.WriteLine("invalid mode");
            return;
        }
        Console.WriteLine("press any key to continue...");
        Console.ReadKey();
    }
}
