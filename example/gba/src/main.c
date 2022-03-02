#include <tonc.h>

#include "mgba.h"

int main(void)
{
    mgba_open();
    mgba_console_open();

    irq_init(NULL);
    irq_enable(II_VBLANK);

    int x = 0;
    x += 35;
    x << 12;

    mgba_printf(MGBA_LOG_INFO, "Hello, World!");

    while (1)
    {
        key_poll();
        x++;
        VBlankIntrWait();
    }
}