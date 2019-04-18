VIC_SPRITE_X_POS        = $d000
VIC_SPRITE_Y_POS        = $d001
VIC_SPRITE_X_EXTEND     = $d010

;| Bit  7   |    Raster Position Bit 8 from $D012
;| Bit  6   |    Extended Color Text Mode: 1 = Enable
;| Bit  5   |    Bitmap Mode: 1 = Enable
;| Bit  4   |    Blank Screen to Border Color: 0 = Blank
;| Bit  3   |    Select 24/25 Row Text Display: 1 = 25 Rows
;| Bits 2-0 |    Smooth Scroll to Y Dot-Position (0-7)
VIC_CONTROL_1           = $d011
VIC_RASTER_POS          = $d012
VIC_STROBE_X            = $d013   ;light pen x
VIC_STROBE_Y            = $d014   ;light pen y
VIC_SPRITE_ENABLE       = $d015 

;| Bits 7-6 |    Unused
;| Bit  5   |    Reset-Bit: 1 = Stop VIC (no Video Out, no RAM refresh, no bus access)
;| Bit  4   |    Multi-Color Mode: 1 = Enable (Text or Bitmap)
;| Bit  3   |    Select 38/40 Column Text Display: 1 = 40 Cols
;| Bits 2-0 |    Smooth Scroll to X Dot-Position (0-7)
VIC_CONTROL_2           = $d016
VIC_SPRITE_EXPAND_Y     = $d017

;| Bits 7-4 |   Video Matrix Base Address (inside VIC)
;| Bit  3   |   Bitmap-Mode: Select Base Address (inside VIC)
;| Bits 3-1 |   Character Dot-Data Base Address (inside VIC)
;| Bit  0   |   Unused
VIC_MEMORY_CONTROL      = $d018
VIC_IRQ_REQUEST         = $d019 
;| Bit 7-4  |   Always 1
;| Bit 3    |   Light-Pen Triggered IRQ Flag
;| Bit 2    |   Sprite to Sprite Collision IRQ Flag     (see $D01E)
;| Bit 1    |   Sprite to Background Collision IRQ Flag (see $D01F)
;| Bit 0    |   Raster Compare IRQ Flag                 (see $D012)
VIC_IRQ_MASK            = $d01a
VIC_SPRITE_PRIORITY     = $d01b
VIC_SPRITE_MULTICOLOR   = $d01c
VIC_SPRITE_EXPAND_X     = $d01d
VIC_SPRITE_COLLISION    = $d01e
VIC_SPRITE_BG_COLLISION = $d01f
VIC_BORDER_COLOR        = $d020
VIC_BACKGROUND_COLOR    = $d021
VIC_CHARSET_MULTICOLOR_1= $d022
VIC_CHARSET_MULTICOLOR_2= $d023
VIC_BACKGROUND_COLOR_3  = $d024
VIC_SPRITE_MULTICOLOR_1 = $d025
VIC_SPRITE_MULTICOLOR_2 = $d026
VIC_SPRITE_COLOR        = $d027
VIC_KEYBOARD_LINES      = $d02f
VIC_CLOCK_SWITCH        = $d030

CIA_DATA_PORT_A         = $dd00


IRQ_RETURN_KERNAL       = $ea81
IRQ_RETURN_KERNAL_KEYBOARD  = $ea31 
 
JOYSTICK_PORT_II        = $dc00
JOYSTICK_PORT_I         = $dc01

;| Bit 7 |   On Read:  1 = Interrupt occured
;|       |   On Write: 1 = Set Int.-Flags, 0 = Clear Int-.Flags
;| Bit 4 |   FLAG1 IRQ (Cassette Read / Serial Bus SRQ Input)
;| Bit 3 |   Serial Port Interrupt ($DC0C full/empty)
;| Bit 2 |   Time-of-Day Clock Alarm Interrupt
;| Bit 1 |   Timer B Interrupt (Tape, Serial Port)
;| Bit 0 |   Timer A Interrupt (Kernal-IRQ, Tape)
CIA_IRQ_CONTROL         = $dc0d

PROCESSOR_PORT          = $01

KERNAL_IRQ_LO           = $fffe
KERNAL_IRQ_HI           = $ffff
