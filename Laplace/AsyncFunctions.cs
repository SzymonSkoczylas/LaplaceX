using System.Printing.IndexedProperties;
using System.Runtime.InteropServices;


namespace Laplace
{
    public class AsyncFunctions
    {
        public static List<Tuple<int, int, byte[]>> threadValues;
        public static List<Tuple<byte[], int, int, byte[]>> threadValuesAsm;
#if FALSE
        // Dll z CPP
        [DllImport(@"C:\Users\Achim\Desktop\studia\Laplace\Laplace\x64\Release\CppDLL.dll")]
        static extern void ApplyFilterCpp(byte[] input, int width, int height, byte[] output);
        // Dll z Asm
        [DllImport(@"C:\Users\Achim\Desktop\studia\Laplace\Laplace\x64\Release\AsmDLL.dll")]
        static extern void ApplyFilterAsm(byte[] input, int width, int height, byte[] output);
#else
        // Dll z CPP
        [DllImport(@"C:\Users\Achim\Desktop\studia\Laplace\LaplaceX\x64\Debug\CppDLL.dll")]
        static extern void ApplyFilterCpp(byte[] input, int width, int height, byte[] output);
        // Dll z Asm
        [DllImport(@"C:\Users\Achim\Desktop\studia\Laplace\LaplaceX\x64\Debug\AsmDLL.dll")]
        static extern void ApplyFilterAsm(byte[] input, int width, int height, byte[] output);
#endif

        private class AsmThread
        {
            public byte[] input;
            public int width;
            public int height;
            public byte[] output;
            public AsmThread(byte[] input, int width, int height, byte[] output)
            {
                this.input = input;
                this.width = width;
                this.height = height;
                this.output = output;
            }
        }

        public static void UseAsmAlgorithm(byte[] bitmap, int threads, byte[] final, int width, int height)
        {

            if (threads > height)
                threads = height;
            int partialHeight = height % threads;
            int partialImageHeight = (int)(height / threads);
            byte[][] partialImage = new byte[threads][];
            byte[][] partialOutput = new byte[threads][];
            int partialImageIndex = 0;
            List<Task> threadList = new List<Task>();
            //image, width, height, output
            List<AsmThread> threadsList = new List<AsmThread>();
            for (int y = 0; y < height - partialHeight - 1; y += partialImageHeight)
            {
                int start = y;
                int end = y + partialImageHeight;
                if (partialHeight > 0)
                {
                    end += 1;
                    partialHeight -= 1;
                    y += 1;
                }
                if (start > 0)
                    start -= 1;
                if (end < height)
                    end += 1;

                partialImage[partialImageIndex] = new byte[end * width * 3 - start * width * 3];
                int tmpI = partialImageIndex;

                Array.Copy(bitmap, start * width * 3, partialImage[tmpI], 0, end * width * 3 - start * width * 3);

                partialOutput[tmpI] = new byte[end * width * 3 - start * width * 3];

                var thread = new AsmThread(partialImage[tmpI], width, end - start, partialOutput[tmpI]);
                var p1 = thread.input;
                var p2 = thread.width;
                var p3 = thread.height;
                var p4 = thread.output;
                var task = Task.Factory.StartNew(() => ApplyFilterAsm(p1, p2, p3, p4));
                threadList.Add(task);

                partialImageIndex += 1;
            }

            Task.WaitAll(threadList.ToArray());

            int currentHeight = 0;
            foreach (var partial in partialOutput)
            {
                if (currentHeight == 0)
                {
                    Array.Copy(partial, 0, final, currentHeight, partial.Length - width * 3);
                    currentHeight += partial.Length / (width * 3) - 1;
                }
                else
                {
                    Array.Copy(partial, width * 3, final, currentHeight * width * 3, partial.Length - width * 3);
                    currentHeight += partial.Length / (width * 3) - 2;
                }
            }
            threadList.Clear();
        }
        public static void UseCppAlgorithm(byte[] bitmap, int threads, byte[] final, int width, int height)
        {

            if (threads > height)
                threads = height;
            int partialHeight = height % threads;
            int partialImageHeight = (int)(height / threads);
            byte[][] partialImage = new byte[threads][];
            byte[][] partialOutput = new byte[threads][];
            int partialImageIndex = 0;
            List<Task> threadList = new List<Task>();
            //image, width, height, output
            List<AsmThread> threadsList = new List<AsmThread>();
            for (int y = 0; y < height - partialHeight - 1; y += partialImageHeight)
            {
                int start = y;
                int end = y + partialImageHeight;
                if (partialHeight > 0)
                {
                    end += 1;
                    partialHeight -= 1;
                    y += 1;
                }
                if (start > 0)
                    start -= 1;
                if (end < height)
                    end += 1;

                partialImage[partialImageIndex] = new byte[end * width * 3 - start * width * 3];
                int tmpI = partialImageIndex;

                Array.Copy(bitmap, start * width * 3, partialImage[tmpI], 0, end * width * 3 - start * width * 3);

                partialOutput[tmpI] = new byte[end * width * 3 - start * width * 3];

                var thread = new AsmThread(partialImage[tmpI], width, end - start, partialOutput[tmpI]);
                var p1 = thread.input;
                var p2 = thread.width;
                var p3 = thread.height;
                var p4 = thread.output;
                var task = Task.Factory.StartNew(() => ApplyFilterCpp(p1, p2, p3, p4));
                threadList.Add(task);

                partialImageIndex += 1;
            }

            Task.WaitAll(threadList.ToArray());

            int currentHeight = 0;
            foreach (var partial in partialOutput)
            {
                if (currentHeight == 0)
                {
                    Array.Copy(partial, 0, final, currentHeight, partial.Length - width * 3);
                    currentHeight += partial.Length / (width * 3) - 1;
                }
                else
                {
                    Array.Copy(partial, width * 3, final, currentHeight * width * 3, partial.Length - width * 3);
                    currentHeight += partial.Length / (width * 3) - 2;
                }
            }
            threadList.Clear();
        }


    }
}
