using OpenCvSharp;
using System.Runtime.InteropServices;

namespace ASCII_Art_Maker;

class VideoToASCII
{
    [StructLayout(LayoutKind.Sequential)]
    public struct COORD
    {
        public short X;
        public short Y;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct SMALL_RECT
    {
        public short Left;
        public short Top;
        public short Right;
        public short Bottom;
    }

    const int STD_OUTPUT_HANDLE = -11;
    const uint GENERIC_READ = 0x80000000;
    const uint GENERIC_WRITE = 0x40000000;
    const uint CONSOLE_TEXTMODE_BUFFER = 1;

    [DllImport("kernel32.dll", SetLastError = true)]
    static extern bool SetConsoleScreenBufferSize(IntPtr hConsoleOutput, COORD size);

    [DllImport("kernel32.dll", SetLastError = true)]
    static extern bool SetConsoleWindowInfo(IntPtr hConsoleOutput, bool bAbsolute, [In] ref SMALL_RECT lpConsoleWindow);

    [DllImport("kernel32.dll", SetLastError = true)]
    static extern IntPtr CreateConsoleScreenBuffer(
        uint dwDesiredAccess,
        uint dwShareMode,
        IntPtr lpSecurityAttributes,
        uint dwFlags,
        IntPtr lpScreenBufferData);

    [DllImport("kernel32.dll", SetLastError = true)]
    static extern bool SetConsoleActiveScreenBuffer(IntPtr hConsoleOutput);

    [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    static extern bool WriteConsoleOutputCharacter(
        IntPtr hConsoleOutput,
        string lpCharacter,
        uint nLength,
        COORD dwWriteCoord,
        out uint lpNumberOfCharsWritten);

    [DllImport("kernel32.dll", SetLastError = true)]
    static extern IntPtr GetStdHandle(int nStdHandle);

    static void SetWindow(int width, int height)
    {
        COORD size;
        size.X = (short)width;
        size.Y = (short)height;
        SMALL_RECT rect;
        rect.Left = 0;
        rect.Top = 0;
        rect.Right = (short)(width - 1);
        rect.Bottom = (short)(height - 1);

        IntPtr hStdOut = GetStdHandle(STD_OUTPUT_HANDLE);
        SetConsoleScreenBufferSize(hStdOut, size);
        SetConsoleWindowInfo(hStdOut, true, ref rect);
    }

    public static void Start(string videoPath, int nwidth)
    {
        double aspect = 11.0 / 24.0;

        using (var capture = new VideoCapture(videoPath))
        {
            if (!capture.IsOpened())
            {
                Console.WriteLine("Не удалось открыть видеофайл.");
                return;
            }

            int im_width = (int)capture.Get(VideoCaptureProperties.FrameWidth);
            int im_height = (int)capture.Get(VideoCaptureProperties.FrameHeight);

            double imasp = (double)im_width / nwidth;
            int nheight = (int)Math.Round(im_height * aspect / imasp);

            SetWindow(nwidth, nheight);

            char[] screen = new char[nwidth * nheight];

            IntPtr hConsole = CreateConsoleScreenBuffer(GENERIC_READ | GENERIC_WRITE, 0, IntPtr.Zero, CONSOLE_TEXTMODE_BUFFER, IntPtr.Zero);
            SetConsoleActiveScreenBuffer(hConsole);
            uint dwBytesWritten = 0;

            string gradient = " .:!/r(lZ4H9W8$@";

            using (var frame = new Mat())
            {

                double fps = capture.Get(VideoCaptureProperties.Fps);
                int delay = (int)Math.Round(1000 / fps);

                while (true)
                {
                    capture.Read(frame);
                    if (frame.Empty())
                        break;

                    Cv2.Resize(frame, frame, new Size(nwidth, nheight), 0, 0, InterpolationFlags.Nearest);

                    for (int x = 0; x < frame.Rows; x++)
                    {
                        for (int y = 0; y < frame.Cols; y++)
                        {
                            Vec3b pixel = frame.At<Vec3b>(x, y);
                            int color = (int)Math.Round((pixel[0] + pixel[1] + pixel[2]) / 3.0);
                            int index = color / 16;
                            if (index >= gradient.Length)
                                index = gradient.Length - 1;
                            screen[x * nwidth + y] = gradient[index];
                        }
                    }

                    string output = new string(screen);
                    WriteConsoleOutputCharacter(hConsole, output, (uint)output.Length, new COORD { X = 0, Y = 0 }, out dwBytesWritten);
                    Thread.Sleep(delay);
                }
            }
        }
    }
}