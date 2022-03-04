#include <tonc.h>

#if DEBUG
#include "mgba.h"
#endif

#include "graphics/onion.h"

#define COLOR16(r, g, b) (((r) & 0x1f) + (((g) & 0x1f) << 5) + (((b) & 0x1f) << 10))

OBJ_ATTR obj_buffer[128];
int onionX = 30;
int onionY = 30;
u8 fade = 0;

u16 onionPaletteModified[graphics__onion_PalLen / sizeof(u16)];

int main(void)
{
    #if DEBUG
    mgba_open();
    mgba_console_open();
    #endif

    irq_init(NULL);
    irq_enable(II_VBLANK);

    pal_bg_bank[0][0] = 0;
	pal_bg_bank[0][1] = 0xffff;

    LZ77UnCompVram(graphics__onion_Tiles, &tile_mem[4][0]);
    memcpy16(onionPaletteModified, graphics__onion_Pal, graphics__onion_PalLen / sizeof(u16));

    oam_init(obj_buffer, 128);
    REG_DISPCNT = DCNT_OBJ | DCNT_OBJ_1D;

    OBJ_ATTR* onion = &obj_buffer[0];
    obj_set_attr(onion, ATTR0_SQUARE, ATTR1_SIZE_16x16, ATTR2_PALBANK(0));
    obj_set_pos(onion, onionX, onionY);

    #if DEBUG
    mgba_printf(MGBA_LOG_INFO, "Hello, World!");
    #endif

    while (1)
    {
        key_poll();

        onionX += key_tri_horz();
        onionY += key_tri_vert();
        obj_set_pos(onion, onionX, onionY);
        
        int t = key_tri_fire();
        fade = CLAMP(fade + t, 0, 31);

        for (int i = 0; i < graphics__onion_PalLen / sizeof(u16); i++)
        {
            u16 c = graphics__onion_Pal[i];
            u8 r = c & 0x1f;
            u8 g = (c >> 5) & 0x1f;
            u8 b = (c >> 10) & 0x1f;

            r = CLAMP(r - fade, 0, 0x1f);
            g = CLAMP(g - fade, 0, 0x1f);
            b = CLAMP(b - fade, 0, 0x1f);

            onionPaletteModified[i] = COLOR16(r, g, b);
        }

        memcpy16(pal_obj_mem, onionPaletteModified, graphics__onion_PalLen / sizeof(u16));
        oam_copy(oam_mem, obj_buffer, 1);

        VBlankIntrWait();
    }
}