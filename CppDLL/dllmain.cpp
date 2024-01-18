// dllmain.cpp : Defines the entry point for the DLL application.
#include "pch.h"
#include <algorithm>

unsigned char CalculateNewPixelValue(unsigned char* fragment, long* masks)
{
    // Initialize pixel value
    int value = 0;

    // According to the algorithm formula, initially add components calculated based on the mask values and pixel values
    for (int j = 0; j < 3; j++)
        for (int i = 0; i < 3; i++)
            value += fragment[i + j * 3] * masks[i + j * 3];

    // In case the value goes beyond the boundaries (0-255), set it to the boundary value.
    value = std::clamp<int>(value, 0, 255);

    // Return the pixel value
    return (unsigned char)value;
}

// Main function applying the LAPL1 filter.
// Takes parameters:
//   inputArrayPointer: Pointer to the input byte array (passed bitmap)
//   outputArrayPointer: Pointer to the output array (where the filtered fragment will be saved)
//   bitmapLength: The length of the bitmap
//   bitmapWidth: The width of the bitmap
//   startIndex: The starting index for filtering the fragment
//   indicesToFilter: The number of indices to be filtered
// The function filters the specified fragment and saves it to the output array.
extern "C" __declspec(dllexport) void __stdcall ApplyFilterCpp(unsigned char* input, int width, int height, unsigned char* output) {
    // Initialize masks with values from the Laplace LAPL1 filter
    int MASK[9] = { 1, 1, 1, 1, -8, 1, 1, 1, 1 };
    for (int y = 1; y < height - 1; y++)                        // p?tla po warto?ciach Y obrazu
    {
        for (int x = 3; x < width * 3 - 3; x += 3)              // p?tla po warto?ciach X obrazu, uwzgl?dniaj?c, ?e ka?dy piksel posiada trzy warto?ci R,G,B
        {
            int RedSum = 0;
            int GreenSum = 0;
            int BlueSum = 0;
            int idMask = 0;
            for (int pxY = -1; pxY <= 1; pxY++)                 //
            {                                                   // p?tle otaczaj?ce dooko?a piksel, dla którego jest aktualnie wyznaczana nowa warto??
                for (int pxX = -1; pxX <= 1; pxX++)             //
                {
                    int r = input[(y - pxY) * width * 3 + x + pxX * 3 + 2] * MASK[idMask];
                    RedSum += r;       // na?o?enie maski na ka?dy kolor
                    int g = input[(y - pxY) * width * 3 + x + pxX * 3 + 1] * MASK[idMask];
                    GreenSum += g;     // i dodanie do odpowiedniej sumy
                    int b = input[(y - pxY) * width * 3 + x + pxX * 3 + 0] * MASK[idMask];
                    BlueSum += b;      // 
                    idMask++;
                }
            }                                                           // sposób wybierania pozycji:
                                                                        // wybór rz?du: (odpowiedni rz?d Y - przesuni?cie wzgl?dem ?rodkowego piksela) * szeroko?? obrazka * 3(RGB)
            RedSum = (RedSum < 0) ? 0 : (RedSum > 255) ? 255 : RedSum;
            GreenSum = (GreenSum < 0) ? 0 : (GreenSum > 255) ? 255 : GreenSum;
            BlueSum = (BlueSum < 0) ? 0 : (BlueSum > 255) ? 255 : BlueSum;

            output[y * width * 3 + x + 2] = RedSum;               // zapisanie nowych warto?ci
            output[y * width * 3 + x + 1] = GreenSum;
            output[y * width * 3 + x + 0] = BlueSum;
        }
    }
}

BOOL APIENTRY DllMain(HMODULE hModule,
    DWORD  ul_reason_for_call,
    LPVOID lpReserved
)
{
    switch (ul_reason_for_call)
    {
    case DLL_PROCESS_ATTACH:
    case DLL_THREAD_ATTACH:
    case DLL_THREAD_DETACH:
    case DLL_PROCESS_DETACH:
        break;
    }
    return TRUE;
}

