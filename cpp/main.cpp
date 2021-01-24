#include <iostream>

#include <sys/types.h>
#include <sys/stat.h>
#include <sys/mman.h>
#include <sys/ioctl.h>
#include <fcntl.h>
#include <linux/fb.h>
#include <unistd.h>
#include <stdio.h>

/**
 * Source: https://de.wikipedia.org/wiki/Framebuffer#Linux-Framebuffer
 */

#include "rawImage.h"

int main(int argc, char** argv)
{
    int row, col, width, height, bitspp, bytespp;
    unsigned int *data;

    int fd = open("/dev/fb0", O_RDWR);

    struct fb_var_screeninfo screeninfo;
    ioctl(fd, FBIOGET_VSCREENINFO, &screeninfo);

    bitspp = screeninfo.bits_per_pixel;
    if(bitspp != 32)
    {
      printf("%i Bits per pixel\n", bitspp);
      close(fd);
      return 1;
    }

    width  = screeninfo.xres;
    height = screeninfo.yres;
    bytespp = bitspp / 8;

    if(sizeof(unsigned int) != bytespp)
    {
        printf("bytespp %i != sizeof(int)\n", bitspp);
       close(fd);
       return 1;
    }

    data = (unsigned int*) mmap(0, width * height * bytespp, PROT_READ | PROT_WRITE, MAP_SHARED, fd, 0);

    std::cout << "Width " << width << " height " << height << " bytes per pixel " << bytespp << std::endl;

    // 0x000000FF (blue) AARRGGBB Format
    for(row = 0; row < height; row++)
      for(col = 0; col < width; col++)
         data[row * width + col] = rawImageAARRGGBB[row * width + col];

    munmap(data, width * height * bytespp);

    close(fd);
    return 0;
}
